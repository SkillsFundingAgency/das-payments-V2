namespace SFA.DAS.Payments.Model.Core.Entities
{
    public enum FundingSourceType : byte
    {
        Levy = 1,
        CoInvestedSfa = 2,
        CoInvestedEmployer = 3,
        FullyFundedSfa = 4,
        Transfer = 5,
    }
}
