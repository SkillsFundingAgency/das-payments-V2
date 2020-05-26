using System;
using System.Data.SqlTypes;
using AutoMapper;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Mapping.FundingSource
{
    public class FundingSourceProfile : Profile
    {
        public FundingSourceProfile()
        {
            CreateMap<FundingSourcePaymentEvent, FundingSourceEventModel>()
                .Include<SfaCoInvestedFundingSourcePaymentEvent, FundingSourceEventModel>()
                .Include<EmployerCoInvestedFundingSourcePaymentEvent, FundingSourceEventModel>()
                .Include<SfaFullyFundedFundingSourcePaymentEvent, FundingSourceEventModel>()
                .Include<LevyFundingSourcePaymentEvent, FundingSourceEventModel>()
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(source => source.EventId))
                .ForMember(dest => dest.CollectionPeriod, opt => opt.MapFrom(source => source.CollectionPeriod))
                .ForMember(dest => dest.EventTime, opt => opt.MapFrom(source => source.EventTime))
                .ForMember(dest => dest.IlrSubmissionDateTime, opt => opt.MapFrom(source => source.IlrSubmissionDateTime))
                .ForMember(dest => dest.JobId, opt => opt.MapFrom(source => source.JobId))
                .ForMember(dest => dest.LearnerReferenceNumber, opt => opt.MapFrom(source => source.Learner.ReferenceNumber))
                .ForMember(dest => dest.LearnerUln, opt => opt.MapFrom(source => source.Learner.Uln))
                .ForMember(dest => dest.LearningAimPathwayCode, opt => opt.MapFrom(source => source.LearningAim.PathwayCode))
                .ForMember(dest => dest.LearningAimFrameworkCode, opt => opt.MapFrom(source => source.LearningAim.FrameworkCode))
                .ForMember(dest => dest.LearningAimFundingLineType, opt => opt.MapFrom(source => source.LearningAim.FundingLineType))
                .ForMember(dest => dest.LearningAimProgrammeType, opt => opt.MapFrom(source => source.LearningAim.ProgrammeType))
                .ForMember(dest => dest.LearningAimReference, opt => opt.MapFrom(source => source.LearningAim.Reference))
                .ForMember(dest => dest.LearningAimStandardCode, opt => opt.MapFrom(source => source.LearningAim.StandardCode))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(source => source.AmountDue))
                .ForMember(dest => dest.DeliveryPeriod, opt => opt.MapFrom(source => source.DeliveryPeriod))
                .ForMember(dest => dest.AgreementId, opt => opt.Ignore())
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(source => source.StartDate))
                .ForMember(dest => dest.PlannedEndDate, opt => opt.ResolveUsing(source => source.PlannedEndDate == DateTime.MinValue ? (DateTime?)SqlDateTime.MinValue : source.PlannedEndDate))
                .ForMember(dest => dest.ActualEndDate, opt => opt.ResolveUsing(source => source.ActualEndDate == DateTime.MinValue ? (DateTime?)SqlDateTime.MinValue : source.ActualEndDate))
                .ForMember(dest => dest.CompletionStatus, opt => opt.MapFrom(source => source.CompletionStatus))
                .ForMember(dest => dest.CompletionAmount, opt => opt.MapFrom(source => source.CompletionAmount))
                .ForMember(dest => dest.InstalmentAmount, opt => opt.MapFrom(source => source.InstalmentAmount))
                .ForMember(dest => dest.NumberOfInstalments, opt => opt.MapFrom(source => source.NumberOfInstalments))
                .ForMember(dest => dest.ApprenticeshipId, opt => opt.MapFrom(source => source.ApprenticeshipId))
                .ForMember(dest => dest.ApprenticeshipPriceEpisodeId, opt => opt.MapFrom(source => source.ApprenticeshipPriceEpisodeId))
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => source.TransactionType))
                .ForMember(dest => dest.FundingSource, opt => opt.MapFrom(source => source.FundingSourceType))
                .ForMember(dest => dest.RequiredPaymentEventId, opt => opt.MapFrom(source => source.RequiredPaymentEventId))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(source => source.ContractType))
                .ForMember(dest => dest.ApprenticeshipEmployerType, opt => opt.MapFrom(source => source.ApprenticeshipEmployerType))
                ;
            CreateMap<SfaCoInvestedFundingSourcePaymentEvent, FundingSourceEventModel>();
            CreateMap<EmployerCoInvestedFundingSourcePaymentEvent, FundingSourceEventModel>();
            CreateMap<SfaFullyFundedFundingSourcePaymentEvent, FundingSourceEventModel>();
            CreateMap<LevyFundingSourcePaymentEvent, FundingSourceEventModel>();
        }
    }
}