using AutoMapper;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
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

        public CoInvestedFundingSourcePaymentEvent MapTo(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent, Payment payment)
        {
            var coInvestedPaymentEvent = mapper.Map<CoInvestedFundingSourcePaymentEvent>(requiredPaymentsEvent);
           
            switch (payment.Type)
            {
                case Domain.Enum.FundingSourceType.CoInvestedSfa:
                    coInvestedPaymentEvent = mapper.Map<SfaCoInvestedFundingSourcePaymentEvent>(coInvestedPaymentEvent);
                    break;
                case Domain.Enum.FundingSourceType.CoInvestedEmployer:
                    coInvestedPaymentEvent = mapper.Map<EmployerCoInvestedFundingSourcePaymentEvent>(coInvestedPaymentEvent);
                    break;
            }

            coInvestedPaymentEvent.AmountDue = payment.AmountDue;

            return coInvestedPaymentEvent;
        }

        public RequiredCoInvestedPayment MapFrom(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent)
        {

            return new RequiredCoInvestedPayment
            {
                AmountDue = requiredPaymentsEvent.AmountDue,
                SfaContributionPercentage = requiredPaymentsEvent.SfaContributionPercentage
            };
        }












    }



}