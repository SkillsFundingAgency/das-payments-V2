using System;
using AutoMapper;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.ProviderPayments.Application.Mapping
{

    public class ProviderPaymentsProfile : Profile
    {
        public ProviderPaymentsProfile()
        {
           
            CreateMap<FundingSourcePaymentEvent, PaymentModel>()
                .Include<EmployerCoInvestedFundingSourcePaymentEvent, PaymentModel>()
                .Include<SfaCoInvestedFundingSourcePaymentEvent, PaymentModel>()
                .ForMember(dest => dest.ExternalId, opt => opt.ResolveUsing(src => Guid.NewGuid()))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CollectionPeriod, opt => opt.MapFrom(source => new CalendarPeriod(source.CollectionPeriod.Year, source.CollectionPeriod.Month)))
                .ForMember(dest => dest.DeliveryPeriod, opt => opt.MapFrom(source => new CalendarPeriod(source.DeliveryPeriod.Year, source.DeliveryPeriod.Month)))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(source => source.AmountDue))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(source => source.ContractType))
                .ForMember(dest => dest.FundingSource, opt => opt.MapFrom(source => source.FundingSourceType))
                .ForMember(dest => dest.IlrSubmissionDateTime, opt => opt.MapFrom(source => source.IlrSubmissionDateTime))
                .ForMember(dest => dest.JobId, opt => opt.MapFrom(source => source.JobId))
                .ForMember(dest => dest.LearnerReferenceNumber, opt => opt.MapFrom(source => source.Learner.ReferenceNumber))
                .ForMember(dest => dest.LearningAimFrameworkCode, opt => opt.MapFrom(source => source.LearningAim.FrameworkCode))
                .ForMember(dest => dest.LearningAimFundingLineType, opt => opt.MapFrom(source => source.LearningAim.FundingLineType))
                .ForMember(dest => dest.LearningAimPathwayCode, opt => opt.MapFrom(source => source.LearningAim.PathwayCode))
                .ForMember(dest => dest.LearningAimProgrammeType, opt => opt.MapFrom(source => source.LearningAim.ProgrammeType))
                .ForMember(dest => dest.LearningAimReference, opt => opt.MapFrom(source => source.LearningAim.Reference))
                .ForMember(dest => dest.LearningAimStandardCode, opt => opt.MapFrom(source => source.LearningAim.StandardCode))
                .ForMember(dest => dest.PriceEpisodeIdentifier, opt => opt.MapFrom(source => source.PriceEpisodeIdentifier))
                .ForMember(dest => dest.SfaContributionPercentage, opt => opt.MapFrom(source => source.SfaContributionPercentage))
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => source.OnProgrammeEarningType))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn));

            CreateMap<EmployerCoInvestedFundingSourcePaymentEvent, PaymentModel>();
            CreateMap<SfaCoInvestedFundingSourcePaymentEvent, PaymentModel>();

            CreateMap<PaymentModel, ProviderPaymentEvent>()
                .Include<PaymentModel, EmployerCoInvestedProviderPaymentEvent>()
                .Include<PaymentModel, SfaCoInvestedProviderPaymentEvent>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(source => source.ExternalId))
                .ForMember(dest => dest.EventTime, opt => opt.ResolveUsing(src => DateTime.UtcNow))
                .ForMember(dest => dest.CollectionPeriod, opt => opt.MapFrom(source =>  new CalendarPeriod(source.CollectionPeriod.Year, source.CollectionPeriod.Month)))
                .ForMember(dest => dest.DeliveryPeriod, opt => opt.MapFrom(source => new CalendarPeriod(source.DeliveryPeriod.Year, source.DeliveryPeriod.Month)))
                .ForMember(dest => dest.AmountDue, opt => opt.MapFrom(source => source.Amount))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(source => source.ContractType))
                .ForMember(dest => dest.FundingSourceType, opt => opt.MapFrom(source => source.FundingSource))
                .ForMember(dest => dest.IlrSubmissionDateTime, opt => opt.MapFrom(source => source.IlrSubmissionDateTime))
                .ForMember(dest => dest.JobId, opt => opt.MapFrom(source => source.JobId))
                .ForPath(dest => dest.Learner.ReferenceNumber, opt => opt.MapFrom(source => source.LearnerReferenceNumber))
                .ForPath(dest => dest.LearningAim.FrameworkCode, opt => opt.MapFrom(source => source.LearningAimFrameworkCode))
                .ForPath(dest => dest.LearningAim.FundingLineType, opt => opt.MapFrom(source => source.LearningAimFundingLineType))
                .ForPath(dest => dest.LearningAim.PathwayCode, opt => opt.MapFrom(source => source.LearningAimPathwayCode))
                .ForPath(dest => dest.LearningAim.ProgrammeType, opt => opt.MapFrom(source => source.LearningAimProgrammeType))
                .ForPath(dest => dest.LearningAim.Reference, opt => opt.MapFrom(source => source.LearningAimReference))
                .ForPath(dest => dest.LearningAim.StandardCode, opt => opt.MapFrom(source => source.LearningAimStandardCode))
                .ForMember(dest => dest.PriceEpisodeIdentifier, opt => opt.MapFrom(source => source.PriceEpisodeIdentifier))
                .ForMember(dest => dest.SfaContributionPercentage, opt => opt.MapFrom(source => source.SfaContributionPercentage))
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(source => source.TransactionType))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn));

            CreateMap<PaymentModel, EmployerCoInvestedProviderPaymentEvent>();
            CreateMap<PaymentModel, SfaCoInvestedProviderPaymentEvent>();
        }
    }
}