using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class RequiredPaymentsInputSteps : StepsBase
    {
        private readonly ScenarioContext context;

        public RequiredPaymentsInputSteps(ScenarioContext context) : base(context) { }

        [When(@"an earning event is received")]
        public async void WhenAnEarningEventIsReceived()
        {

            // Get all the input data
            var processingPeriod = (short)context["ProcessingPeriod"];

            var learner = Get<Payments.AcceptanceTests.Core.Data.Learner>();

            IEnumerable<Course> courses = null;

            if (context.ContainsKey("Courses"))
            {
                courses = context["Courses"] as IEnumerable<Course>;
            }

            var course = courses?.FirstOrDefault();

            var learningAim = course.AsLearningAim();

            var learningEarning = CreateEarning("ContractType2OnProgrammeEarningsLearning", OnProgrammeEarningType.Learning, e => e.Learning_1);
            var completionEarning = CreateEarning("ContractType2OnProgrammeEarningsCompletion", OnProgrammeEarningType.Completion, e => e.Completion_2);

            var onProgrammeEarnings = new List<OnProgrammeEarning>();

            if (learningEarning != null)
            {
                onProgrammeEarnings.Add(learningEarning);
            }

            if (completionEarning != null)
            {
                onProgrammeEarnings.Add(completionEarning);
            }

            var jobId = $"job-{Guid.NewGuid():N}";

            var payments = onProgrammeEarnings
                .SelectMany(earning => earning.Periods, (earning, period) => new {earning, period})
                .Select(earningPeriod => new ApprenticeshipContractType2PaymentDueEvent
                    {
                        JobId = jobId,
                        Ukprn = learner.Ukprn,
                        EventTime = DateTimeOffset.UtcNow,
                        AmountDue = earningPeriod.period.Amount,
                        LearningAim = learningAim,
                        PriceEpisodeIdentifier = "p-1",
                        Learner = new Learner
                        {
                            ReferenceNumber = learner.GeneratedLearnRefNumber,
                            Uln = learner.Uln
                        },
                        SfaContributionPercentage = 0.9m,
                        Type = earningPeriod.earning.Type,
                        DeliveryPeriod = new CalendarPeriod
                        {
                            Period = earningPeriod.period.Period,
                            Month = (byte)(earningPeriod.period.Period >= 5 ? earningPeriod.period.Period - 4 : earningPeriod.period.Period + 8), 
                            Year = (short) DateTime.Today.Year,
                            Name = $"{CollectionYear}-R{earningPeriod.period.Period}"
                        },
                        CollectionPeriod = new CalendarPeriod
                        {
                            Period = earningPeriod.period.Period,
                            Month = (byte)(earningPeriod.period.Period >= 5 ? earningPeriod.period.Period - 4 : earningPeriod.period.Period + 8),  //TODO: change to current month???
                            Year = (short) DateTime.Today.Year,
                            Name = $"{CollectionYear}-R{earningPeriod.period.Period}"
                        }
                    })
                .ToList();

            var options = new SendOptions();
            options.RequireImmediateDispatch();
            foreach (var paymentEvent in payments)
            {
                await MessageSession.Send(paymentEvent, options).ConfigureAwait(false);
            }
            
        }

        private decimal GetAgreedPrice(string storageName)
        {
            if (context.ContainsKey(storageName))
            {
                if (context[storageName] is ContractTypeEarning earning)
                {
                    return earning.TotalNegotiatedPrice;
                }
            }

            return decimal.Zero;
        }

        private string GetPriceEpisodeIdentifier(string storageName)
        {
            if (context.ContainsKey(storageName))
            {
                if (context[storageName] is ContractTypeEarning earning)
                {
                    return earning.PriceEpisodeIdentifier;
                }
            }

            return string.Empty;
        }

        private OnProgrammeEarning CreateEarning(string storageName, OnProgrammeEarningType earningType, Func<ContractTypeEarning, decimal?> amount)
        {
            OnProgrammeEarning result = null;
            if (context.ContainsKey(storageName))
            {
                if (context[storageName] is ContractTypeEarning earning && amount(earning).HasValue)
                {
                    var learningAmount = amount(earning).Value;
                    var earningPeriods = new List<EarningPeriod>();

                    for (var period = earning.FromPeriod; period <= earning.ToPeriod; period++)
                    {
                        earningPeriods.Add(new EarningPeriod { Period = period, Amount = learningAmount });
                    }

                    var learningEarning = new OnProgrammeEarning
                    { Type = earningType, Periods = earningPeriods.AsReadOnly() };

                    result = learningEarning;
                }
            }

            return result;
        }
    }
}