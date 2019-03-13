using AutoMapper;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration
{
    public class RequiredPaymentsProfile : Profile
    {
        public RequiredPaymentsProfile()
        {
            CreateMap<PaymentHistoryEntity, Payment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DeliveryPeriod, opt => opt.ResolveUsing(src => src.DeliveryPeriod))
                .ForMember(dest => dest.CollectionPeriod, opt => opt.ResolveUsing(src => src.CollectionPeriod.Clone()));

            CreateMap<IEarningEvent, RequiredPaymentEvent>()
                .Include<PayableEarningEvent, CalculatedRequiredLevyAmount>()
                .Include<IEarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .Include<FunctionalSkillEarningsEvent, CalculatedRequiredIncentiveAmount>()
                .ForMember(requiredPayment => requiredPayment.EarningEventId, opt => opt.MapFrom(earning => earning.EventId))
                .ForMember(requiredPayment => requiredPayment.AmountDue, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.DeliveryPeriod, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.PriceEpisodeIdentifier, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.CollectionPeriod, opt => opt.MapFrom(earning => earning.CollectionPeriod.Clone()))
                .ForMember(requiredPayment => requiredPayment.Learner, opt => opt.MapFrom(earning => earning.Learner.Clone()))
                .ForMember(requiredPayment => requiredPayment.LearningAim, opt => opt.MapFrom(earning => earning.LearningAim.Clone()))
                .ForMember(requiredPayment => requiredPayment.EventId, opt => opt.Ignore())
                ;

            CreateMap<IEarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .Include<PayableEarningEvent, CalculatedRequiredLevyAmount>()
                .ForMember(requiredPayment => requiredPayment.OnProgrammeEarningType, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.SfaContributionPercentage, opt => opt.Ignore());

            CreateMap<FunctionalSkillEarningsEvent, CalculatedRequiredIncentiveAmount>()
                .ForMember(requiredPayment => requiredPayment.Type, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.ContractType, opt => opt.Ignore());

            CreateMap<PayableEarningEvent, CalculatedRequiredLevyAmount>()
                .ForMember(requiredPayment => requiredPayment.OnProgrammeEarningType, opt => opt.Ignore());

            CreateMap<EarningPeriod, RequiredPaymentEvent>()
                .ForMember(requiredPayment => requiredPayment.AmountDue, opt => opt.MapFrom(period => period.Amount))
                .ForMember(requiredPayment => requiredPayment.PriceEpisodeIdentifier, opt => opt.MapFrom(period => period.PriceEpisodeIdentifier))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
