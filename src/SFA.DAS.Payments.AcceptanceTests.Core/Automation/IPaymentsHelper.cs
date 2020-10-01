using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public interface IPaymentsHelper
    {
        int GetPaymentsCount(long ukprn, CollectionPeriod collectionPeriod);
        List<PaymentModel> GetPayments(long ukprn, CollectionPeriod collectionPeriod);

        int GetRequiredPaymentsCount(long ukprn, CollectionPeriod collectionPeriod);
    }
}