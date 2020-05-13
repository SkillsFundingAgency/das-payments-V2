using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.FundingSource.Application.Extensions;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public interface ILevyTransactionBatchStorageService
    {
        Task StoreLevyTransactions(IList<CalculatedRequiredLevyAmount> levyTransactions, CancellationToken cancellationToken, bool isFailedTransfer = false);
    }

    public class LevyTransactionBatchStorageService : ILevyTransactionBatchStorageService
    {
        private readonly IPaymentLogger logger;

        private readonly IFundingSourceDataContext dataContext;

        public LevyTransactionBatchStorageService(IPaymentLogger logger, IFundingSourceDataContext dataContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task StoreLevyTransactions(IList<CalculatedRequiredLevyAmount> calculatedRequiredLevyAmounts, CancellationToken cancellationToken, bool isFailedTransfer = false)
        {
            logger.LogDebug($"Got {calculatedRequiredLevyAmounts.Count} levy transactions.");

            var models = calculatedRequiredLevyAmounts.Select(levyAmount => new LevyTransactionModel
            {
                CollectionPeriod = levyAmount.CollectionPeriod.Period,
                AcademicYear = levyAmount.CollectionPeriod.AcademicYear,
                JobId = levyAmount.JobId,
                Ukprn = levyAmount.Ukprn,
                Amount = levyAmount.AmountDue,
                EarningEventId = levyAmount.EarningEventId,
                DeliveryPeriod = levyAmount.DeliveryPeriod,
                AccountId = levyAmount.AccountId ?? 0,
                RequiredPaymentEventId = levyAmount.EventId,
                TransferSenderAccountId = levyAmount.TransferSenderAccountId,
                MessagePayload = levyAmount.ToJson(),
                MessageType = levyAmount.GetType().FullName,
                IlrSubmissionDateTime = levyAmount.IlrSubmissionDateTime,
                FundingAccountId = levyAmount.CalculateFundingAccountId(isFailedTransfer),
            }).ToList();
            cancellationToken.ThrowIfCancellationRequested();

            await dataContext.SaveBatch(models, cancellationToken).ConfigureAwait(false);

            logger.LogInfo($"Saved {calculatedRequiredLevyAmounts.Count} levy transactions to db.");
        }
    }
}