using System;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    // ReSharper disable once IdentifierTypo
    public class PeriodisedRequiredPaymentEventFactory: IPeriodisedRequiredPaymentEventFactory
    {
        private readonly IPaymentLogger logger;

        public PeriodisedRequiredPaymentEventFactory(IPaymentLogger logger)
        {
            this.logger = logger?? throw new ArgumentNullException(nameof(logger));
        }

        public PeriodisedRequiredPaymentEvent Create(EarningType earningType, int transactionType)
        {
            switch (earningType)
            {
                case EarningType.CoInvested:
                {
                    var coinvestedEarningType = ParsePaymentType<OnProgrammeEarningType>(earningType, transactionType);
                    if (!coinvestedEarningType.HasValue)
                    {
                        return null;
                    }

                    return new CalculatedRequiredCoInvestedAmount
                    {
                        OnProgrammeEarningType = coinvestedEarningType.Value
                    };
                }
                case EarningType.Incentive:
                {
                    var incentivePaymentType = ParsePaymentType<IncentivePaymentType>(earningType, transactionType);
                    if (!incentivePaymentType.HasValue)
                    {
                        return null;
                    }

                    return new CalculatedRequiredIncentiveAmount
                    {
                        Type = incentivePaymentType.Value,
                    };
                }
                case EarningType.Levy:
                {
                    var levyPaymentType = ParsePaymentType<OnProgrammeEarningType>(earningType, transactionType);
                    if (!levyPaymentType.HasValue)
                    {
                        return null;
                    }

                    return new CalculatedRequiredLevyAmount
                    {
                        OnProgrammeEarningType = levyPaymentType.Value,
                    };
                }
                default:
                    throw new InvalidOperationException(
                        $"Unknown earning type found: {earningType:G}. Cannot create the PeriodisedRequiredPaymentEvent.");
            }
        }

        private T? ParsePaymentType<T>(EarningType earningType, int transactionType) where T: struct, IConvertible
        {
            Enum.TryParse<T>(transactionType.ToString(), out var paymentType);

            if (!Enum.IsDefined(typeof(T), paymentType))
            {
                // We should never in theory get to this point - the RefundRemovedLearningAimProcessor now passed in the correct
                // transaction type records from the payment cache so we should not have a crossover in transaction types and 
                // earning types.
                logger.LogError(
                    $"Invalid EarningType and TransactionType combination: EarningType: {earningType:G}, TransactionType: {transactionType}");
                return null;
            }

            return paymentType;
        }
    }
}