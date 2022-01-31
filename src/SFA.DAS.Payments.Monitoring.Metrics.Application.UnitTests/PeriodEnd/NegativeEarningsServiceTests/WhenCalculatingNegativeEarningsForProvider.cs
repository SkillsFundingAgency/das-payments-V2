using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Application.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Model;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests.PeriodEnd.NegativeEarningsServiceTests
{
    [TestFixture]
    public class WhenCalculatingNegativeEarningsForProvider
    {
        private Fixture fixture;
        private Random random;

        private long ukprn;

        private List<ProviderLearnerNegativeEarningsTotal> providerLearnerNegativeEarnings;
        private List<ProviderLearnerContractTypeAmounts> providerLearnerPayments;
        private List<ProviderLearnerDataLockEarningsTotal> providerLearnerDataLocks;

        private NegativeEarningsService sut;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            random = new Random();

            ukprn = fixture.Create<long>();

            providerLearnerNegativeEarnings = fixture.Create<List<ProviderLearnerNegativeEarningsTotal>>();
            providerLearnerNegativeEarnings.ForEach(x =>
            {
                x.Ukprn = ukprn;
                x.ContractType = random.Next(0, 2) == 1 ? ContractType.Act1 : ContractType.Act2;
                x.NegativeEarningsTotal = random.Next(1, 100000);
            });
            providerLearnerPayments = new List<ProviderLearnerContractTypeAmounts>();
            providerLearnerDataLocks = new List<ProviderLearnerDataLockEarningsTotal>();

            sut = new NegativeEarningsService();
        }

        [Test]
        public void WithEmptyNegativeLearnerEarnings_ThenReturnsEmptyResult()
        {
            //Arrange
            providerLearnerNegativeEarnings.Clear();

            //Act
            var result = sut.CalculateNegativeEarningsForProvider(providerLearnerNegativeEarnings, providerLearnerPayments, providerLearnerDataLocks);

            //Assert
            result.ContractType1.Should().BeNull();
            result.ContractType2.Should().BeNull();
        }

        [Test]
        public void WithNullNegativeLearnerEarnings_ThenReturnsEmptyResult()
        {
            //Arrange
            providerLearnerNegativeEarnings = null;

            //Act
            var result = sut.CalculateNegativeEarningsForProvider(providerLearnerNegativeEarnings, providerLearnerPayments, providerLearnerDataLocks);

            //Assert
            result.ContractType1.Should().BeNull();
            result.ContractType2.Should().BeNull();
        }

        [Test]
        public void WithSingleLearnerWithNegativeEarnings_AndLearnerHasPayments_ThenReturnsEmptyResult()
        {
            //Arrange
            providerLearnerNegativeEarnings.RemoveRange(1, providerLearnerNegativeEarnings.Count - 1);
            AddPaymentForLearner(providerLearnerNegativeEarnings.First().Uln);

            //Act
            var result = sut.CalculateNegativeEarningsForProvider(providerLearnerNegativeEarnings, providerLearnerPayments, providerLearnerDataLocks);

            //Assert
            result.ContractType1.Should().BeNull();
            result.ContractType2.Should().BeNull();
        }

        [Test]
        public void WithSingleLearnerWithNegativeEarnings_AndLearnerHasNoPayments_AndLearnerHasDataLock_ThenReturnsEmptyResult()
        {
            //Arrange
            providerLearnerNegativeEarnings.RemoveRange(1, providerLearnerNegativeEarnings.Count - 1);
            AddDataLockForLearner(providerLearnerNegativeEarnings.First().Uln);

            //Act
            var result = sut.CalculateNegativeEarningsForProvider(providerLearnerNegativeEarnings, providerLearnerPayments, providerLearnerDataLocks);

            //Assert
            result.ContractType1.Should().BeNull();
            result.ContractType2.Should().BeNull();
        }

        [Test]
        public void WithSingleLearnerWithNegativeEarnings_AndLearnerHasNoPayments_AndLearnerNoDataLocks_ThenCorrectlyCalculatesNegativeEarnings()
        {
            //Arrange
            providerLearnerNegativeEarnings.RemoveRange(1, providerLearnerNegativeEarnings.Count - 1);

            var expectedAct1NegativeEarningsTotal = providerLearnerNegativeEarnings.Where(x => x.ContractType == ContractType.Act1).Sum(x => x.NegativeEarningsTotal);
            var expectedAct2NegativeEarningsTotal = providerLearnerNegativeEarnings.Where(x => x.ContractType == ContractType.Act2).Sum(x => x.NegativeEarningsTotal);

            //Act
            var result = sut.CalculateNegativeEarningsForProvider(providerLearnerNegativeEarnings, providerLearnerPayments, providerLearnerDataLocks);

            //Assert
            if (expectedAct1NegativeEarningsTotal == 0m)
            {
                result.ContractType1.Should().BeNull();
                result.ContractType2.Should().Be(expectedAct2NegativeEarningsTotal);
            }
            else
            {
                result.ContractType1.Should().Be(expectedAct1NegativeEarningsTotal);
                result.ContractType2.Should().BeNull();
            }
        }

        [Test]
        public void WithMultipleLearnersWithNegativeEarnings_AndOnlyOneHasNoPaymentsAndNoDataLocks_ThenCorrectlyCalculatesNegativeEarnings()
        {
            //Arrange
            var ulnUnderTest = providerLearnerNegativeEarnings.First().Uln;
            var remainingUlns = providerLearnerNegativeEarnings.Where(x => x.Uln != ulnUnderTest).ToList();
            
            remainingUlns.ForEach(x =>
            {
                AddDataLockForLearner(x.Uln);
                AddPaymentForLearner(x.Uln);
            });

            var expectedAct1NegativeEarningsTotal = providerLearnerNegativeEarnings.Where(x => x.ContractType == ContractType.Act1 && x.Uln == ulnUnderTest).Sum(x => x.NegativeEarningsTotal);
            var expectedAct2NegativeEarningsTotal = providerLearnerNegativeEarnings.Where(x => x.ContractType == ContractType.Act2 && x.Uln == ulnUnderTest).Sum(x => x.NegativeEarningsTotal);

            //Act
            var result = sut.CalculateNegativeEarningsForProvider(providerLearnerNegativeEarnings, providerLearnerPayments, providerLearnerDataLocks);

            //Assert
            if (expectedAct1NegativeEarningsTotal == 0m)
            {
                result.ContractType1.Should().BeNull();
                result.ContractType2.Should().Be(expectedAct2NegativeEarningsTotal);
            }
            else
            {
                result.ContractType1.Should().Be(expectedAct1NegativeEarningsTotal);
                result.ContractType2.Should().BeNull();
            }
        }

        [Test]
        public void WithMultipleLearnersWithNegativeEarnings_AndManyLearnersHaveNoPaymentsAndNoDataLocks_ThenCorrectlyCalculatesNegativeEarnings()
        {
            //Arrange
            var ulnsUnderTest = providerLearnerNegativeEarnings.Take(2).Select(x => x.Uln);
            var remainingUlns = providerLearnerNegativeEarnings.Where(x => !ulnsUnderTest.Contains(x.Uln)).ToList();
            
            remainingUlns.ForEach(x =>
            {
                AddDataLockForLearner(x.Uln);
                AddPaymentForLearner(x.Uln);
            });

            var expectedAct1NegativeEarningsTotal = providerLearnerNegativeEarnings.Where(x => x.ContractType == ContractType.Act1 && ulnsUnderTest.Contains(x.Uln)).Sum(x => x.NegativeEarningsTotal);
            var expectedAct2NegativeEarningsTotal = providerLearnerNegativeEarnings.Where(x => x.ContractType == ContractType.Act2 && ulnsUnderTest.Contains(x.Uln)).Sum(x => x.NegativeEarningsTotal);

            //Act
            var result = sut.CalculateNegativeEarningsForProvider(providerLearnerNegativeEarnings, providerLearnerPayments, providerLearnerDataLocks);

            //Assert
            if (expectedAct1NegativeEarningsTotal == 0m)
            {
                result.ContractType1.Should().BeNull();
                result.ContractType2.Should().Be(expectedAct2NegativeEarningsTotal);
            }
            else if (expectedAct2NegativeEarningsTotal == 0m)
            {
                result.ContractType1.Should().Be(expectedAct1NegativeEarningsTotal);
                result.ContractType2.Should().BeNull();
            }
            else
            {
                result.ContractType1.Should().Be(expectedAct1NegativeEarningsTotal);
                result.ContractType2.Should().Be(expectedAct2NegativeEarningsTotal);
            }
        }

        private void AddDataLockForLearner(long uln)
        {
            var dataLock = fixture.Create<ProviderLearnerDataLockEarningsTotal>();
            dataLock.LearnerUln = uln;
            dataLock.Ukprn = ukprn;
            providerLearnerDataLocks.Add(dataLock);
        }

        private void AddPaymentForLearner(long uln)
        {
            var payment = fixture.Create<ProviderLearnerContractTypeAmounts>();
            payment.LearnerUln = uln;
            payment.Ukprn = ukprn;
            providerLearnerPayments.Add(payment);
        }
    }
}