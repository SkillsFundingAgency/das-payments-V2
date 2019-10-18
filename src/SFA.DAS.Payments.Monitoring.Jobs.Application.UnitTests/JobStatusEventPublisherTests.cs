using System;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class JobStatusEventPublisherTests
    {
        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            var endpointInstance = mocker.Mock<IEndpointInstance>();
            mocker.Mock<IEndpointInstanceFactory>()
                .Setup(factory => factory.GetEndpointInstance())
                .ReturnsAsync(endpointInstance.Object);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Publishes_SubmissionJobCompleted_Event(bool succeeded)
        {
            var submissionTime = DateTime.Now;
            await mocker.Create<JobStatusEventPublisher>()
                .SubmissionFinished(succeeded, 99, 1234, 1920, 01, submissionTime);

            mocker.Mock<IEndpointInstance>()
                .Verify(x => x.Publish(It.Is<SubmissionJobFinished>(ev => ev.Succeeded == succeeded &&
                                                                          ev.JobId == 99 &&
                                                                          ev.AcademicYear == 1920 &&
                                                                          ev.CollectionPeriod == 01 &&
                                                                          ev.IlrSubmissionDateTime == submissionTime &&
                                                                          ev.Ukprn == 1234)));

        }
    }
}