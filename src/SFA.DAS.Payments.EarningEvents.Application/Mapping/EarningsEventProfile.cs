using System;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class EarningsEventProfile : Profile
    {
        public EarningsEventProfile()
        {
            CreateMap<IntermediateLearningAim, EarningEvent>()
                .Include<IntermediateLearningAim, ApprenticeshipContractTypeEarningsEvent>()
                .Include<IntermediateLearningAim, FunctionalSkillEarningsEvent>()
                .ForMember(destinationMember => destinationMember.PriceEpisodes, opt => opt.MapFrom(source => source.PriceEpisodes))
                .ForMember(destinationMember => destinationMember.LearningAim, opt => opt.MapFrom(source => source.Aim))
                .ForMember(destinationMember => destinationMember.CollectionYear, opt => opt.MapFrom(source => source.CollectionYear))
                .ForMember(destinationMember => destinationMember.EventTime, opt => opt.UseValue(DateTimeOffset.UtcNow))
                .ForMember(destinationMember => destinationMember.Ukprn, opt => opt.MapFrom(source => source.Ukprn))
                .ForMember(destinationMember => destinationMember.JobId, opt => opt.MapFrom(source => source.JobId))
                .ForMember(dest => dest.CollectionPeriod, opt => opt.ResolveUsing(src => new CalendarPeriod(src.CollectionYear, (byte)src.CollectionPeriod)))
                .ForMember(dest => dest.LearningAim, opt => opt.MapFrom(source => source))
                ;

            CreateMap<IntermediateLearningAim, ApprenticeshipContractTypeEarningsEvent>()
                .Include<IntermediateLearningAim, ApprenticeshipContractType1EarningEvent>()
                .Include<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>()
                .ForMember(destinationMember => destinationMember.OnProgrammeEarnings, opt => opt.ResolveUsing<OnProgrammeEarningValueResolver>())
                .ForMember(destinationMember => destinationMember.IncentiveEarnings, opt => opt.ResolveUsing<IncentiveEarningValueResolver>())
                .ForMember(destinationMember => destinationMember.SfaContributionPercentage, opt => opt.ResolveUsing((cmd, ev) => cmd.Learner.PriceEpisodes.GetLatestPriceEpisode()?.PriceEpisodeValues.PriceEpisodeSFAContribPct));

            CreateMap<IntermediateLearningAim, ApprenticeshipContractType1EarningEvent>()
                .ForMember(destinationMember => destinationMember.AgreementId, opt => opt.Ignore());

            CreateMap<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>();

            CreateMap<IntermediateLearningAim, FunctionalSkillEarningsEvent>()
                .ForMember(destinationMember => destinationMember.Earnings, opt => opt.ResolveUsing<FunctionalSkillsEarningValueResolver>());

            CreateMap<FM36Learner, Learner>()
                .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(source => source.LearnRefNumber))
                .ForMember(dest => dest.Uln, opt => opt.MapFrom(source => source.ULN));

            CreateMap<IntermediateLearningAim, LearningAim>()
                .ForMember(dest => dest.PathwayCode, opt => opt.MapFrom(source => source.Aim.LearningDeliveryValues.PwayCode))
                .ForMember(dest => dest.FrameworkCode, opt => opt.MapFrom(source => source.Aim.LearningDeliveryValues.FworkCode))
                .ForMember(dest => dest.FundingLineType, opt => opt.MapFrom(source => source.Aim.LearningDeliveryValues.LearnDelInitialFundLineType))
                .ForMember(dest => dest.ProgrammeType, opt => opt.MapFrom(source => source.Aim.LearningDeliveryValues.ProgType))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(source => source.Aim.LearningDeliveryValues.LearnAimRef))
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.Aim.LearningDeliveryValues.StdCode))
                ;

            CreateMap<ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode, Model.Core.PriceEpisode>()
                .ForMember(dest => dest.Identifier, opt => opt.MapFrom(source => source.PriceEpisodeIdentifier))
                .ForMember(dest => dest.TotalNegotiatedPrice1, opt => opt.MapFrom(source => source.PriceEpisodeValues.TNP1))
                .ForMember(dest => dest.TotalNegotiatedPrice2, opt => opt.MapFrom(source => source.PriceEpisodeValues.TNP2))
                .ForMember(dest => dest.TotalNegotiatedPrice3, opt => opt.MapFrom(source => source.PriceEpisodeValues.TNP3))
                .ForMember(dest => dest.TotalNegotiatedPrice4, opt => opt.MapFrom(source => source.PriceEpisodeValues.TNP4))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(source => source.PriceEpisodeValues.EpisodeStartDate))
                .ForMember(dest => dest.PlannedEndDate, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodePlannedEndDate))
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodeActualEndDate))
                .ForMember(dest => dest.NumberOfInstalments, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodePlannedInstalments)) //TODO: should this be actual if there is an actual end date?
                .ForMember(dest => dest.InstalmentAmount, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodeInstalmentValue))
                .ForMember(dest => dest.CompletionAmount, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodeCompletionElement))
                .ForMember(dest => dest.Completed, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodeCompleted));
        }
    }
}