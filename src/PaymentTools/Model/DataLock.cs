namespace PaymentTools.Model
{
    public class DataLock : ICommitmentItem
    {
        public long Id { get; set; }

        public decimal Amount { get; set; }

        public int DataLockNumber { get; set; }

        public string Type => "Data Lock";
    }
}