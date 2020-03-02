namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class LevyAccountModel
    {
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal Balance { get; set; }
        public bool IsLevyPayer { get; set; }
        public decimal TransferAllowance { get; set; }
    }

    //todo write apprenticeship and price episode to datacontext
    //create a builder for this - max levy by default
    //write this to DB too
}
