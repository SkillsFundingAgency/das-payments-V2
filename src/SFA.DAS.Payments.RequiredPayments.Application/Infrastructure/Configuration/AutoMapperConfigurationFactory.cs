using AutoMapper;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration
{
    //TODO: should use automapper profiles instead
    public class AutoMapperConfigurationFactory
    {
        public static MapperConfiguration CreateMappingConfig()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PaymentEntity, Payment>()
                    .ForMember(dest => dest.DeliveryPeriod, opt => opt.ResolveUsing(src => new CalendarPeriod(src.DeliveryPeriod)))
                    .ForMember(dest => dest.CollectionPeriod, opt => opt.ResolveUsing(src => new CalendarPeriod(src.CollectionPeriod)));
            });
        }
    }
}
