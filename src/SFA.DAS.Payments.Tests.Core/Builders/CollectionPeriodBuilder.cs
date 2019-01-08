using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Tests.Core.Builders
{
    public class CollectionPeriodBuilder : CalendarPeriodBuilder<CollectionPeriodBuilder, CollectionPeriod>
    {
        public CollectionPeriod Build()
        {
            var builtInstance = BuildInstance();

            if (builtInstance.Month < 8)
            {
                builtInstance.AcademicYear = $"{2000 - builtInstance.Year - 1}{2000 - builtInstance.Year}";
            }
            else
            {
                builtInstance.AcademicYear = $"{2000 - builtInstance.Year}{2000 - builtInstance.Year + 1}";
            }
            builtInstance.Name = $"{builtInstance.AcademicYear}-R{builtInstance.Month:D2}";
            return builtInstance;
        }
    }
}
