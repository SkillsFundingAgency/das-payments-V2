using System;
using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Steps
{
    public abstract class ProviderPaymentsStepsBase : StepsBase
    {

        public List<FundingSourcePayment> FundingSourcePayments { get => Get<List<FundingSourcePayment>>(); set => Set(value); }
        public IPaymentsDataContext DataContext => Container.Resolve<IPaymentsDataContext>();

        protected ProviderPaymentsStepsBase(ScenarioContext scenarioContext) : base(scenarioContext)
        {

        }

        protected async Task<List<PaymentModel>> GetPaymentsAsync(long jobId)
        {
            var paymentDataContext = Container.Resolve<IPaymentsDataContext>();
            return await paymentDataContext.Payment
                                  .Where(o => o.JobId == jobId)
                                  .ToListAsync().ConfigureAwait(false);
        }
    }
}