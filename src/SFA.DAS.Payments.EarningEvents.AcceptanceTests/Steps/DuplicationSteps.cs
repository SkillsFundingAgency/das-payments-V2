using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Newtonsoft.Json;
using NServiceBus;
using NUnit.Framework.Internal;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.EarningEvents.AcceptanceTests.Handlers;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using TechTalk.SpecFlow;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Steps
{
    [Binding]
    //[Scope(Feature = "DuplicationTests")]
    public class DuplicationSteps : StepsBase
    {
        const string ProcessLearnerCommand = "ProcessLearnerCommand";

        public DuplicationSteps(ScenarioContext context) : base(context)
        {
        }


        [When(@"a process learner command is handled by the process learner service")]
        public async Task WhenAProcessLearnerCommandIsHandledByTheProcessLearnerService()
        {
            var command = new ProcessLearnerCommand()
            {
                CollectionPeriod = 1,
                CollectionYear = 1920,
                CommandId = Guid.NewGuid(),
                IlrFileName = "SomeFile",
                IlrSubmissionDateTime = DateTime.Now,
                Ukprn = TestSession.GenerateId(),
                Learner = GetFm36Learner()
            };

            Context.Add(ProcessLearnerCommand, command);

            await MessageSession.Send(command);
        }

        private FM36Learner GetFm36Learner()
        {
            var fileName = $"SFA.DAS.Payments.EarningEvents.AcceptanceTests.TestData.FM36Learner.json";

            FM36Learner result;
            using (var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(fileName)
            )
            using (var reader = new StreamReader(stream))
            {
                result = JsonSerializer.Create().Deserialize<FM36Learner>(new JsonTextReader(reader));
            }

            return result;
        }


        [When(@"if the event is duplicated")]
        public async Task WhenIfTheEventIsDuplicated()
        {
          var command =  Context.Get<ProcessLearnerCommand>(ProcessLearnerCommand);
          command.Ukprn++;
          await MessageSession.Send(command);
        }

        [Then(@"the duplicate event is ignored")]
        public void ThenTheDuplicateEventIsIgnored()
        {
        }

        [Then(@"only one set of earning events is generated for the learner")]
        public async Task ThenOnlyOneSetOfEarningEventsIsGeneratedForTheLearner()
        {
            await WaitForIt(() => ApprenticeshipContractType1EarningEventHandler.ReceivedEvents.Count == 1,
                "Failed to find a Payable Earning and no Data Locks");
        }
    }
}