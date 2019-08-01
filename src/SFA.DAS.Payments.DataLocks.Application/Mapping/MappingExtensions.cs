using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.Payments.DataLocks.Application.Mapping
{
    public static class MappingExtensions
    {

        public static int? ToStandardCode(this string apprenticeshipTrainingCode, ProgrammeType trainingType)
        {
            return trainingType == ProgrammeType.Standard ? int.Parse(apprenticeshipTrainingCode) : 0;
        }
        public static int? ToFrameworkCode(this string apprenticeshipTrainingCode, ProgrammeType trainingType)
        {
            return trainingType == ProgrammeType.Framework
                ? int.Parse(GetTrainingCode(apprenticeshipTrainingCode, 0, "Failed to parse the training code field to get the Framework code"))
                : 0;
        }

        public static int? ToProgrammeType(this string apprenticeshipTrainingCode, ProgrammeType trainingType)
        {
            return trainingType == ProgrammeType.Framework
                ? int.Parse(GetTrainingCode(apprenticeshipTrainingCode, 1, "Failed to parse the training code field to get the Programme Type"))
                : 25;
        }

        public static int? ToPathwayCode(this string apprenticeshipTrainingCode, ProgrammeType trainingType)
        {
            return trainingType == ProgrammeType.Framework
                ? int.Parse(GetTrainingCode(apprenticeshipTrainingCode, 2, "Failed to parse the training code field to get the Pathway code"))
                : 0;
        }

        private static string GetTrainingCode(string apprenticeshipTrainingCode, byte trainingCodeSegment, string errorMsg )
        {
            var trainingCodes = apprenticeshipTrainingCode.Split('-');

            if (trainingCodes.Length < trainingCodeSegment + 1)
                throw new InvalidOperationException($"{errorMsg}. Data: {apprenticeshipTrainingCode}");

            return trainingCodes[trainingCodeSegment];
        }

    }
}
