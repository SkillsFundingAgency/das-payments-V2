using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Domain.Services
{
    public class LearnerKey
    {
        public long JobId { get; set; }
        public long Ukprn { get; set; }
        public short AcademicYear { get; set; }
        public int CollectionPeriod { get; set; }
        public long LearnerUln { get; set; }
        public virtual string Key => CreateKey();
        public virtual string LogSafeKey => CreateLogSafeKey();


        public LearnerKey()
        {
            
        }

        public LearnerKey(ProcessLearnerCommand processLearnerCommand)
        {
            JobId = processLearnerCommand.JobId;
            AcademicYear = processLearnerCommand.CollectionYear;
            CollectionPeriod = processLearnerCommand.CollectionPeriod;
            LearnerUln = processLearnerCommand.Learner.ULN;
        }

        public string CreateKey()
        {
            return $"{LearnerUln}-{JobId}-{AcademicYear}-{CollectionPeriod}-{GetType().Name}";
        }

        public string CreateLogSafeKey()
        {
            return $"{JobId}-{AcademicYear}-{CollectionPeriod}-{GetType().Name}";
        }
    }
}