﻿using System.IO;
using System.Reflection;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Newtonsoft.Json;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers
{
    public static class FM36GlobalDeserialiser
    {
        public static FM36Global DeserialiseByFeatureForPeriod(string featureTitle, string collectionPeriodMonthText)
        {
            FM36Global result;

            using (var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream($"SFA.DAS.Payments.AcceptanceTests.EndToEnd.FM36TestFiles.{featureTitle}-{collectionPeriodMonthText}.json")
            )
            using (var reader = new StreamReader(stream))
            {
                result = JsonSerializer.Create().Deserialize<FM36Global>(new JsonTextReader(reader));
            }

            return result;
        }
    }
}
