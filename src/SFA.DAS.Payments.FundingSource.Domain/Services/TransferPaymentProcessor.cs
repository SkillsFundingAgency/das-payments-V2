using System;
using System.Collections.Generic;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class TransferPaymentProcessor
    {
        private readonly ILevyBalanceService levyBalanceService;

        public TransferPaymentProcessor(ILevyBalanceService levyBalanceService)
        {
            this.levyBalanceService = levyBalanceService ?? throw new ArgumentNullException(nameof(levyBalanceService));
        }

        public IReadOnlyList<FundingSourcePayment> Process(RequiredPayment requiredPayment)
        {
            //var transferAmount = levyBalanceService.TryFundTransfer(requiredPayment.AmountDue).AsRounded();
            throw new  NotImplementedException();

        }
    }
}