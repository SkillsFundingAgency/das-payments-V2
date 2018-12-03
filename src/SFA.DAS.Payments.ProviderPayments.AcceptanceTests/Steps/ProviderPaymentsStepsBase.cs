﻿using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Steps
{
    public abstract class ProviderPaymentsStepsBase : StepsBase
    {

        public List<FundingSourcePayment> FundingSourcePayments { get => Get<List<FundingSourcePayment>>(); set => Set(value); }

        protected ProviderPaymentsStepsBase(ScenarioContext scenarioContext) : base(scenarioContext)
        {

        }

        protected async Task<List<PaymentModel>> GetPaymentsAsync(long jobId)
        {
            var paymentDataContext = Container.Resolve<IPaymentsDataContext>();
            var payments = await paymentDataContext.Payment
                .Where(o => o.JobId == jobId)
                .ToListAsync();

            payments.ForEach(o =>
            {
                o.CollectionPeriod = new CalendarPeriod(o.CollectionPeriod.Year, o.CollectionPeriod.Month);
                o.DeliveryPeriod = new CalendarPeriod(o.DeliveryPeriod.Year, o.DeliveryPeriod.Month);
            });

            return payments;

        }

        protected FundingSourcePaymentEvent CreateFundingSourcePaymentEvent(FundingSourcePayment fundingSourcePayment, DateTime? submissionTime )
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
                case FundingSourceType.FullyFundedSfa:
                    paymentEvent = new SfaFullyFundedFundingSourcePaymentEvent();
                    break;
                default:
                    throw new NotImplementedException("Unhandled Funding Source Type");
            }

            paymentEvent.FundingSourceType = fundingSourcePayment.FundingSourceType;
            paymentEvent.IlrSubmissionDateTime = submissionTime?? DateTime.UtcNow;
            paymentEvent.ContractType = (ContractType)ContractType;
            paymentEvent.Learner = TestSession.Learner.ToLearner();
            paymentEvent.Ukprn = TestSession.Ukprn;
            paymentEvent.TransactionType = fundingSourcePayment.Type;
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