﻿using SFA.DAS.Payments.FundingSource.Domain.Models;

namespace SFA.DAS.Payments.FundingSource.Domain.Interface
{
    // TODO: this is to be removed when NonLevy is refactored to use composite ICoInvestedPaymentProcessor
    public interface ICoInvestedPaymentProcessorOld
    {
        FundingSourcePayment Process(RequiredPayment requiredPayment);
    }

    public interface IEmployerCoInvestedPaymentProcessor : ICoInvestedPaymentProcessorOld
    {
    }

    public interface ISfaCoInvestedPaymentProcessor : ICoInvestedPaymentProcessorOld
    {
    }
}