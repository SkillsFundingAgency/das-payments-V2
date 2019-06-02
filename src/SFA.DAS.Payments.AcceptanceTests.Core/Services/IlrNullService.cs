using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Services
{
    public class IlrNullService : IIlrService
    {
        public Task PublishLearnerRequest(List<Training> currentIlr, List<Learner> learners, string collectionPeriodText, string featureNumber) => Task.CompletedTask;
    }
}
