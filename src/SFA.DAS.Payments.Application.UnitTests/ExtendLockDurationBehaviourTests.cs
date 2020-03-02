using System;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Messaging;

namespace SFA.DAS.Payments.Application.UnitTests
{
    public class ExtendLockDurationBehaviourTests
    {
        private TestableTransportReceiveContext context;
        private ExtendLockDurationBehaviour behavior;
        private Mock<IMessageReceiver> messageReceiver;

        [SetUp]
        public void Setup()
        {
            using (var mocker = AutoMock.GetLoose())
            {
                messageReceiver = mocker.Mock<IMessageReceiver>();
                behavior = mocker.Create<ExtendLockDurationBehaviour>();
                context = new TestableTransportReceiveContext();
            }
        }

        private void SetupMockMessage(DateTime lockedUntilUtc)
        {
            var message = new Message { UserProperties = { ["NServiceBus.EnclosedMessageTypes"] = "Test" } };

            var syspropInstance = new Message.SystemPropertiesCollection();

            typeof(Message.SystemPropertiesCollection)
                .GetProperty("LockedUntilUtc")
                ?.SetValue(syspropInstance, lockedUntilUtc);

            typeof(Message.SystemPropertiesCollection)
                .GetProperty("SequenceNumber")
                ?.SetValue(syspropInstance, 1);

            typeof(Message)
                .GetProperty("SystemProperties")
                ?.SetValue(message, syspropInstance);

            context.Extensions.Set(message);
        }

        [Test]
        public async Task ShouldExtendLockDurationWhenLockedUntilLessThenOrEqualToOneMinute()
        {
            SetupMockMessage(DateTime.UtcNow.AddSeconds(59));

            await behavior.Invoke(context, () => Task.CompletedTask).ConfigureAwait(false);

            messageReceiver.Verify(x => x.RenewLockAsync(It.IsAny<Message>()), Times.Once);
        }

        [Test]
        public async Task ShouldNOTExtendLockDurationWhenLockedUntilGreaterThenOneMinute()
        {
            SetupMockMessage(DateTime.UtcNow.AddSeconds(61));

            await behavior.Invoke(context, () => Task.CompletedTask).ConfigureAwait(false);

            messageReceiver.Verify(x => x.RenewLockAsync(It.IsAny<Message>()), Times.Never);
        }
    }
}