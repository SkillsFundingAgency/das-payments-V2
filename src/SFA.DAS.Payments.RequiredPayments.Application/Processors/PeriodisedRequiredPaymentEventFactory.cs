using System;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    // ReSharper disable once IdentifierTypo
    public class PeriodisedRequiredPaymentEventFactory: IPeriodisedRequiredPaymentEventFactory
    {
        public PeriodisedRequiredPaymentEvent Create(EarningType earningType, int transactionType)
        {
            switch (earningType)
            {
                case EarningType.CoInvested:
                    return new CalculatedRequiredCoInvestedAmount
                    {
                        OnProgrammeEarningType = (OnProgrammeEarningType) transactionType,
                    };
                case EarningType.Incentive:
                    return new CalculatedRequiredIncentiveAmount
                    {
                        Type = (IncentivePaymentType) transactionType,
                    };
                case EarningType.Levy:
                    return new CalculatedRequiredLevyAmount
                    {
                        OnProgrammeEarningType = (OnProgrammeEarningType) transactionType,
                    };
                default:
                    throw new InvalidOperationException($"Unknown earning type found: {earningType:G}. Cannot create the PeriodisedRequiredPaymentEvent.");
            }

        }
    }
}