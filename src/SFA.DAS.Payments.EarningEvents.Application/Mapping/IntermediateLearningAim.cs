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
            PriceEpisodes.AddRange(FilterPriceEpisodes(priceEpisodes, command.CollectionYear));
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
            PriceEpisodes.AddRange(FilterPriceEpisodes(priceEpisodes, collectionYear));
            Ukprn = ukprn;
            AcademicYear = collectionYear;
            CollectionPeriod = collectionPeriod;
            IlrSubmissionDateTime = ilrSubmissionDateTime;
            JobId = jobId;
        }

        private IEnumerable<PriceEpisode> FilterPriceEpisodes(IEnumerable<PriceEpisode> priceEpisodes, short currentAcademicYear)
        {
            var calendarYear = currentAcademicYear / 100 + 2000;
            var yearStartDate = new DateTime(calendarYear, 8, 1);
            var yearEndDate = yearStartDate.AddYears(1);
            var currentYearPriceEpisodes = new List<PriceEpisode>();

            foreach (var episode in priceEpisodes)
            {
                var episodeStartDate = episode.PriceEpisodeValues.EpisodeStartDate;

                if (!episodeStartDate.HasValue)
                    throw new ApplicationException($"Price episode {episode.PriceEpisodeIdentifier} has no EpisodeStartDate");

                if (episodeStartDate.Value >= yearStartDate && episodeStartDate.Value < yearEndDate) 
                    currentYearPriceEpisodes.Add(episode);
            }

            return currentYearPriceEpisodes;
        }

        public IntermediateLearningAim CopyReplacingPriceEpisodes(IEnumerable<PriceEpisode> priceEpisodes)
        {
            var copy = new IntermediateLearningAim(Learner, Aim, priceEpisodes, Ukprn, 
                AcademicYear, CollectionPeriod, IlrSubmissionDateTime, JobId);
            return copy;
        }
    }
}