using System;

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
        bool initialised;

        public void Initialise(decimal newBalance)
        {
            balance = newBalance;
            initialised = true;
        }

        public decimal TryFund(decimal requiredAmount)
        {
            if (!initialised)
                throw new ApplicationException("LevyBalanceService is not initialised");

            var amountAvailable = requiredAmount > 0 ? Math.Min(balance, requiredAmount) : requiredAmount;

            balance -= amountAvailable;

            return amountAvailable;
        }
    }
}
