namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class Employer
    {
        public string Identifier { get; set; }
        public long AccountId { get; set; }
        public string AccountHashId { get; set; }
        public string AccountName { get; set; }
        public decimal Balance { get; set; }
        public long SequenceId { get; set; }
        public bool IsLevyPayer { get; set; }
        public decimal TransferAllowance => Balance * .25M;
    }
}
