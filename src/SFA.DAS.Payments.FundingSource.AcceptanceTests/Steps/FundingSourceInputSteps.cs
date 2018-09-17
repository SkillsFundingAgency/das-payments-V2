using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.FundingSource.AcceptanceTests.Data;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using TechTalk.SpecFlow;
using Learner = SFA.DAS.Payments.AcceptanceTests.Core.Data.Learner;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    [Binding]
    public class FundingSourceInputSteps: StepsBase
    {
        private readonly ScenarioContext context;

        public FundingSourceInputSteps(ScenarioContext context)
        {
            this.context = context;
        }

        [When(@"a payable earning event is received")]
        public void WhenAPayableEarningEventIsReceived()
        {
            var payableEarnings =
                context.Get<IEnumerable<RequiredPayment>>().ToList();

            var learner = context.Get<Learner>();

            var options = new SendOptions();
            options.RequireImmediateDispatch();

            payableEarnings.ForEach(p =>
            {
                var paymentEvent = new ApprenticeshipContractType2RequiredPaymentEvent
                {
                    Learner = new Model.Core.Learner
                    {
                        ReferenceNumber = learner.GeneratedLearnRefNumber,
                        Uln = learner.Uln
                    },
                    OnProgrammeEarningType = (OnProgrammeEarningType) p.TransactionType,
                    AmountDue = p.Amount,
                    JobId = Guid.NewGuid().ToString(),
                    EventTime = DateTimeOffset.UtcNow,
                    Period = (byte) p.Period,
                    PriceEpisodeIdentifier = p.PriceEpisodeIdentifier,
                    SfaContributionPercentage = 0.9M,
                    Ukprn = learner.Ukprn,
                    LearningAim = new LearningAim
                    {
                        AgreedPrice = p.Amount,
                        FrameworkCode = 403,
                        PathwayCode = 2,
                        FundingLineType = "TEST",
                        ProgrammeType = 1,
                        Reference = "ZPROG001",
                        StandardCode = 0
                    },
                    CollectionPeriod = new NamedCalendarPeriod {Month = 8, Name = "R01", Year = 2017},
                    DeliveryPeriod = new CalendarPeriod {Month = 8, Year = 8}
                };

                MessageSession.Send(paymentEvent, options).ConfigureAwait(false);
            });
        }
    }
}