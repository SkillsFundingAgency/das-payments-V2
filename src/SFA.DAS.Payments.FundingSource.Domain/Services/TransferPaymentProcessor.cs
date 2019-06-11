using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class TransferPaymentProcessor: ITransferPaymentProcessor
    {
        private readonly ILevyBalanceService levyBalanceService;

        public TransferPaymentProcessor(ILevyBalanceService levyBalanceService)
        {
            this.levyBalanceService = levyBalanceService ?? throw new ArgumentNullException(nameof(levyBalanceService));
        }

        public IReadOnlyList<FundingSourcePayment> Process(RequiredPayment requiredPayment)
        {
            var transferAmount = levyBalanceService.TryFundTransfer(requiredPayment.AmountDue).AsRounded();
            return transferAmount == 0
                ? new FundingSourcePayment[0]
                : new[] { new TransferPayment { Type = FundingSourceType.Transfer, AmountDue = transferAmount }, };
        }
    }
}