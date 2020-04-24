using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public interface ILevyTransactionBatchStorageService
    {
        Task StoreLevyTransactions(IList<CalculatedRequiredLevyAmount> levyTransactions, CancellationToken cancellationToken);
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

        public async Task StoreLevyTransactions(IList<CalculatedRequiredLevyAmount> levyTransactions, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Got {levyTransactions.Count} levy transactions.");

            var models = levyTransactions.Select(levyTransaction => new LevyTransactionModel
            {
                CollectionPeriod = levyTransaction.CollectionPeriod.Period,
                AcademicYear = levyTransaction.CollectionPeriod.AcademicYear,
                JobId = levyTransaction.JobId,
                Ukprn = levyTransaction.Ukprn,
                Amount = levyTransaction.AmountDue,
                EarningEventId = levyTransaction.EarningEventId,
                DeliveryPeriod = levyTransaction.DeliveryPeriod,
                AccountId = levyTransaction.AccountId ?? 0,
                RequiredPaymentEventId = levyTransaction.EventId,
                TransferSenderAccountId = levyTransaction.TransferSenderAccountId,
                MessagePayload = levyTransaction.ToJson(),
                MessageType = levyTransaction.GetType().FullName
            }).ToList();
            cancellationToken.ThrowIfCancellationRequested();

            await dataContext.SaveBatch(models, cancellationToken).ConfigureAwait(false);

            logger.LogInfo($"Saved {levyTransactions.Count} levy transactions to db.");
        }
    }
}