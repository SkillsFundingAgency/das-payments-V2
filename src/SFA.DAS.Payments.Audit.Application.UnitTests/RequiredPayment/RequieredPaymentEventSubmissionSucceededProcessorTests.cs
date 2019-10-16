using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMoqCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Data.EarningEvent;
using SFA.DAS.Payments.Audit.Application.Data.RequiredPayment;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.RequiredPayment;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.RequiredPayment
{
    [TestFixture]

    public class RequieredPaymentEventSubmissionSucceededProcessorTests
    {
        private AutoMoqer mocker;
        private SubmissionSucceededEvent succeededEvent;

        [SetUp]
        public void SetUp()
        {
            mocker = new AutoMoqer();
            succeededEvent = new SubmissionSucceededEvent
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
        public async Task Deletes_Previous_Required_Payment_Event_Data()
        {
            var processor = mocker.Create<RequiredPaymentEventSubmissionSucceededProcessor>();
            await processor.Process(succeededEvent, CancellationToken.None).ConfigureAwait(false);

            mocker.GetMock<IRequiredPaymentEventRepository>()
                .Verify(repo => repo.RemovePriorEvents(It.Is<long>(ukprn => ukprn == succeededEvent.Ukprn),
                        It.Is<short>(academicYear => academicYear == succeededEvent.AcademicYear),
                        It.Is<byte>(collectionPeriod => collectionPeriod == succeededEvent.CollectionPeriod),
                        It.Is<DateTime>(submissionTime => submissionTime == succeededEvent.IlrSubmissionDateTime),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Test]
        public async Task Writes_Pending_Required_Payment_Events_To_Db_Before_Deletion()
        {
            var processor = mocker.Create<RequiredPaymentEventSubmissionSucceededProcessor>();
            await processor.Process(succeededEvent, CancellationToken.None).ConfigureAwait(false);

            mocker.GetMock<IPaymentsEventModelBatchService<RequiredPaymentEventModel>>()
                .Verify(service => service.StorePayments(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}