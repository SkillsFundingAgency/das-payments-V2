using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource
{
    public interface IFundingSourcePaymentsEventProcessor
    {
        Task ProcessPaymentsEvent(FundingSourcePaymentEvent message);
    }

    public class FundingSourcePaymentsEventProcessor : PaymentsEventProcessor<FundingSourcePaymentEvent, FundingSourceEventModel>, IFundingSourcePaymentsEventProcessor
    {
        public FundingSourcePaymentsEventProcessor(IPaymentsEventModelCache<FundingSourceEventModel> cache, IMapper mapper) : base(cache, mapper)
        {
        }
    }

    public class FundingSourcePaymentsEventModelBatchProcessor
    {
        private readonly IPaymentsEventModelCache<FundingSourceEventModel> cache;
        private readonly TimeSpan batchInterval;

        public FundingSourcePaymentsEventModelBatchProcessor(IPaymentsEventModelCache<FundingSourceEventModel> cache, IConfigurationHelper configurationHelper)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            batchInterval = TimeSpan.FromSeconds(int.Parse(configurationHelper.GetSetting("BatchIntervalInSeconds")));
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                


            //    using (var tx = this.StateManager.CreateTransaction())
            //    {
            //        var result = await myDictionary.TryGetValueAsync(tx, "Counter");

            //        ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
            //            result.HasValue ? result.Value.ToString() : "Value does not exist.");

            //        await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

            //        // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
            //        // discarded, and nothing is saved to the secondary replicas.
            //        await tx.CommitAsync();
            //    }

                await Task.Delay(batchInterval, cancellationToken);
            }
        }
    }
}