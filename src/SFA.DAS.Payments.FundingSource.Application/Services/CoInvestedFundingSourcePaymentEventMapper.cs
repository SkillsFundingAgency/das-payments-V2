using System;
using AutoMapper;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.FundingSource.Model.Enum;


namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class CoInvestedFundingSourcePaymentEventMapper : ICoInvestedFundingSourcePaymentEventMapper
    {
        private readonly IMapper mapper;

        public CoInvestedFundingSourcePaymentEventMapper(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public CoInvestedFundingSourcePaymentEvent MapToCoInvestedPaymentEvent(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent, CoInvestedPayment payment)
        {

            switch (payment.Type)
            {
                case FundingSourceType.CoInvestedSfa:
                    return MapCommonCoInvestedPaymentEventData(payment, mapper.Map<SfaCoInvestedFundingSourcePaymentEvent>(requiredPaymentsEvent));
                case FundingSourceType.CoInvestedEmployer:
                    return MapCommonCoInvestedPaymentEventData(payment, mapper.Map<EmployerCoInvestedFundingSourcePaymentEvent>(requiredPaymentsEvent));
                default:
                    throw new NotImplementedException();
            }
        }

        public RequiredCoInvestedPayment MapToRequiredCoInvestedPayment(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent)
        {
            return mapper.Map<RequiredCoInvestedPayment>(requiredPaymentsEvent);
        }

        private static CoInvestedFundingSourcePaymentEvent MapCommonCoInvestedPaymentEventData(CoInvestedPayment payment, CoInvestedFundingSourcePaymentEvent coInvestedPaymentEvent)
        {
            coInvestedPaymentEvent.AmountDue = payment.AmountDue;
            return coInvestedPaymentEvent;
        }
    }
}