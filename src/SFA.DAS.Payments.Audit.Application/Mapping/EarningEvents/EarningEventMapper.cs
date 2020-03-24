using System;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents
{
    public interface IEarningEventMapper
    {
        EarningEventModel Map(EarningEvent earningEvent);
    }

    public class AutoMapperEarningEventMapper : IEarningEventMapper
    {
        private readonly IMapper mapper;

        public AutoMapperEarningEventMapper(IMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public EarningEventModel Map(EarningEvent earningEvent)
        {
            return mapper.Map<EarningEventModel>(earningEvent);
        }
    }

    public class EarningEventMapper : IEarningEventMapper
    {
        public EarningEventModel Map(EarningEvent earningEvent)
        {
            var earningEventModel = MapCommon(earningEvent);


            switch (earningEvent)
            {
                case ApprenticeshipContractType1EarningEvent act1OnProgEarning:
                    MapAct1Earning(act1OnProgEarning, earningEventModel);
                    break;
            }

            return earningEventModel;
        }

        protected void MapAct1Earning(ApprenticeshipContractType1EarningEvent earningEvent, EarningEventModel model)
        {
            model.ContractType = ContractType.Act1;
        }

        protected virtual EarningEventModel MapCommon(EarningEvent earningEvent)
        {
            var model = new EarningEventModel
            {
                EventId = earningEvent.EventId,
                EventTime = earningEvent.EventTime,
                JobId = earningEvent.JobId,
                IlrFileName = earningEvent.IlrFileName,
                IlrSubmissionDateTime = earningEvent.IlrSubmissionDateTime,
                AcademicYear = earningEvent.CollectionPeriod.AcademicYear,
                CollectionPeriod = earningEvent.CollectionPeriod.Period,
            };
            MapLearner(earningEvent, model);
            MapCourse(earningEvent, model);
            return model;
        }

        protected virtual void MapLearner(EarningEvent earningEvent, EarningEventModel model)
        {
            if (earningEvent.Learner == null)
                throw new InvalidOperationException($"No learner details found in earning event: {earningEvent.GetType().FullName}, Job: {earningEvent.JobId}");
            model.LearnerUln = earningEvent.Learner.Uln;
            model.LearnerReferenceNumber = earningEvent.Learner.ReferenceNumber;
        }

        protected virtual void MapCourse(EarningEvent earningEvent, EarningEventModel model)
        {
            if (earningEvent.LearningAim == null)
                throw new InvalidOperationException($"No learner course details found in earning event: {earningEvent.GetType().FullName}, Job: {earningEvent.JobId}, Learner ref: {earningEvent.Learner.ReferenceNumber}");

            model.LearningAimSequenceNumber = earningEvent.LearningAim.SequenceNumber;
            model.LearningAimFrameworkCode = earningEvent.LearningAim.FrameworkCode;
            model.LearningAimPathwayCode = earningEvent.LearningAim.PathwayCode;
            model.LearningAimProgrammeType = earningEvent.LearningAim.ProgrammeType;
            model.LearningAimStandardCode = earningEvent.LearningAim.StandardCode;
            model.LearningAimReference = earningEvent.LearningAim.Reference;
            model.LearningAimFundingLineType = earningEvent.LearningAim.FundingLineType;
            model.LearningStartDate = earningEvent.LearningAim.StartDate;
        }
    }
}