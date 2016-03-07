using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BeeHive;
using BeeHive.Actors;
using BeeHive.Azure;
using BeeHive.Configuration;
using BeeHive.DataStructures;
using BeeHive.ServiceLocator.Windsor;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Woodpecker.Core;
using Woodpecker.Core.Actors;
using Woodpecker.Core.Events;

namespace Woodpecker.Worker.Role
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private static IConfigurationValueProvider _configurationValueProvider;
        private static Orchestrator _orchestrator;
        private static MasterScheduler _scheduler;

        public override void Run()
        {
            Trace.TraceInformation("Woodpecker.Worker.Role is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 1000;
            ThreadPool.SetMinThreads(100, 100);

            var container = new WindsorContainer();
            var serviceLocator = new WindsorServiceLocator(container);

            _configurationValueProvider = new AzureConfigurationValueProvider();
            var storageConnectionString = _configurationValueProvider.GetValue(ConfigurationKeys.TableStorageConnectionString);
            var servicebusConnectionString = _configurationValueProvider.GetValue(ConfigurationKeys.ServiceBusConnectionString);

            container.Register(

                 Component.For<Orchestrator>()
                    .ImplementedBy<Orchestrator>()
                    .LifestyleSingleton(),
                 Component.For<MasterScheduler>()
                    .ImplementedBy<MasterScheduler>()
                    .LifestyleSingleton(),
                 Component.For<IConfigurationValueProvider>()
                    .Instance(_configurationValueProvider),
                Component.For<IServiceLocator>()
                    .Instance(serviceLocator),
                Component.For<IActorConfiguration>()
                    .Instance(
                    ActorDescriptors.FromAssemblyContaining<BusSourceScheduled>()
                    .ToConfiguration()),
                Component.For<IFactoryActor>()
                    .ImplementedBy<FactoryActor>()
                    .LifestyleTransient(),
                Component.For<BusSourceProcessor>()
                    .ImplementedBy<BusSourceProcessor>()
                    .LifestyleTransient(),
                Component.For<ILockStore>()
                    .Instance(new AzureLockStore(new BlobSource()
                    {
                        ConnectionString = storageConnectionString,
                        ContainerName = "locks",
                        Path = "woodpecker/locks/master_Keys/"
                    })),
                Component.For<IEventQueueOperator>()
                    .Instance(new ServiceBusOperator(servicebusConnectionString))

                );

            _orchestrator = container.Resolve<Orchestrator>();
            _scheduler = container.Resolve<MasterScheduler>();

            Task.Run(() => _orchestrator.SetupAsync()).Wait();
            _orchestrator.Start();

            Console.WriteLine("Working ...");


            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("Woodpecker.Worker.Role has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("Woodpecker.Worker.Role is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();
            _orchestrator.Stop();

            base.OnStop();

            Trace.TraceInformation("Woodpecker.Worker.Role has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");

                while (true)
                {
                    var then = DateTimeOffset.UtcNow;
                    try
                    {
                        Console.WriteLine("Scheduling ...");
                        await _scheduler.ScheduleSourcesAsync();
                        Console.WriteLine("Scheduled");

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }

                    var seconds = DateTimeOffset.UtcNow.Subtract(then).TotalSeconds;
                    if (seconds < 30)
                        await Task.Delay(TimeSpan.FromSeconds(30 - seconds), cancellationToken);

                }
            }
        }
    }
}
