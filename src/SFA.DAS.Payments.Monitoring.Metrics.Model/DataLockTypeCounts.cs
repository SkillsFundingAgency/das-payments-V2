using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.Payments.Monitoring.Metrics.Model
{
    [NotMapped]
    public class DataLockTypeCounts
    {
        public int DataLock1 { get; set; }
        public int DataLock2 { get; set; }
        public int DataLock3 { get; set; }
        public int DataLock4 { get; set; }
        public int DataLock5 { get; set; }
        public int DataLock6 { get; set; }
        public int DataLock7 { get; set; }
        public int DataLock8 { get; set; }
        public int DataLock9 { get; set; }
        public int DataLock10 { get; set; }
        public int DataLock11 { get; set; }
        public int DataLock12 { get; set; }
        public int Total =>
            DataLock1 +
            DataLock2 +
            DataLock3 +
            DataLock4 +
            DataLock5 +
            DataLock6 +
            DataLock7 +
            DataLock8 +
            DataLock9 +
            DataLock10 +
            DataLock11 +
            DataLock12;
    }
}