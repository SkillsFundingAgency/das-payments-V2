using SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Mapping
{
    using System;
    using AutoMapper;
    using Core.Data;
    using Payments.Tests.Core;
    using DCT.TestDataGenerator.Model;

    public class IlrProfile : Profile
    {
        public IlrProfile()
        {
            CreateMap<Training, LearnerRequest>()
                .ForMember(x => x.StartDate, opt => opt.MapFrom(src => src.StartDate.ToDate()))
                .ForMember(x => x.PlannedDurationInMonths, opt => opt.MapFrom(src => (src.PlannedDuration !=null) ? int.Parse(src.PlannedDuration.Split(' ')[0]) : (int?)null))
                .ForMember(x => x.TotalTrainingPrice, opt => opt.MapFrom(src => (int)src.TotalTrainingPrice))
                .ForMember(x => x.TotalTrainingPriceEffectiveDate, opt => opt.MapFrom(src=> (!string.IsNullOrWhiteSpace(src.TotalTrainingPriceEffectiveDate)) ? src.TotalTrainingPriceEffectiveDate.ToDate() : (DateTime?)null))
                .ForMember(x => x.TotalAssessmentPrice, opt => opt.MapFrom(src => (int)src.TotalAssessmentPrice))
                .ForMember(x => x.TotalAssessmentPriceEffectiveDate, opt => opt.MapFrom(src=> (!string.IsNullOrWhiteSpace(src.TotalAssessmentPriceEffectiveDate)) ? src.TotalAssessmentPriceEffectiveDate.ToDate() : (DateTime?)null))
                .ForMember(x => x.ActualDurationInMonths,
                    opt => opt.MapFrom(
                        src => (!string.IsNullOrWhiteSpace(src.ActualDuration)) ? int.Parse(src.ActualDuration.Split(' ')[0]) : (int?) null))
                .ForMember(x => x.CompletionStatus,
                    opt => opt.MapFrom(src =>
                        (CompletionStatus) Enum.Parse(typeof(CompletionStatus), src.CompletionStatus, true)))
                .ForMember(x => x.AimSequenceNumber, opt => opt.MapFrom(src => src.AimSequenceNumber))
                .ForMember(x => x.AimReferenceNumber, opt => opt.MapFrom(src => src.AimReference))
                .ForMember(x => x.FrameworkCode, opt => opt.MapFrom(src => src.FrameworkCode))
                .ForMember(x => x.PathwayCode, opt => opt.MapFrom(src => src.PathwayCode))
                .ForMember(x => x.ProgrammeType, opt => opt.MapFrom(src => src.ProgrammeType))
                .ForMember(x => x.SfaContributionPercentage,
                    opt => opt.MapFrom(src => src.SfaContributionPercentage));
        }
    }
}
