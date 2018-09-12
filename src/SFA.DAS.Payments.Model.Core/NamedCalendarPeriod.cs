using System;

namespace SFA.DAS.Payments.Model.Core
{
    public class NamedCalendarPeriod : CalendarPeriod
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Concat(Name, "-", Year, "-", Month);
        }

        public static NamedCalendarPeriod FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (name.Length != 7
                || !int.TryParse(name.Substring(5), out var month)
                || !int.TryParse(name.Substring(0, 2), out var year)
                || month < 1
                || month > 14
                || year < 0
                || year > 99
                )
            {
                throw new ArgumentException("Invalid period name", nameof(name));
            }

            var increment = month < 6 ? 0 : 1;

            return new NamedCalendarPeriod
            {
                Year = (short) (2000 + year + increment),
                Month = (byte) (month < 6 ? month + 7 : month - 5),
                Name = name
            };
        }
    }
}