using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

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
            return GetPayments(ukprn, collectionPeriod).Count;
        }

        public List<PaymentModel> GetPayments(long ukprn, CollectionPeriod collectionPeriod)
        {
            return dataContext.Payment
                .Where(x => x.Ukprn == ukprn && 
                            x.CollectionPeriod.Period == collectionPeriod.Period && 
                            x.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear)
                .ToList();
        }

        public int GetRequiredPaymentsCount(long ukprn, CollectionPeriod collectionPeriod)
        {
            return dataContext.RequiredPaymentEvent.Count(x => x.Ukprn == ukprn && x.CollectionPeriod.Period == collectionPeriod.Period && x.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear);
        }
    }
}