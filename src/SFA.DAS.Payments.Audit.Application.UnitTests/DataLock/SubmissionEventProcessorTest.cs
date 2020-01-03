using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.DataLock;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.DataLock
{
    [TestFixture]
    public class SubmissionEventProcessorTest
    {
        private ISubmissionEventProcessor processor;
        private Mock<IDataLockEventRepository> repositoryMock;
        private Mock<IPaymentsEventModelBatchService<DataLockEventModel>> batchService;

        [SetUp]
        public void SetUp()
        {
            batchService = new Mock<IPaymentsEventModelBatchService<DataLockEventModel>>(MockBehavior.Strict);
            repositoryMock = new Mock<IDataLockEventRepository>(MockBehavior.Strict);
            processor = new SubmissionEventProcessor(repositoryMock.Object, batchService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            repositoryMock.Verify();
            batchService.Verify();
        }

        [Test]
        public async Task TestCacheFlushedAndEventsDeletedOnFailure()
        {
            var submissionFailedEvent = new SubmissionJobFailed
            {
                AcademicYear = 1,
                CollectionPeriod = 2,
                JobId = 3,
                Ukprn = 4,
                IlrSubmissionDateTime = DateTime.Now
            };

            batchService.Setup(b => b.StorePayments(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            repositoryMock.Setup(r => r.DeleteEventsOfSubmission(4, 1, 2, submissionFailedEvent.IlrSubmissionDateTime))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await processor.ProcessSubmissionFailedEvent(submissionFailedEvent).ConfigureAwait(false);
        }

        [Test]
        public async Task TestCacheFlushedAndEventsDeletedOnSuccess()
        {
            var submissionSucceededEvent = new SubmissionJobSucceeded
            {
                AcademicYear = 1,
                CollectionPeriod = 2,
                JobId = 3,
                Ukprn = 4,
                IlrSubmissionDateTime = DateTime.Now
            };

            batchService.Setup(b => b.StorePayments(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            repositoryMock.Setup(r => r.DeleteEventsPriorToSubmission(4, 1, 2, submissionSucceededEvent.IlrSubmissionDateTime))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await processor.ProcessSubmissionSucceededEvent(submissionSucceededEvent).ConfigureAwait(false);
        }
    }
}
