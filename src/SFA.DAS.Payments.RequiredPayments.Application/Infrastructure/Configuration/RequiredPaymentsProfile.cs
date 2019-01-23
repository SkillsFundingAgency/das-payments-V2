using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoMapper;
using SFA.DAS.Payments.DataLocks.Messages;
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
                .ForMember(dest => dest.DeliveryPeriod, opt => opt.ResolveUsing(src => new CalendarPeriod(src.DeliveryPeriod)))
                .ForMember(dest => dest.CollectionPeriod, opt => opt.ResolveUsing(src => new CalendarPeriod(src.CollectionPeriod)));

            CreateMap<IEarningEvent, RequiredPaymentEvent>()
                .Include<IEarningEvent, ApprenticeshipContractTypeRequiredPaymentEvent>()
                .Include<PayableEarningEvent, ApprenticeshipContractType1RequiredPaymentEvent>()
                .ForMember(requiredPayment => requiredPayment.AmountDue, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.DeliveryPeriod, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.PriceEpisodeIdentifier, opt => opt.Ignore())
                ;

            CreateMap<IEarningEvent, ApprenticeshipContractTypeRequiredPaymentEvent>()
                .ForMember(requiredPayment => requiredPayment.OnProgrammeEarningType, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.SfaContributionPercentage, opt => opt.Ignore())
                ;

            CreateMap<PayableEarningEvent, ApprenticeshipContractType1RequiredPaymentEvent>()
                .ForMember(requiredPayment => requiredPayment.OnProgrammeEarningType, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.Learner, opt => opt.MapFrom(earning => earning.Learner.Clone()))
                .ForMember(requiredPayment => requiredPayment.LearningAim, opt => opt.MapFrom(earning => earning.LearningAim.Clone()))
                ;

            CreateMap<EarningPeriod, RequiredPaymentEvent>()
                .ForMember(requiredPayment => requiredPayment.AmountDue, opt => opt.MapFrom(period => period.Amount))
                .ForMember(requiredPayment => requiredPayment.PriceEpisodeIdentifier, opt => opt.MapFrom(period => period.PriceEpisodeIdentifier))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<PayableEarningEvent, ReadOnlyCollection<RequiredPaymentEvent>>()
                .ConvertUsing<PayableEarningEventConverter>();
        }
    }

    public class PayableEarningEventConverter : ITypeConverter<PayableEarningEvent, ReadOnlyCollection<RequiredPaymentEvent>>
    {
        public ReadOnlyCollection<RequiredPaymentEvent> Convert(PayableEarningEvent earningEvent, ReadOnlyCollection<RequiredPaymentEvent> destination, ResolutionContext context)
        {
            var result = new List<RequiredPaymentEvent>();

            foreach (var onProgrammeEarning in earningEvent.OnProgrammeEarnings)
            {
                foreach (var period in onProgrammeEarning.Periods)
                {
                    var requiredPaymentEvent = context.Mapper.Map<ApprenticeshipContractType1RequiredPaymentEvent>(earningEvent);
                    context.Mapper.Map(period, requiredPaymentEvent);
                    requiredPaymentEvent.DeliveryPeriod = new CalendarPeriod(earningEvent.CollectionYear, period.Period);
                    result.Add(requiredPaymentEvent);
                }
            }

            return result.AsReadOnly();
        }
    }
}
