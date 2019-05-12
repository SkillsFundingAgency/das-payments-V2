using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Activators.Reflection;
using Autofac.Extras.Moq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure;
using SFA.DAS.Payments.RequiredPayments.Application.Mapping;
using SFA.DAS.Payments.RequiredPayments.Application.Processors;
using SFA.DAS.Payments.RequiredPayments.Application.UnitTests.TestHelpers;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;
using Earning = SFA.DAS.Payments.RequiredPayments.Domain.Entities.Earning;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Processors
{
    [TestFixture]
    public class ApprenticeshipContractType2EarningEventProcessorTest
    {
        private AutoMock mocker;
        private ApprenticeshipContractType2EarningEventProcessor act2EarningEventProcessor;
        private Mock<IDataCache<PaymentHistoryEntity[]>> paymentHistoryCacheMock;
        private Mock<IRequiredPaymentProcessor> requiredPaymentService;

        private Mapper mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RequiredPaymentsProfile>());
            config.AssertConfigurationIsValid();
            mapper = new Mapper(config);
        }

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetStrict();
            paymentHistoryCacheMock = mocker.Mock<IDataCache<PaymentHistoryEntity[]>>();
            requiredPaymentService = mocker.Mock<IRequiredPaymentProcessor>();

            act2EarningEventProcessor = mocker.Create<ApprenticeshipContractType2EarningEventProcessor>(
                new NamedParameter("apprenticeshipKey", "key"), 
                new NamedParameter("mapper", mapper),
                new AutowiringParameter());
        }

        [TearDown]
        public void TearDown()
        {
            mocker.Dispose();
        }

        [Test]
        public async Task TestHandleNullEvent()
        {
            // arrange            
            // act
            // assert
            try
            {
                await act2EarningEventProcessor.HandleEarningEvent(null, paymentHistoryCacheMock.Object, CancellationToken.None);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("earningEvent", ex.ParamName);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public async Task TestHandleNormalEvent()
        {
            // arrange
            var period = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2);
            byte deliveryPeriod = 2;

            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Ukprn = 1,
                CollectionPeriod = period,
                CollectionYear = period.AcademicYear,
                Learner = EarningEventDataHelper.CreateLearner(),
                LearningAim = EarningEventDataHelper.CreateLearningAim(),
                OnProgrammeEarnings = new List<OnProgrammeEarning>()
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>()
                        {
                            new EarningPeriod
                            {
                                Amount = 100,
                                Period = deliveryPeriod,
                                PriceEpisodeIdentifier = "2",
                                SfaContributionPercentage = 0.9m,
                            }
                        })
                    }
                }
            };

            var requiredPayments = new List<RequiredPayment>
            {
                new RequiredPayment
                {
                    Amount = 1,
                    EarningType = EarningType.CoInvested,
                },
            };

            var paymentHistoryEntities = new[] { new PaymentHistoryEntity
            {
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2),
                DeliveryPeriod = 2,
                LearnAimReference = earningEvent.LearningAim.Reference,
                TransactionType = (int)OnProgrammeEarningType.Learning
            } };

            var paymentHistory = new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities);
            paymentHistoryCacheMock.Setup(c => c.TryGet(It.Is<string>(key => key == CacheKeys.PaymentHistoryKey),It.IsAny<CancellationToken>())).ReturnsAsync(paymentHistory).Verifiable();
            requiredPaymentService
                .Setup(p => p.GetRequiredPayments(It.Is<Earning>(x => x.Amount == 100), It.Is<List<Payment>>(x => x.Count == 1)))
                .Returns(requiredPayments)
                .Verifiable();

            // act
            var actualRequiredPayment = await act2EarningEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.IsNotNull(actualRequiredPayment);
            Assert.AreEqual(1, actualRequiredPayment.Count);
            Assert.AreEqual(1, actualRequiredPayment.First().AmountDue);
            Assert.AreEqual(earningEvent.LearningAim.Reference, actualRequiredPayment.First().LearningAim.Reference);
        }

        [Test]
        public async Task TestNoEventProducedWhenZeroToPay()
        {
            // arrange
            var period = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2);
            byte deliveryPeriod = 2;

            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Ukprn = 1,
                CollectionPeriod = period,
                CollectionYear = period.AcademicYear,
                Learner = EarningEventDataHelper.CreateLearner(),
                LearningAim = EarningEventDataHelper.CreateLearningAim(),
                OnProgrammeEarnings = new List<OnProgrammeEarning>()
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>()
                        {
                            new EarningPeriod
                            {
                                Amount = 100,
                                Period = deliveryPeriod,
                                PriceEpisodeIdentifier = "2",
                                SfaContributionPercentage = 0.9m,
                            }
                        })
                    }
                }
            };

            var requiredPayments = new List<RequiredPayment>();

            var paymentHistoryEntities = new PaymentHistoryEntity[0];

            var paymentHistory = new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities);
            paymentHistoryCacheMock.Setup(c => c.TryGet(It.Is<string>(key => key == CacheKeys.PaymentHistoryKey),It.IsAny<CancellationToken>())).ReturnsAsync(paymentHistory).Verifiable();

            requiredPaymentService
                .Setup(p => p.GetRequiredPayments(It.Is<Earning>(x => x.Amount == 100), It.Is<List<Payment>>(x => x.Count == 0)))
                .Returns(requiredPayments)
                .Verifiable();

            // act
            var actualRequiredPayment = await act2EarningEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.AreEqual(0, actualRequiredPayment.Count);
        }

        [Test]
        [TestCase(0, "1")]
        [TestCase(0, null)]
        [TestCase(50, "1")]
        public async Task TestPriceEpisodeIdentifierPickedFromHistoryForRefunds(decimal amount, string priceEpisodeIdentifier)
        {
            // arrange
            var period = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 3);
            var deliveryPeriod = (byte)2;

            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Ukprn = 1,
                CollectionPeriod = period,
                CollectionYear = period.AcademicYear,
                Learner = EarningEventDataHelper.CreateLearner(),
                LearningAim = EarningEventDataHelper.CreateLearningAim(),
                OnProgrammeEarnings = new List<OnProgrammeEarning>()
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Balancing,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>()
                        {
                            new EarningPeriod
                            {
                                Amount = amount,
                                Period = deliveryPeriod,
                                PriceEpisodeIdentifier = priceEpisodeIdentifier,
                                SfaContributionPercentage = 0.9m,
                            }
                        })
                    }
                }
            };

            var paymentHistoryEntities = new PaymentHistoryEntity[0];
            var requiredPayments = new List<RequiredPayment>
            {
                new RequiredPayment
                {
                    Amount = 1,
                    EarningType = EarningType.CoInvested,
                    SfaContributionPercentage = 0.8m,
                    PriceEpisodeIdentifier = "2",
                },
            };
            var paymentHistory = new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities);
            paymentHistoryCacheMock.Setup(c => c.TryGet(It.Is<string>(key => key == CacheKeys.PaymentHistoryKey),It.IsAny<CancellationToken>())).ReturnsAsync(paymentHistory).Verifiable();

            requiredPaymentService
                .Setup(p => p.GetRequiredPayments(It.Is<Earning>(x => x.Amount == amount), It.Is<List<Payment>>(x => x.Count == 0)))
                .Returns(requiredPayments)
                .Verifiable();

            // act
            var actualRequiredPayment = await act2EarningEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.IsNotNull(actualRequiredPayment);
            Assert.AreEqual("2", actualRequiredPayment.First().PriceEpisodeIdentifier);

        }

        [Test]
        public async Task TestSfaContributionIsCalculatedForZeroEarningRefunds()
        {
            // arrange
            var period = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 3);
            byte deliveryPeriod = 2;

            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Ukprn = 1,
                SfaContributionPercentage = 0,
                CollectionPeriod = period,
                CollectionYear = period.AcademicYear,
                Learner = EarningEventDataHelper.CreateLearner(),
                LearningAim = EarningEventDataHelper.CreateLearningAim(),
                OnProgrammeEarnings = new List<OnProgrammeEarning>
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Balancing,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        {
                            new EarningPeriod
                            {
                                Period = deliveryPeriod,
                                Amount = 0,
                                PriceEpisodeIdentifier = "priceEpisodeIdentifier",
                                SfaContributionPercentage = 0
                            }
                        })
                    }
                }
            };

            var requiredPayments = new List<RequiredPayment>
            {
                new RequiredPayment
                {
                    Amount = 1,
                    EarningType = EarningType.CoInvested,
                    SfaContributionPercentage = 0.77m,
                },
            };

            var paymentHistoryEntities = new List<PaymentHistoryEntity>();
            var paymentHistory = new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities.ToArray());
            paymentHistoryCacheMock.Setup(c => c.TryGet(It.Is<string>(key => key == CacheKeys.PaymentHistoryKey),It.IsAny<CancellationToken>())).ReturnsAsync(paymentHistory).Verifiable();
            requiredPaymentService
                .Setup(p => p.GetRequiredPayments(It.Is<Earning>(x => x.Amount == 0), It.Is<List<Payment>>(x => x.Count == 0)))
                .Returns(requiredPayments)
                .Verifiable();

            // act
            var actualRequiredPayment = await act2EarningEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.IsNotEmpty(actualRequiredPayment);
            Assert.AreEqual(.77m, ((CalculatedRequiredCoInvestedAmount)actualRequiredPayment[0]).SfaContributionPercentage);
        }

        [Test]
        public async Task TestFuturePeriodsCutOff()
        {
            // arrange
            var period = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2);
            
            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Ukprn = 1,
                CollectionPeriod = period,
                CollectionYear = period.AcademicYear,
                Learner = EarningEventDataHelper.CreateLearner(),
                LearningAim = EarningEventDataHelper.CreateLearningAim(),
                OnProgrammeEarnings = new List<OnProgrammeEarning>()
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>()
                        {
                            new EarningPeriod
                            {
                                Amount = 100,
                                Period = period.Period,
                                PriceEpisodeIdentifier = "2",
                                SfaContributionPercentage = 0.9m,
                            },
                            new EarningPeriod
                            {
                                Amount = 200,
                                Period = (byte)(period.Period + 1),
                                PriceEpisodeIdentifier = "2",
                                SfaContributionPercentage = 0.9m,
                            }
                        })
                    }
                }
            };

            var requiredPayments = new List<RequiredPayment>
            {
                new RequiredPayment
                {
                    Amount = 100,
                    EarningType = EarningType.CoInvested,
                },
            };

            var paymentHistoryEntities = new PaymentHistoryEntity[0];
            var paymentHistory = new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities);
            paymentHistoryCacheMock.Setup(c => c.TryGet(It.Is<string>(key => key == CacheKeys.PaymentHistoryKey),It.IsAny<CancellationToken>())).ReturnsAsync(paymentHistory).Verifiable();
            requiredPaymentService
                .Setup(p => p.GetRequiredPayments(It.Is<Earning>(x => x.Amount == 100), It.Is<List<Payment>>(x => x.Count == 0)))
                .Returns(requiredPayments)
                .Verifiable();


            // act
            var actualRequiredPayment = await act2EarningEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.IsNotNull(actualRequiredPayment);
            Assert.AreEqual(1, actualRequiredPayment.Count);
            Assert.AreEqual(100, actualRequiredPayment.First().AmountDue);
            Assert.AreEqual(earningEvent.LearningAim.Reference, actualRequiredPayment.First().LearningAim.Reference);
        }
    }
}