using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.RequiredPayments.Application.Data;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Application.UnitTests.TestHelpers;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Repositories
{
    [TestFixture]
    public class PaymentHistoryRepositoryTest
    {
        private PaymentHistoryRepository _repository;
        private Mock<DbSet<PaymentEntity>> _paymentSetMock;
        private Mock<RequiredPaymentsDataContext> _dedsContextMock;
        private List<PaymentEntity> _paymentEntities;

        [SetUp]
        public void FixtureSetUp()
        {
            _paymentEntities = new List<PaymentEntity>();

            _paymentSetMock = new Mock<DbSet<PaymentEntity>>();

            _paymentSetMock.As<IAsyncEnumerable<PaymentEntity>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new TestAsyncEnumerator<PaymentEntity>(_paymentEntities.GetEnumerator()));

            _paymentSetMock.As<IQueryable<PaymentEntity>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<PaymentEntity>(_paymentEntities.AsQueryable().Provider));

            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.Expression).Returns(_paymentEntities.AsQueryable().Expression);
            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.ElementType).Returns(_paymentEntities.AsQueryable().ElementType);
            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.GetEnumerator()).Returns(() => _paymentEntities.AsQueryable().GetEnumerator());

            _dedsContextMock = new Mock<RequiredPaymentsDataContext>(MockBehavior.Loose);
        }

        [TearDown]
        public void TearDown()
        {
            _dedsContextMock.Verify();
            _paymentSetMock.Verify();
        }

        [Test]
        public async Task TestGetPaymentHistory()
        {
            // arrange
            var apprenticeshipKey = "key";

            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});

            _dedsContextMock.Setup(c => c.PaymentHistory).Returns(_paymentSetMock.Object).Verifiable();
            _repository = new PaymentHistoryRepository(_dedsContextMock.Object);

            // act 
            var result = (await _repository.GetPaymentHistory(apprenticeshipKey).ConfigureAwait(false)).ToArray();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(_paymentEntities.First().Id, result.First().Id);
            Assert.AreEqual(_paymentEntities.Last().Id, result.Last().Id);
        }
    }
}