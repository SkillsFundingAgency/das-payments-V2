using System;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class LevyTransactionModel
    {
        public long Id { get; set; }
        public long Ukprn { get; set; }
        public byte CollectionPeriod { get; set; }
        public short AcademicYear { get; set; }
        public byte DeliveryPeriod { get; set; }
        public long JobId { get; set; }
        public long AccountId { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public Guid RequiredPaymentEventId { get; set; }
        public Guid EarningEventId { get; set; }
        public decimal Amount { get; set; }
        public string MessagePayload { get; set; }
        public string MessageType { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public long FundingAccountId { get; set; }
    }
}