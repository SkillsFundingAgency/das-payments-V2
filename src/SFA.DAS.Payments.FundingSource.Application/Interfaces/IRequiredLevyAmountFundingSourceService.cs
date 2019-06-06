﻿using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IRequiredLevyAmountFundingSourceService
    {
        Task AddRequiredPayment(CalculatedRequiredLevyAmount paymentEvent);
        Task<ReadOnlyCollection<FundingSourcePaymentEvent>> ProcessReceiverTransferPayment(ProcessUnableToFundTransferFundingSourcePayment message);
        Task<ReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(long employerAccountId, long jobId);
    }
}