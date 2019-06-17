namespace SFA.DAS.Payments.FundingSource.Domain.Interface
{
    public interface ILevyBalanceService
    {
        decimal RemainingBalance { get;  }
        decimal RemainingTransferAllowance { get; }
        void Initialise(decimal newBalance, decimal transferAllowance);
        decimal TryFund(decimal requiredAmount);
        decimal TryFundTransfer(decimal requiredAmount);
    }
}