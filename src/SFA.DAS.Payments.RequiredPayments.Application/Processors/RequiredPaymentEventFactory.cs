using System;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    // ReSharper disable IdentifierTypo
    public interface IRequiredPaymentEventFactory
    {
        PeriodisedRequiredPaymentEvent Create(EarningType earningType, TransactionType transactionType, decimal sfaContributionPercentage, decimal amount);
    }

    public class RequiredPaymentEventFactory : IRequiredPaymentEventFactory
    {
        private readonly IPaymentLogger logger;

        public RequiredPaymentEventFactory(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public PeriodisedRequiredPaymentEvent Create(EarningType earningType, TransactionType transactionType, decimal sfaContributionPercentage, decimal amount)
        {
            PeriodisedRequiredPaymentEvent paymentEvent = null;

            switch (earningType)
            {
                case EarningType.CoInvested:
                    if (IsValidPaymentType<OnProgrammeEarningType>(transactionType))
                    {
                        paymentEvent = new CalculatedRequiredCoInvestedAmount
                        {
                            OnProgrammeEarningType = (OnProgrammeEarningType)transactionType,
                            AmountDue = amount,
                        };
                    }

                    break;
                case EarningType.Incentive:
                    if (IsValidPaymentType<IncentivePaymentType>(transactionType))
                    {
                        paymentEvent = new CalculatedRequiredIncentiveAmount
                        {
                            Type = (IncentivePaymentType)transactionType,
                            AmountDue = amount,
                        };
                    }

                    break;

                case EarningType.Levy:
                    if (IsValidPaymentType<OnProgrammeEarningType>(transactionType))
                    {
                        paymentEvent = new CalculatedRequiredLevyAmount
                        {
                            OnProgrammeEarningType = (OnProgrammeEarningType)transactionType,
                            SfaContributionPercentage = sfaContributionPercentage,
                            AmountDue = amount,
                        };
                    }

                    break;
                default:
                    throw new InvalidOperationException($"Unknown earning type found: {earningType:G}. Cannot create the PeriodisedRequiredPaymentEvent.");
            }

            if (paymentEvent == null)
            {
                logger.LogError($"Invalid EarningType and TransactionType combination: EarningType: {earningType:G}, TransactionType: {transactionType}");
            }

            return paymentEvent;
        }

        private static bool IsValidPaymentType<T>(TransactionType transactionType) where T : struct, IConvertible
        {
            return Enum.IsDefined(typeof(T), transactionType);
        }
    }
}