using Azure.Messaging.ServiceBus;
using NUnit.Framework;
using TaskManagement.Contracts;
using TaskManagement.ServiceBus;

namespace TaskManagement.Tests
{
    [TestFixture]
    public class ServiceBusTests
    {

        private const string ServiceBusConnectionString = "";
        private const string TopicName = "task-management";
        private ServiceBusClient _serviceBusClient;
        private ServiceBusHandler _serviceBusHandler;

        [SetUp]
        public void Setup()
        {
            _serviceBusClient = new ServiceBusClient(ServiceBusConnectionString);
            _serviceBusHandler = new ServiceBusHandler(ServiceBusConnectionString, TopicName);
        }

        [Test]
        public async Task SendMessageToTopicTest()
        {
            var sender = _serviceBusClient.CreateSender(TopicName);
            var testAssgnment = new Assignment()
            {
                Name = "TestAssignment"
            };
            await _serviceBusHandler.SendMessageAsync(testAssgnment, SubscriptionConstants.CreateSubscription);

            Assert.Pass("Message sent successfully.");
        }

        //TODO: add more tests to check functionality
    }
}

