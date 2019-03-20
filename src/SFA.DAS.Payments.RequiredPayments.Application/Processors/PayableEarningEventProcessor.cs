using System;
using AutoMapper;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public class PayableEarningEventProcessor : ApprenticeshipContractTypeEarningEventProcessor<CalculatedRequiredLevyAmount, PayableEarningEvent>, IPayableEarningEventProcessor
    {
        public PayableEarningEventProcessor(IPaymentKeyService paymentKeyService, IMapper mapper, IRequiredPaymentProcessor requiredPaymentProcessor)
            : base(paymentKeyService, mapper, requiredPaymentProcessor)
        {
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