using AutoMapper;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

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

            CreateMap<Act1FunctionalSkillEarningsEvent, PayableFunctionalSkillEarningEvent>();
            CreateMap<Act1FunctionalSkillEarningsEvent, FunctionalSkillEarningFailedDataLockMatching>();

            CreateMap<FunctionalSkillEarningsEvent, FunctionalSkillDataLockEvent>()
                .Include<Act1FunctionalSkillEarningsEvent, PayableFunctionalSkillEarningEvent>()
                .Include<Act1FunctionalSkillEarningsEvent,
                    FunctionalSkillEarningFailedDataLockMatching>()
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(src => src.ContractType))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.AgreementId, opt => opt.Ignore())
                .ForMember(dest => dest.EarningEventId, opt => opt.Ignore())
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
                ;

            CreateMap<DataLockEvent, DataLockStatusChanged>()
                .ForMember(destinationMember => destinationMember.TransactionTypesAndPeriods, opt => opt.Ignore());

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
                .ForMember(dest => dest.ApprenticeshipPriceEpisodes, opt => opt.MapFrom(source => source.PriceEpisodes))
                ;


        }
    }
}

