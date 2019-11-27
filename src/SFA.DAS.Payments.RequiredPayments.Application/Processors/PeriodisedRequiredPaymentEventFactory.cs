using System;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    // ReSharper disable once IdentifierTypo
    public class PeriodisedRequiredPaymentEventFactory : IPeriodisedRequiredPaymentEventFactory
    {
        private readonly IPaymentLogger logger;

        // ReSharper disable once IdentifierTypo
        public PeriodisedRequiredPaymentEventFactory(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public PeriodisedRequiredPaymentEvent Create(EarningType earningType, int transactionType)
        {
            bool IsValidPaymentType<T>() where T : struct, IConvertible
            {
                if (!Enum.IsDefined(typeof(T), transactionType))
                {
                    logger.LogError(
                        $"Invalid EarningType and TransactionType combination: EarningType: {earningType:G}, TransactionType: {transactionType}");
                    return false;
                }
                else
                {
                    return true;
                }
            }

            PeriodisedRequiredPaymentEvent paymentEvent = null;

            switch (earningType)
            {
                case EarningType.CoInvested:
                    if (IsValidPaymentType<OnProgrammeEarningType>())
                    {
                        paymentEvent = new CalculatedRequiredCoInvestedAmount
                        {
                            OnProgrammeEarningType = (OnProgrammeEarningType) transactionType,
                        };
                    }

                    break;
                case EarningType.Incentive:
                    if (IsValidPaymentType<IncentivePaymentType>())
                    {
                        paymentEvent = new CalculatedRequiredIncentiveAmount
                        {
                            Type = (IncentivePaymentType) transactionType,
                        };
                    }

                    break;

                case EarningType.Levy:
                    if (IsValidPaymentType<OnProgrammeEarningType>())
                    {
                        paymentEvent = new CalculatedRequiredLevyAmount
                        {
                            OnProgrammeEarningType = (OnProgrammeEarningType) transactionType,
                        };
                    }

                    break;
                default:
                    throw new InvalidOperationException(
                        $"Unknown earning type found: {earningType:G}. Cannot create the PeriodisedRequiredPaymentEvent.");
            }

            return paymentEvent;
        }
    }
}