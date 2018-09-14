using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Domain.Enums;

namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface IApprenticeshipKeyService
    {
        string GenerateApprenticeshipKey(long ukprn, string learnerReferenceNumber, int frameworkCode, int pathwayCode, ProgrammeType programmeType, int standardCode, string learnAimRef);

        string GeneratePaymentKey(string priceEpisodeIdentifier, string learnAimReference, int transactionType, CalendarPeriod deliveryPeriod);
    }
}
