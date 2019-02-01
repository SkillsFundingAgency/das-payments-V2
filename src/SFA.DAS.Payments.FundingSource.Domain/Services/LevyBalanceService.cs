using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public interface ILevyBalanceService
    {
        void Initialise(decimal newBalance);
        decimal TryFund(decimal requiredAmount);
    }

    public class LevyBalanceService : ILevyBalanceService
    {
        private decimal balance;

        public void Initialise(decimal newBalance)
        {
            balance = newBalance;
        }

        public decimal TryFund(decimal requiredAmount)
        {
            var amountAvailable = requiredAmount > 0 ? Math.Min(balance, requiredAmount) : requiredAmount;

            balance -= amountAvailable;

            return amountAvailable;
        }
    }
}
