using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.PaymentsDue.Application.Repositories;
using SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueService;
using SFA.DAS.Payments.PaymentsDue.Domain.Entities;
using SFA.DAS.Payments.PaymentsDue.Model.Entities;

namespace SFA.DAS.Payments.PaymentsDue.UnitTests.Application.Repositories
{
    [TestFixture]
    public class ReliableCollectionCacheTest
    {
        private IRepositoryCache<IEnumerable<PaymentEntity>> _repositoryCache;
        private Mock<IActorStateManager> _stateManagerMock;

        [SetUp]
        public void SetUp()
        {
            _stateManagerMock = new Mock<IActorStateManager>(MockBehavior.Strict);
            _repositoryCache = new ReliableCollectionCache<IEnumerable<PaymentEntity>>(_stateManagerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _stateManagerMock.Verify();
        }

        [Test]
        public async Task TestContains()
        {
            // arrange
            var key = "key";
            _stateManagerMock.Setup(s => s.ContainsStateAsync(key, default(CancellationToken))).ReturnsAsync(true).Verifiable();

            // act
            var result = await _repositoryCache.Contains(key).ConfigureAwait(false);

            // assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task TestAdd()
        {
            // arrange
            var key = "key";
            var entities = new[]
            {
                new PaymentEntity {ApprenticeshipKey = "1"},
                new PaymentEntity {ApprenticeshipKey = "2"}
            };
            _stateManagerMock.Setup(s => s.AddStateAsync(key, (IEnumerable<PaymentEntity>)entities, It.IsAny<CancellationToken>())).Returns(Task.FromResult(0)).Verifiable();

            // act
            await _repositoryCache.Add(key, entities).ConfigureAwait(false);

            // assert            
        }

        [Test]
        public async Task TestTryGet()
        {
            // arrange
            var key = "key";
            var entities = new[]
            {
                new PaymentEntity {ApprenticeshipKey = "1"},
                new PaymentEntity {ApprenticeshipKey = "2"}
            };
            var conditionalValue = new Microsoft.ServiceFabric.Data.ConditionalValue<IEnumerable<PaymentEntity>>(true, entities);
            _stateManagerMock.Setup(s => s.TryGetStateAsync<IEnumerable<PaymentEntity>>(key, default(CancellationToken))).ReturnsAsync(conditionalValue).Verifiable();

            // act
            var result = await _repositoryCache.TryGet(key).ConfigureAwait(false);

            // assert    
            Assert.IsTrue(result.HasValue);
            Assert.AreSame(entities, result.Value);
        }
        [Test]
        public async Task TestClear()
        {
            // arrange
            var key = "key";
            _stateManagerMock.Setup(s => s.TryRemoveStateAsync(key, default(CancellationToken))).ReturnsAsync(true).Verifiable();

            // act
            await _repositoryCache.Clear(key).ConfigureAwait(false);

            // assert    
        }
    }
}
