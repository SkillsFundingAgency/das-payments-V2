using System.Globalization;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Domain.Enums;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class ApprenticeshipKeyService : IApprenticeshipKeyService
    {
        public string GenerateApprenticeshipKey(long ukprn, string learnerReferenceNumber, int frameworkCode, int pathwayCode, ProgrammeType programmeType, int standardCode, string learnAimRef)
        {
            return string.Join("-", 
                new[]
                {
                    ukprn.ToString(CultureInfo.InvariantCulture),
                    learnerReferenceNumber,
                    frameworkCode.ToString(CultureInfo.InvariantCulture),
                    pathwayCode.ToString(CultureInfo.InvariantCulture),
                    ((int)programmeType).ToString(CultureInfo.InvariantCulture),
                    standardCode.ToString(CultureInfo.InvariantCulture),
                    learnAimRef // we may need to remove this as apprenticeship should handle both zprog and maths&eng
                }
            ).ToLowerInvariant();
        }

        public string GeneratePaymentKey(string priceEpisodeIdentifier, string learnAimReference, int transactionType, CalendarPeriod deliveryPeriod)
        {
            return string.Join("-", 
                new[]
                {
                    priceEpisodeIdentifier.ToLowerInvariant(),
                    learnAimReference.ToLowerInvariant(),
                    transactionType.ToString(CultureInfo.InvariantCulture),
                    deliveryPeriod.Name
                }
            );
        }
    }
}
