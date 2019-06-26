﻿using System;
using System.Linq;
using AutoMapper;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.CommitmentsV2.Types;
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


            CreateMap<ApprenticeshipCreatedEvent, ApprenticeshipModel>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(source => source.AccountId))
                .ForMember(dest => dest.AgreedOnDate, opt => opt.MapFrom(source => source.AgreedOn))
                .ForMember(dest => dest.AgreementId, opt => opt.MapFrom(source => source.AccountLegalEntityPublicHashedId))
                .ForMember(dest => dest.EstimatedEndDate, opt => opt.MapFrom(source => source.EndDate))
                .ForMember(dest => dest.EstimatedStartDate, opt => opt.MapFrom(source => source.StartDate))
                .ForMember(dest => dest.StandardCode, opt => opt.ResolveUsing(source => source.TrainingType == ProgrammeType.Standard ? int.Parse(source.TrainingCode) : 0))
                .ForMember(dest => dest.FrameworkCode, opt => opt.ResolveUsing(source => source.TrainingType == ProgrammeType.Framework ? int.Parse(source.TrainingCode.Split('-').FirstOrDefault() ?? throw new InvalidOperationException($"Failed to parse the training code field to get the Framework code. Data: {source.TrainingCode}")) : 0))
                .ForMember(dest => dest.ProgrammeType, opt => opt.ResolveUsing(source => source.TrainingType == ProgrammeType.Framework ? int.Parse(source.TrainingCode.Split('-').Skip(1).FirstOrDefault() ?? throw new InvalidOperationException($"Failed to parse the training code field to get the Programme Type. Data: {source.TrainingCode}")) : 25))
                .ForMember(dest => dest.PathwayCode, opt => opt.ResolveUsing(source => source.TrainingType == ProgrammeType.Framework ? int.Parse(source.TrainingCode.Split('-').Skip(2).FirstOrDefault() ?? throw new InvalidOperationException($"Failed to parse the training code field to get the Pathway code. Data: {source.TrainingCode}")) : 0))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.ApprenticeshipId))
                .ForMember(dest => dest.LegalEntityName, opt => opt.MapFrom(source => source.LegalEntityName))
                .ForMember(dest => dest.Status, opt => opt.UseValue(ApprenticeshipStatus.Active))
                .ForMember(dest => dest.TransferSendingEmployerAccountId, opt => opt.MapFrom(source => source.TransferSenderId))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.ProviderId))
                .ForMember(dest => dest.Uln, opt => opt.MapFrom(source => source.Uln))
                .ForMember(dest => dest.ApprenticeshipPriceEpisodes, opt => opt.MapFrom(source => source.PriceEpisodes))
                .ForMember(dest => dest.Priority, opt => opt.Ignore())
                .ForMember(dest => dest.IsLevyPayer, opt => opt.Ignore())
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
                .ForMember(dest => dest.Duplicates, opt => opt.Ignore())
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
                .ForMember(dest => dest.Priority, opt => opt.Ignore())
                .ForMember(dest => dest.IsLevyPayer, opt => opt.Ignore())
                ;
        }
    }
}

