using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class EarningYearResolver : IValueResolver<IntermediateLearningAim, EarningEvent, short>
    {
        public short Resolve(IntermediateLearningAim source, EarningEvent destination, short destMember, ResolutionContext context)
        {
            var period = new CalendarPeriod(source.CollectionYear, (byte)source.CollectionPeriod);

            return period.Year;
        }
    }
}