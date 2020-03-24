using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents
{
    public interface IEarningEventMapper
    {
        EarningEventModel Map(ApprenticeshipContractType1EarningEvent earningEvent);
    }

    public class EarningEventMapper : IEarningEventMapper
    {
        public EarningEventModel Map(ApprenticeshipContractType1EarningEvent earningEvent)
        {
            var earningEventModel = MapCommon(earningEvent);
            earningEventModel.ContractType = ContractType.Act1;
            return earningEventModel;
        }

        protected virtual EarningEventModel MapCommon(EarningEvent earningEvent)
        {
            //builder.Ignore(x => x.ActualEndDate);
            //builder.Ignore(x => x.CompletionAmount);
            //builder.Ignore(x => x.CompletionStatus);
            //builder.Ignore(x => x.InstalmentAmount);
            //builder.Ignore(x => x.PlannedEndDate);
            //builder.Ignore(x => x.StartDate);
            //builder.Ignore(x => x.NumberOfInstalments);
            return new EarningEventModel
            {
                EventId = earningEvent.EventId,
                AcademicYear = earningEvent.CollectionPeriod.AcademicYear,
                CollectionPeriod = earningEvent.CollectionPeriod.Period,
            };
        }
    }
}