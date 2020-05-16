using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class PaymentsHelper : IPaymentsHelper
    {
        private readonly TestPaymentsDataContext dataContext;

        public PaymentsHelper(TestPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public int GetPaymentsCount(long ukprn, CollectionPeriod collectionPeriod)
        {
            return dataContext.Payment.Count(x => x.Ukprn == ukprn && x.CollectionPeriod.Period == collectionPeriod.Period && x.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear);
        }

        public int GetRequiredPaymentsCount(long ukprn, CollectionPeriod collectionPeriod)
        {
            return dataContext.RequiredPaymentEvent.Count(x => x.Ukprn == ukprn && x.CollectionPeriod.Period == collectionPeriod.Period && x.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear);
        }
    }
}