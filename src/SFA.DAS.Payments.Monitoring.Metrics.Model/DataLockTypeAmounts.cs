namespace SFA.DAS.Payments.Monitoring.Metrics.Model
{
    public class DataLockTypeAmounts
    {
        public decimal DataLock1 { get; set; }
        public decimal DataLock2 { get; set; }
        public decimal DataLock3 { get; set; }
        public decimal DataLock4 { get; set; }
        public decimal DataLock5 { get; set; }
        public decimal DataLock6 { get; set; }
        public decimal DataLock7 { get; set; }
        public decimal DataLock8 { get; set; }
        public decimal DataLock9 { get; set; }
        public decimal DataLock10 { get; set; }
        public decimal DataLock11 { get; set; }
        public decimal DataLock12 { get; set; }
        public decimal Total =>
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