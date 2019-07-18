using System;

namespace SFA.DAS.Payments.ProviderPayments.Model
{
    public class MonthEndDetails : IEquatable<MonthEndDetails>
    {
        public bool Equals(MonthEndDetails other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Ukprn == other.Ukprn && AcademicYear == other.AcademicYear && CollectionPeriod == other.CollectionPeriod;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MonthEndDetails) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Ukprn.GetHashCode();
                hashCode = (hashCode * 397) ^ AcademicYear.GetHashCode();
                hashCode = (hashCode * 397) ^ CollectionPeriod.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(MonthEndDetails left, MonthEndDetails right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MonthEndDetails left, MonthEndDetails right)
        {
            return !Equals(left, right);
        }

        public long Ukprn { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public long JobId { get; set; }
    }
}