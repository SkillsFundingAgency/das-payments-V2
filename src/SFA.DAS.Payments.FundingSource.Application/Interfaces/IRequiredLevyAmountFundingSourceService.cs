﻿using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IRequiredLevyAmountFundingSourceService
    {
        Task IlrSubmitted(ReceivedProviderEarningsEvent message);
        Task AddRequiredPayment(CalculatedRequiredLevyAmount paymentEvent);
        Task<ReadOnlyCollection<FundingSourcePaymentEvent>> GetFundedPayments(long employerAccountId, long jobId);
    }
}