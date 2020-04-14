using System;
using System.Linq;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class EarningsEventProfile : Profile
    {
        public EarningsEventProfile()
        {
            CreateMap<IntermediateLearningAim, EarningEvent>()
                .Include<IntermediateLearningAim, ApprenticeshipContractTypeEarningsEvent>()
                .Include<IntermediateLearningAim, FunctionalSkillEarningsEvent>()
                .Include<IntermediateLearningAim, Act1FunctionalSkillEarningsEvent>()
                .Include<IntermediateLearningAim, Act2FunctionalSkillEarningsEvent>()
                .ForMember(destinationMember => destinationMember.PriceEpisodes, opt => opt.MapFrom(source => source.PriceEpisodes))
                .ForMember(destinationMember => destinationMember.LearningAim, opt => opt.MapFrom(source => source.Aims))
                .ForMember(destinationMember => destinationMember.CollectionYear, opt => opt.MapFrom(source => source.AcademicYear))
                .ForMember(destinationMember => destinationMember.Ukprn, opt => opt.MapFrom(source => source.Ukprn))
                .ForMember(destinationMember => destinationMember.JobId, opt => opt.MapFrom(source => source.JobId))
                .ForMember(destinationMember => destinationMember.EventTime, opt => opt.Ignore())
                .ForMember(destinationMember => destinationMember.EventId, opt => opt.Ignore())
                .ForMember(dest => dest.CollectionPeriod, opt => opt.ResolveUsing(src => CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(src.AcademicYear, (byte)src.CollectionPeriod)))
                .ForMember(dest => dest.LearningAim, opt => opt.MapFrom(source => source))
                ;

            CreateMap<IntermediateLearningAim, ApprenticeshipContractTypeEarningsEvent>()
                .Include<IntermediateLearningAim, ApprenticeshipContractType1EarningEvent>()
                .Include<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>()
                .ForMember(destinationMember => destinationMember.OnProgrammeEarnings, opt => opt.ResolveUsing<OnProgrammeEarningValueResolver>())
                .ForMember(destinationMember => destinationMember.IncentiveEarnings, opt => opt.ResolveUsing<IncentiveEarningValueResolver>())
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Aims.Min(aim => aim.LearningDeliveryValues.LearnStartDate)))
                .Ignore(dest => dest.SfaContributionPercentage)
                ;


            CreateMap<IntermediateLearningAim, ApprenticeshipContractType1EarningEvent>()
                .ForMember(dest => dest.AgreementId, opt => opt.MapFrom(aim => aim.PriceEpisodes[0].PriceEpisodeValues.PriceEpisodeAgreeId))
                ;

            CreateMap<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>();

            CreateMap<IntermediateLearningAim, Act1FunctionalSkillEarningsEvent>();
            CreateMap<IntermediateLearningAim, Act2FunctionalSkillEarningsEvent>();

            CreateMap<IntermediateLearningAim, FunctionalSkillEarningsEvent>()
                .Include<IntermediateLearningAim, Act1FunctionalSkillEarningsEvent>()
                .Include<IntermediateLearningAim, Act2FunctionalSkillEarningsEvent>()
                .ForMember(destinationMember => destinationMember.Earnings, opt => opt.ResolveUsing<FunctionalSkillsEarningValueResolver>())
                .ForMember(destinationMember => destinationMember.StartDate, opt => opt.MapFrom(source => source.Aims.Min(aim => aim.LearningDeliveryValues.LearnStartDate)))
                .Ignore(x => x.ContractType)
                ;

            CreateMap<ApprenticeshipContractType1EarningEvent,
                    ApprenticeshipContractType1RedundancyEarningEvent>()
                .Ignore(x => x.EventId);
          
                
            CreateMap<ApprenticeshipContractType2EarningEvent,
                ApprenticeshipContractType2RedundancyEarningEvent>()
                .Ignore(x => x.EventId);


            CreateMap<Act1FunctionalSkillEarningsEvent, Act1RedundancyFunctionalSkillEarningsEvent>()
                .Ignore(x => x.EventId);


              CreateMap<Act2FunctionalSkillEarningsEvent, Act2RedundancyFunctionalSkillEarningsEvent>()
                  .Ignore(x => x.EventId);


            CreateMap<FunctionalSkillEarning, FunctionalSkillEarning>();
            CreateMap<OnProgrammeEarning, OnProgrammeEarning>();
            CreateMap<EarningPeriod, EarningPeriod>();

            CreateMap<FM36Learner, Learner>()
                .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(source => source.LearnRefNumber))
                .ForMember(dest => dest.Uln, opt => opt.MapFrom(source => source.ULN));

            CreateMap<IntermediateLearningAim, LearningAim>()
                .ForMember(dest => dest.PathwayCode, opt => opt.MapFrom(source => source.Aims.First().LearningDeliveryValues.PwayCode))
                .ForMember(dest => dest.FrameworkCode, opt => opt.MapFrom(source => source.Aims.First().LearningDeliveryValues.FworkCode))
                .Ignore(x => x.FundingLineType)
                .ForMember(dest => dest.ProgrammeType, opt => opt.MapFrom(source => source.Aims.First().LearningDeliveryValues.ProgType))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(source => source.Aims.First().LearningDeliveryValues.LearnAimRef))
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.Aims.First().LearningDeliveryValues.StdCode))
                .ForMember(dest => dest.SequenceNumber, opt => opt.MapFrom(source => source.Aims.First().AimSeqNumber))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(source => source.Aims.Min(aim => aim.LearningDeliveryValues.LearnStartDate)))
                ;

            CreateMap<IntermediateLearningAim, SubmittedLearnerAimModel>()
                .ForMember(model => model.LearnerReferenceNumber, opt => opt.MapFrom(aim => aim.Learner.LearnRefNumber))
                .ForMember(model => model.LearningAimFrameworkCode, opt => opt.MapFrom(aim => aim.Aims.First().LearningDeliveryValues.FworkCode))
                .ForMember(model => model.LearningAimPathwayCode, opt => opt.MapFrom(aim => aim.Aims.First().LearningDeliveryValues.PwayCode))
                .ForMember(model => model.LearningAimProgrammeType, opt => opt.MapFrom(aim => aim.Aims.First().LearningDeliveryValues.ProgType))
                .ForMember(model => model.LearningAimStandardCode, opt => opt.MapFrom(aim => aim.Aims.First().LearningDeliveryValues.StdCode))
                .ForMember(model => model.LearningAimReference, opt => opt.MapFrom(aim => aim.Aims.First().LearningDeliveryValues.LearnAimRef))
                .ForMember(model => model.ContractType, opt => opt.MapFrom(aim => aim.ContractType))
                .Ignore(model => model.Id)
                ;

            CreateMap<ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode, Model.Core.PriceEpisode>()
                .ForMember(dest => dest.Identifier, opt => opt.MapFrom(source => source.PriceEpisodeIdentifier))
                .ForMember(dest => dest.TotalNegotiatedPrice1, opt => opt.MapFrom(source => source.PriceEpisodeValues.TNP1))
                .ForMember(dest => dest.TotalNegotiatedPrice2, opt => opt.MapFrom(source => source.PriceEpisodeValues.TNP2))
                .ForMember(dest => dest.TotalNegotiatedPrice3, opt => opt.MapFrom(source => source.PriceEpisodeValues.TNP3))
                .ForMember(dest => dest.TotalNegotiatedPrice4, opt => opt.MapFrom(source => source.PriceEpisodeValues.TNP4))
                .ForMember(dest => dest.AgreedPrice, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodeTotalTNPPrice))
                .ForMember(dest => dest.CourseStartDate, opt => opt.MapFrom(source => source.PriceEpisodeValues.EpisodeStartDate))
                .ForMember(dest => dest.EffectiveTotalNegotiatedPriceStartDate, opt => opt.MapFrom(source => source.PriceEpisodeValues.EpisodeEffectiveTNPStartDate))
                .ForMember(dest => dest.PlannedEndDate, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodePlannedEndDate))
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodeActualEndDate))
                .ForMember(dest => dest.NumberOfInstalments, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodePlannedInstalments)) //TODO: should this be actual if there is an actual end date?
                .ForMember(dest => dest.InstalmentAmount, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodeInstalmentValue))
                .ForMember(dest => dest.CompletionAmount, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodeCompletionElement))
                .ForMember(dest => dest.Completed, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodeCompleted))
                .ForMember(dest => dest.EmployerContribution, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodeCumulativePMRs))
                .ForMember(dest => dest.CompletionHoldBackExemptionCode, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodeCompExemCode))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(source => source.PriceEpisodeValues.EpisodeStartDate))
                .ForMember(dest => dest.FundingLineType, opt => opt.MapFrom(source => source.PriceEpisodeValues.PriceEpisodeFundLineType))
                ;
        }
    }
}