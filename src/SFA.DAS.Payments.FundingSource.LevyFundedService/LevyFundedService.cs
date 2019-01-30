using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.LevyFundedService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class LevyFundedService : Actor, ILevyFundedService
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly ITelemetry telemetry;
        private readonly IContractType1RequiredPaymentEventFundingSourceService fundingSourceService;

        public LevyFundedService(
            ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger,
            ITelemetry telemetry, 
            IContractType1RequiredPaymentEventFundingSourceService fundingSourceService) 
            : base(actorService, actorId)
        {
            this.paymentLogger = paymentLogger;
            this.telemetry = telemetry;
            this.fundingSourceService = fundingSourceService;
        }

        public async Task HandleRequiredPayment(ApprenticeshipContractType1RequiredPaymentEvent message)
        {
            paymentLogger.LogVerbose($"Handling RequiredPayment for {Id}");

            using (var operation = telemetry.StartOperation())
            {
                await fundingSourceService.RegisterRequiredPayment(message).ConfigureAwait(false);
                telemetry.StopOperation(operation);
            }
        }

        public async Task<IReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(ProcessLevyPaymentsOnMonthEndCommand command)
        {
            paymentLogger.LogVerbose($"Handling ProcessLevyPaymentsOnMonthEndCommand for {Id}");

            using (var operation = telemetry.StartOperation())
            {
                var fundingSourceEvents = await fundingSourceService.GetFundedPayments();
                telemetry.StopOperation(operation);
                return fundingSourceEvents;
            }
        }
    }
}
