using SFA.DAS.Payments.EarningEvents.Model.Entities;

namespace SFA.DAS.Payments.EarningEvents.Domain
{
    public interface ISubmissionEventProcessor
    {
        LegacySubmissionEvent ProcessSubmission(SubmittedPriceEpisodeEntity currentEpisode, SubmittedPriceEpisodeEntity lastSeenEpisode);
    }

    public class SubmissionEventProcessor : ISubmissionEventProcessor
    {
        public LegacySubmissionEvent ProcessSubmission(SubmittedPriceEpisodeEntity currentEpisode, SubmittedPriceEpisodeEntity lastSeenEpisode)
        {
            return null;
        }
    }
}
