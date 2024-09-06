using System;
using AutoMapper;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration
{
    public static class AutoMapperConfigurationFactory
    {
        public static MapperConfiguration CreateMappingConfig()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PeriodisedRequiredPaymentEvent, FundingSourcePaymentEvent>()
                    .Include<CalculatedRequiredOnProgrammeAmount, FundingSourcePaymentEvent>()
                    .Include<CalculatedRequiredIncentiveAmount, SfaFullyFundedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.EventId, opt => opt.Ignore())
                    .ForMember(dest => dest.EventTime, opt => opt.Ignore())
                    .ForMember(dest => dest.RequiredPaymentEventId, opt => opt.MapFrom(source => source.EventId))
                    .ForMember(dest => dest.EarningEventId, opt => opt.MapFrom(source => source.EarningEventId))
                    .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                    .ForMember(dest => dest.PlannedEndDate, opt => opt.MapFrom(src => src.PlannedEndDate))
                    .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(src => src.ActualEndDate))
                    .ForMember(dest => dest.CompletionStatus, opt => opt.MapFrom(src => src.CompletionStatus))
                    .ForMember(dest => dest.CompletionAmount, opt => opt.MapFrom(src => src.CompletionAmount))
                    .ForMember(dest => dest.InstalmentAmount, opt => opt.MapFrom(src => src.InstalmentAmount))
                    .ForMember(dest => dest.NumberOfInstalments, opt => opt.MapFrom(src => src.NumberOfInstalments))
                    .ForMember(dest => dest.ApprenticeshipId, opt => opt.MapFrom(src => src.ApprenticeshipId))
                    .ForMember(dest => dest.ApprenticeshipPriceEpisodeId,                         opt => opt.MapFrom(src => src.ApprenticeshipPriceEpisodeId))
                    .ForMember(dest => dest.FundingPlatformType, opt => opt.UseValue(FundingPlatformType.SubmitLearnerData));  //For now the default mapping is Submit Learner, this may change if/when Flexi do incentives, redundancy and refunds

                cfg.CreateMap<CalculatedRequiredLevyAmount, EmployerCoInvestedFundingSourcePaymentEvent>();
                cfg.CreateMap<CalculatedRequiredLevyAmount, SfaCoInvestedFundingSourcePaymentEvent>();
                cfg.CreateMap<CalculatedRequiredLevyAmount, SfaFullyFundedFundingSourcePaymentEvent>();

                cfg.CreateMap<CalculatedRequiredLevyAmount, LevyFundingSourcePaymentEvent>()
                    .ForMember(fundingSourcePaymentEvent => fundingSourcePaymentEvent.AgreementId, opt => opt.MapFrom(requiredPayment => requiredPayment.AgreementId))
                    .ForMember(dest => dest.AgeAtStartOfLearning, opt => opt.MapFrom(source => source.AgeAtStartOfLearning))
                    .ForMember(dest => dest.FundingPlatformType, opt => opt.MapFrom(source => source.FundingPlatformType));

                cfg.CreateMap<CalculatedRequiredLevyAmount, TransferFundingSourcePaymentEvent>()
                    .ForMember(fundingSourcePaymentEvent => fundingSourcePaymentEvent.AgreementId, opt => opt.MapFrom(requiredPayment => requiredPayment.AgreementId))
                    .ForMember(dest => dest.AgeAtStartOfLearning, opt => opt.MapFrom(source => source.AgeAtStartOfLearning))
                    .ForMember(dest => dest.FundingPlatformType, opt => opt.MapFrom(source => source.FundingPlatformType));

                cfg.CreateMap<CalculatedRequiredLevyAmount, FundingSourcePaymentEvent>()
                    .Include<CalculatedRequiredLevyAmount, LevyFundingSourcePaymentEvent>()
                    .Include<CalculatedRequiredLevyAmount, TransferFundingSourcePaymentEvent>()
                    .Include<CalculatedRequiredLevyAmount, EmployerCoInvestedFundingSourcePaymentEvent>()
                    .Include<CalculatedRequiredLevyAmount, SfaCoInvestedFundingSourcePaymentEvent>()
                    .Include<CalculatedRequiredLevyAmount, SfaFullyFundedFundingSourcePaymentEvent>()
                    .ForMember(fundingSourcePaymentEvent => fundingSourcePaymentEvent.AmountDue, opt => opt.Ignore())
                    .ForMember(fundingSourcePaymentEvent => fundingSourcePaymentEvent.TransactionType, opt => opt.MapFrom(source => (TransactionType)source.OnProgrammeEarningType))
                    .ForMember(fundingSourcePaymentEvent => fundingSourcePaymentEvent.AgeAtStartOfLearning, opt => opt.MapFrom(source => source.AgeAtStartOfLearning))
                    .ForMember(fundingSourcePaymentEvent => fundingSourcePaymentEvent.FundingPlatformType, opt => opt.MapFrom(source => source.FundingPlatformType));

                cfg.CreateMap<CalculatedRequiredCoInvestedAmount, EmployerCoInvestedFundingSourcePaymentEvent>();
                cfg.CreateMap<CalculatedRequiredCoInvestedAmount, SfaCoInvestedFundingSourcePaymentEvent>();
                cfg.CreateMap<CalculatedRequiredCoInvestedAmount, SfaFullyFundedFundingSourcePaymentEvent>();

                cfg.CreateMap<CalculatedRequiredOnProgrammeAmount, EmployerCoInvestedFundingSourcePaymentEvent>()
                    .Include<CalculatedRequiredCoInvestedAmount, EmployerCoInvestedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.FundingSourceType, opt => opt.UseValue(FundingSourceType.CoInvestedEmployer))
                    .ForMember(dest => dest.AgeAtStartOfLearning, opt => opt.MapFrom(source => source.AgeAtStartOfLearning))
                    //.ForMember(dest => dest.FundingPlatformType, opt => opt.Ignore())
                    ;

                cfg.CreateMap<CalculatedRequiredOnProgrammeAmount, SfaCoInvestedFundingSourcePaymentEvent>()
                    .Include<CalculatedRequiredCoInvestedAmount, SfaCoInvestedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.FundingSourceType, opt => opt.UseValue(FundingSourceType.CoInvestedSfa))
                    .ForMember(dest => dest.AgeAtStartOfLearning, opt => opt.MapFrom(source => source.AgeAtStartOfLearning))
                    //.ForMember(dest => dest.FundingPlatformType, opt => opt.Ignore())
                    ;

                cfg.CreateMap<CalculatedRequiredOnProgrammeAmount, SfaFullyFundedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.FundingSourceType, opt => opt.UseValue(FundingSourceType.FullyFundedSfa))
                    .ForMember(dest => dest.AgeAtStartOfLearning, opt => opt.MapFrom(source => source.AgeAtStartOfLearning))
                    //.ForMember(dest => dest.FundingPlatformType, opt => opt.Ignore())
                    ;

                cfg.CreateMap<CalculatedRequiredCoInvestedAmount, FundingSourcePaymentEvent>()
                    .Include<CalculatedRequiredCoInvestedAmount, EmployerCoInvestedFundingSourcePaymentEvent>()
                    .Include<CalculatedRequiredCoInvestedAmount, SfaCoInvestedFundingSourcePaymentEvent>()
                    .Include<CalculatedRequiredCoInvestedAmount, SfaFullyFundedFundingSourcePaymentEvent>();
          
                cfg.CreateMap<CalculatedRequiredOnProgrammeAmount, FundingSourcePaymentEvent>()
                    .Include<CalculatedRequiredLevyAmount, FundingSourcePaymentEvent>()
                    .Include<CalculatedRequiredCoInvestedAmount, FundingSourcePaymentEvent>()
                    .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => (TransactionType)source.OnProgrammeEarningType))
                    .ForMember(dest => dest.AgeAtStartOfLearning, opt => opt.MapFrom(source => source.AgeAtStartOfLearning))
                    //.ForMember(dest => dest.FundingPlatformType, opt => opt.Ignore())
                    ;

                cfg.CreateMap<CalculatedRequiredLevyAmount, RequiredPayment>();
                cfg.CreateMap<CalculatedRequiredCoInvestedAmount, RequiredCoInvestedPayment>();

                cfg.CreateMap<CalculatedRequiredIncentiveAmount, SfaFullyFundedFundingSourcePaymentEvent>()
                    .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => (TransactionType)source.Type))
                    .ForMember(dest => dest.FundingSourceType, opt => opt.UseValue(FundingSourceType.FullyFundedSfa))
                    //.ForMember(dest => dest.FundingPlatformType, opt => opt.Ignore())
                    ;

                cfg.CreateMap<FundingSourcePayment, FundingSourcePaymentEvent>()
                    .Include<EmployerCoInvestedPayment, EmployerCoInvestedFundingSourcePaymentEvent>()
                    .Include<SfaCoInvestedPayment, SfaCoInvestedFundingSourcePaymentEvent>()
                    .Include<LevyPayment, LevyFundingSourcePaymentEvent>()
                    .Include<TransferPayment, TransferFundingSourcePaymentEvent>()
                    .Include<UnableToFundTransferPayment, ProcessUnableToFundTransferFundingSourcePayment>()
                    .ForMember(fundingSourcePaymentEvent => fundingSourcePaymentEvent.FundingSourceType,
                        opt => opt.MapFrom(payment => payment.Type))
                    .ForMember(fundingSourcePaymentEvent => fundingSourcePaymentEvent.FundingPlatformType, payment => payment.MapFrom(source => source.FundingPlatformType));

                cfg.CreateMap<EmployerCoInvestedPayment, EmployerCoInvestedFundingSourcePaymentEvent>();
                cfg.CreateMap<SfaCoInvestedPayment, SfaCoInvestedFundingSourcePaymentEvent>();
                cfg.CreateMap<LevyPayment, LevyFundingSourcePaymentEvent>();
                cfg.CreateMap<TransferPayment, TransferFundingSourcePaymentEvent>();
                cfg.CreateMap<UnableToFundTransferPayment, ProcessUnableToFundTransferFundingSourcePayment>();

                cfg.CreateMap<ProcessUnableToFundTransferFundingSourcePayment, CalculatedRequiredLevyAmount>()
                    .ForMember(dest => dest.OnProgrammeEarningType, opt => opt.MapFrom(source => (OnProgrammeEarningType)source.TransactionType))
                    .ForMember(dest => dest.Priority, opt => opt.Ignore())
                    .ForMember(dest => dest.AgreementId, opt => opt.Ignore())
                    .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.RequiredPaymentEventId))
                    .ForMember(dest => dest.AgeAtStartOfLearning, opt => opt.MapFrom(source => source.AgeAtStartOfLearning))
                    .ForMember(dest => dest.FundingPlatformType, opt => opt.MapFrom(source => source.FundingPlatformType));

                cfg.CreateMap<LevyAccountModel, LevyAccountModel>();

                cfg.CreateMap<CalculateOnProgrammePayment, CalculatedRequiredLevyAmount>()
                    .ForMember(dest => dest.ContractType, opt => opt.UseValue(ContractType.Act1))
                    .ForMember(dest => dest.JobId, opt => opt.UseValue(-1))
                    .ForMember(dest => dest.IlrSubmissionDateTime, opt => opt.UseValue(new DateTime(1753,1,1)))
                    .ForMember(dest => dest.AgeAtStartOfLearning, opt => opt.MapFrom(source => source.AgeAtStartOfLearning))
                    .ForMember(dest => dest.FundingPlatformType, opt => opt.MapFrom(source => source.FundingPlatformType));

            });
        }
    }
}
