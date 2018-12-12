using System;

namespace SFA.DAS.Payments.Model.Core
{
    public class CalendarPeriod
    {
        private string name;
        public short Year { get; set; }
        public byte Month { get; set; }
        public byte Period { get; set; }

        public string Name
        {
            get => name;
            set => FromName(value);
        }

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
            Name = string.Concat(year, year + 1, "-R", period.ToString("00"));
        }

        public CalendarPeriod(string name)
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
            return Year == other.Year && (Month == other.Month || Period == other.Period);
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