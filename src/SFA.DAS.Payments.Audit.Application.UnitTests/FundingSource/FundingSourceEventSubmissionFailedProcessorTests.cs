using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMoqCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Data.FundingSource;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.FundingSource
{
    [TestFixture]
    public class FundingSourceEventSubmissionFailedProcessorTests
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
        public async Task Deletes_Failed_Job_Funding_Source_Event_Data()
        {
            var processor = mocker.Create<FundingSourceEventSubmissionFailedProcessor>();
            await processor.Process(failedEvent, CancellationToken.None).ConfigureAwait(false);

            mocker.GetMock<IFundingSourceEventRepository>()
                .Verify(repo => repo.RemoveFailedSubmissionEvents(It.Is<long>(jobId => jobId == failedEvent.JobId),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Test]
        public async Task Writes_Pending_Funding_Source_Events_To_Db_Before_Deletion()
        {
            var processor = mocker.Create<FundingSourceEventSubmissionFailedProcessor>();
            await processor.Process(failedEvent, CancellationToken.None).ConfigureAwait(false);

            mocker.GetMock<IPaymentsEventModelBatchService<FundingSourceEventModel>>()
                .Verify(service => service.StorePayments(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}