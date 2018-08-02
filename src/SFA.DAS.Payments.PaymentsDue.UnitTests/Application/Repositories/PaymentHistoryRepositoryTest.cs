using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.PaymentsDue.Application.Data;
using SFA.DAS.Payments.PaymentsDue.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.PaymentsDue.Application.Repositories;
using SFA.DAS.Payments.PaymentsDue.Model.Entities;

namespace SFA.DAS.Payments.PaymentsDue.UnitTests.Application.Repositories
{
    [TestFixture]
    public class PaymentHistoryRepositoryTest
    {
        private IPaymentHistoryRepository _repository;
        private Mock<IRepositoryCache<IEnumerable<PaymentEntity>>> _cacheMock;
        private Mock<DbSet<PaymentEntity>> _paymentSetMock;
        private Mock<DedsContext> _dedsContextMock;
        private List<PaymentEntity> _paymentEntities;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Mapper.Initialize(cfg => AutoMapperConfigurationFactory.CreateMappingConfig());
        }

        [SetUp]
        public void FixtureSetUp()
        {
            _cacheMock = new Mock<IRepositoryCache<IEnumerable<PaymentEntity>>>(MockBehavior.Strict);
            _dedsContextMock = new Mock<DedsContext>(MockBehavior.Loose);

            _paymentEntities = new List<PaymentEntity>();
            _paymentSetMock = new Mock<DbSet<PaymentEntity>>(MockBehavior.Loose);
            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.Provider).Returns(_paymentEntities.AsQueryable().Provider);
            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.Expression).Returns(_paymentEntities.AsQueryable().Expression);
            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.ElementType).Returns(_paymentEntities.AsQueryable().ElementType);
            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.GetEnumerator()).Returns(_paymentEntities.GetEnumerator());

            _paymentSetMock.As<IAsyncEnumerable<PaymentEntity>>().Setup(m => m.GetEnumerator()).Returns(_paymentEntities.ToAsyncEnumerable().GetEnumerator);
        }

        [TearDown]
        public void TearDown()
        {
            _cacheMock.Verify();
            _dedsContextMock.Verify();
            _paymentSetMock.Verify();
        }

        [Test]
        public async Task TestGetPaymentHistoryPopulatesCache()
        {
            // arrange
            var apprenticeshipKey = "key";

            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});

            _cacheMock.Setup(c => c.IsInitialised).Returns(false).Verifiable();
            _dedsContextMock.Setup(c => c.PaymentHistory).Returns(_paymentSetMock.Object).Verifiable();
            _cacheMock.Setup(c => c.Reset()).Returns(Task.FromResult(false)).Verifiable();
            _cacheMock.Setup(c => c.Add(apprenticeshipKey, It.IsAny<IEnumerable<PaymentEntity>>())).Returns(Task.FromResult(false)).Verifiable();
            _cacheMock.Setup(c => c.Get(apprenticeshipKey)).ReturnsAsync(_paymentEntities).Verifiable();
            _cacheMock.SetupSet(c => c.IsInitialised = true).Verifiable();

            _repository = new PaymentHistoryRepository(_cacheMock.Object, _dedsContextMock.Object);

            // act 
            var result = (await _repository.GetPaymentHistory(apprenticeshipKey)).ToArray();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(_paymentEntities[0].Id, result.First().Id);
            Assert.AreEqual(_paymentEntities[1].Id, result.Last().Id);
        }

        [Test]
        public async Task TestGetPaymentHistoryUsesCache()
        {
            // arrange
            var apprenticeshipKey = "key";

            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});

            _cacheMock.Setup(c => c.IsInitialised).Returns(true).Verifiable();
            _cacheMock.Setup(c => c.Get(apprenticeshipKey)).ReturnsAsync(_paymentEntities).Verifiable();

            _repository = new PaymentHistoryRepository(_cacheMock.Object, _dedsContextMock.Object);


            // act 
            var result = (await _repository.GetPaymentHistory(apprenticeshipKey)).ToArray();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(_paymentEntities[0].Id, result.First().Id);
            Assert.AreEqual(_paymentEntities[1].Id, result.Last().Id);
        }



        [Test]
        public async Task TestGetPaymentHistoryCanWorkWithoutCache()
        {
            // arrange
            var apprenticeshipKey = "key";

            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = "some other key"});

            _dedsContextMock.Setup(c => c.PaymentHistory).Returns(_paymentSetMock.Object).Verifiable();
            _repository = new PaymentHistoryRepository(_dedsContextMock.Object);


            // act 
            var result = (await _repository.GetPaymentHistory(apprenticeshipKey)).ToArray();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(_paymentEntities[0].Id, result.First().Id);
            Assert.AreEqual(_paymentEntities[1].Id, result.Last().Id);
        }
    }
}