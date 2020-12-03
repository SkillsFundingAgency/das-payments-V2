using System;
using AutoMapper;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using PriceEpisode = SFA.DAS.CommitmentsV2.Messages.Events.PriceEpisode;

namespace SFA.DAS.Payments.DataLocks.Application.Mapping
{
    public class DataLocksProfile : Profile
    {
        public DataLocksProfile()
        {
            CreateMap<ApprenticeshipContractType1EarningEvent, PayableEarningEvent>()
                .ForMember(destinationMember => destinationMember.EarningEventId, opt => opt.MapFrom(source => source.EventId))
                .ForMember(destinationMember => destinationMember.EventId, opt => opt.Ignore());

            CreateMap<ApprenticeshipContractType1EarningEvent, EarningFailedDataLockMatching>()
                .ForMember(destinationMember => destinationMember.EarningEventId, opt => opt.MapFrom(source => source.EventId))
                .ForMember(destinationMember => destinationMember.EventId, opt => opt.Ignore());

            CreateMap<Act1FunctionalSkillEarningsEvent, PayableFunctionalSkillEarningEvent>()
                .ForMember(destinationMember => destinationMember.EarningEventId, opt => opt.MapFrom(source => source.EventId))
                .ForMember(destinationMember => destinationMember.EventId, opt => opt.Ignore());


            CreateMap<Act1FunctionalSkillEarningsEvent, FunctionalSkillEarningFailedDataLockMatching>()
                .ForMember(destinationMember => destinationMember.EarningEventId, opt => opt.MapFrom(source => source.EventId))
                .ForMember(destinationMember => destinationMember.EventId, opt => opt.Ignore());


            CreateMap<FunctionalSkillEarningsEvent, FunctionalSkillDataLockEvent>()
                .Include<Act1FunctionalSkillEarningsEvent, PayableFunctionalSkillEarningEvent>()
                .Include<Act1FunctionalSkillEarningsEvent, FunctionalSkillEarningFailedDataLockMatching>()
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(src => src.ContractType))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))

                .ForMember(destinationMember => destinationMember.EarningEventId, opt => opt.MapFrom(source => source.EventId))
                .ForMember(destinationMember => destinationMember.EventId, opt => opt.Ignore())

                .ForMember(dest => dest.AgreementId, opt => opt.Ignore())
               .ForMember(dest => dest.OnProgrammeEarnings, opt => opt.Ignore())
                .ForMember(dest => dest.IncentiveEarnings, opt => opt.Ignore());

            CreateMap<ApprenticeshipCreatedEvent, ApprenticeshipModel>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(source => source.AccountId))
                .ForMember(dest => dest.AgreedOnDate, opt => opt.MapFrom(source => source.AgreedOn))
                .ForMember(dest => dest.AgreementId, opt => opt.MapFrom(source => source.AccountLegalEntityPublicHashedId))
                .ForMember(dest => dest.EstimatedEndDate, opt => opt.MapFrom(source => source.EndDate))
                .ForMember(dest => dest.EstimatedStartDate, opt => opt.MapFrom(source => source.StartDate))
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.TrainingCode.ToStandardCode(source.TrainingType)))
                .ForMember(dest => dest.FrameworkCode, opt => opt.MapFrom(source => source.TrainingCode.ToFrameworkCode(source.TrainingType)))
                .ForMember(dest => dest.ProgrammeType, opt => opt.MapFrom(source => source.TrainingCode.ToProgrammeType(source.TrainingType)))
                .ForMember(dest => dest.PathwayCode, opt => opt.MapFrom(source => source.TrainingCode.ToPathwayCode(source.TrainingType)))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.ApprenticeshipId))
                .ForMember(dest => dest.LegalEntityName, opt => opt.MapFrom(source => source.LegalEntityName))
                .ForMember(dest => dest.Status, opt => opt.UseValue(ApprenticeshipStatus.Active))
                .ForMember(dest => dest.TransferSendingEmployerAccountId, opt => opt.MapFrom(source => source.TransferSenderId))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.ProviderId))
                .ForMember(dest => dest.Uln, opt => opt.MapFrom(source => source.Uln))
                .ForMember(dest => dest.ApprenticeshipPriceEpisodes, opt => opt.MapFrom(source => source.PriceEpisodes))
                .ForMember(dest => dest.Priority, opt => opt.Ignore())
                .ForMember(dest => dest.IsLevyPayer, opt => opt.UseValue(true))
                .ForMember(dest => dest.StopDate, dest => dest.Ignore())
                .ForMember(dest => dest.ApprenticeshipEmployerType, opt => opt.ResolveUsing<ApprenticeshipEmployerTypeResolver>())
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
                .ForMember(dest => dest.ApprenticeshipPauses, opt => opt.Ignore())
                ;

            CreateMap<PriceEpisode, ApprenticeshipPriceEpisodeModel>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(source => source.FromDate))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(source => source.Cost))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(source => source.ToDate))
                .ForMember(dest => dest.ApprenticeshipId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Removed, opt => opt.Ignore())
                ;

            CreateMap<ApprenticeshipModel, ApprenticeshipUpdated>()
                .ForMember(dest => dest.EmployerAccountId, opt => opt.MapFrom(source => source.AccountId))
                .ForMember(dest => dest.AgreedOnDate, opt => opt.MapFrom(source => source.AgreedOnDate))
                .ForMember(dest => dest.AgreementId, opt => opt.MapFrom(source => source.AgreementId))
                .ForMember(dest => dest.EstimatedEndDate, opt => opt.MapFrom(source => source.EstimatedEndDate))
                .ForMember(dest => dest.EstimatedStartDate, opt => opt.MapFrom(source => source.EstimatedStartDate))
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
                .ForMember(dest => dest.FrameworkCode, opt => opt.MapFrom(source => source.FrameworkCode))
                .ForMember(dest => dest.PathwayCode, opt => opt.MapFrom(source => source.PathwayCode))
                .ForMember(dest => dest.ProgrammeType, opt => opt.MapFrom(source => source.ProgrammeType))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.LegalEntityName, opt => opt.MapFrom(source => source.LegalEntityName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.Status))
                .ForMember(dest => dest.TransferSendingEmployerAccountId, opt => opt.MapFrom(source => source.TransferSendingEmployerAccountId))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn))
                .ForMember(dest => dest.Uln, opt => opt.MapFrom(source => source.Uln))
                .ForMember(dest => dest.ApprenticeshipPriceEpisodes, opt => opt.MapFrom(source => source.ApprenticeshipPriceEpisodes))
                .ForMember(dest => dest.IsLevyPayer, opt => opt.MapFrom(source => source.IsLevyPayer))
                .ForMember(dest => dest.Duplicates, opt => opt.Ignore())
                .ForMember(dest => dest.EventId, opt => opt.Ignore())
                .ForMember(dest => dest.EventTime, opt => opt.Ignore())
                .ForMember(dest => dest.ApprenticeshipEmployerType, opt => opt.MapFrom(source => source.ApprenticeshipEmployerType))
                ;

            CreateMap<ApprenticeshipUpdated, ApprenticeshipModel>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(source => source.EmployerAccountId))
                .ForMember(dest => dest.AgreedOnDate, opt => opt.MapFrom(source => source.AgreedOnDate))
                .ForMember(dest => dest.AgreementId, opt => opt.MapFrom(source => source.AgreementId))
                .ForMember(dest => dest.EstimatedEndDate, opt => opt.MapFrom(source => source.EstimatedEndDate))
                .ForMember(dest => dest.EstimatedStartDate, opt => opt.MapFrom(source => source.EstimatedStartDate))
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
                .ForMember(dest => dest.FrameworkCode, opt => opt.MapFrom(source => source.FrameworkCode))
                .ForMember(dest => dest.PathwayCode, opt => opt.MapFrom(source => source.PathwayCode))
                .ForMember(dest => dest.ProgrammeType, opt => opt.MapFrom(source => source.ProgrammeType))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.LegalEntityName, opt => opt.MapFrom(source => source.LegalEntityName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.Status))
                .ForMember(dest => dest.TransferSendingEmployerAccountId, opt => opt.MapFrom(source => source.TransferSendingEmployerAccountId))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn))
                .ForMember(dest => dest.Uln, opt => opt.MapFrom(source => source.Uln))
                .ForMember(dest => dest.ApprenticeshipPriceEpisodes, opt => opt.MapFrom(source => source.ApprenticeshipPriceEpisodes))
                .ForMember(dest => dest.IsLevyPayer, opt => opt.MapFrom(source => source.IsLevyPayer))
                .ForMember(dest => dest.Priority, opt => opt.Ignore())
                .ForMember(dest => dest.ApprenticeshipEmployerType, opt => opt.MapFrom(source => source.ApprenticeshipEmployerType))
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
                .ForMember(dest => dest.ApprenticeshipPauses, opt => opt.Ignore())
                ;
            
            CreateMap<ApprenticeshipUpdatedApprovedEvent, UpdatedApprenticeshipApprovedModel>()
                .ForMember(dest => dest.ApprenticeshipId, opt => opt.MapFrom(source => source.ApprenticeshipId))
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.TrainingCode.ToStandardCode(source.TrainingType)))
                .ForMember(dest => dest.FrameworkCode, opt => opt.MapFrom(source => source.TrainingCode.ToFrameworkCode(source.TrainingType)))
                .ForMember(dest => dest.ProgrammeType, opt => opt.MapFrom(source => source.TrainingCode.ToProgrammeType(source.TrainingType)))
                .ForMember(dest => dest.PathwayCode, opt => opt.MapFrom(source => source.TrainingCode.ToPathwayCode(source.TrainingType)))
                .ForMember(dest => dest.Uln, opt => opt.MapFrom(source => source.Uln))
                .ForMember(dest => dest.ApprenticeshipPriceEpisodes, opt => opt.MapFrom(source => source.PriceEpisodes))
                .ForMember(dest => dest.AgreedOnDate, opt => opt.MapFrom(source => source.ApprovedOn))
                .ForMember(dest => dest.EstimatedEndDate, opt => opt.MapFrom(source => source.EndDate))
                .ForMember(dest => dest.EstimatedStartDate, opt => opt.MapFrom(source => source.StartDate))
                ;

            CreateMap<DataLockTriageApprovedEvent, UpdatedApprenticeshipDataLockTriageModel>()
                .ForMember(dest => dest.ApprenticeshipId, opt => opt.MapFrom(source => source.ApprenticeshipId))
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.TrainingCode.ToStandardCode(source.TrainingType)))
                .ForMember(dest => dest.FrameworkCode, opt => opt.MapFrom(source => source.TrainingCode.ToFrameworkCode(source.TrainingType)))
                .ForMember(dest => dest.ProgrammeType, opt => opt.MapFrom(source => source.TrainingCode.ToProgrammeType(source.TrainingType)))
                .ForMember(dest => dest.PathwayCode, opt => opt.MapFrom(source => source.TrainingCode.ToPathwayCode(source.TrainingType)))
                .ForMember(dest => dest.AgreedOnDate, opt => opt.MapFrom(source => source.ApprovedOn))
                .ForMember(dest => dest.ApprenticeshipPriceEpisodes, opt => opt.MapFrom(source => source.PriceEpisodes));

            CreateMap<EarningEventModel, ApprenticeshipContractType1EarningEvent>()
                .ForMember(dest => dest.AgreementId, opt => opt.MapFrom(source => source.AgreementId))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn))
                .ForMember(dest => dest.CollectionPeriod, opt => opt.MapFrom(source => CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(source.AcademicYear, source.CollectionPeriod)))
                .ForMember(dest => dest.CollectionYear, opt => opt.MapFrom(source => source.AcademicYear))
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(source => source.EventId))
                .ForMember(dest => dest.EventTime, opt => opt.MapFrom(source => source.EventTime))
                .ForMember(dest => dest.IlrSubmissionDateTime, opt => opt.MapFrom(source => source.IlrSubmissionDateTime))
                .ForMember(dest => dest.JobId, opt => opt.MapFrom(source => source.JobId))
                .ForMember(dest => dest.IlrFileName, opt => opt.MapFrom(source => source.IlrFileName))
                .ForMember(dest => dest.SfaContributionPercentage, opt => opt.MapFrom(source => source.SfaContributionPercentage))
                .ForMember(dest => dest.PriceEpisodes, opt => opt.MapFrom(source => source.PriceEpisodes))
                .ForMember(dest => dest.Learner, opt => opt.Ignore())
                .ForMember(dest => dest.LearningAim, opt => opt.Ignore())
                .ForMember(dest => dest.IncentiveEarnings, opt => opt.Ignore())
                .ForMember(dest => dest.OnProgrammeEarnings, opt => opt.Ignore());
          
            CreateMap<EarningEventPeriodModel, EarningPeriod>()
                .ForMember(dest => dest.PriceEpisodeIdentifier, opt => opt.MapFrom(source => source.PriceEpisodeIdentifier))
                .ForMember(dest => dest.Period, opt => opt.MapFrom(source => source.DeliveryPeriod))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(source => source.Amount))
                .ForMember(dest => dest.SfaContributionPercentage, opt => opt.MapFrom(source => source.SfaContributionPercentage))
                .ForMember(dest => dest.ApprenticeshipEmployerType, opt => opt.Ignore())
                .ForMember(dest => dest.AccountId, opt => opt.Ignore())
                .ForMember(dest => dest.ApprenticeshipId, opt => opt.Ignore())
                .ForMember(dest => dest.ApprenticeshipPriceEpisodeId, opt => opt.Ignore())
                .ForMember(dest => dest.TransferSenderAccountId, opt => opt.Ignore())
                .ForMember(dest => dest.Priority, opt => opt.Ignore())
                .ForMember(dest => dest.DataLockFailures, opt => opt.Ignore())
                .ForMember(dest => dest.AgreedOnDate, opt => opt.Ignore());

            CreateMap<EarningEventModel, Learner>()
                .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(source => source.LearnerReferenceNumber))
                .ForMember(dest => dest.Uln, opt => opt.MapFrom(source => source.LearnerUln));

            CreateMap<EarningEventModel, LearningAim>()
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(source => source.LearningAimReference))
                .ForMember(dest => dest.ProgrammeType, opt => opt.MapFrom(source => source.LearningAimProgrammeType))
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.LearningAimStandardCode))
                .ForMember(dest => dest.FrameworkCode, opt => opt.MapFrom(source => source.LearningAimFrameworkCode))
                .ForMember(dest => dest.PathwayCode, opt => opt.MapFrom(source => source.LearningAimPathwayCode))
                .ForMember(dest => dest.FundingLineType, opt => opt.MapFrom(source => source.LearningAimFundingLineType))
                .ForMember(dest => dest.SequenceNumber, opt => opt.MapFrom(source => source.LearningAimSequenceNumber));

            CreateMap<EarningEventPriceEpisodeModel, SFA.DAS.Payments.Model.Core.PriceEpisode>()
                .ForMember(dest => dest.Identifier, opt => opt.MapFrom(source => source.PriceEpisodeIdentifier))
                .ForMember(dest => dest.TotalNegotiatedPrice1, opt => opt.MapFrom(source => source.TotalNegotiatedPrice1))
                .ForMember(dest => dest.TotalNegotiatedPrice2, opt => opt.MapFrom(source => source.TotalNegotiatedPrice2))
                .ForMember(dest => dest.TotalNegotiatedPrice3, opt => opt.MapFrom(source => source.TotalNegotiatedPrice3))
                .ForMember(dest => dest.TotalNegotiatedPrice4, opt => opt.MapFrom(source => source.TotalNegotiatedPrice4))
                .ForMember(dest => dest.AgreedPrice, opt => opt.MapFrom(source => source.AgreedPrice))
                .ForMember(dest => dest.CourseStartDate, opt => opt.MapFrom(source => source.CourseStartDate))
                .ForMember(dest => dest.EffectiveTotalNegotiatedPriceStartDate, opt => opt.MapFrom(source => source.StartDate))
                .ForMember(dest => dest.PlannedEndDate, opt => opt.MapFrom(source => source.PlannedEndDate))
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(source => source.ActualEndDate))
                .ForMember(dest => dest.NumberOfInstalments, opt => opt.MapFrom(source => source.NumberOfInstalments))
                .ForMember(dest => dest.InstalmentAmount, opt => opt.MapFrom(source => source.InstalmentAmount))
                .ForMember(dest => dest.CompletionAmount, opt => opt.MapFrom(source => source.CompletionAmount))
                .ForMember(dest => dest.Completed, opt => opt.MapFrom(source => source.Completed))
                .ForMember(dest => dest.EmployerContribution, opt => opt.MapFrom(source => source.EmployerContribution))
                .ForMember(dest => dest.CompletionHoldBackExemptionCode, opt => opt.MapFrom(source => source.CompletionHoldBackExemptionCode))
                .ForMember(dest => dest.FundingLineType, opt => opt.Ignore())
                ;


            CreateMap<EarningEventModel, Act1FunctionalSkillEarningsEvent>()
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn))
                .ForMember(dest => dest.CollectionPeriod, opt => opt.MapFrom(source => CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(source.AcademicYear, source.CollectionPeriod)))
                .ForMember(dest => dest.CollectionYear, opt => opt.MapFrom(source => source.AcademicYear))
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(source => source.EventId))
                .ForMember(dest => dest.EventTime, opt => opt.MapFrom(source => source.EventTime))
                .ForMember(dest => dest.IlrSubmissionDateTime, opt => opt.MapFrom(source => source.IlrSubmissionDateTime))
                .ForMember(dest => dest.JobId, opt => opt.MapFrom(source => source.JobId))
                .ForMember(dest => dest.IlrFileName, opt => opt.MapFrom(source => source.IlrFileName))
                .ForMember(dest => dest.PriceEpisodes, opt => opt.MapFrom(source => source.PriceEpisodes))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(source => source.StartDate))
                .ForMember(dest => dest.ContractType, opt => opt.UseValue(ContractType.Act1))
                .ForMember(dest => dest.Learner, opt => opt.Ignore())
                .ForMember(dest => dest.LearningAim, opt => opt.Ignore())
                .ForMember(dest => dest.Earnings, opt => opt.Ignore());

        }
    }
}

