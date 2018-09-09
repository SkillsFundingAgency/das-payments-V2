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
            coInvestedPaymentEvent.AmountDue = payment.AmountDue;

            switch (payment.Type)
            {
                case Domain.Enum.FundingSourceType.CoInvestedSfa:
                  return  mapper.Map<SfaCoInvestedFundingSourcePaymentEvent>(coInvestedPaymentEvent);
                case Domain.Enum.FundingSourceType.CoInvestedEmployer:
                    return mapper.Map<EmployerCoInvestedFundingSourcePaymentEvent>(coInvestedPaymentEvent);
            }

            return coInvestedPaymentEvent;
        }

        public CoInvestedPayment MapFrom(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent)
        {

            return new CoInvestedPayment
            {
                AmountDue = requiredPaymentsEvent.AmountDue,
                SfaContributionPercentage = requiredPaymentsEvent.SfaContributionPercentage
            };
        }












    }



}