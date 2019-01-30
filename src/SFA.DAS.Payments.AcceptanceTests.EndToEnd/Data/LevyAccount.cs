namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class LevyAccount
    {
        public long AccountId { get; set; }
        public string AccountHashId { get; set; }
        public string AccountName { get; set; }
        public decimal Balance { get; set; }
        public long SequenceId { get; set; }
        public bool IsLevyPayer { get; set; }
        public decimal TransferAllowance { get; set; }
    }
}
