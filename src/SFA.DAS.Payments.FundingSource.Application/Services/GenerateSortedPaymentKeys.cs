using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Model;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class GenerateSortedPaymentKeys : IGenerateSortedPaymentKeys
    {
        private readonly IDataCache<List<EmployerProviderPriorityModel>> employerProviderPriorities;
        private readonly IDataCache<List<string>> refundSortKeysCache;
        private readonly IDataCache<List<TransferPaymentSortKeyModel>> transferPaymentSortKeysCache;
        private readonly IDataCache<List<RequiredPaymentSortKeyModel>> requiredPaymentSortKeysCache;

        public GenerateSortedPaymentKeys(
            IDataCache<List<EmployerProviderPriorityModel>> employerProviderPriorities,
            IDataCache<List<string>> refundSortKeysCache,
            IDataCache<List<TransferPaymentSortKeyModel>> transferPaymentSortKeysCache,
            IDataCache<List<RequiredPaymentSortKeyModel>> requiredPaymentSortKeysCache)
        {
            this.employerProviderPriorities = employerProviderPriorities ?? throw new ArgumentNullException(nameof(employerProviderPriorities));
            this.refundSortKeysCache = refundSortKeysCache ?? throw new ArgumentNullException(nameof(refundSortKeysCache));
            this.transferPaymentSortKeysCache = transferPaymentSortKeysCache ?? throw new ArgumentNullException(nameof(transferPaymentSortKeysCache));
            this.requiredPaymentSortKeysCache = requiredPaymentSortKeysCache ?? throw new ArgumentNullException(nameof(requiredPaymentSortKeysCache));
        }
        
        public async Task<List<string>> GeyKeys()
        {
            var sortedPaymentKeys = new List<string>();

            //Add Refunds 
            var refundKeys = await GetRefundPaymentKeys().ConfigureAwait(false);
            sortedPaymentKeys.AddRange(refundKeys);

            //Transfer
            var transferKeys = await GetTransferPaymentKeys().ConfigureAwait(false);
            sortedPaymentKeys.AddRange(transferKeys);

            //Required Payment
            var requirePaymentKeys = await GetRequiredPaymentKeys().ConfigureAwait(false);
            sortedPaymentKeys.AddRange(requirePaymentKeys);

            return sortedPaymentKeys;
        }

        private async Task<List<string>> GetRefundPaymentKeys()
        {
            var keysValue = await refundSortKeysCache.TryGet(CacheKeys.RefundPaymentsKeyListKey).ConfigureAwait(false);
            var keys = keysValue.HasValue ? keysValue.Value : new List<string>();
            return keys;
        }

        private async Task<List<string>> GetTransferPaymentKeys()
        {
            var keysValue = await transferPaymentSortKeysCache.TryGet(CacheKeys.SenderTransferKeyListKey).ConfigureAwait(false);
            var keys = keysValue.HasValue ? keysValue.Value : new List<TransferPaymentSortKeyModel>();
            var transferPaymentKeys = keys.OrderBy(x => x.AgreedOnDate).ThenBy(o => o.Uln).Select(o => o.Id).ToList();
            return transferPaymentKeys;
        }

        private async Task<List<string>> GetRequiredPaymentKeys()
        {
            var sortedRequiredPaymentKeys = new List<string>();

            var keysValue = await requiredPaymentSortKeysCache.TryGet(CacheKeys.RequiredPaymentKeyListKey).ConfigureAwait(false);
            var keys = keysValue.HasValue ? keysValue.Value : new List<RequiredPaymentSortKeyModel>();

            var providerPrioritiesValue = await employerProviderPriorities.TryGet(CacheKeys.EmployerPaymentPriorities).ConfigureAwait(false);
            var providerPriorities = providerPrioritiesValue.HasValue ? providerPrioritiesValue.Value : new List<EmployerProviderPriorityModel>();
            var orderedProviderPriorityList = providerPriorities.OrderBy(x => x.Order).ToList();

            foreach (var providerPriority in orderedProviderPriorityList)
            {
                var providerKeys = keys
                    .Where(x => x.Ukprn == providerPriority.Ukprn)
                    .ToList();

                sortedRequiredPaymentKeys.AddRange(SortRequiredPaymentKeys(providerKeys));
            }

            var unprioritisedProviderKeys = keys.Where(x => !sortedRequiredPaymentKeys.Contains(x.Id)).ToList();
            sortedRequiredPaymentKeys.AddRange(SortRequiredPaymentKeys(unprioritisedProviderKeys));

            return sortedRequiredPaymentKeys;
        }

        private List<string> SortRequiredPaymentKeys(List<RequiredPaymentSortKeyModel> requiredPaymentSortKeyModels)
        {
            return requiredPaymentSortKeyModels
                  .OrderBy(x => x.StarDate)
                  .ThenBy(x => x.Uln)
                  .Select(x => x.Id)
                  .ToList();
        }

      
    }



}
