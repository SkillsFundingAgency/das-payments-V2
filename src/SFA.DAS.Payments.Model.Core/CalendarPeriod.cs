using System;

namespace SFA.DAS.Payments.Model.Core
{
    public class CollectionPeriod
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Period { get; set; }
        public string AcademicYear { get; set; }
        public string Name { get; set; }

        public static CollectionPeriod CreateFromAcademicYearAndPeriod(string academicYear, int period)
        {
            if (string.IsNullOrEmpty(academicYear) || academicYear.Length != 4)
            {
                throw new ArgumentNullException(nameof(academicYear), "Please ensure that the academic year is of format '1819'");
            }

            var result = new CollectionPeriod();
            if (period < 6)
            {
                result.Month = period + 7;
                if (int.TryParse(academicYear.Substring(0,2), out var year)) result.Year = 2000 + year;
            }
            else
            {
                result.Month = period - 5;
                if (int.TryParse(academicYear.Substring(2), out var year)) result.Year = 2000 + year;
            }

            result.Period = period;
            result.AcademicYear = academicYear;
            result.Name = $"{academicYear}-R{period:D2}";

            return result;
        }

        public CollectionPeriod Clone()
        {
            return (CollectionPeriod) MemberwiseClone();
        }
    }

    public class DeliveryPeriod
    {
        public DeliveryPeriod Clone()
        {
            return (DeliveryPeriod) MemberwiseClone();
        }

        public int Period { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Identifier { get; set; }

        public static DeliveryPeriod CreateFromAcademicYearAndPeriod(string academicYear, int period)
        {
            if (string.IsNullOrEmpty(academicYear) || academicYear.Length != 4)
            {
                throw new ArgumentNullException(nameof(academicYear), "Please ensure that the academic year is of format '1819'");
            }

            var result = new DeliveryPeriod();
            if (period < 6)
            {
                result.Month = period + 7;
                if (int.TryParse(academicYear.Substring(0, 2), out var year)) result.Year = 2000 + year;
            }
            else
            {
                result.Month = period - 5;
                if (int.TryParse(academicYear.Substring(2), out var year)) result.Year = 2000 + year;
            }

            result.Period = period;
            result.Identifier = $"{result.Year}-{result.Month:D2}";

            return result;
        }
    }

    public class CalendarPeriod2
    {
        private string name;
        public short Year { get; set; }
        public byte Month { get; set; }
        public byte Period { get; set; }
        public string AcademicYear { get; set; }

        public string Name
        {
            get => name;
            set => FromName(value);
        }

        public CalendarPeriod2() 
            : this((short) DateTime.UtcNow.Year, (byte) DateTime.UtcNow.Month)
        {
        }

        public CalendarPeriod2(short year, byte month)
        {
            Year = year;
            Month = month;
            Period = (byte)(month > 7 ? month - 7 : month + 5);
            var firstYear = (month < 8 ? year - 1 : year) - 2000;
            AcademicYear = string.Concat(firstYear, firstYear + 1);
            Name = string.Concat(AcademicYear, "-R", Period.ToString("00"));
        }

        public CalendarPeriod2(string years, byte period)
        {
            if (years == null)
                throw new ArgumentNullException(nameof(years));

            if (years.Length != 4
                || !int.TryParse(years.Substring(0, 2), out var year)
                || year < 0
                || year > 99
            )
                throw new ArgumentException("Invalid years", nameof(years));

            if (period < 1 || period > 14)
                throw new ArgumentException("Invalid period", nameof(years));

            Year = (short)(2000 + year + (period < 6 ? 0 : 1));
            Month = (byte)(period < 6 ? period + 7 : period - 5);
            Period = period;
            AcademicYear = string.Concat(year, year + 1);
            Name = string.Concat(AcademicYear, "-R", period.ToString("00"));
        }

        public CalendarPeriod2(string name)
        {
            FromName(name);
        }

        private void FromName(string newName)
        {
            if (newName == null)
                throw new ArgumentNullException("name");

            if (newName.Length == 7)
                newName = string.Concat(newName.Substring(0, 4), "-", newName.Substring(4));

            if (newName.Length != 8
                || newName[4] != '-'
                || newName[5] != 'R'
                || !int.TryParse(newName.Substring(6), out var period)
                || !int.TryParse(newName.Substring(0, 2), out var year)
                || period < 1
                || period > 14
                || year < 0
                || year > 99
            )
            {
                throw new ArgumentException("Invalid period name", "name");
            }

            var increment = period < 6 ? 0 : 1;

            Year = (short) (2000 + year + increment);
            Month = (byte) (period < 6 ? period + 7 : period - 5);
            name = newName;
            Period = (byte) period;
            AcademicYear = newName.Substring(0, 4);
        }

        public DateTime LastDayOfMonthAfter(int periods)
        {
            var month = new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month));
            return month.AddMonths(periods);
        }

        public override string ToString()
        {
            return string.Concat(Name, " ", Year, "-", Month);
        }

        public CalendarPeriod2 Clone()
        {
            return (CalendarPeriod2) MemberwiseClone();
        }

        public override int GetHashCode()
        {
            return string.Concat(Year, Month, Period).GetHashCode();
        }

        public override bool Equals(object other)
        {
            return Equals(other as CalendarPeriod2);
        }

        public bool Equals(CalendarPeriod2 other)
        {
            if (other == null) return false;
            return Year == other.Year && (Month == other.Month || Period == other.Period);
        }

        public static bool operator ==(CalendarPeriod2 a, CalendarPeriod2 b)
        {
            if ((object)a == null)
                return (object)b == null;

            return a.Equals(b);
        }

        public static bool operator !=(CalendarPeriod2 a, CalendarPeriod2 b)
        {
            return !(a == b);
        }
    }
}