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
            PeriodisedRequiredPaymentEvent paymentEvent = null;

            switch (earningType)
            {
                case EarningType.CoInvested:
                    if (IsValidPaymentType<OnProgrammeEarningType>(transactionType))
                    {
                        paymentEvent = new CalculatedRequiredCoInvestedAmount
                        {
                            OnProgrammeEarningType = (OnProgrammeEarningType) transactionType,
                        };
                    }

                    break;
                case EarningType.Incentive:
                    if (IsValidPaymentType<IncentivePaymentType>(transactionType))
                    {
                        paymentEvent = new CalculatedRequiredIncentiveAmount
                        {
                            Type = (IncentivePaymentType) transactionType,
                        };
                    }

                    break;

                case EarningType.Levy:
                    if (IsValidPaymentType<OnProgrammeEarningType>(transactionType))
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

            if (paymentEvent == null)
            {
                logger.LogError(
                    $"Invalid EarningType and TransactionType combination: EarningType: {earningType:G}, TransactionType: {transactionType}");
            }

            return paymentEvent;
        }

        private  bool IsValidPaymentType<T>(int transactionType) where T : struct, IConvertible
        {
            return Enum.IsDefined(typeof(T), transactionType);
        }
    }
}