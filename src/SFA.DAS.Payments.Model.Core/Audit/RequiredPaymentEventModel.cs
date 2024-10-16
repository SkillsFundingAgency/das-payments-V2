﻿using System;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Model.Core.Audit
{
    public class RequiredPaymentEventModel
    {
        public long Id { get; set; }
        public Guid EventId { get; set; }
        public Guid EarningEventId { get; set; }
        public Guid? ClawbackSourcePaymentEventId { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public long Ukprn { get; set; }
        public ContractType ContractType { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal SfaContributionPercentage { get; set; }
        public decimal Amount { get; set; }
        public CollectionPeriod CollectionPeriod { get; set; }
        public byte DeliveryPeriod { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public long LearnerUln { get; set; }
        public string LearningAimReference { get; set; }
        public int LearningAimProgrammeType { get; set; }
        public int LearningAimStandardCode { get; set; }
        public int LearningAimFrameworkCode { get; set; }
        public int LearningAimPathwayCode { get; set; }
        public string LearningAimFundingLineType { get; set; }
        public string AgreementId { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public long JobId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public long? AccountId { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public byte? CompletionStatus { get; set; }
        public decimal? CompletionAmount { get; set; }
        public decimal? InstalmentAmount { get; set; }
        public short? NumberOfInstalments { get; set; }
        public DateTime? LearningStartDate { get; set; }
        public long? ApprenticeshipId { get; set; }
        public long? ApprenticeshipPriceEpisodeId { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        public NonPaymentReason? NonPaymentReason { get; set; }
        public string EventType { get; set; }
        public int? AgeAtStartOfLearning { get; set; }
    }
}