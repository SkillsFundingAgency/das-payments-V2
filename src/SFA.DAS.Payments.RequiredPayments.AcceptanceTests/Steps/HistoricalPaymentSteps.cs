using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using Autofac;
using Microsoft.SqlServer.Dac;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class HistoricalPaymentSteps : StepsBase
    {
        public HistoricalPaymentSteps(ScenarioContext context) : base(context)
        {
        }

        [BeforeTestRun(Order = 40)]
        public static void SetUpPaymentsDataContext()
        {
            Builder.Register((c, p) =>
            {
                var configHelper = c.Resolve<TestsConfiguration>();
                return new PaymentsDataContext(configHelper.GetConnectionString("PaymentsConnectionString"));
            }).As<IPaymentsDataContext>().InstancePerDependency();
        }

#if DEBUG
        [BeforeTestRun(Order = 60)]
        public static void SetUpPaymentsDb()
        {
            if (!IsDevEnvironment) return;
            var instance = new DacServices(Config.PaymentsConnectionString);
            var path = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(typeof(HistoricalPaymentSteps).Assembly.Location) ?? throw new InvalidOperationException("Failed to get assembly location path"),
                @"..\..\..\SFA.DAS.Payments.Database\bin\Debug\SFA.DAS.Payments.Database.dacpac"));

            var builder = new SqlConnectionStringBuilder(Config.PaymentsConnectionString);

            using (var dacpac = DacPackage.Load(path))
            {
                instance.Deploy(dacpac, builder.InitialCatalog, true);
            }
        }
#endif

        private void AddHistoricalPayments(IList<HistoricalPayment> payments)
        {
            var paymentHistoryDataContext = Container.Resolve<IPaymentsDataContext>();
            paymentHistoryDataContext.Payment.AddRange(payments.SelectMany(ToPayments));
            paymentHistoryDataContext.SaveChanges();
        }

        [Given(@"the following historical contract type (.*) payments exist:")]
        public void GivenTheFollowingHistoricalContractTypePaymentsExist(int p0, Table table)
        {
            AddHistoricalPayments(table.CreateSet<HistoricalPayment>().ToList());
        }

        private List<PaymentModel> ToPayments(HistoricalPayment payment)
        {
            return new List<PaymentModel>
            {
                ToPayment(payment, FundingSourceType.CoInvestedEmployer),
                ToPayment(payment, FundingSourceType.CoInvestedSfa)
            };
        }

        private PaymentModel ToPayment(HistoricalPayment payment, FundingSourceType fundingSource)
        {
            return new PaymentModel
            {
                ExternalId = Guid.NewGuid(),
                Ukprn = TestSession.Ukprn,
                LearnerReferenceNumber = TestSession.Learner.LearnRefNumber,
                LearnerUln = TestSession.Learner.Uln,
                PriceEpisodeIdentifier = payment.PriceEpisodeIdentifier,
                Amount = GetFundingAmount(payment.Amount, fundingSource),
                CollectionPeriod = new CalendarPeriod(CollectionYear, CollectionPeriod),
                DeliveryPeriod = new CalendarPeriod(CollectionYear, payment.Delivery_Period),
                LearningAimReference = TestSession.Learner.Course.LearnAimRef,
                LearningAimProgrammeType = TestSession.Learner.Course.ProgrammeType,
                LearningAimStandardCode = TestSession.Learner.Course.StandardCode,
                LearningAimFrameworkCode = TestSession.Learner.Course.FrameworkCode,
                LearningAimPathwayCode = TestSession.Learner.Course.PathwayCode,
                LearningAimFundingLineType = TestSession.Learner.Course.FundingLineType,
                FundingSource = fundingSource,
                ContractType = Payments.Model.Core.Entities.ContractType.ContractWithEmployer,
                SfaContributionPercentage = SfaContributionPercentage,
                JobId = TestSession.JobId,
                TransactionType = (TransactionType)payment.Type,
            };
        }

        private decimal GetFundingAmount(decimal amount, FundingSourceType fundingSource)
        {
            switch (fundingSource)
            {
                case FundingSourceType.CoInvestedEmployer:
                    return amount * .2m;
                case FundingSourceType.CoInvestedSfa:
                    return amount * .8m;
                default:
                    return amount;
            }
        }
    }
}