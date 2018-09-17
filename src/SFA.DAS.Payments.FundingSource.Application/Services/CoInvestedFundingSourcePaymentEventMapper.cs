using AutoMapper;
using blah.Events;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

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
            var coInvestedPaymentEvent = mapper.Map<CoInvestedFundingSourcePaymentEvent>(requiredPaymentsEvent);
           return  MapCommonCoInvestedPaymentEventData(payment, coInvestedPaymentEvent);
        }

        public CoInvestedFundingSourcePaymentEvent MapToCoInvestedPaymentEvent(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent, SfaCoInvestedPayment payment)
        {
            var coInvestedPaymentEvent = mapper.Map<SfaCoInvestedFundingSourcePaymentEvent>(requiredPaymentsEvent);
            return MapCommonCoInvestedPaymentEventData(payment, coInvestedPaymentEvent);
        }

        public CoInvestedFundingSourcePaymentEvent MapToCoInvestedPaymentEvent(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent, EmployerCoInvestedPayment payment)
        {
            var coInvestedPaymentEvent = mapper.Map<EmployerCoInvestedFundingSourcePaymentEvent>(requiredPaymentsEvent);
            return MapCommonCoInvestedPaymentEventData(payment, coInvestedPaymentEvent);
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