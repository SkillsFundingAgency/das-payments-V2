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
            CreateMap<FM36Learner, EarningEvent>()
                .Include<FM36Learner, ApprenticeshipContractTypeEarningsEvent>()
                .Include<FM36Learner, FunctionalSkillEarningsEvent>()
                .ForMember(destinationMember => destinationMember.CollectionYear, opt => opt.Ignore())
                .ForMember(destinationMember => destinationMember.EventTime, opt => opt.UseValue(DateTimeOffset.UtcNow))
                .ForMember(destinationMember => destinationMember.LearningAim, opt => opt.Ignore())
                .ForMember(destinationMember => destinationMember.Ukprn, opt => opt.Ignore())
                .ForMember(destinationMember => destinationMember.JobId, opt => opt.Ignore())
                .ForMember(destinationMember => destinationMember.Learner, opt => opt.ResolveUsing((l, ee) => Mapper.Map<FM36Learner, Learner>(l)));

            CreateMap<FM36Learner, ApprenticeshipContractTypeEarningsEvent>()
                .Include<FM36Learner, ApprenticeshipContractType1EarningEvent>()
                .Include<FM36Learner, ApprenticeshipContractType2EarningEvent>()
                .ForMember(destinationMember => destinationMember.IncentiveEarnings, opt => opt.Ignore())
                .ForMember(destinationMember => destinationMember.OnProgrammeEarnings, opt => opt.ResolveUsing<OnProgrammeEarningValueResolver>())
                .ForMember(destinationMember => destinationMember.SfaContributionPercentage, opt => opt.Ignore());

            CreateMap<FM36Learner, ApprenticeshipContractType1EarningEvent>()
                .ForMember(destinationMember => destinationMember.AgreementId, opt => opt.Ignore());

            CreateMap<FM36Learner, ApprenticeshipContractType2EarningEvent>();

            CreateMap<FM36Learner, FunctionalSkillEarningsEvent>()
                .ForMember(dest => dest.Earnings, opt => opt.Ignore());

            CreateMap<FM36Learner, Learner>()
                .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(source => source.LearnRefNumber))
                .ForMember(dest => dest.Ukprn, opt => opt.Ignore())
                .ForMember(dest => dest.Uln, opt => opt.Ignore());

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