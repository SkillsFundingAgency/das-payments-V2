using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService
{
    [StatePersistence(StatePersistence.Volatile)]
    public class RequiredPaymentsService : Actor, IRequiredPaymentsService
    {
        private readonly IPaymentLogger _paymentLogger;
        private readonly IExecutionContextFactory _executionContextFactory;

        public RequiredPaymentsService(ActorService actorService,
                                        ActorId actorId,
                                        IPaymentLogger paymentLogger,
                                        IExecutionContextFactory executionContextFactory) : base(actorService, actorId)
        {
            _paymentLogger = paymentLogger;
            _executionContextFactory = executionContextFactory ?? throw new ArgumentNullException(nameof(executionContextFactory));
        }

        public async Task<ApprenticeshipContractType2RequiredPaymentEvent[]> HandleAct2Earning(ApprenticeshipContractType2EarningEvent earningEvent, CancellationToken cancellationToken)
        {
            var executionContext = _executionContextFactory.GetExecutionContext();
            ((ESFA.DC.Logging.ExecutionContext)executionContext).JobId = earningEvent.JobId;  //TODO: move to a message dispatcher

            _paymentLogger.LogInfo($"Handling Earning for {earningEvent?.Ukprn} ");
            //TODO: use handler in application layer to process the earning event.
            return earningEvent.OnProgrammeEarnings
                .SelectMany(earnings => earnings.Periods, (earning, period) => new {earning, period})
                .Select(earningPeriod => new ApprenticeshipContractType2RequiredPaymentEvent
                {
                    AmountDue = earningPeriod.period.Amount,
                    Amount = earningPeriod.period.Amount,
                    CollectionPeriod = new NamedCalendarPeriod
                    {
                        Month = (byte) DateTime.Today.Month,
                        Year = (short) DateTime.Today.Year
                    },
                    DeliveryPeriod = new CalendarPeriod
                    {
                        Month = earningPeriod.period.Period,
                        Year = earningEvent.EarningYear
                    },
                    EventTime = DateTimeOffset.UtcNow,
                    JobId = earningEvent.JobId,
                    Learner = earningEvent.Learner,
                    LearningAim = earningEvent.LearningAim,
                    OnProgrammeEarningType = earningPeriod.earning.Type,
                    PriceEpisodeIdentifier = earningPeriod.period.PriceEpisodeIdentifier,
                    SfaContributionPercentage = earningEvent.SfaContributionPercentage,
                    Ukprn = earningEvent.Ukprn,
                    Period = earningPeriod.period.Period
                })
                .ToArray();
        }
    }
}