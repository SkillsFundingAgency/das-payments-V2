using System;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.ServiceFabric.Core.Messaging
{
    public class EarningEventKey
    {
        public long JobId { get; set; }
        public long Ukprn { get; set; }
        public Learner Learner { get; set; }
        public LearningAim LearningAim { get; set; }
        public CollectionPeriod CollectionPeriod { get; set; }
        public virtual string Key => CreateKey();
        public virtual string LogSafeKey => CreateLogSafeKey();

        protected EarningEventKey()
        {

        }

        public EarningEventKey(PaymentsEvent earningEvent)
        {
            if (earningEvent == null) throw new ArgumentNullException(nameof(earningEvent));
            JobId = earningEvent.JobId;
            Ukprn = earningEvent.Ukprn;
            CollectionPeriod = earningEvent.CollectionPeriod;
            Learner = new Learner
            {
                Uln = earningEvent.Learner.Uln,
                ReferenceNumber = earningEvent.Learner.ReferenceNumber
            };
            LearningAim = new LearningAim
            {
                StartDate = earningEvent.LearningAim.StartDate,
                FrameworkCode = earningEvent.LearningAim.FrameworkCode,
                FundingLineType = earningEvent.LearningAim.FundingLineType,
                Reference = earningEvent.LearningAim.Reference,
                SequenceNumber = earningEvent.LearningAim.SequenceNumber,
                PathwayCode = earningEvent.LearningAim.PathwayCode,
                StandardCode = earningEvent.LearningAim.StandardCode,
                ProgrammeType = earningEvent.LearningAim.ProgrammeType
            };
        }

        private string CreateKey()
        {
            return $@"{JobId}-{Ukprn}-{CollectionPeriod.AcademicYear}-{CollectionPeriod.Period}-
                        {Learner.Uln}-{Learner.ReferenceNumber}-{LearningAim.Reference}-
                        {LearningAim.ProgrammeType}-{LearningAim.StandardCode}-{LearningAim.FrameworkCode}-
                        {LearningAim.PathwayCode}-{LearningAim.FundingLineType}-{LearningAim.SequenceNumber}-
                        {LearningAim.StartDate:G}-{GetType().Name}";
        }

        private string CreateLogSafeKey()
        {
            return $@"{JobId}-{CollectionPeriod.AcademicYear}-{CollectionPeriod.Period}-
                        {Learner.ReferenceNumber}-{LearningAim.Reference}-
                        {LearningAim.ProgrammeType}-{LearningAim.StandardCode}-{LearningAim.FrameworkCode}-
                        {LearningAim.PathwayCode}-{LearningAim.FundingLineType}-{LearningAim.SequenceNumber}-
                        {LearningAim.StartDate:G}-{GetType().Name}";
        }
    }
}