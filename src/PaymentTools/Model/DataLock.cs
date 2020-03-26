using SFA.DAS.Payments.Model.Core;

namespace PaymentTools.Model
{
    public class DataLock : ICommitmentItem
    {
        public long Id { get; set; }

        public decimal Amount { get; set; }

        public DataLockErrorCode DataLockErrorCode { get; set; }

        public byte DeliveryPeriod { get; set; }

        public string Type => "Data Lock";
    }
}