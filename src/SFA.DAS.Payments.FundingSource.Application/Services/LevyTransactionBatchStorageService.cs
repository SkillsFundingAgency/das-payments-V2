﻿using SFA.DAS.Payments.Application.Infrastructure.Logging;
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
using AutoMapper;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.Messages.Core;

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
        private readonly IMapper mapper;

        public LevyTransactionBatchStorageService(IPaymentLogger logger, ILevyTransactionRepository levyTransactionRepository, IMapper mapper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.levyTransactionRepository = levyTransactionRepository ?? throw new ArgumentNullException(nameof(levyTransactionRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
                FundingPlatformType = levyAmount.FundingPlatformType
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


        public async Task StoreLevyTransactions(IList<CalculateOnProgrammePayment> calculateOnProgrammePaymentCommands, CancellationToken cancellationToken, bool isReceiverTransferPayment = false)
        {
            logger.LogDebug($"Got {calculateOnProgrammePaymentCommands.Count} levy transactions.");

            //TODO: Update the LevyTransactionModel to hold all RequiredLevyAmountFields and stop storing required payments in the DB.  
            var requiredLevyAmounts = calculateOnProgrammePaymentCommands.Select(command => 
                    mapper.Map<CalculateOnProgrammePayment, CalculatedRequiredLevyAmount>(command))
                .ToList();

            await StoreLevyTransactions(requiredLevyAmounts, cancellationToken, isReceiverTransferPayment);

            logger.LogInfo($"Saved levy transactions from CalculateOnProgrammePaymentCommands to the db for later processing. Duplicates skipped.");
        }
    }
}