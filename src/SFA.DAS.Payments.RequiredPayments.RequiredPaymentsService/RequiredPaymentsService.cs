using System;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Application;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService
{
    [StatePersistence(StatePersistence.Volatile)]
    public class RequiredPaymentsService : Actor, IRequiredPaymentsService
    {
        private readonly IPaymentLogger _paymentLogger;
        private readonly IExecutionContextFactory _executionContextFactory;
        private readonly IPaymentDueEventHanlder _paymentDueEventHanlder;

        public RequiredPaymentsService(ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger,
            IExecutionContextFactory executionContextFactory,
            IPaymentDueEventHanlder paymentDueEventHanlder) : base(actorService, actorId)
        {
            _paymentLogger = paymentLogger;
            _executionContextFactory = executionContextFactory ?? throw new ArgumentNullException(nameof(executionContextFactory));
            _paymentDueEventHanlder = paymentDueEventHanlder;
        }

        public async Task<RequiredPaymentEvent> HandleEarning(PaymentDueEvent paymentDueEvent, CancellationToken cancellationToken)
        {
            var executionContext = _executionContextFactory.GetExecutionContext();
            executionContext.JobId = paymentDueEvent.JobId;

            _paymentLogger.LogInfo($"Handling PaymentDue for {paymentDueEvent.Ukprn} ");

            var requiredPaymentEvents = await _paymentDueEventHanlder.HandlePaymentDue(paymentDueEvent, cancellationToken).ConfigureAwait(false);

            return requiredPaymentEvents;
        }

        // TODO: initialise actor and populate payment history cache
    }
}