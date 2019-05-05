using System;
using System.Globalization;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class ApprenticeshipKeyService : IApprenticeshipKeyService
    {
        private const string keyDelimiter = "~";

        public string GenerateApprenticeshipKey(long ukprn, string learnerReferenceNumber, int frameworkCode, int pathwayCode, int programmeType, int standardCode, string learnAimRef, short academicYear)
        {
            return string.Join(keyDelimiter,
                new[]
                {
                    ukprn.ToString(CultureInfo.InvariantCulture),
                    learnerReferenceNumber,
                    frameworkCode.ToString(CultureInfo.InvariantCulture),
                    pathwayCode.ToString(CultureInfo.InvariantCulture),
                    programmeType.ToString(CultureInfo.InvariantCulture),
                    standardCode.ToString(CultureInfo.InvariantCulture),
                    learnAimRef, // we may need to remove this as apprenticeship should handle both zprog and maths&eng
                    academicYear.ToString(CultureInfo.InvariantCulture)
                }
            ).ToLowerInvariant();
        }

        public ApprenticeshipKey ParseApprenticeshipKey(string apprenticeshipKey)
        {
            var keyParts = apprenticeshipKey.Split(Convert.ToChar(keyDelimiter));
            if (keyParts.Length != 8)
                throw new InvalidOperationException($"Cannot parse the apprenticeship key. invalid number of parts.  Expected 8 but was {keyParts.Length}. Key: {apprenticeshipKey}");
            return new ApprenticeshipKey
            {
                Ukprn = ParseToLong(keyParts[0], "Ukprn"),
                LearnerReferenceNumber = keyParts[1],
                FrameworkCode = ParseToInt(keyParts[2], "FrameworkCode"),
                PathwayCode = ParseToInt(keyParts[3], "PathwayCode"),
                ProgrammeType = ParseToInt(keyParts[4], "ProgrammeType"),
                StandardCode = ParseToInt(keyParts[5], "StandardCode"),
                LearnAimRef = keyParts[6],
                AcademicYear = ParseToShort(keyParts[7], "AcademicYear")
            };
        }

        private int ParseToInt(string source, string destinationName)
        {
            if (!int.TryParse(source, out var result))
                throw new ArgumentException($"Cannot parse the key part to an int. Destination: {destinationName}, source value: '{source}'");
            return result;
        }

        private long ParseToLong(string source, string destinationName)
        {
            if (!long.TryParse(source, out var result))
                throw new ArgumentException($"Cannot parse the key part to a long. Destination: {destinationName}, source value: '{source}'");
            return result;
        }

        private short ParseToShort(string source, string destinationName)
        {
            if (!short.TryParse(source, out var result))
                throw new ArgumentException($"Cannot parse the key part to a long. Destination: {destinationName}, source value: '{source}'");
            return result;
        }
    }
}
