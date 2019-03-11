using System;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public class ApprenticeshipContractType2EarningEventProcessor : ApprenticeshipContractTypeEarningEventProcessor<CalculatedRequiredCoInvestedAmount, ApprenticeshipContractType2EarningEvent>, IApprenticeshipContractType2EarningsEventProcessor
    {
        public ApprenticeshipContractType2EarningEventProcessor(IPaymentKeyService paymentKeyService, IMapper mapper,
            IRequiredPaymentService requiredPaymentService)
            : base(paymentKeyService, mapper, requiredPaymentService)
        {
        }

        protected override EarningType GetEarningType(int type)
        {
            if (Enum.IsDefined(typeof(OnProgrammeEarningType), type))
            {
                return EarningType.CoInvested;
            }

            return EarningType.Incentive;
        }
    }
}