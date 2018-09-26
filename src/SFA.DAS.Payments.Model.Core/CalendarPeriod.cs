using System;

namespace SFA.DAS.Payments.Model.Core
{
    public class CalendarPeriod
    {
        public short Year { get; set; }
        public byte Month { get; set; }
        public byte Period { get; set; }
        public string Name { get; set; }

        public CalendarPeriod()
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

            if (name.Length == 8 && name[4] == '-')
                name = name.Replace("-", null);

            if (name.Length != 7
                || !int.TryParse(name.Substring(5), out var period)
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
    }
}