using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.PaymentsDue.Application.Data;
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

        [SetUp]
        public void FixtureSetUp()
        {
            _cacheMock = new Mock<IRepositoryCache<IEnumerable<PaymentEntity>>>(MockBehavior.Strict);
            _dedsContextMock = new Mock<DedsContext>(MockBehavior.Strict);

            _paymentEntities = new List<PaymentEntity>();
            _paymentSetMock = new Mock<DbSet<PaymentEntity>>(MockBehavior.Default);
            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.Provider).Returns(_paymentEntities.AsQueryable().Provider);
            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.Expression).Returns(_paymentEntities.AsQueryable().Expression);
            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.ElementType).Returns(_paymentEntities.AsQueryable().ElementType);
            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.GetEnumerator()).Returns(_paymentEntities.GetEnumerator());
        }

        [TearDown]
        public void TearDown()
        {
            _cacheMock.VerifyAll();
            _dedsContextMock.VerifyAll();
            _paymentSetMock.VerifyAll();
        }

        [Test]
        public async Task TestGetPaymentHistoryPopulatesCache()
        {
            // arrange
            var key = "some apprenticeship";
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid()});
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid()});

            _cacheMock.Setup(c => c.IsInitialised).Returns(false).Verifiable();
            _dedsContextMock.Setup(c => c.PaymentHistory).Returns(_paymentSetMock.Object).Verifiable();
            _cacheMock.Setup(c => c.Add(key, _paymentEntities)).Verifiable();
            _cacheMock.SetupSet(c => c.IsInitialised = true).Verifiable();

            _repository = new PaymentHistoryRepository(_cacheMock.Object, _dedsContextMock.Object);

            // act 
            var result = (await _repository.GetPaymentHistory(key)).ToArray();

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
            var key = "some apprenticeship";
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid()});
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid()});

            _cacheMock.Setup(c => c.IsInitialised).Returns(true).Verifiable();
            _cacheMock.Setup(c => c.Get(key)).ReturnsAsync(_paymentEntities).Verifiable();

            _repository = new PaymentHistoryRepository(_cacheMock.Object, _dedsContextMock.Object);


            // act 
            var result = (await _repository.GetPaymentHistory(key)).ToArray();

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
            var key = "some apprenticeship";
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid()});
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid()});

            _dedsContextMock.Setup(c => c.PaymentHistory).Returns(_paymentSetMock.Object).Verifiable();
            _repository = new PaymentHistoryRepository(_dedsContextMock.Object);


            // act 
            var result = (await _repository.GetPaymentHistory(key)).ToArray();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(_paymentEntities[0].Id, result.First().Id);
            Assert.AreEqual(_paymentEntities[1].Id, result.Last().Id);
        }
    }
}