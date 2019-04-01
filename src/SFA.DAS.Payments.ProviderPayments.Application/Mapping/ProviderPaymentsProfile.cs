using AutoMapper;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.ProviderPayments.Model;
using System;

namespace SFA.DAS.Payments.ProviderPayments.Application.Mapping
{

    public class ProviderPaymentsProfile : Profile
    {
        public ProviderPaymentsProfile()
        {
            CreateMap<FundingSourcePaymentEvent, ProviderPaymentEventModel>()
                .Include<EmployerCoInvestedFundingSourcePaymentEvent, ProviderPaymentEventModel>()
                .Include<SfaCoInvestedFundingSourcePaymentEvent, ProviderPaymentEventModel>()
                .Include<SfaFullyFundedFundingSourcePaymentEvent, ProviderPaymentEventModel>()
                .Include<LevyFundingSourcePaymentEvent, ProviderPaymentEventModel>()
                .ForMember(dest => dest.EventId, opt => opt.ResolveUsing(src => Guid.NewGuid()))
                .ForMember(dest => dest.FundingSourceId, opt => opt.MapFrom(source => source.EventId))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CollectionPeriod, opt => opt.MapFrom(source => source.CollectionPeriod.Period))
                .ForMember(dest => dest.AcademicYear, opt => opt.MapFrom(source => source.CollectionPeriod.AcademicYear))
                .ForMember(dest => dest.DeliveryPeriod, opt => opt.MapFrom(source => source.DeliveryPeriod))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(source => source.AmountDue))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(source => source.ContractType))
                .ForMember(dest => dest.FundingSource, opt => opt.MapFrom(source => source.FundingSourceType))
                .ForMember(dest => dest.IlrSubmissionDateTime, opt => opt.MapFrom(source => source.IlrSubmissionDateTime))
                .ForMember(dest => dest.JobId, opt => opt.MapFrom(source => source.JobId))
                .ForMember(dest => dest.LearnerUln, opt => opt.MapFrom(source => source.Learner.Uln))
                .ForMember(dest => dest.LearnerReferenceNumber, opt => opt.MapFrom(source => source.Learner.ReferenceNumber))
                .ForMember(dest => dest.LearningAimFrameworkCode, opt => opt.MapFrom(source => source.LearningAim.FrameworkCode))
                .ForMember(dest => dest.LearningAimFundingLineType, opt => opt.MapFrom(source => source.LearningAim.FundingLineType))
                .ForMember(dest => dest.LearningAimPathwayCode, opt => opt.MapFrom(source => source.LearningAim.PathwayCode))
                .ForMember(dest => dest.LearningAimProgrammeType, opt => opt.MapFrom(source => source.LearningAim.ProgrammeType))
                .ForMember(dest => dest.LearningAimReference, opt => opt.MapFrom(source => source.LearningAim.Reference))
                .ForMember(dest => dest.LearningAimStandardCode, opt => opt.MapFrom(source => source.LearningAim.StandardCode))
                .ForMember(dest => dest.PriceEpisodeIdentifier, opt => opt.MapFrom(source => source.PriceEpisodeIdentifier))
                .ForMember(dest => dest.SfaContributionPercentage, opt => opt.MapFrom(source => source.SfaContributionPercentage))
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => source.TransactionType))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn))
                .ForMember(dest => dest.AgreementId, opt => opt.Ignore())
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(source => source.StartDate))
                .ForMember(dest => dest.PlannedEndDate, opt => opt.MapFrom(source => source.PlannedEndDate))
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(source => source.ActualEndDate))
                .ForMember(dest => dest.CompletionStatus, opt => opt.MapFrom(source => source.CompletionStatus))
                .ForMember(dest => dest.CompletionAmount, opt => opt.MapFrom(source => source.CompletionAmount))
                .ForMember(dest => dest.InstalmentAmount, opt => opt.MapFrom(source => source.InstalmentAmount))
                .ForMember(dest => dest.NumberOfInstalments, opt => opt.MapFrom(source => source.NumberOfInstalments));

            CreateMap<EmployerCoInvestedFundingSourcePaymentEvent, ProviderPaymentEventModel>();
            CreateMap<SfaCoInvestedFundingSourcePaymentEvent, ProviderPaymentEventModel>();
            CreateMap<SfaFullyFundedFundingSourcePaymentEvent, ProviderPaymentEventModel>();
            CreateMap<LevyFundingSourcePaymentEvent, ProviderPaymentEventModel>();

            CreateMap<PaymentModel, ProviderPaymentEvent>()
                .Include<PaymentModel, EmployerCoInvestedProviderPaymentEvent>()
                .Include<PaymentModel, SfaCoInvestedProviderPaymentEvent>()
                .Include<PaymentModel, SfaFullyFundedProviderPaymentEvent>()
                .Include<PaymentModel, LevyProviderPaymentEvent>()
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(source => source.EventId))
                .ForMember(dest => dest.EventTime, opt => opt.ResolveUsing(src => DateTimeOffset.UtcNow))
                .ForMember(dest => dest.CollectionPeriod, opt => opt.MapFrom(source => source.CollectionPeriod))
                .ForMember(dest => dest.DeliveryPeriod, opt => opt.MapFrom(source => source.DeliveryPeriod))
                .ForMember(dest => dest.AmountDue, opt => opt.MapFrom(source => source.Amount))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(source => source.ContractType))
                .ForMember(dest => dest.FundingSourceType, opt => opt.MapFrom(source => source.FundingSource))
                .ForMember(dest => dest.IlrSubmissionDateTime,
                    opt => opt.MapFrom(source => source.IlrSubmissionDateTime))
                .ForMember(dest => dest.JobId, opt => opt.MapFrom(source => source.JobId))
                .ForPath(dest => dest.Learner.Uln, opt => opt.MapFrom(source => source.LearnerUln))
                .ForPath(dest => dest.Learner.ReferenceNumber,
                    opt => opt.MapFrom(source => source.LearnerReferenceNumber))
                .ForPath(dest => dest.LearningAim.FrameworkCode,
                    opt => opt.MapFrom(source => source.LearningAimFrameworkCode))
                .ForPath(dest => dest.LearningAim.FundingLineType,
                    opt => opt.MapFrom(source => source.LearningAimFundingLineType))
                .ForPath(dest => dest.LearningAim.PathwayCode,
                    opt => opt.MapFrom(source => source.LearningAimPathwayCode))
                .ForPath(dest => dest.LearningAim.ProgrammeType,
                    opt => opt.MapFrom(source => source.LearningAimProgrammeType))
                .ForPath(dest => dest.LearningAim.Reference, opt => opt.MapFrom(source => source.LearningAimReference))
                .ForPath(dest => dest.LearningAim.StandardCode,
                    opt => opt.MapFrom(source => source.LearningAimStandardCode))
                .ForMember(dest => dest.PriceEpisodeIdentifier,
                    opt => opt.MapFrom(source => source.PriceEpisodeIdentifier))
                .ForMember(dest => dest.SfaContributionPercentage,
                    opt => opt.MapFrom(source => source.SfaContributionPercentage))
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => source.TransactionType))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(source => source.AccountId))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(source => source.StartDate))
                .ForMember(dest => dest.PlannedEndDate, opt => opt.MapFrom(source => source.PlannedEndDate))
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(source => source.ActualEndDate))
                .ForMember(dest => dest.CompletionStatus, opt => opt.MapFrom(source => source.CompletionStatus))
                .ForMember(dest => dest.CompletionAmount, opt => opt.MapFrom(source => source.CompletionAmount))
                .ForMember(dest => dest.InstalmentAmount, opt => opt.MapFrom(source => source.InstalmentAmount))
                .ForMember(dest => dest.NumberOfInstalments, opt => opt.MapFrom(source => source.NumberOfInstalments));

            CreateMap<PaymentModel, EmployerCoInvestedProviderPaymentEvent>();
            CreateMap<PaymentModel, SfaCoInvestedProviderPaymentEvent>();
            CreateMap<PaymentModel, SfaFullyFundedProviderPaymentEvent>();
            CreateMap<PaymentModel, LevyProviderPaymentEvent>();
                
            CreateMap<FundingSourcePaymentEvent, ProviderPaymentEvent>()
                .Include<EmployerCoInvestedFundingSourcePaymentEvent, EmployerCoInvestedProviderPaymentEvent>()
                .Include<SfaCoInvestedFundingSourcePaymentEvent, SfaCoInvestedProviderPaymentEvent>()
                .Include<SfaFullyFundedFundingSourcePaymentEvent, SfaFullyFundedProviderPaymentEvent>()
                .Include<LevyFundingSourcePaymentEvent, LevyProviderPaymentEvent>()
                .ForMember(dest => dest.EventId, opt => opt.Ignore());

            CreateMap<EmployerCoInvestedFundingSourcePaymentEvent, EmployerCoInvestedProviderPaymentEvent>();
            CreateMap<SfaCoInvestedFundingSourcePaymentEvent, SfaCoInvestedProviderPaymentEvent>();
            CreateMap<SfaFullyFundedFundingSourcePaymentEvent, SfaFullyFundedProviderPaymentEvent>();
            CreateMap<LevyFundingSourcePaymentEvent, LevyProviderPaymentEvent>();
        }
    }
}