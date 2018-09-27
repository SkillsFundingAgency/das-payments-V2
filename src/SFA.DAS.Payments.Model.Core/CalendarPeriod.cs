using System;

namespace SFA.DAS.Payments.Model.Core
{
    public class CalendarPeriod
    {
        public short Year { get; }
        public byte Month { get; }
        public byte Period { get; }
        public string Name { get; set; }

        public CalendarPeriod() 
            : this((short) DateTime.UtcNow.Year, (byte) DateTime.UtcNow.Month)
        {
        }

        public CalendarPeriod(short year, byte month)
        {
            Year = year;
            Month = month;
            Period = (byte)(month > 7 ? month - 7 : month + 5);
            var firstYear = (month < 8 ? year - 1 : year) - 2000;
            Name = string.Concat(firstYear, firstYear + 1, "-R", Period.ToString("00"));
        }

        public CalendarPeriod(string years, byte period)
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
            Name = string.Concat(2000 + year, 2001 + year, "-R", period.ToString("00"));
        }

        public CalendarPeriod(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (name.Length == 7)
                name = string.Concat(name.Substring(0, 4), "-", name.Substring(4));

            if (name.Length != 8
                || name[4] != '-'
                || name[5] != 'R'
                || !int.TryParse(name.Substring(6), out var period)
                || !int.TryParse(name.Substring(0, 2), out var year)
                || period < 1
                || period > 14
                || year < 0
                || year > 99
            )
            {
                throw new ArgumentException("Invalid period name", nameof(name));
            }

            var increment = period < 6 ? 0 : 1;

            Year = (short) (2000 + year + increment);
            Month = (byte) (period < 6 ? period + 7 : period - 5);
            Name = name;
            Period = (byte)period;
        }

        public override string ToString()
        {
            return string.Concat(Name, " ", Year, "-", Month);
        }

        public CalendarPeriod Clone()
        {
            return (CalendarPeriod) MemberwiseClone();
        }

        public override int GetHashCode()
        {
            return string.Concat(Year, Month, Period).GetHashCode();
        }

        public override bool Equals(object other)
        {
            return Equals(other as CalendarPeriod);
        }

        public bool Equals(CalendarPeriod other)
        {
            if (other == null) return false;
            return Year == other.Year && Month == other.Month && Period == other.Period;
        }

        public static bool operator ==(CalendarPeriod a, CalendarPeriod b)
        {
            if ((object)a == null)
                return (object)b == null;

            return a.Equals(b);
        }

        public static bool operator !=(CalendarPeriod a, CalendarPeriod b)
        {
            return !(a == b);
        }
    }
}