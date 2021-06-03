using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.RepositoriesTests
{
    [TestFixture]
    public class PaymentClawbackRepositoryTests
    {
        private IPaymentsDataContext context;
        private PaymentClawbackRepository sut;

        [SetUp]
        public void Setup()
        {
            var dbName = Guid.NewGuid().ToString();

            var contextBuilder = new DbContextOptionsBuilder<PaymentsDataContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            context = new PaymentsDataContext(contextBuilder);
            sut = new PaymentClawbackRepository(context);
        }

        [TestCase(ContractType.Act1)]
        [TestCase(ContractType.Act2)]
        public async Task GetLearnerPaymentHistoryReturnsAllPaymentsForLearner(ContractType contractType)
        {
            var payment1 = CreateTestPayment(50.99m, contractType);
            var payment2 = CreateTestPayment(-50.99m, contractType);

            context.Payment.Add(payment1);
            context.Payment.Add(payment2);
            await context.SaveChangesAsync();

            var results = await sut.GetLearnerPaymentHistory(101, contractType, "123456", "ref", 102, 103, 104, 105, 2021, 2, CancellationToken.None);

            results.Count.Should().Be(2);
        }

        [Test]
        public async Task GetLearnerPaymentHistoryReturnsOnlyPaymentsForCurrentAcademicYear()
        {
            var contractType = ContractType.Act2;

            var payment1 = CreateTestPayment(50.99m, contractType);
            
            var payment2 = CreateTestPayment(-50.99m, contractType);
            payment2.CollectionPeriod = new CollectionPeriod { AcademicYear = 1920, Period = 2 };

            context.Payment.Add(payment1);
            context.Payment.Add(payment2);
            await context.SaveChangesAsync();

            var results = await sut.GetLearnerPaymentHistory(101, contractType, "123456", "ref", 102, 103, 104, 105, 2021, 2, CancellationToken.None);

            results.Count.Should().Be(1);
        }

        [TestCase(ContractType.Act1)]
        [TestCase(ContractType.Act2)]
        public async Task GetLearnerPaymentHistoryReturnsAllPaymentsForPreviousCollectionPeriodsExcludingCurrentCollectionPeriod(ContractType contractType)
        {
            var payment1 = CreateTestPayment(50.99m, contractType);
            var payment2 = CreateTestPayment(-50.99m, contractType);
            payment2.CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 2 };

            context.Payment.Add(payment1);
            context.Payment.Add(payment2);
            await context.SaveChangesAsync();

            var results = await sut.GetLearnerPaymentHistory(101, contractType, "123456", "ref", 102, 103, 104, 105, 2021, 2, CancellationToken.None);

            results.Count.Should().Be(1);
        }

        [Test]
        public async Task SaveClawbackPaymentsSavesNewRecordsAndDoesNotUpdateExistingRecords()
        {
            var contractType = ContractType.Act2;

            var payment1 = CreateTestPayment(50.99m, contractType);
            var payment2 = CreateTestPayment(-50.99m, contractType);

            context.Payment.Add(payment1);
            context.Payment.Add(payment2);
            await context.SaveChangesAsync();

            var paymentHistory = await sut.GetLearnerPaymentHistory(101, contractType, "123456", "ref", 102, 103, 104, 105, 2021, 2, CancellationToken.None);

            paymentHistory.Count.Should().Be(2);

            paymentHistory.ForEach(p =>
            {
                p.Id = 0; 
                p.Amount *= -1; 
                p.CollectionPeriod = new CollectionPeriod { Period = 2, AcademicYear = 2021 };
            });

            await sut.SaveClawbackPayments(paymentHistory);

            var result = await context.Payment.ToListAsync();

            result.Count.Should().Be(4);
        }


        private PaymentModel CreateTestPayment(decimal amount, ContractType contractType)
        {
            return new PaymentModel
            {
                Ukprn = 101,
                LearnerReferenceNumber = "123456",
                LearningAimReference = "ref",
                LearningAimFrameworkCode = 102,
                LearningAimPathwayCode = 103,
                LearningAimProgrammeType = 104,
                LearningAimStandardCode = 105,
                FundingSource = FundingSourceType.CoInvestedEmployer,
                CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                Amount = amount,
                ContractType = contractType,
            };
        }
    }
}
