using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class ApprenticeshipContractTypeEarningsEventBuilder : IApprenticeshipContractTypeEarningsEventBuilder
    {
        private readonly IApprenticeshipContractTypeEarningsEventFactory factory;
        private readonly IMapper mapper;

        public ApprenticeshipContractTypeEarningsEventBuilder(IApprenticeshipContractTypeEarningsEventFactory factory, IMapper mapper)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public List<ApprenticeshipContractTypeEarningsEvent> Build(ProcessLearnerCommand learnerSubmission)
        {
            var intermediateResults = InitialLearnerTransform(learnerSubmission);
            var results = new List<ApprenticeshipContractTypeEarningsEvent>();

            foreach (var intermediateLearningAim in intermediateResults)
            {
                var episodesByContractType =
                    intermediateLearningAim.PriceEpisodes.GroupBy(x => x.PriceEpisodeValues.PriceEpisodeContractType);

                foreach (var priceEpisodes in episodesByContractType)
                {
                    var learnerWithSortedPriceEpisodes = intermediateLearningAim
                        .CopyReplacingPriceEpisodes(priceEpisodes);
                    var earningEvent = factory.Create(priceEpisodes.Key);
                    mapper.Map(learnerWithSortedPriceEpisodes, earningEvent);
                    results.Add(earningEvent);
                }
            }
            
            return results;
        }

        List<IntermediateLearningAim> InitialLearnerTransform(ProcessLearnerCommand learnerSubmission)
        {
            var results = new List<IntermediateLearningAim>();

            foreach (var learningDelivery in learnerSubmission.Learner.LearningDeliveries)
            {
                var priceEpisodes = learnerSubmission.Learner.PriceEpisodes
                    .Where(x => x.PriceEpisodeValues.PriceEpisodeAimSeqNumber == learningDelivery.AimSeqNumber);
                var intermediateAim = new IntermediateLearningAim(learnerSubmission, priceEpisodes, learningDelivery);
                results.Add(intermediateAim);
            }

            return results;
        }
    }

    class IntermediateLearningAim
    {
        public IntermediateLearningAim(
            ProcessLearnerCommand command, IEnumerable<PriceEpisode> priceEpisodes, LearningDelivery aim)
        {
            Aim = aim;
            PriceEpisodes.AddRange(priceEpisodes);
            Learner = command.Learner;
            Ukprn = command.Ukprn;
            CollectionYear = command.CollectionYear;
            CollectionPeriod = command.CollectionPeriod;
            IlrSubmissionDateTime = command.IlrSubmissionDateTime;
            JobId = command.JobId;
        }

        protected IntermediateLearningAim(FM36Learner learner, LearningDelivery aim,
            IEnumerable<PriceEpisode> priceEpisodes, long ukprn, string collectionYear,
            int collectionPeriod, DateTime ilrSubmissionDateTime, long jobId)
        {
            Aim = aim;
            Learner = learner;
            PriceEpisodes.AddRange(priceEpisodes);
            Ukprn = ukprn;
            CollectionYear = collectionYear;
            CollectionPeriod = collectionPeriod;
            IlrSubmissionDateTime = ilrSubmissionDateTime;
            JobId = jobId;
        }

        public IntermediateLearningAim CopyReplacingPriceEpisodes(IEnumerable<PriceEpisode> priceEpisodes)
        {
            var copy = new IntermediateLearningAim(Learner, Aim, priceEpisodes, Ukprn, 
                CollectionYear, CollectionPeriod, IlrSubmissionDateTime, JobId);
            return copy;
        }

        public LearningDelivery Aim { get; protected set; }
        public List<PriceEpisode> PriceEpisodes { get; protected set; } = new List<PriceEpisode>();
        public FM36Learner Learner { get; protected set; }
        public long Ukprn { get; protected set; }
        public string CollectionYear { get; protected set; }
        public int CollectionPeriod { get; protected set; }
        public DateTime IlrSubmissionDateTime { get; protected set; }
        public long JobId { get; set; }
    }
}