using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Tests.Core.Builders
{
    public class CollectionPeriodBuilder : CalendarPeriodBuilder<CollectionPeriodBuilder, CollectionPeriod>
    {
        private byte Period { get; set; } = byte.MaxValue;

        public CollectionPeriod Build()
        {
            string academicYear;

            if (Month < 8)
            {
                academicYear = $"{Year - 2000 - 1}{Year - 2000}";
            }
            else
            {
                academicYear = $"{Year - 2000}{Year - 2000 + 1}";
            }
            
            var name = $"{academicYear}-R{Period:D2}";

            var instance = new CollectionPeriod
            {
                AcademicYear = academicYear,
                Name = name,
                Period = Period,
            };
            return instance;
        }

        public override CollectionPeriodBuilder WithSpecDate(string date)
        {
            if (date.StartsWith("R13") || date.StartsWith("R14"))
            {
                var modifiedDate = date.Replace("R13", "R12").Replace("R14", "R12");
                base.WithSpecDate(modifiedDate);
                Year = Date.Year;
                Month = Date.Month;
                Period = byte.Parse(date.Substring(1, 2));
            }
            else
            {
                base.WithSpecDate(date);
            }

            return this;
        }

        public CollectionPeriod BuildFromName(string name)
        {
            var collectionPeriod = new CollectionPeriod
            {
                Name = name,
                Period = byte.Parse(name.Substring(6,2)),
                AcademicYear = name.Substring(0, 4),
            };

            return collectionPeriod;
        }
    }
}
