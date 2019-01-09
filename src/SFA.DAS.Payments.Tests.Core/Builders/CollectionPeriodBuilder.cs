using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Tests.Core.Builders
{
    public class CollectionPeriodBuilder : CalendarPeriodBuilder<CollectionPeriodBuilder, CollectionPeriod>
    {
        private byte Period { get; set; } = byte.MaxValue;

        public CollectionPeriod Build()
        {
            var builtInstance = BuildInstance();

            if (builtInstance.Month < 8)
            {
                builtInstance.AcademicYear = $"{builtInstance.Year - 2000 - 1}{builtInstance.Year - 2000}";
            }
            else
            {
                builtInstance.AcademicYear = $"{builtInstance.Year - 2000}{builtInstance.Year - 2000 + 1}";
            }
            
            if (Period != byte.MaxValue)
            {
                builtInstance.Period = Period;
            }

            builtInstance.Name = $"{builtInstance.AcademicYear}-R{builtInstance.Period:D2}";

            return builtInstance;
        }

        public override CollectionPeriodBuilder WithSpecDate(string date)
        {
            if (date.StartsWith("R13") || date.StartsWith("R14"))
            {
                var modifiedDate = date.Replace("R13", "R12").Replace("R14", "R12");
                base.WithSpecDate(modifiedDate);
                Period = byte.Parse(date.Substring(1, 2));
            }
            else
            {
                base.WithSpecDate(date);
            }

            return this;
        }
    }
}
