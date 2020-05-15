using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public interface IPaymentsHelper
    {
        int GetPaymentsCount(long ukprn, CollectionPeriod collectionPeriod);

        int GetRequiredPaymentsCount(long ukprn, CollectionPeriod collectionPeriod);
    }
}