using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public class PayableEarningEventProcessor : ApprenticeshipContractTypeEarningEventProcessor<PayableEarningEvent>, IPayableEarningEventProcessor
    {
        private readonly ICoInvestmentCalculationService coInvestmentCalculationService;
        public PayableEarningEventProcessor(
            IMapper mapper,
            IRequiredPaymentProcessor requiredPaymentProcessor,
            IHoldingBackCompletionPaymentService holdingBackCompletionPaymentService,
            IPaymentHistoryRepository paymentHistoryRepository,
            IApprenticeshipKeyProvider apprenticeshipKeyProvider,
            INegativeEarningService negativeEarningService,
            IPaymentLogger paymentLogger, IDuplicateEarningEventService duplicateEarningEventService,
            ICoInvestmentCalculationService coInvestmentCalculationService,
            ITelemetry telemetry
        ) : base(
            mapper,
            requiredPaymentProcessor,
            holdingBackCompletionPaymentService,
            paymentHistoryRepository,
            apprenticeshipKeyProvider,
            negativeEarningService,
            paymentLogger,
            duplicateEarningEventService,
            telemetry
        )
        {
            this.coInvestmentCalculationService = coInvestmentCalculationService;
        }

        protected override IReadOnlyCollection<(EarningPeriod period, int type)> GetPeriods(PayableEarningEvent earningEvent)
        {
            return coInvestmentCalculationService.ProcessPeriodsForRecalculation(earningEvent, base.GetPeriods(earningEvent));
        }

        protected override EarningType GetEarningType(int type)
        {
            if (Enum.IsDefined(typeof(OnProgrammeEarningType), type))
            {
                return EarningType.Levy;
            }

            return EarningType.Incentive;
        }
    }
}