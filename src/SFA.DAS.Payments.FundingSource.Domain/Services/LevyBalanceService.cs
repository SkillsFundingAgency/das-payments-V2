using System;
using SFA.DAS.Payments.FundingSource.Domain.Interface;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{



    public class LevyBalanceService : ILevyBalanceService
    {
        private decimal balance;
        private decimal transferAllowance;
        bool initialised;

        public void Initialise(decimal newBalance, decimal transferAllowance)
        {
            balance = newBalance;
            this.transferAllowance = Math.Min(transferAllowance, balance);
            initialised = true;
        }

        public decimal TryFund(decimal requiredAmount)
        {
            if (!initialised)
                throw new InvalidOperationException("LevyBalanceService is not initialised");

            var amountAvailable = requiredAmount > 0 ? Math.Min(balance, requiredAmount) : requiredAmount;

            balance -= amountAvailable;
            transferAllowance = Math.Min(balance, transferAllowance);
            return amountAvailable;
        }

        public decimal TryFundTransfer(decimal requiredAmount)
        {
            if (!initialised)
                throw new InvalidOperationException("LevyBalanceService is not initialised");

            var amountFunded = requiredAmount > 0 ? Math.Min(transferAllowance, requiredAmount) : requiredAmount;

            balance -= amountFunded;
            transferAllowance -= amountFunded;
            return amountFunded;
        }
    }
}
