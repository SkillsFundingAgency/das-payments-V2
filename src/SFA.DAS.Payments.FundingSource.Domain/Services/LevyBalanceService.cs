using System;
using SFA.DAS.Payments.FundingSource.Domain.Interface;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{



    public class LevyBalanceService : ILevyBalanceService
    {
        public decimal RemainingBalance { get; private set; }
        public decimal RemainingTransferAllowance { get; private set; }
        bool initialised;

        public void Initialise(decimal newBalance, decimal transferAllowance)
        {
            RemainingBalance = Math.Max(newBalance, 0);
            this.RemainingTransferAllowance = Math.Min(Math.Max(transferAllowance, 0), RemainingBalance);
            initialised = true;
        }

        public decimal TryFund(decimal requiredAmount)
        {
            if (!initialised)
                throw new InvalidOperationException("LevyBalanceService is not initialised");

            var amountAvailable = requiredAmount > 0 ? Math.Min(RemainingBalance, requiredAmount) : requiredAmount;

            RemainingBalance -= amountAvailable;
            RemainingTransferAllowance = Math.Min(RemainingBalance, RemainingTransferAllowance);
            return amountAvailable;
        }

        public decimal TryFundTransfer(decimal requiredAmount)
        {
            if (!initialised)
                throw new InvalidOperationException("LevyBalanceService is not initialised");

            var amountFunded = requiredAmount > 0 ? Math.Min(RemainingTransferAllowance, requiredAmount) : requiredAmount;

            RemainingBalance -= amountFunded;
            RemainingTransferAllowance -= amountFunded;
            return amountFunded;
        }
    }
}
