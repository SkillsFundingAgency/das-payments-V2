using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMoqCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Data.EarningEvent;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.EarningEvent
{
    [TestFixture]

    public class EarningEventSubmissionFailedProcessorTests
    {
        private AutoMoqer mocker;
        private SubmissionFailedEvent failedEvent;

        [SetUp]
        public void SetUp()
        {
            mocker = new AutoMoqer();
            failedEvent = new SubmissionFailedEvent
            {
                JobId = 99,
                AcademicYear = 1920,
                CollectionPeriod = 01,
                EventTime = DateTimeOffset.UtcNow,
                IlrSubmissionDateTime = DateTime.Now,
                Ukprn = 1234
            };
        }

        [Test]
        public async Task Deletes_Failed_Job_Earning_Event_Data()
        {
            var processor = mocker.Create<EarningEventSubmissionFailedProcessor>();
            await processor.Process(failedEvent, CancellationToken.None).ConfigureAwait(false);

            mocker.GetMock<IEarningEventRepository>()
                .Verify(repo => repo.RemoveFailedSubmissionEvents(It.Is<long>(jobId => jobId == failedEvent.JobId),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Test]
        public async Task Writes_Pending_Earning_Events_To_Db_Before_Deletion()
        {
            var processor = mocker.Create<EarningEventSubmissionFailedProcessor>();
            await processor.Process(failedEvent, CancellationToken.None).ConfigureAwait(false);

            mocker.GetMock<IPaymentsEventModelBatchService<EarningEventModel>>()
                .Verify(service => service.StorePayments(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}