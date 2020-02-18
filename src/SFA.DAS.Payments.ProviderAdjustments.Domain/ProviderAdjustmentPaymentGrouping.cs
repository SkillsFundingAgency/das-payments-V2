using System;

namespace SFA.DAS.Payments.ProviderAdjustments.Domain
{
    public class ProviderAdjustmentPaymentGrouping : IEquatable<ProviderAdjustmentPaymentGrouping>
    {
        public long Ukprn { get; }
        public int PaymentType { get; }
        public int Period { get; }

        // Non-equality members
        public Guid SubmissionId { get; }
        public string PaymentTypeName { get; set; }

        public ProviderAdjustmentPaymentGrouping(ProviderAdjustment adjustment)
        {
            Ukprn = adjustment.Ukprn;
            PaymentType = adjustment.PaymentType;
            Period = adjustment.SubmissionCollectionPeriod;
            SubmissionId = adjustment.SubmissionId;
            PaymentTypeName = adjustment.PaymentTypeName;
        }

        public bool Equals(ProviderAdjustmentPaymentGrouping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Ukprn == other.Ukprn &&
                   PaymentType == other.PaymentType &&
                   Period == other.Period
                   //SubmissionId == other.SubmissionId
                   ;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as ProviderAdjustmentPaymentGrouping;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = 31 * hash + Ukprn.GetHashCode();
                hash = 31 * hash + PaymentType.GetHashCode();
                hash = 31 * hash + Period.GetHashCode();
                //hash = 31 * hash + SubmissionId.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(ProviderAdjustmentPaymentGrouping left, ProviderAdjustmentPaymentGrouping right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProviderAdjustmentPaymentGrouping left, ProviderAdjustmentPaymentGrouping right)
        {
            return !Equals(left, right);
        }
    }
}
