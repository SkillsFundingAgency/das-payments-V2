using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Application.Processors;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Processors
{
    [TestFixture]
    public class RefundRemovedLearningAimProcessorTests
    {
        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
        }

        public async Task Refunds_Levy_Payments()
        {
            var identifiedLearner = new IdentifiedRemovedLearningAim
            {
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = 1819,
                    Period = 1
                },
                EventId = Guid.NewGuid(),
                EventTime = DateTimeOffset.UtcNow,
                IlrSubmissionDateTime = DateTime.Now,
                JobId = 1,
                Learner = new Learner
                {
                    ReferenceNumber = "12345",
                    Uln = 2
                },
                LearningAim = new LearningAim
                {
                    FrameworkCode = 3,
                    FundingLineType = "funding line type",
                    PathwayCode = 4,
                    ProgrammeType = 5,
                    Reference = "learner-ref",
                    StandardCode = 6
                },
                Ukprn = 7
            };

            var processor = mocker.Create<RefundRemovedLearningAimProcessor>();
            var refunds = await processor.RefundLearningAim(identifiedLearner,CancellationToken.None).ConfigureAwait(false);


        }
    }
}