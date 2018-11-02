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
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Steps
{
    public abstract class ProviderPaymentsStepsBase : StepsBase
    {

        public List<FundingSourcePayment> FundingSourcePayments { get => Get<List<FundingSourcePayment>>(); set => Set(value); }

        protected ProviderPaymentsStepsBase(ScenarioContext scenarioContext) : base(scenarioContext)
        {

        }

        protected List<PaymentModel> GetPayments(long jobId)
        {
            var paymentDataContext = Container.Resolve<IPaymentsDataContext>();
            var payments = paymentDataContext.Payment
                                  .Where(o => o.JobId == jobId)
                .ToList();

            payments.ForEach(o =>
            {
                o.CollectionPeriod = new CalendarPeriod(o.CollectionPeriod.Year, o.CollectionPeriod.Month);
                o.DeliveryPeriod = new CalendarPeriod(o.DeliveryPeriod.Year, o.DeliveryPeriod.Month);
            });

            return payments;

        }

        protected FundingSourcePaymentEvent CreateFundingSourcePaymentEvent(FundingSourcePayment fundingSourcePayment)
        {
            FundingSourcePaymentEvent paymentEvent;

            switch (fundingSourcePayment.FundingSourceType)
            {
                case FundingSourceType.CoInvestedSfa:
                    paymentEvent = new SfaCoInvestedFundingSourcePaymentEvent();
                    break;
                case FundingSourceType.CoInvestedEmployer:
                    paymentEvent = new EmployerCoInvestedFundingSourcePaymentEvent();
                    break;
                default:
                    //TODO Implement other FundingSourceTypes
                    throw new NotImplementedException("Unhandled Funding Source Type");
            }

            paymentEvent.FundingSourceType = fundingSourcePayment.FundingSourceType;
            paymentEvent.IlrSubmissionDateTime = DateTime.UtcNow;
            paymentEvent.ContractType = (ContractType)ContractType;
            paymentEvent.Learner = TestSession.Learner.ToLearner();
            paymentEvent.Ukprn = TestSession.Ukprn;
            paymentEvent.OnProgrammeEarningType = fundingSourcePayment.Type;
            paymentEvent.AmountDue = fundingSourcePayment.Amount;
            paymentEvent.JobId = TestSession.JobId;
            paymentEvent.EventTime = DateTimeOffset.UtcNow;
            paymentEvent.SfaContributionPercentage = SfaContributionPercentage;
            paymentEvent.CollectionPeriod = new CalendarPeriod(GetYear(CollectionPeriod, CollectionYear).ToString(), CollectionPeriod);
            paymentEvent.DeliveryPeriod = new CalendarPeriod(GetYear(fundingSourcePayment.DeliveryPeriod, CollectionYear).ToString(), fundingSourcePayment.DeliveryPeriod);
            paymentEvent.LearningAim = TestSession.Learner.Course.ToLearningAim();
            paymentEvent.PriceEpisodeIdentifier = "P1";
            return paymentEvent;
        }
    }
}