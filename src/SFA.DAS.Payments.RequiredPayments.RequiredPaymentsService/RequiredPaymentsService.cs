using System;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

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

        public async Task<RequiredPaymentEvent[]> HandleEarning(IEarningEvent earningEvent, CancellationToken cancellationToken)
        {
            var executionContext = _executionContextFactory.GetExecutionContext();
            executionContext.JobId = earningEvent.JobId;

            _paymentLogger.LogInfo($"Handling Earning for {earningEvent?.Ukprn} ");
            //TODO: use handler in application layer to process the earning event.
            return new [] {
                new RequiredPaymentEvent
                {
                    JobId = earningEvent.JobId, 
                    EventTime = DateTimeOffset.UtcNow
                }
            };
        }
    }
}