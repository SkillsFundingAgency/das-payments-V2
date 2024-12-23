using System;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Internal;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Application.Extensions;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Mapping
{
    public class RequiredPaymentsProfile : Profile
    {
        public RequiredPaymentsProfile()
        {

            CreateMap<PaymentModel, CalculatedRequiredLevyAmount>()
                //These 4 Fields are not available in PaymentModel and therefore will not be written to DB so no need to map them, also SequenceNumber
                .ForMember(dest => dest.IlrFileName, opt => opt.Ignore())
                .ForMember(dest => dest.AgreedOnDate, opt => opt.Ignore())
                .ForMember(dest => dest.Priority, opt => opt.Ignore())
                .ForMember(dest => dest.AgeAtStartOfLearning, opt => opt.Ignore())

                .ForMember(dest => dest.AgreementId, opt => opt.MapFrom(s => s.AgreementId))
                .ForMember(dest => dest.AmountDue, opt => opt.MapFrom(s => s.Amount))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(s => s.AccountId))
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(s => s.ActualEndDate))
                .ForMember(dest => dest.ApprenticeshipEmployerType, opt => opt.MapFrom(s => s.ApprenticeshipEmployerType))
                .ForMember(dest => dest.ApprenticeshipId, opt => opt.MapFrom(s => s.ApprenticeshipId))
                .ForMember(dest => dest.ApprenticeshipPriceEpisodeId, opt => opt.MapFrom(s => s.ApprenticeshipPriceEpisodeId))
                .ForMember(dest => dest.CollectionPeriod, opt => opt.ResolveUsing(src => src.CollectionPeriod.Clone()))
                .ForMember(dest => dest.CompletionAmount, opt => opt.MapFrom(s => s.CompletionAmount))
                .ForMember(dest => dest.CompletionStatus, opt => opt.MapFrom(s => s.CompletionStatus))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(s => s.ContractType))
                .ForMember(dest => dest.ClawbackSourcePaymentEventId, opt => opt.MapFrom(s => s.ClawbackSourcePaymentEventId))
                .ForMember(dest => dest.DeliveryPeriod, opt => opt.ResolveUsing(src => src.DeliveryPeriod))
                .ForMember(dest => dest.EarningEventId, opt => opt.MapFrom(s => s.EarningEventId))
                .ForMember(dest => dest.IlrSubmissionDateTime, opt => opt.MapFrom(s => s.IlrSubmissionDateTime))
                .ForMember(dest => dest.InstalmentAmount, opt => opt.MapFrom(s => s.InstalmentAmount))
                .ForMember(dest => dest.JobId, opt => opt.MapFrom(s => s.JobId))
                .ForMember(dest => dest.LearningAim, opt => opt.ResolveUsing(s => new LearningAim
                {
                    PathwayCode = s.LearningAimPathwayCode,
                    FrameworkCode = s.LearningAimFrameworkCode,
                    FundingLineType = s.LearningAimFundingLineType,
                    ProgrammeType = s.LearningAimProgrammeType,
                    Reference = s.LearningAimReference,
                    StandardCode = s.LearningAimStandardCode,
                    StartDate = s.StartDate,
                }))
                .ForMember(dest => dest.Learner, opt => opt.MapFrom(s => new Learner
                {
                    Uln = s.LearnerUln,
                    ReferenceNumber = s.LearnerReferenceNumber,
                }))
                .ForMember(dest => dest.LearningStartDate, opt => opt.MapFrom(s => s.LearningStartDate))
                .ForMember(dest => dest.NumberOfInstalments, opt => opt.MapFrom(s => s.NumberOfInstalments))
                .ForMember(dest => dest.PlannedEndDate, opt => opt.MapFrom(s => s.PlannedEndDate))
                .ForMember(dest => dest.PriceEpisodeIdentifier, opt => opt.MapFrom(s => s.PriceEpisodeIdentifier))
                .ForMember(dest => dest.ReportingAimFundingLineType, opt => opt.MapFrom(s => s.ReportingAimFundingLineType))
                .ForMember(dest => dest.SfaContributionPercentage, opt => opt.MapFrom(s => s.SfaContributionPercentage))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(s => s.StartDate))
                .ForMember(dest => dest.OnProgrammeEarningType, opt => opt.MapFrom(s => (OnProgrammeEarningType)s.TransactionType))
                //NOTE:TransactionType must be derived from OnProgrammeEarningType because of private setter on CalculatedRequiredLevyAmount
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(s => s.TransactionType))
                .ForMember(dest => dest.TransferSenderAccountId, opt => opt.MapFrom(s => s.TransferSenderAccountId))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(s => s.Ukprn))
                .ForMember(dest => dest.FundingPlatformType, opt => opt.MapFrom(source => source.FundingPlatformType))
                ;

            CreateMap<PaymentHistoryEntity, Payment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DeliveryPeriod, opt => opt.ResolveUsing(src => src.DeliveryPeriod))
                .ForMember(dest => dest.CollectionPeriod, opt => opt.ResolveUsing(src => src.CollectionPeriod.Clone()))
                .ForMember(payment => payment.StartDate, opt => opt.MapFrom(episode => episode.StartDate))
                .ForMember(payment => payment.PlannedEndDate, opt => opt.MapFrom(episode => episode.PlannedEndDate))
                .ForMember(payment => payment.ActualEndDate, opt => opt.MapFrom(episode => episode.ActualEndDate))
                .ForMember(payment => payment.LearnAimReference, opt => opt.MapFrom(episode => episode.LearnAimReference))
                .ForMember(payment => payment.TransactionType, opt => opt.MapFrom(episode => episode.TransactionType))
                .ForMember(payment => payment.CompletionStatus, opt => opt.Ignore())
                .ForMember(payment => payment.CompletionAmount, opt => opt.MapFrom(episode => episode.CompletionAmount))
                .ForMember(payment => payment.InstalmentAmount, opt => opt.MapFrom(episode => episode.InstalmentAmount))
                .ForMember(payment => payment.NumberOfInstalments, opt => opt.MapFrom(episode => episode.NumberOfInstalments))
                .ForMember(payment => payment.AccountId, opt => opt.MapFrom(episode => episode.AccountId))
                .ForMember(payment => payment.TransferSenderAccountId, opt => opt.MapFrom(episode => episode.TransferSenderAccountId))
                .ForMember(payment => payment.LearningStartDate, opt => opt.MapFrom(episode => episode.LearningStartDate))
                .ForMember(payment => payment.ApprenticeshipId, opt => opt.MapFrom(episode => episode.ApprenticeshipId))
                ;

            // Earning event --> required payment event
            CreateMap<IEarningEvent, PeriodisedRequiredPaymentEvent>()
                .Include<PayableEarningEvent, CompletionPaymentHeldBackEvent>()
                .Include<ApprenticeshipContractType2EarningEvent, CompletionPaymentHeldBackEvent>()
                .Include<ApprenticeshipContractType1RedundancyEarningEvent, CompletionPaymentHeldBackEvent>()
                .Include<ApprenticeshipContractType2RedundancyEarningEvent, CompletionPaymentHeldBackEvent>()
                .Include<IEarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .Include<IEarningEvent, CalculatedRequiredIncentiveAmount>()
                .ForMember(requiredPayment => requiredPayment.EarningEventId,
                    opt => opt.MapFrom(earning => earning.EventId))
                .ForMember(requiredPayment => requiredPayment.AmountDue, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.DeliveryPeriod, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.PriceEpisodeIdentifier, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.CollectionPeriod,
                    opt => opt.MapFrom(earning => earning.CollectionPeriod.Clone()))
                .ForMember(requiredPayment => requiredPayment.Learner,
                    opt => opt.MapFrom(earning => earning.Learner.Clone()))
                .ForMember(requiredPayment => requiredPayment.LearningAim,
                    opt => opt.MapFrom(earning => earning.LearningAim.Clone()))
                .ForMember(requiredPayment => requiredPayment.EventId, opt => opt.Ignore())
                //pull start date from price episode if available (in the case of on prog earnings) or from the aim if not (in the case of functional skills)
                .ForMember(requiredPayment => requiredPayment.LearningStartDate,
                    opt => opt.ResolveUsing(earning =>
                        earning.PriceEpisodes.FirstOrDefault(x => x.LearningAimSequenceNumber == earning.LearningAim.SequenceNumber)?.CourseStartDate ?? earning.LearningAim.StartDate))



                .Ignore(x => x.ApprenticeshipId)
                .Ignore(x => x.ApprenticeshipPriceEpisodeId)
                .Ignore(x => x.ContractType)
                .Ignore(x => x.AccountId)
                .Ignore(x => x.TransferSenderAccountId)
                .Ignore(x => x.StartDate)
                .Ignore(x => x.TransactionType)
                .Ignore(x => x.PlannedEndDate)
                .Ignore(x => x.ActualEndDate)
                .Ignore(x => x.CompletionStatus)
                .Ignore(x => x.CompletionAmount)
                .Ignore(x => x.IlrFileName)
                .Ignore(x => x.InstalmentAmount)
                .Ignore(x => x.NumberOfInstalments)
                .Ignore(x => x.ApprenticeshipEmployerType)
                .Ignore(x => x.ReportingAimFundingLineType)
                .Ignore(x => x.LearningAimSequenceNumber)
                .Ignore(x => x.ClawbackSourcePaymentEventId)
                ;

            CreateMap<IEarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .Include<PayableEarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .Include<FunctionalSkillEarningsEvent, CalculatedRequiredOnProgrammeAmount>()
                .Include<ApprenticeshipContractType2EarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .Include<ApprenticeshipContractType2RedundancyEarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .Include<ApprenticeshipContractType1RedundancyEarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .ForMember(requiredPayment => requiredPayment.AgeAtStartOfLearning, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.OnProgrammeEarningType, opt => opt.Ignore())
                .ForMember(requiredPayment => requiredPayment.SfaContributionPercentage, opt => opt.Ignore());

            CreateMap<IEarningEvent, CalculatedRequiredIncentiveAmount>()
                .Include<PayableEarningEvent, CalculatedRequiredIncentiveAmount>()
                .Include<ApprenticeshipContractType2EarningEvent, CalculatedRequiredIncentiveAmount>()
                .Include<FunctionalSkillEarningsEvent, CalculatedRequiredIncentiveAmount>()
                .Include<PayableFunctionalSkillEarningEvent, CalculatedRequiredIncentiveAmount>()
                .Include<ApprenticeshipContractType1RedundancyEarningEvent, CalculatedRequiredIncentiveAmount>()
                .Include<ApprenticeshipContractType2RedundancyEarningEvent, CalculatedRequiredIncentiveAmount>()
                .Ignore(x => x.Type)
                .Ignore(x => x.EarningEventId)
                .Ignore(x => x.PriceEpisodeIdentifier)
                .Ignore(x => x.AmountDue)
                .Ignore(x => x.DeliveryPeriod)
                .Ignore(x => x.ContractType)
                ;

            CreateMap<PayableEarningEvent, CompletionPaymentHeldBackEvent>()
                .ForMember(x => x.EarningEventId, opt => opt.MapFrom(source => source.EarningEventId))
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act1))
                .Ignore(x => x.TransactionType)
                ;

            CreateMap<ApprenticeshipContractType2EarningEvent, CompletionPaymentHeldBackEvent>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act2))
                .Ignore(x => x.TransactionType)
                ;

            CreateMap<PayableEarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .Include<PayableEarningEvent, CalculatedRequiredLevyAmount>()
                .ForMember(x => x.EarningEventId, opt => opt.MapFrom(source => source.EarningEventId))
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act1))
                .ForMember(x => x.AgeAtStartOfLearning, opt => opt.MapFrom(source => source.AgeAtStartOfLearning))
                .ForMember(x => x.LearningStartDate, opt => opt.Ignore());


            CreateMap<FunctionalSkillEarningsEvent, CalculatedRequiredOnProgrammeAmount>()
                .ForMember(x => x.StartDate, opt => opt.MapFrom(source => source.StartDate))
                .ForMember(x => x.ContractType, opt => opt.MapFrom(x => x.ContractType))
                ;

            CreateMap<ApprenticeshipContractType1RedundancyEarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act1))
                ;

            CreateMap<ApprenticeshipContractType2RedundancyEarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act2))
                ;

            CreateMap<ApprenticeshipContractType2EarningEvent, CalculatedRequiredOnProgrammeAmount>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act2))
                ;

            CreateMap<ApprenticeshipContractType1RedundancyEarningEvent, CalculatedRequiredIncentiveAmount>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act1))
                ;

            CreateMap<ApprenticeshipContractType2RedundancyEarningEvent, CalculatedRequiredIncentiveAmount>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act2))
                ;

            CreateMap<ApprenticeshipContractType1RedundancyEarningEvent, CompletionPaymentHeldBackEvent>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act1))
                .Ignore(x => x.TransactionType)
                ;

            CreateMap<ApprenticeshipContractType2RedundancyEarningEvent, CompletionPaymentHeldBackEvent>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act2))
                .Ignore(x => x.TransactionType)
                ;

            CreateMap<PayableEarningEvent, CalculatedRequiredIncentiveAmount>()
                .ForMember(x => x.EarningEventId, opt => opt.MapFrom(source => source.EarningEventId))
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act1))
                ;

            CreateMap<ApprenticeshipContractType2EarningEvent, CalculatedRequiredIncentiveAmount>()
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act2))
                ;
            CreateMap<FunctionalSkillEarningsEvent, CalculatedRequiredIncentiveAmount>()
                .ForMember(x => x.StartDate, opt => opt.MapFrom(source => source.StartDate))
                .ForMember(x => x.ContractType, opt => opt.MapFrom(x => x.ContractType))
                ;

            CreateMap<PayableFunctionalSkillEarningEvent, CalculatedRequiredIncentiveAmount>()
                .ForMember(x => x.EarningEventId, opt => opt.MapFrom(src => src.EarningEventId))
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act1))
                .ForMember(x => x.Learner, opt => opt.MapFrom(src => src.Learner))
                .ForMember(x => x.LearningAim, opt => opt.MapFrom(src => src.LearningAim))
                .ForMember(x => x.Ukprn, opt => opt.MapFrom(src => src.Ukprn))
                .Ignore(x => x.AccountId)
                .Ignore(x => x.TransferSenderAccountId)
                .Ignore(x => x.StartDate)
                .Ignore(x => x.PlannedEndDate)
                .Ignore(x => x.ActualEndDate)
                .Ignore(x => x.CompletionStatus)
                .Ignore(x => x.CompletionAmount)
                .Ignore(x => x.IlrFileName)
                .Ignore(x => x.InstalmentAmount)
                .Ignore(x => x.NumberOfInstalments)
                ;

            CreateMap<PayableEarningEvent, CalculatedRequiredLevyAmount>()
                .ForMember(x => x.EarningEventId, opt => opt.MapFrom(source => source.EarningEventId))
                .ForMember(x => x.ContractType, opt => opt.UseValue(ContractType.Act1))
                .ForMember(x => x.Priority, opt => opt.Ignore())
                .ForMember(x => x.ApprenticeshipId, opt => opt.Ignore())
                .ForMember(x => x.ApprenticeshipPriceEpisodeId, opt => opt.Ignore())
                .ForMember(x => x.AgreedOnDate, opt => opt.Ignore())
                .ForMember(x => x.AgeAtStartOfLearning, opt => opt.MapFrom(source => source.AgeAtStartOfLearning))
                .Ignore(x => x.ClawbackSourcePaymentEventId)
                .Ignore(x => x.FundingPlatformType)
                ;
            // End Earning Event --> Required Payment Event

            CreateMap<EarningPeriod, PeriodisedRequiredPaymentEvent>()
                .Include<EarningPeriod, CalculatedRequiredOnProgrammeAmount>()
                .Include<EarningPeriod, CalculatedRequiredCoInvestedAmount>()
                .Include<EarningPeriod, CalculatedRequiredLevyAmount>()
                .Include<EarningPeriod, CalculatedRequiredIncentiveAmount>()
                .ForMember(requiredPayment => requiredPayment.DeliveryPeriod, opt => opt.MapFrom(period => period.Period))
                .ForMember(requiredPayment => requiredPayment.ApprenticeshipId, opt => opt.MapFrom(period => period.ApprenticeshipId))
                .ForMember(requiredPayment => requiredPayment.ApprenticeshipPriceEpisodeId, opt => opt.MapFrom(period => period.ApprenticeshipPriceEpisodeId))
                .ForAllOtherMembers(opt => opt.Ignore())
                ;

            CreateMap<EarningPeriod, CalculatedRequiredOnProgrammeAmount>()
                .Include<EarningPeriod, CalculatedRequiredCoInvestedAmount>()
                .Include<EarningPeriod, CalculatedRequiredLevyAmount>()
                .ForMember(requiredPayment => requiredPayment.SfaContributionPercentage, opt => opt.MapFrom(period => period.SfaContributionPercentage))
                .Ignore(x => x.OnProgrammeEarningType)
                .Ignore(x => x.AgeAtStartOfLearning)
                ;

            CreateMap<EarningPeriod, CalculatedRequiredCoInvestedAmount>()
                .Ignore(x => x.FundingPlatformType)
                ;

            CreateMap<EarningPeriod, CalculatedRequiredLevyAmount>()
                .ForMember(requiredPayment => requiredPayment.ApprenticeshipId, opt => opt.MapFrom(period => period.ApprenticeshipId))
                .ForMember(requiredPayment => requiredPayment.ApprenticeshipPriceEpisodeId, opt => opt.MapFrom(period => period.ApprenticeshipPriceEpisodeId))
                .ForMember(requiredPayment => requiredPayment.ApprenticeshipEmployerType, opt => opt.MapFrom(period => period.ApprenticeshipEmployerType))
                .ForMember(requiredPayment => requiredPayment.Priority, opt => opt.MapFrom(period => period.Priority))
                .ForMember(requiredPayment => requiredPayment.AgreedOnDate, opt => opt.MapFrom(period => period.AgreedOnDate))
                .ForMember(x => x.AgreementId, opt => opt.Ignore())
                .Ignore(x => x.ClawbackSourcePaymentEventId)
                .Ignore(x => x.FundingPlatformType)
                ;

            CreateMap<EarningPeriod, CalculatedRequiredIncentiveAmount>()
                .ForMember(requiredPayment => requiredPayment.ApprenticeshipEmployerType, opt => opt.MapFrom(period => period.ApprenticeshipEmployerType))
                .Ignore(x => x.Type)
                ;

            // Required Payment --> RequiredPaymentEvent
            CreateMap<RequiredPayment, PeriodisedRequiredPaymentEvent>()
                .Include<RequiredPayment, CalculatedRequiredCoInvestedAmount>()
                .Include<RequiredPayment, CalculatedRequiredIncentiveAmount>()
                .Include<RequiredPayment, CalculatedRequiredLevyAmount>()
                .ForMember(x => x.AmountDue, opt => opt.MapFrom(x => x.Amount))
                .ForMember(x => x.AccountId, opt => opt.MapFrom(x => x.AccountId))
                .ForMember(x => x.TransferSenderAccountId, opt => opt.MapFrom(x => x.TransferSenderAccountId))
                .Ignore(x => x.EarningEventId)
                .Ignore(x => x.DeliveryPeriod)
                .Ignore(x => x.JobId)
                .Ignore(x => x.EventTime)
                .Ignore(x => x.EventId)
                .Ignore(x => x.Ukprn)
                .Ignore(x => x.Learner)
                .Ignore(x => x.LearningAim)
                .Ignore(x => x.IlrSubmissionDateTime)
                .Ignore(x => x.IlrFileName)
                .Ignore(x => x.CollectionPeriod)
                .Ignore(x => x.ContractType)
                .Ignore(x => x.StartDate)
                .Ignore(x => x.PlannedEndDate)
                .Ignore(x => x.ActualEndDate)
                .Ignore(x => x.CompletionStatus)
                .Ignore(x => x.CompletionAmount)
                .Ignore(x => x.InstalmentAmount)
                .Ignore(x => x.NumberOfInstalments)
                .Ignore(x => x.LearningStartDate)
                .Ignore(x => x.ApprenticeshipId)
                .Ignore(x => x.TransactionType)
                .Ignore(x => x.ApprenticeshipPriceEpisodeId)
                .Ignore(x => x.NumberOfInstalments)
                .Ignore(x => x.ApprenticeshipEmployerType)
                .Ignore(x => x.ReportingAimFundingLineType)
                .Ignore(x => x.LearningAimSequenceNumber)
                .Ignore(x => x.ClawbackSourcePaymentEventId)
                ;

            CreateMap<RequiredPayment, CalculatedRequiredCoInvestedAmount>()
                .ForMember(x => x.SfaContributionPercentage, opt => opt.MapFrom(x => x.SfaContributionPercentage))
                .Ignore(x => x.OnProgrammeEarningType)
                .Ignore(x => x.AgeAtStartOfLearning)
                .Ignore(x => x.FundingPlatformType)
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
                .Ignore(x => x.AgreedOnDate)
                .Ignore(x => x.ClawbackSourcePaymentEventId)
                .Ignore(x => x.AgeAtStartOfLearning)
                .Ignore(x => x.FundingPlatformType)
                ;

            CreateMap<IdentifiedRemovedLearningAim, PeriodisedRequiredPaymentEvent>()
                .Include<IdentifiedRemovedLearningAim, CalculatedRequiredCoInvestedAmount>()
                .Include<IdentifiedRemovedLearningAim, CalculatedRequiredIncentiveAmount>()
                .Include<IdentifiedRemovedLearningAim, CalculatedRequiredLevyAmount>()
                .ForPath(x => x.Learner.ReferenceNumber, opt => opt.MapFrom(src => src.Learner.ReferenceNumber))
                .ForPath(x => x.Learner.Uln, opt => opt.Ignore())
                .Ignore(x => x.TransferSenderAccountId)
                .Ignore(x => x.EventId)
                .Ignore(x => x.AmountDue)
                .Ignore(x => x.EarningEventId)
                .Ignore(x => x.AccountId)
                .Ignore(x => x.PriceEpisodeIdentifier)
                .Ignore(x => x.DeliveryPeriod)
                .Ignore(x => x.ContractType)
                .Ignore(x => x.StartDate)
                .Ignore(x => x.PlannedEndDate)
                .Ignore(x => x.ActualEndDate)
                .Ignore(x => x.CompletionStatus)
                .Ignore(x => x.CompletionAmount)
                .Ignore(x => x.InstalmentAmount)
                .Ignore(x => x.TransactionType)
                .Ignore(x => x.NumberOfInstalments)
                .Ignore(x => x.LearningStartDate)
                .Ignore(x => x.ApprenticeshipEmployerType)
                .Ignore(x => x.ReportingAimFundingLineType)
                .Ignore(x => x.IlrFileName)
                .Ignore(x => x.ApprenticeshipId)
                .Ignore(x => x.ApprenticeshipPriceEpisodeId)
                .Ignore(x => x.LearningAimSequenceNumber)
                .Ignore(x => x.ClawbackSourcePaymentEventId)
                ;

            CreateMap<IdentifiedRemovedLearningAim, CalculatedRequiredCoInvestedAmount>()
                .Ignore(x => x.SfaContributionPercentage)
                .Ignore(x => x.OnProgrammeEarningType)
                .Ignore(x => x.AgeAtStartOfLearning)
                .Ignore(x => x.FundingPlatformType)
                ;

            CreateMap<IdentifiedRemovedLearningAim, CalculatedRequiredIncentiveAmount>()
                .Ignore(x => x.Type)
                ;

            CreateMap<IdentifiedRemovedLearningAim, CalculatedRequiredLevyAmount>()
                .Ignore(x => x.Priority)
                .Ignore(x => x.ApprenticeshipId)
                .Ignore(x => x.ApprenticeshipPriceEpisodeId)
                .Ignore(x => x.AgreementId)
                .Ignore(x => x.SfaContributionPercentage)
                .Ignore(x => x.OnProgrammeEarningType)
                .Ignore(x => x.AgreedOnDate)
                .Ignore(x => x.AgeAtStartOfLearning)
                .Ignore(x => x.FundingPlatformType)
                ;

            CreateMap<PaymentHistoryEntity, PeriodisedRequiredPaymentEvent>()
                .Include<PaymentHistoryEntity, CalculatedRequiredCoInvestedAmount>()
                .Include<PaymentHistoryEntity, CalculatedRequiredIncentiveAmount>()
                .Include<PaymentHistoryEntity, CalculatedRequiredLevyAmount>()
                .ForMember(x => x.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(x => x.TransferSenderAccountId, opt => opt.MapFrom(src => src.TransferSenderAccountId))
                .ForMember(x => x.CompletionAmount, opt => opt.MapFrom(src => src.CompletionAmount))
                .ForMember(x => x.LearningStartDate, opt => opt.MapFrom(src => src.LearningStartDate))
                .ForMember(x => x.ApprenticeshipId, opt => opt.MapFrom(src => src.ApprenticeshipId))
                .ForMember(x => x.ApprenticeshipPriceEpisodeId, opt => opt.MapFrom(src => src.ApprenticeshipPriceEpisodeId))
                .ForMember(x => x.ApprenticeshipEmployerType, opt => opt.MapFrom(src => src.ApprenticeshipEmployerType))
                .ForMember(x => x.ReportingAimFundingLineType, opt => opt.MapFrom(src => src.ReportingAimFundingLineType))
                .ForPath(x => x.Learner.Uln, opt => opt.MapFrom(src => src.LearnerUln))
                .Ignore(x => x.EventId)
                .Ignore(x => x.AmountDue)
                .Ignore(x => x.EarningEventId)
                .Ignore(x => x.JobId)
                .Ignore(x => x.EventTime)
                .Ignore(x => x.LearningAim)
                .Ignore(x => x.IlrSubmissionDateTime)
                .Ignore(x => x.IlrFileName)
                .Ignore(x => x.LearningAimSequenceNumber)
                .Ignore(x => x.ClawbackSourcePaymentEventId)

                ;

            CreateMap<PaymentHistoryEntity, CalculatedRequiredCoInvestedAmount>()
                .Ignore(x => x.TransactionType)
                .Ignore(x => x.OnProgrammeEarningType)
                .Ignore(x => x.AgeAtStartOfLearning)
                .Ignore(x => x.FundingPlatformType)
                ;

            CreateMap<PaymentHistoryEntity, CalculatedRequiredIncentiveAmount>()
                .Ignore(x => x.TransactionType)
                .Ignore(x => x.Type)
                ;

            CreateMap<PaymentHistoryEntity, CalculatedRequiredLevyAmount>()
                .Ignore(x => x.TransactionType)
                .Ignore(x => x.Priority)
                .Ignore(x => x.ApprenticeshipId)
                .Ignore(x => x.ApprenticeshipPriceEpisodeId)
                .Ignore(x => x.AgreementId)
                .Ignore(x => x.OnProgrammeEarningType)
                .Ignore(x => x.AgreedOnDate)
                .Ignore(x => x.AgeAtStartOfLearning)
                .Ignore(x => x.FundingPlatformType)
                ;

            CreateMap<PriceEpisode, LearningAim>()
                .ForMember(payment => payment.FundingLineType, opt => opt.MapFrom(episode => episode.FundingLineType))
                .Ignore(x => x.Reference)
                .Ignore(x => x.ProgrammeType)
                .Ignore(x => x.StandardCode)
                .Ignore(x => x.FrameworkCode)
                .Ignore(x => x.PathwayCode)
                .Ignore(x => x.SequenceNumber)
                .Ignore(x => x.StartDate);


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
                .Ignore(x => x.TransferSenderAccountId)
                .Ignore(x => x.TransactionType)
                .Ignore(x => x.ContractType)
                .Ignore(x => x.JobId)
                .Ignore(x => x.EventTime)
                .Ignore(x => x.EventId)
                .Ignore(x => x.EarningEventId)
                .Ignore(x => x.Ukprn)
                .Ignore(x => x.Learner)
                .Ignore(x => x.LearningAim)
                .Ignore(x => x.IlrSubmissionDateTime)
                .Ignore(x => x.IlrFileName)
                .Ignore(x => x.CollectionPeriod)
                .Ignore(x => x.LearningStartDate)
                .Ignore(x => x.ApprenticeshipId)
                .Ignore(x => x.ApprenticeshipPriceEpisodeId)
                .Ignore(x => x.ApprenticeshipEmployerType)
                .Ignore(x => x.ReportingAimFundingLineType)
                .Ignore(x => x.ClawbackSourcePaymentEventId)
                ;
            // End Required Payment --> RequiredPaymentEvent

        }
    }
}
