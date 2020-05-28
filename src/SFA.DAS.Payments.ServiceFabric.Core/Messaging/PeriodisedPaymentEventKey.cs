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

        public PeriodisedPaymentEventKey(IPeriodisedPaymentEvent periodisedPaymentEvent) : base(periodisedPaymentEvent)
        {
            if (periodisedPaymentEvent == null) throw new ArgumentNullException(nameof(periodisedPaymentEvent));
            ApprenticeshipId = periodisedPaymentEvent.ApprenticeshipId;
            AccountId = periodisedPaymentEvent.AccountId;
            TransferSenderAccountId = periodisedPaymentEvent.TransferSenderAccountId;
            ApprenticeshipEmployerType = periodisedPaymentEvent.ApprenticeshipEmployerType;
        }

        protected override string CreateKey()
        {
            return $"{base.CreateKey()}-{ApprenticeshipId ?? 0}-{AccountId ?? 0}-{TransferSenderAccountId ?? 0}-{ApprenticeshipEmployerType}";
        }

        protected override string CreateLogSafeKey()
        {
            return $"{base.CreateLogSafeKey()}-{ApprenticeshipId ?? 0}-{AccountId ?? 0}-{TransferSenderAccountId ?? 0}-{ApprenticeshipEmployerType}";
        }
    }
}