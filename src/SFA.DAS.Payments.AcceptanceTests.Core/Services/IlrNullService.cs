using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.Services.Intefaces
{
    public class IlrNullService : IIlrService
    {
        public Task PublishLearnerRequest(List<Training> currentIlr, string collectionPeriodText, string featureNumber, Func<Task> verifyIlr) => verifyIlr();
    }
}
