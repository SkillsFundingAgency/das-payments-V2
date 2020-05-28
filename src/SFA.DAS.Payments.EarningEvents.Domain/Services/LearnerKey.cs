using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Domain.Services
{
    public sealed class LearnerKey
    {
        private long JobId { get; }
        private long Ukprn { get; }
        private short AcademicYear { get; }
        private int CollectionPeriod { get; }
        private long LearnerUln { get; }
        private string LearnerRefNo { get; }
        public string Key => CreateKey();
        public string LogSafeKey => CreateLogSafeKey();
        
        public LearnerKey(ProcessLearnerCommand processLearnerCommand)
        {
            JobId = processLearnerCommand.JobId;
            AcademicYear = processLearnerCommand.CollectionYear;
            CollectionPeriod = processLearnerCommand.CollectionPeriod;
            LearnerUln = processLearnerCommand.Learner.ULN;
            LearnerRefNo = processLearnerCommand.Learner.LearnRefNumber;
            Ukprn = processLearnerCommand.Ukprn;
        }

        private string CreateKey()
        {
            return $"{JobId}-{Ukprn}-{LearnerRefNo}-{LearnerUln}-{AcademicYear}-{CollectionPeriod}";
        }

        private string CreateLogSafeKey()
        {
            return $"{JobId}-{LearnerRefNo}-{AcademicYear}-{CollectionPeriod}";
        }
    }
}