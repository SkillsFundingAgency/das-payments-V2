using System.Collections.Generic;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class DataLockFailureModel
    {
        public long Id { get; set; }
        public long Ukprn { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public long LearnerUln { get; set; }
        public string LearningAimReference { get; set; }
        public int LearningAimProgrammeType { get; set; }
        public int LearningAimStandardCode { get; set; }
        public int LearningAimFrameworkCode { get; set; }
        public int LearningAimPathwayCode { get; set; }
        public short AcademicYear { get; set; }
        public byte DeliveryPeriod { get; set; }
        public byte CollectionPeriod { get; set; }

        public TransactionType TransactionType { get; set; }
        public string EarningPeriod { get; set; }
    }
}