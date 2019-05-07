﻿using AutoMapper;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Application.Extensions;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration
{
    public class RequiredPaymentsProfile : Profile
    {
        public RequiredPaymentsProfile()
        {
            CreateMap<PaymentHistoryEntity, Payment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DeliveryPeriod, opt => opt.ResolveUsing(src => src.DeliveryPeriod))
                .ForMember(dest => dest.CollectionPeriod, opt => opt.ResolveUsing(src => src.CollectionPeriod.Clone()))
                .ForMember(payment => payment.StartDate, opt => opt.MapFrom(episode => episode.StartDate))
                .ForMember(payment => payment.PlannedEndDate, opt => opt.MapFrom(episode => episode.PlannedEndDate))
                .ForMember(payment => payment.ActualEndDate, opt => opt.MapFrom(episode => episode.ActualEndDate))
                .ForMember(payment => payment.CompletionStatus, opt => opt.Ignore())
                .ForMember(payment => payment.CompletionAmount, opt => opt.MapFrom(episode => episode.CompletionAmount))
                .ForMember(payment => payment.InstalmentAmount, opt => opt.MapFrom(episode => episode.InstalmentAmount))
                .ForMember(payment => payment.NumberOfInstalments, opt => opt.MapFrom(episode => episode.NumberOfInstalments));

            // Earning event --> required payment event
            CreateMap<IEarningEvent, RequiredPaymentEvent>()
                .Include<PayableEarningEvent, CompletionPaymentHeldBackEvent>()
                .Include<ApprenticeshipContractType2EarningEvent, CompletionPaymentHeldBackEvent>()
                .Include<IEarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .Include<IEarningEvent, CalculatedRequiredIncentiveAmount>()
                .ForMember(requiredPayment => requiredPayment.EarningEventId, opt => opt.MapFrom(earning => earning.EventId))
                .ForMember(requiredPayment => requiredPayment.AmountDue, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.DeliveryPeriod, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.PriceEpisodeIdentifier, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.CollectionPeriod, opt => opt.MapFrom(earning => earning.CollectionPeriod.Clone()))
                .ForMember(requiredPayment => requiredPayment.Learner, opt => opt.MapFrom(earning => earning.Learner.Clone()))
                .ForMember(requiredPayment => requiredPayment.LearningAim, opt => opt.MapFrom(earning => earning.LearningAim.Clone()))
                .ForMember(requiredPayment => requiredPayment.EventId, opt => opt.Ignore())
                .Ignore(x => x.ContractType)
                .ForMember(requiredPayment => requiredPayment.AccountId, opt => opt.Ignore())
                .Ignore(x => x.StartDate)
                .Ignore(x => x.PlannedEndDate)
                .Ignore(x => x.ActualEndDate)
                .Ignore(x => x.CompletionStatus)
                .Ignore(x => x.CompletionAmount)
                .Ignore(x => x.InstalmentAmount)
                .Ignore(x => x.NumberOfInstalments);
         
            CreateMap<IEarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .Include<PayableEarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .Include<FunctionalSkillEarningsEvent, CalculatedRequiredOnProgrammeAmount>()
                .Include<ApprenticeshipContractType2EarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .ForMember(requiredPayment => requiredPayment.OnProgrammeEarningType, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.SfaContributionPercentage, opt => opt.Ignore())
                ;
            CreateMap<IEarningEvent, CalculatedRequiredIncentiveAmount>()
                .Include<PayableEarningEvent, CalculatedRequiredIncentiveAmount>()
                .Include<ApprenticeshipContractType2EarningEvent, CalculatedRequiredIncentiveAmount>()
                .Include<FunctionalSkillEarningsEvent, CalculatedRequiredIncentiveAmount>()
                .Ignore(x => x.Type)
                .Ignore(x => x.EarningEventId)
                .Ignore(x => x.PriceEpisodeIdentifier)
                .Ignore(x => x.AmountDue)
                .Ignore(x => x.DeliveryPeriod)
                .Ignore(x => x.ContractType)
                ;

            CreateMap<PayableEarningEvent, CompletionPaymentHeldBackEvent>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act1))
                ;
            CreateMap<ApprenticeshipContractType2EarningEvent, CompletionPaymentHeldBackEvent>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act2))
                ;

            CreateMap<PayableEarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .Include<PayableEarningEvent, CalculatedRequiredLevyAmount>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act1))
                ;
            CreateMap<FunctionalSkillEarningsEvent, CalculatedRequiredOnProgrammeAmount>()
                .ForMember(x => x.StartDate, opt => opt.MapFrom(source => source.StartDate))
                .ForMember(x => x.ContractType, opt => opt.MapFrom(x => x.ContractType))
                ;
            CreateMap<ApprenticeshipContractType2EarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act2))
                ;

            CreateMap<PayableEarningEvent, CalculatedRequiredIncentiveAmount>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act1))
                ;

            CreateMap<ApprenticeshipContractType2EarningEvent, CalculatedRequiredIncentiveAmount>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act2))
                ;
            CreateMap<FunctionalSkillEarningsEvent, CalculatedRequiredIncentiveAmount>()
                .ForMember(x => x.StartDate, opt => opt.MapFrom(source => source.StartDate))
                .ForMember(x => x.ContractType, opt => opt.MapFrom(x => x.ContractType))
                ;

            CreateMap<PayableEarningEvent, CalculatedRequiredLevyAmount>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act1))
                .ForMember(x => x.Priority, opt => opt.Ignore())
                .ForMember(x => x.ApprenticeshipId, opt => opt.Ignore())
                .ForMember(x => x.ApprenticeshipPriceEpisodeId, opt => opt.Ignore()) ;

            // End Earning Event --> Required Payment Event

            CreateMap<EarningPeriod, RequiredPaymentEvent>()
                .Include<EarningPeriod, CalculatedRequiredOnProgrammeAmount>()
                .Include<EarningPeriod, CalculatedRequiredCoInvestedAmount>()
                .Include<EarningPeriod, CalculatedRequiredIncentiveAmount>()
                .Include<EarningPeriod, CalculatedRequiredLevyAmount>()
                .ForMember(requiredPayment => requiredPayment.DeliveryPeriod, opt => opt.MapFrom(period => period.Period))
                .ForMember(requiredPayment => requiredPayment.AccountId, opt => opt.MapFrom(period => period.AccountId))
                .ForAllOtherMembers(opt => opt.Ignore())
                ;


            CreateMap<EarningPeriod, CalculatedRequiredOnProgrammeAmount>()
                .Include<EarningPeriod, CalculatedRequiredCoInvestedAmount>()
                .Include<EarningPeriod, CalculatedRequiredLevyAmount>()
                .ForMember(requiredPayment => requiredPayment.SfaContributionPercentage, opt => opt.MapFrom(period => period.SfaContributionPercentage))
                .Ignore(x => x.OnProgrammeEarningType)
                ;

            CreateMap<EarningPeriod, CalculatedRequiredCoInvestedAmount>()
                ;

            CreateMap<EarningPeriod, CalculatedRequiredLevyAmount>()
                .ForMember(requiredPayment => requiredPayment.ApprenticeshipId, opt => opt.MapFrom(period => period.ApprenticeshipId))
                .ForMember(requiredPayment => requiredPayment.ApprenticeshipPriceEpisodeId, opt => opt.MapFrom(period => period.ApprenticeshipPriceEpisodeId))
                .ForMember(requiredPayment => requiredPayment.Priority, opt => opt.MapFrom(period => period.Priority))
                .ForMember(x => x.AgreementId, opt => opt.Ignore())
                //.ForAllOtherMembers(opt => opt.Ignore())
                ;

            CreateMap<EarningPeriod, CalculatedRequiredIncentiveAmount>()
                .Ignore(x => x.Type)
                ;

            // Required Payment --> RequiredPaymentEvent
            CreateMap<RequiredPayment, RequiredPaymentEvent>()
                .Include<RequiredPayment, CalculatedRequiredCoInvestedAmount>()
                .Include<RequiredPayment, CalculatedRequiredIncentiveAmount>()
                .Include<RequiredPayment, CalculatedRequiredLevyAmount>()
                .ForMember(x => x.AmountDue, opt => opt.MapFrom(x => x.Amount))
                .Ignore(x => x.EarningEventId)
                .Ignore(x => x.DeliveryPeriod)
                .Ignore(x => x.JobId)
                .Ignore(x => x.EventTime)
                .Ignore(x => x.EventId)
                .Ignore(x => x.Ukprn)
                .Ignore(x => x.Learner)
                .Ignore(x => x.LearningAim)
                .Ignore(x => x.IlrSubmissionDateTime)
                .Ignore(x => x.CollectionPeriod)
                .Ignore(x => x.ContractType)
                .Ignore(x => x.AccountId)
                .Ignore(x => x.StartDate)
                .Ignore(x => x.PlannedEndDate)
                .Ignore(x => x.ActualEndDate)
                .Ignore(x => x.CompletionStatus)
                .Ignore(x => x.CompletionAmount)
                .Ignore(x => x.InstalmentAmount)
                .Ignore(x => x.NumberOfInstalments);

            CreateMap<RequiredPayment, CalculatedRequiredCoInvestedAmount>()
                .ForMember(x => x.SfaContributionPercentage, opt => opt.MapFrom(x => x.SfaContributionPercentage))
                .Ignore(x => x.OnProgrammeEarningType)
                ;
            CreateMap<RequiredPayment, CalculatedRequiredIncentiveAmount>()
                .Ignore(x => x.ContractType)
                .Ignore(x => x.Type)
                ;
            CreateMap<RequiredPayment, CalculatedRequiredLevyAmount>()
                .Ignore(x => x.OnProgrammeEarningType)
                .Ignore(x => x.Priority)
                .Ignore(x => x.ApprenticeshipId)
                .Ignore(x => x.ApprenticeshipPriceEpisodeId)
                .Ignore(x => x.AgreementId)
                ;

            CreateMap<PriceEpisode, PeriodisedPaymentEvent>()
                .ForMember(payment => payment.StartDate, opt => opt.MapFrom(episode => episode.EffectiveTotalNegotiatedPriceStartDate))
                .ForMember(payment => payment.PlannedEndDate, opt => opt.MapFrom(episode => episode.PlannedEndDate))
                .ForMember(payment => payment.ActualEndDate, opt => opt.MapFrom(episode => episode.ActualEndDate))
                .ForMember(payment => payment.CompletionStatus, opt => opt.Ignore())
                .ForMember(payment => payment.CompletionAmount, opt => opt.MapFrom(episode => episode.CompletionAmount))
                .ForMember(payment => payment.InstalmentAmount, opt => opt.MapFrom(episode => episode.InstalmentAmount))
                .ForMember(payment => payment.NumberOfInstalments, opt => opt.MapFrom(episode => episode.NumberOfInstalments))
                .Ignore(x => x.PriceEpisodeIdentifier)
                .Ignore(x => x.AmountDue)
                .Ignore(x => x.DeliveryPeriod)
                .Ignore(x => x.AccountId)
                .Ignore(x => x.ContractType)
                .Ignore(x => x.JobId)
                .Ignore(x => x.EventTime)
                .Ignore(x => x.EventId)
                .Ignore(x => x.Ukprn)
                .Ignore(x => x.Learner)
                .Ignore(x => x.LearningAim)
                .Ignore(x => x.IlrSubmissionDateTime)
                .Ignore(x => x.CollectionPeriod);
            // End Required Payment --> RequiredPaymentEvent
        }
    }
}
