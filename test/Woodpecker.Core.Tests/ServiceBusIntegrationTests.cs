using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Woodpecker.Core.Azure;
using Xunit;

namespace Woodpecker.Core.Tests
{

    /// <summary>
    /// !!!!!! NOTE: !!!!!!!
    /// This test requires an environment variable to have been set:
    /// Machine/User: AzureServiceBusConnectionStringForTest=... 
    /// </summary>
    public class ServiceBusIntegrationTests
    {
        private string _connectionString;
        public ServiceBusIntegrationTests()
        {
            _connectionString =
                Environment.GetEnvironmentVariable("AzureServiceBusConnectionStringForTest", EnvironmentVariableTarget.Machine);

            if (string.IsNullOrEmpty(_connectionString))
                _connectionString = Environment.GetEnvironmentVariable("AzureServiceBusConnectionStringForTest", EnvironmentVariableTarget.User);

            if(string.IsNullOrEmpty(_connectionString))
                throw new InvalidProgramException("Please set env var for the test");
        }

        [Fact]
        public void TestQueues()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);
            var queues = namespaceManager.GetQueues();
            foreach (var q in queues)
            {
                var result = q.Peck(new PeckSource()
                {
                    Name = "he he"
                });

                Trace.WriteLine(JsonConvert.SerializeObject(result));
            }
        }

        [Fact]
        public void TestTopics()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);
            var queues = namespaceManager.GetTopics();
            foreach (var q in queues)
            {
                var result = q.Peck(new PeckSource()
                {
                    Name = "he he"
                });

                Trace.WriteLine(JsonConvert.SerializeObject(result));
            }
        }

        [Fact]
        public void TestSubscriptions()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);
            var topics = namespaceManager.GetTopics();
            foreach (var t in topics)
            {
                foreach (var q in namespaceManager.GetSubscriptions(t.Path))
                {
                    var result = q.Peck(new PeckSource()
                    {
                        Name = "he he"
                    });

                    Trace.WriteLine(JsonConvert.SerializeObject(result));
                }
            }
        }
    }
}
