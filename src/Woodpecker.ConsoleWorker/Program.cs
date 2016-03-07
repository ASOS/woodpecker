using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
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
using Woodpecker.Core;
using Woodpecker.Core.Actors;
using Woodpecker.Core.Events;

namespace Woodpecker.ConsoleWorker
{
    class Program
    {
        private static IConfigurationValueProvider _configurationValueProvider;
        private static Orchestrator _orchestrator;
        private static MasterScheduler _scheduler;

        static void Main(string[] args)
        {

            ServicePointManager.DefaultConnectionLimit = 1000;
            ThreadPool.SetMinThreads(100, 100);

            var container = new WindsorContainer();
            var serviceLocator = new WindsorServiceLocator(container);

            _configurationValueProvider = new AppSettingsConfigurationValueProvider();
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
            Work();
            _orchestrator.Stop();

        }

        private static void Work()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Scheduling ...");                    
                    _scheduler.ScheduleSourcesAsync().Wait();
                    Console.WriteLine("Scheduled");                    

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());                                        
                }

                Thread.Sleep(30 * 1000);
            }
        }
    }
}
