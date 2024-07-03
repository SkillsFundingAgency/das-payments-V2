using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Application.Extensions;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.FundingSource.Messages.Commands;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public interface ILevyTransactionBatchStorageService
    {
        Task StoreLevyTransactions(IList<CalculatedRequiredLevyAmount> levyTransactions, CancellationToken cancellationToken, bool isReceiverTransferPayment = false);
        Task StoreLevyTransactions(IList<CalculateOnProgrammePayment> levyTransactions, CancellationToken cancellationToken, bool isReceiverTransferPayment = false);
    }

    public class LevyTransactionBatchStorageService : ILevyTransactionBatchStorageService
    {
        private readonly IPaymentLogger logger;
        private readonly ILevyTransactionRepository levyTransactionRepository;

        public LevyTransactionBatchStorageService(IPaymentLogger logger, ILevyTransactionRepository levyTransactionRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.levyTransactionRepository = levyTransactionRepository ?? throw new ArgumentNullException(nameof(levyTransactionRepository));
        }

        public async Task StoreLevyTransactions(IList<CalculatedRequiredLevyAmount> calculatedRequiredLevyAmounts, CancellationToken cancellationToken, bool isReceiverTransferPayment = false)
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
                ClawbackSourcePaymentEventId = levyAmount.ClawbackSourcePaymentEventId ?? Guid.Empty,
                TransferSenderAccountId = levyAmount.TransferSenderAccountId,
                MessagePayload = levyAmount.ToJson(),
                MessageType = levyAmount.GetType().FullName,
                IlrSubmissionDateTime = levyAmount.IlrSubmissionDateTime,
                FundingAccountId = levyAmount.CalculateFundingAccountId(isReceiverTransferPayment),
                ApprenticeshipEmployerType = levyAmount.ApprenticeshipEmployerType,
                ApprenticeshipId = levyAmount.ApprenticeshipId,
                LearnerUln = levyAmount.Learner.Uln,
                LearnerReferenceNumber = levyAmount.Learner.ReferenceNumber,
                LearningAimFrameworkCode = levyAmount.LearningAim.FrameworkCode,
                LearningAimPathwayCode = levyAmount.LearningAim.PathwayCode,
                LearningAimFundingLineType = levyAmount.LearningAim.FundingLineType,
                LearningAimProgrammeType = levyAmount.LearningAim.ProgrammeType,
                LearningAimReference = levyAmount.LearningAim.Reference,
                LearningAimStandardCode = levyAmount.LearningAim.StandardCode,
                LearningStartDate = levyAmount.LearningStartDate,
                SfaContributionPercentage = levyAmount.SfaContributionPercentage,
                TransactionType = levyAmount.TransactionType,
                FundingPlatform = (byte?)FundingPlatformType.SubmitLearnerData
            }).ToList();
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await levyTransactionRepository.SaveLevyTransactions(models, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (!e.IsUniqueKeyConstraintException() && !e.IsDeadLockException()) throw;

                logger.LogWarning($"Batch contained a duplicate LevyTransaction. Will store each individually and discard duplicate.");

                await levyTransactionRepository.SaveLevyTransactionsIndividually(models, cancellationToken);
            }

            logger.LogInfo($"Saved levy transactions to db. Duplicates skipped.");
        }


        public async Task StoreLevyTransactions(IList<CalculateOnProgrammePayment> calculatedRequiredLevyAmounts, CancellationToken cancellationToken, bool isReceiverTransferPayment = false)
        {
            logger.LogDebug($"Got {calculatedRequiredLevyAmounts.Count} levy transactions.");

            var models = calculatedRequiredLevyAmounts.Select(levyAmount => new LevyTransactionModel
            {
                CollectionPeriod = levyAmount.CollectionPeriod.Period,
                AcademicYear = levyAmount.CollectionPeriod.AcademicYear,
                JobId = -1, 
                Ukprn = 0, // TODO: Verify
                Amount = levyAmount.AmountDue,
                EarningEventId = Guid.Empty,
                DeliveryPeriod = levyAmount.DeliveryPeriod,
                AccountId = levyAmount.AccountId,
                RequiredPaymentEventId = levyAmount.EventId,
                ClawbackSourcePaymentEventId = null,
                TransferSenderAccountId = levyAmount.TransferSenderAccountId,
                MessagePayload = levyAmount.ToJson(),
                MessageType = levyAmount.GetType().FullName,
                IlrSubmissionDateTime = new DateTime(1753, 1, 1),
                FundingAccountId = 0, //TODO CHECK 
                ApprenticeshipEmployerType = levyAmount.ApprenticeshipEmployerType,
                ApprenticeshipId = levyAmount.ApprenticeshipId,
                LearnerUln = levyAmount.Learner.Uln,
                LearnerReferenceNumber = levyAmount.Learner.ReferenceNumber,
                LearningAimFrameworkCode = levyAmount.LearningAim.FrameworkCode,
                LearningAimPathwayCode = levyAmount.LearningAim.PathwayCode,
                LearningAimFundingLineType = levyAmount.LearningAim.FundingLineType,
                LearningAimProgrammeType = levyAmount.LearningAim.ProgrammeType,
                LearningAimReference = levyAmount.LearningAim.Reference,
                LearningAimStandardCode = levyAmount.LearningAim.StandardCode,
                LearningStartDate = levyAmount.LearningStartDate,
                SfaContributionPercentage = levyAmount.SfaContributionPercentage,
                TransactionType = (TransactionType)levyAmount.OnProgrammeEarningType,
                FundingPlatform = (byte?)FundingPlatformType.DigitalApprenticeshipService // TODO: Verify
            }).ToList();
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await levyTransactionRepository.SaveLevyTransactions(models, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (!e.IsUniqueKeyConstraintException() && !e.IsDeadLockException()) throw;

                logger.LogWarning($"Batch contained a duplicate LevyTransaction. Will store each individually and discard duplicate.");

                await levyTransactionRepository.SaveLevyTransactionsIndividually(models, cancellationToken);
            }

            logger.LogInfo($"Saved levy transactions to db. Duplicates skipped.");
        }
    }
}