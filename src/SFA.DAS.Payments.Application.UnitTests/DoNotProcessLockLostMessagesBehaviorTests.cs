using System;
using System.Threading.Tasks;
using AutoMoqCore;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Messaging;

namespace SFA.DAS.Payments.Application.UnitTests
{
    public class DoNotProcessLockLostMessagesBehaviorTests
    {
        private DoNotProcessLockLostMessagesBehavior behavior;
        private TestableTransportReceiveContext context;

        [SetUp]
        public void Setup()
        {
            var mocker = new AutoMoqer();
            behavior = mocker.Create<DoNotProcessLockLostMessagesBehavior>();
            context = new TestableTransportReceiveContext { ReceiveOperationAborted = false };
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
        public async Task ShouldAbortReceiveOperationWhenLockedUntilIsInPast()
        {
            SetupMockMessage(DateTime.UtcNow.AddSeconds(-60));

            await behavior.Invoke(context, () => Task.CompletedTask).ConfigureAwait(false);

            context.ReceiveOperationAborted.Should().BeTrue();
        }

        [Test]
        public async Task ShouldNotAbortReceiveOperationWhenLockedUntilIsInFuture()
        {
            SetupMockMessage(DateTime.UtcNow.AddSeconds(60));

            await behavior.Invoke(context, () => Task.CompletedTask).ConfigureAwait(false);

            context.ReceiveOperationAborted.Should().BeFalse();
        }
    }
}