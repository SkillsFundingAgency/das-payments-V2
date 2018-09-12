using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
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

                //cfg.CreateMap<SFA.DAS.Payments.Model.Core.Learner, Learner>()
                //    .ForMember(dst => dst.IsTemp, opt => opt.Ignore())
                //    .ForMember(dst => dst.LearnerReferenceNumber, opt => opt.MapFrom(src => src.ReferenceNumber));

                //cfg.CreateMap<RequiredPayment, RequiredPaymentEvent>()
                //    //.ForMember(dst => dst.Learner, opt => opt.MapFrom(src => src.PaymentDue.Learner))
                //    //.ForMember(dst => dst.Ukprn, opt => opt.MapFrom(src => src.PaymentDue.Ukprn))
                //    //.ForMember(dst => dst.LearningAim, opt => opt.MapFrom(src => src.PaymentDue.LearningAim))
                //    // TODO: map these properly when relevant props added to source
                //    //.ForMember(dst => dst.Period, opt => opt.Ignore())
                //    .ForMember(dst => dst.EventTime, opt => opt.Ignore())
                //    .ForMember(dst => dst.JobId, opt => opt.Ignore())
                //    .ForMember(dst => dst.Amount, opt => opt.Ignore())
                //    //.ForMember(dst => dst.AmountDue, opt => opt.Ignore())
                //    .ForMember(dst => dst.CollectionPeriod, opt => opt.Ignore())
                //    .ForMember(dst => dst.DeliveryPeriod, opt => opt.Ignore())
                //    .ForMember(dst => dst.PriceEpisodeIdentifier, opt => opt.Ignore());

                //cfg.CreateMap<EarningEvent, PaymentDue>()
                //    .ForMember(dst => dst.LearningAim, opt => opt.Ignore())
                //    .AfterMap((src, dst) =>
                //    {
                //        dst.LearningAim = new LearningAim
                //        {
                //            //TODO: map course
                //        };
                //    });
            });
        }
    }
}
