using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Services
{
    public interface IIlrService
    {
        Task PublishLearnerRequest(List<Training> currentIlr, List<Learner> learners, string collectionPeriodText, string featureNumber, Func<Task> clearCache);
    }
}
