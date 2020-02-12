namespace PaymentTools.Model
{
    public interface ICommitmentItem
    {
        public string Type { get; }

        public decimal Amount { get; }
    }
}