using System.IO;
using System.Linq;
using System.Reflection;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests.Helpers
{
    public static class FileHelpers
    {
        public static ProcessLearnerCommand CreateFromFile(string filename, string learnerRefNo)
        {
            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            using (var reader = embeddedProvider.GetFileInfo($"DataFiles\\{filename}").CreateReadStream())
            {
                using (var sr = new StreamReader(reader))
                {
                    var fm36Global = JsonConvert.DeserializeObject<FM36Global>(sr.ReadToEnd());
                    return new ProcessLearnerCommand
                    {
                        CollectionPeriod = 3,
                        CollectionYear = short.Parse(fm36Global.Year),
                        Ukprn = fm36Global.UKPRN,
                        Learner = fm36Global.Learners.Single(l => l.LearnRefNumber.Equals(learnerRefNo))
                    };
                }
            }
        }


    }
}