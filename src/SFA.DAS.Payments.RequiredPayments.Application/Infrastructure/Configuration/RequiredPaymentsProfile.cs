using AutoMapper;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
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
        }
    }
}
