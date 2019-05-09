namespace SFA.DAS.Payments.AcceptanceTests.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AcceptanceTests.Services.Intefaces;
    using Data;

    public class IlrNullService : IIlrService
    {
        public async Task PublishLearnerRequest(List<Training> currentIlr, string collectionPeriodText, string featureNumber, Func<Task> verifyIlr) => await verifyIlr();
    }
}
