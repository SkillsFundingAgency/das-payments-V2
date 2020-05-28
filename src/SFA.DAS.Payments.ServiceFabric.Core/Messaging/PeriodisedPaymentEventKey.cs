using System;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ServiceFabric.Core.Messaging
{
    public class PeriodisedPaymentEventKey : EarningEventKey
    {
        public long? ApprenticeshipId { get; set; }
        public long? AccountId { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        public decimal Amount { get; set; }
        public byte DeliveryPeriod { get; set; }
        public string PriceEpisodeIdentifier { get; set; }

        protected PeriodisedPaymentEventKey() { }
        public PeriodisedPaymentEventKey(IPeriodisedPaymentEvent periodisedPaymentEvent) : base(periodisedPaymentEvent)
        {
            if (periodisedPaymentEvent == null) throw new ArgumentNullException(nameof(periodisedPaymentEvent));
            ApprenticeshipId = periodisedPaymentEvent.ApprenticeshipId;
            AccountId = periodisedPaymentEvent.AccountId;
            TransferSenderAccountId = periodisedPaymentEvent.TransferSenderAccountId;
            ApprenticeshipEmployerType = periodisedPaymentEvent.ApprenticeshipEmployerType;
            Amount = periodisedPaymentEvent.AmountDue;
            DeliveryPeriod = periodisedPaymentEvent.DeliveryPeriod;
            PriceEpisodeIdentifier = periodisedPaymentEvent.PriceEpisodeIdentifier;
        }

        protected override string CreateKey()
        {
            return $"{base.CreateKey()}-{Amount}-{DeliveryPeriod}-{PriceEpisodeIdentifier}-{ApprenticeshipId ?? 0}-{AccountId ?? 0}-{TransferSenderAccountId ?? 0}-{ApprenticeshipEmployerType}";
        }

        protected override string CreateLogSafeKey()
        {
            return $"{base.CreateLogSafeKey()}-{Amount}-{DeliveryPeriod}-{PriceEpisodeIdentifier}-{ApprenticeshipId ?? 0}-{AccountId ?? 0}-{TransferSenderAccountId ?? 0}-{ApprenticeshipEmployerType}";
        }
    }
}