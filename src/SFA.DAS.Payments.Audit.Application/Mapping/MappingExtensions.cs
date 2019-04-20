﻿using AutoMapper;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Audit.Application.Mapping
{
    public static class MappingExtensions
    {
        public static IMappingExpression<TSource, TDest> MapPeriodisedCommon<TSource, TDest>(this IMappingExpression<TSource, TDest> mappingExpression)
            where TSource : PeriodisedPaymentEvent
            where TDest : PeriodisedPaymentsEventModel
        {
            return mappingExpression
                    .MapCommon()
                    .ForMember(dest => dest.Amount, opt => opt.MapFrom(source => source.AmountDue))
                    .ForMember(dest => dest.DeliveryPeriod, opt => opt.MapFrom(source => source.DeliveryPeriod))
                    .ForMember(dest => dest.AgreementId, opt => opt.Ignore())
                    .ForMember(dest => dest.StartDate, opt => opt.MapFrom(source => source.StartDate))
                    .ForMember(dest => dest.PlannedEndDate, opt => opt.MapFrom(source => source.PlannedEndDate))
                    .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(source => source.ActualEndDate))
                    .ForMember(dest => dest.CompletionStatus, opt => opt.MapFrom(source => source.CompletionStatus))
                    .ForMember(dest => dest.CompletionAmount, opt => opt.MapFrom(source => source.CompletionAmount))
                    .ForMember(dest => dest.InstalmentAmount, opt => opt.MapFrom(source => source.InstalmentAmount))
                    .ForMember(dest => dest.NumberOfInstalments, opt => opt.MapFrom(source => source.NumberOfInstalments));
        }

        public static IMappingExpression<TSource, TDest> MapCommon<TSource, TDest>(this IMappingExpression<TSource, TDest> mappingExpression)
            where TSource : PaymentsEvent
            where TDest : PaymentsEventModel
        {
            return mappingExpression
                    .ForMember(dest => dest.EventId, opt => opt.MapFrom(source => source.EventId))
                    .ForMember(dest => dest.CollectionPeriod, opt => opt.MapFrom(source => source.CollectionPeriod.Period))
                    .ForMember(dest => dest.AcademicYear, opt => opt.MapFrom(source => source.CollectionPeriod.AcademicYear))
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
                    .ForMember(dest => dest.LearningAimStandardCode, opt => opt.MapFrom(source => source.LearningAim.StandardCode));
        }

    }
}