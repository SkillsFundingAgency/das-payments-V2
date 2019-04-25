using System;
using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class IntermediateLearningAim
    {
        public LearningDelivery Aim { get; protected set; }
        public List<PriceEpisode> PriceEpisodes { get; protected set; } = new List<PriceEpisode>();
        public FM36Learner Learner { get; protected set; }
        public long Ukprn { get; protected set; }
        public short AcademicYear { get; protected set; }
        public int CollectionPeriod { get; protected set; }
        public DateTime IlrSubmissionDateTime { get; protected set; }
        public long JobId { get; set; }
        
        public IntermediateLearningAim(
            ProcessLearnerCommand command, 
            IEnumerable<PriceEpisode> priceEpisodes, 
            LearningDelivery aim)
        {
            Aim = aim;
            PriceEpisodes.AddRange(priceEpisodes);
            Learner = command.Learner;
            Ukprn = command.Ukprn;
            AcademicYear = command.CollectionYear;
            CollectionPeriod = command.CollectionPeriod;
            IlrSubmissionDateTime = command.IlrSubmissionDateTime;
            JobId = command.JobId;
        }

        protected IntermediateLearningAim(
            FM36Learner learner, 
            LearningDelivery aim,
            IEnumerable<PriceEpisode> priceEpisodes, 
            long ukprn, 
            short collectionYear,
            int collectionPeriod, 
            DateTime ilrSubmissionDateTime, 
            long jobId)
        {
            Aim = aim;
            Learner = learner;
            PriceEpisodes.AddRange(priceEpisodes);
            Ukprn = ukprn;
            AcademicYear = collectionYear;
            CollectionPeriod = collectionPeriod;
            IlrSubmissionDateTime = ilrSubmissionDateTime;
            JobId = jobId;
        }

        public IntermediateLearningAim CopyReplacingPriceEpisodes(IEnumerable<PriceEpisode> priceEpisodes)
        {
            var copy = new IntermediateLearningAim(Learner, Aim, priceEpisodes, Ukprn, 
                AcademicYear, CollectionPeriod, IlrSubmissionDateTime, JobId);
            return copy;
        }
    }
}