using System.Collections.Generic;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class LevyPaymentProcessor : ILevyPaymentProcessor
    {
        private readonly ILevyBalanceService levyBalanceService;

        public LevyPaymentProcessor(ILevyBalanceService levyBalanceService)
        {
            this.levyBalanceService = levyBalanceService;
        }

        public IReadOnlyList<FundingSourcePayment> Process(RequiredPayment requiredPayment)
        {
            var amountDue = levyBalanceService.TryFund(requiredPayment.AmountDue).AsRounded();

            if (amountDue == 0) 
                return new FundingSourcePayment[0];

            return new[]
            {
                new LevyPayment
                {
                    AmountDue = amountDue,
                    Type = FundingSourceType.Levy
                }
            };
        }
    }
}