using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client.UnitTests
{
    [TestFixture]
    public class EarningsJobClientTests
    {
        private Autofac.Extras.Moq.AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mocker.Mock<IEndpointInstanceFactory>()
                .Setup(factory => factory.GetEndpointInstance())
                .ReturnsAsync(mocker.Mock<IEndpointInstance>().Object);
        }

        [Test,Ignore("Client doesn't use message session.")]
        public async Task Batches_Earnings_Jobs()
        {
            var messages = new List<GeneratedMessage>();
            for (var i = 0; i < 2001; i++)
            {
                messages.Add(new GeneratedMessage
                {
                    MessageName = "Test message",
                    MessageId = Guid.NewGuid(),
                    StartTime = DateTimeOffset.Now
                });
            }
            var client = mocker.Create<EarningsJobClient>();
            await client.StartJob(1, 12345, DateTime.Now, 1920, 1, messages, DateTimeOffset.Now);
            mocker.Mock<IEndpointInstance>().Verify(session => session.Send(It.IsAny<RecordEarningsJob>(), It.IsAny<SendOptions>()), Times.Exactly(3));
            messages.Take(1000).ToList().ForEach(msg => mocker.Mock<IEndpointInstance>().Verify(session => session.Send(It.Is<RecordEarningsJob>(job => job.GeneratedMessages.Any(genMsg => genMsg.MessageId == msg.MessageId)), It.IsAny<SendOptions>()), Times.Once));
            messages.Skip(1000).Take(1000).ToList().ForEach(msg => mocker.Mock<IEndpointInstance>().Verify(session => session.Send(It.Is<RecordEarningsJob>(job => job.GeneratedMessages.Any(genMsg => genMsg.MessageId == msg.MessageId)), It.IsAny<SendOptions>()), Times.Once));
            messages.Skip(2000).Take(1000).ToList().ForEach(msg => mocker.Mock<IEndpointInstance>().Verify(session => session.Send(It.Is<RecordEarningsJob>(job => job.GeneratedMessages.Any(genMsg => genMsg.MessageId == msg.MessageId)), It.IsAny<SendOptions>()), Times.Once));
        }
    }
}