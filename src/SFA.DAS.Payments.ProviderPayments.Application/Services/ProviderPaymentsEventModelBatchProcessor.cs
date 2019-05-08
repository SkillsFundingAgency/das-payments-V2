using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class ProviderPaymentsEventModelBatchProcessor: PaymentsEventModelBatchProcessor<ProviderPaymentEventModel>
    {
        private readonly IDataCache<ReceivedProviderEarningsEvent> ilrSubmittedEventCache;

        public ProviderPaymentsEventModelBatchProcessor(IPaymentsEventModelCache<ProviderPaymentEventModel> cache, 
            IPaymentsEventModelDataTable<ProviderPaymentEventModel> dataTable, IConfigurationHelper configurationHelper, 
            IPaymentLogger logger, IDataCache<ReceivedProviderEarningsEvent> ilrSubmittedEventCache) 
            : base(cache, dataTable, configurationHelper, logger)
        {
            this.ilrSubmittedEventCache = ilrSubmittedEventCache ?? throw new ArgumentNullException(nameof(ilrSubmittedEventCache));
        }

        protected override async Task<bool> AllowPayment(ProviderPaymentEventModel paymentModel)
        {
            var item = await ilrSubmittedEventCache.TryGet(paymentModel.Ukprn.ToString());
            if (!item.HasValue)
                return true;
            return paymentModel.IlrSubmissionDateTime >= item.Value.IlrSubmissionDateTime;
        }
    }
}