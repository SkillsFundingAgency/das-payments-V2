using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
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
                cfg.CreateMap<PaymentEntity, Payment>();

                cfg.CreateMap<SFA.DAS.Payments.Model.Core.Learner, Learner>()
                    .ForMember(dst => dst.IsTemp, opt => opt.Ignore());

                cfg.CreateMap<PaymentDue, RequiredPaymentEvent>()
                    .ForMember(dst => dst.EventTime, opt => opt.Ignore())
                    .ForMember(dest =>dest.JobId, opt => opt.Ignore());

                cfg.CreateMap<EarningEvent, PayableEarning>()
                    .ForMember(dst => dst.Course, opt => opt.Ignore())
                    .AfterMap((src, dst) =>
                    {
                        dst.Course = new Course
                        {
                            //TODO: map course
                        };
                    });
            });
        }
    }
}
