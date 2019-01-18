using AutoMapper;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents
{
    public class EarningEventProfile : Profile
    {
        public EarningEventProfile()
        {
            CreateMap<EarningEvent, EarningEventModel>()
                .Include<ApprenticeshipContractType2EarningEvent, EarningEventModel>()
                .Include<FunctionalSkillEarningsEvent, EarningEventModel>()
                .MapCommon()
                .ForMember(dest => dest.ContractType, opt => opt.Ignore())
                .ForMember(dest => dest.AgreementId, opt => opt.Ignore())
                .ForMember(dest => dest.PriceEpisodes, opt => opt.ResolveUsing<EarningEventPriceEpisodeModelListResolver>())
                ;
            CreateMap<ApprenticeshipContractType2EarningEvent, EarningEventModel>()
                .ForMember(dest => dest.ContractType, opt => opt.UseValue(ContractType.Act2))
                .ForMember(dest => dest.Periods,opt => opt.ResolveUsing<ApprenticeshipContractTypeEarningPeriodResolver>())
                ;

            CreateMap<FunctionalSkillEarningsEvent, EarningEventModel>()
                .ForMember(dest => dest.ContractType, opt => opt.UseValue(ContractType.Act2))  //TODO: fix for ACT1 events
                .ForMember(dest => dest.Periods, opt => opt.ResolveUsing<FunctionalSkillEarningResolver>());

            CreateMap<PriceEpisode, EarningEventPriceEpisodeModel>()
                .ForMember(dest => dest.SfaContributionPercentage, opt => opt.Ignore())
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(source => source.ActualEndDate))
                .ForMember(dest => dest.Completed, opt => opt.MapFrom(source => source.Completed))
                .ForMember(dest => dest.CompletionAmount, opt => opt.MapFrom(source => source.CompletionAmount))
                .ForMember(dest => dest.EarningEventId, opt => opt.Ignore())
                .ForMember(dest => dest.InstalmentAmount, opt => opt.MapFrom(source => source.InstalmentAmount))
                .ForMember(dest => dest.NumberOfInstalments, opt => opt.MapFrom(source => source.NumberOfInstalments))
                .ForMember(dest => dest.PlannedEndDate, opt => opt.MapFrom(source => source.PlannedEndDate))
                .ForMember(dest => dest.PriceEpisodeIdentifier, opt => opt.MapFrom(source => source.Identifier))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(source => source.StartDate))
                .ForMember(dest => dest.TotalNegotiatedPrice1, opt => opt.MapFrom(source => source.TotalNegotiatedPrice1))
                .ForMember(dest => dest.TotalNegotiatedPrice2, opt => opt.MapFrom(source => source.TotalNegotiatedPrice2))
                .ForMember(dest => dest.TotalNegotiatedPrice3, opt => opt.MapFrom(source => source.TotalNegotiatedPrice3))
                .ForMember(dest => dest.TotalNegotiatedPrice4, opt => opt.MapFrom(source => source.TotalNegotiatedPrice4))
                ;
        }
    }
}