using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core;
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
            var payments = await paymentDataContext.Payment
                                  .Where(o => o.JobId == jobId)
                                  .ToListAsync().ConfigureAwait(false);

            payments?.ForEach(o =>
            {
                o.CollectionPeriod = new CalendarPeriod(o.CollectionPeriod.Year, o.CollectionPeriod.Month);
                o.DeliveryPeriod = new CalendarPeriod(o.DeliveryPeriod.Year, o.DeliveryPeriod.Month);
            });

            return payments;

        }
    }
}