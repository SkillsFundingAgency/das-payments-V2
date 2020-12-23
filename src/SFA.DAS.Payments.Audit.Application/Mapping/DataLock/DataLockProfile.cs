using AutoMapper;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Mapping.DataLock
{
    public class DataLockProfile : Profile
    {
        public DataLockProfile()
        {
            CreateMap<DataLockEvent, DataLockEventModel>()
                .Include<PayableEarningEvent, DataLockEventModel>()
                .Include<EarningFailedDataLockMatching, DataLockEventModel>()
                .MapCommon()
                .ForMember(dest => dest.ContractType, opt => opt.Ignore())
                .ForMember(dest => dest.AgreementId, opt => opt.MapFrom(source => source.AgreementId))
                .ForMember(dest => dest.PriceEpisodes, opt => opt.ResolveUsing<DataLockEventPriceEpisodeModelListResolver>())
                .ForMember(dest => dest.LearningAimSequenceNumber, opt => opt.MapFrom(x => x.LearningAim.SequenceNumber))
                .ForMember(dest => dest.LearningStartDate, opt => opt.MapFrom(src => src.LearningAim.StartDate))
                .ForMember(dest => dest.IlrFileName, opt => opt.MapFrom(x => x.IlrFileName))
                .ForMember(dest => dest.EventType, opt => opt.MapFrom(x => x.GetType().FullName))
                ;

            CreateMap<PayableEarningEvent, DataLockEventModel>()
                .ForMember(x => x.IsPayable, opt => opt.UseValue(true))
                .ForMember(dest => dest.PayablePeriods, opt => opt.ResolveUsing<PayablePeriodResolver>())
                ;

            CreateMap<EarningFailedDataLockMatching, DataLockEventModel>()
                .ForMember(x => x.IsPayable, opt => opt.UseValue(false))
                .ForMember(dest => dest.NonPayablePeriods, opt => opt.ResolveUsing<NonPayablePeriodResolver>())
                ;

            CreateMap<FunctionalSkillDataLockEvent, DataLockEventModel>()
                .Include<PayableFunctionalSkillEarningEvent, DataLockEventModel>()
                .Include<FunctionalSkillEarningFailedDataLockMatching, DataLockEventModel>()
                .MapCommon()
                .ForMember(dest => dest.ContractType, opt => opt.Ignore())
                .ForMember(dest => dest.AgreementId, opt => opt.MapFrom(source => source.AgreementId))
                .ForMember(dest => dest.PriceEpisodes, opt => opt.ResolveUsing<DataLockEventPriceEpisodeModelListResolver>())
                .ForMember(dest => dest.LearningAimSequenceNumber, opt => opt.MapFrom(x => x.LearningAim.SequenceNumber))
                .ForMember(dest => dest.LearningStartDate, opt => opt.MapFrom(src => src.LearningAim.StartDate))
                .ForMember(dest => dest.IlrFileName, opt => opt.MapFrom(x => x.IlrFileName))
                .ForMember(dest => dest.EventType, opt => opt.MapFrom(x => x.GetType().FullName))
                ;
            CreateMap<PayableFunctionalSkillEarningEvent, DataLockEventModel>()
                .ForMember(dest => dest.PayablePeriods, opt => opt.ResolveUsing<FunctionalSkillEarningsPayablePeriodsResolver>())
                .ForMember(x => x.IsPayable, opt => opt.UseValue(true));

            CreateMap<FunctionalSkillEarningFailedDataLockMatching, DataLockEventModel>()
                .ForMember(x => x.IsPayable, opt => opt.UseValue(false))
                .ForMember(dest => dest.NonPayablePeriods, opt => opt.ResolveUsing<FunctionalSkillEarningsNonPayablePeriodsResolver>())
                ;


            CreateMap<PriceEpisode, DataLockEventPriceEpisodeModel>()
                .ForMember(dest => dest.SfaContributionPercentage, opt => opt.Ignore())
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(source => source.ActualEndDate))
                .ForMember(dest => dest.Completed, opt => opt.MapFrom(source => source.Completed))
                .ForMember(dest => dest.CompletionAmount, opt => opt.MapFrom(source => source.CompletionAmount))
                .ForMember(dest => dest.DataLockEventId, opt => opt.Ignore())
                .ForMember(dest => dest.InstalmentAmount, opt => opt.MapFrom(source => source.InstalmentAmount))
                .ForMember(dest => dest.NumberOfInstalments, opt => opt.MapFrom(source => source.NumberOfInstalments))
                .ForMember(dest => dest.PlannedEndDate, opt => opt.MapFrom(source => source.PlannedEndDate))
                .ForMember(dest => dest.PriceEpisodeIdentifier, opt => opt.MapFrom(source => source.Identifier))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(source => source.EffectiveTotalNegotiatedPriceStartDate))
                .ForMember(dest => dest.TotalNegotiatedPrice1, opt => opt.MapFrom(source => source.TotalNegotiatedPrice1))
                .ForMember(dest => dest.TotalNegotiatedPrice2, opt => opt.MapFrom(source => source.TotalNegotiatedPrice2))
                .ForMember(dest => dest.TotalNegotiatedPrice3, opt => opt.MapFrom(source => source.TotalNegotiatedPrice3))
                .ForMember(dest => dest.TotalNegotiatedPrice4, opt => opt.MapFrom(source => source.TotalNegotiatedPrice4))
                ;

            CreateMap<DataLockEventModel, DataLockEventModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<DataLockEventPriceEpisodeModel, DataLockEventPriceEpisodeModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<DataLockEventPayablePeriodModel, DataLockEventPayablePeriodModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<DataLockEventNonPayablePeriodModel, DataLockEventNonPayablePeriodModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            
            CreateMap<DataLockEventNonPayablePeriodFailureModel, DataLockEventNonPayablePeriodFailureModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
