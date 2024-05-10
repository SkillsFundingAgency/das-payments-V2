using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autofac.Extras.Moq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpisodeChanges;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    [TestFixture]
    public class PriceEpisodeStatusCalculatorTests
    {
        private AutoMock mocker;
        private const short DefaultAcademicYear = 2324;
         
        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
        }


        [Test]
        public void No_Previous_Price_Episode_Returns_New()
        {
            var calc = mocker.Create<PriceEpisodeStatusCalculator>();

            var priceEpisode = new PriceEpisode
            {
                Identifier = "1234",
                AgreedPrice = 1000
            };

            var earnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {
                    Type = OnProgrammeEarningType.Learning,
                    Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                    {
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 1, Period = 1, 
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>()
                        }
                    })
                }

            };

            calc.DetermineStatus(2324, priceEpisode, earnings, new List<PriceEpisodeStatusChange>()).Should().Be(PriceEpisodeStatus.New);
        }

        [Test]
        public void Price_Episode_Not_Found_In_Previous_Price_Episodes_Returns_New()
        {
            var calc = mocker.Create<PriceEpisodeStatusCalculator>();

            var priceEpisode = new PriceEpisode
            {
                Identifier = "1234",
                AgreedPrice = 1000, 
            };

            var earnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {
                    
                    Type = OnProgrammeEarningType.Learning,
                    Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                    {
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 1, Period = 1,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>()
                        }
                    })
                }
            };

            var previousPriceEpisodeStatuses = new List<PriceEpisodeStatusChange>
            {
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = "4321" , AcademicYear = "2324" }
                }
            };

            calc.DetermineStatus(2324,priceEpisode, earnings, previousPriceEpisodeStatuses).Should().Be(PriceEpisodeStatus.New);
        }

        [Test]
        public void Price_Episode_Not_Found_In_Current_Year_Returns_New()
        {
            var calc = mocker.Create<PriceEpisodeStatusCalculator>();

            var priceEpisode = new PriceEpisode
            {
                Identifier = "1234",
                AgreedPrice = 1000,
            };

            var earnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {

                    Type = OnProgrammeEarningType.Learning,
                    Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                    {
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 1, Period = 1,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>()
                        }
                    })
                }
            };

            var previousPriceEpisodeStatuses = new List<PriceEpisodeStatusChange>
            {
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = "1234" , AcademicYear = "2223" }, 
                    AgreedPrice = 1000
                }
            };

            calc.DetermineStatus(2324, priceEpisode, earnings, previousPriceEpisodeStatuses).Should().Be(PriceEpisodeStatus.New);
        }

        [Test]
        public void Changed_Price_Returns_Updated()
        {
            var calc = mocker.Create<PriceEpisodeStatusCalculator>();

            var priceEpisode = new PriceEpisode
            {
                Identifier = "1234",
                AgreedPrice = 1500,
            };

            var earnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {

                    Type = OnProgrammeEarningType.Learning,
                    Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                    {
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 1, Period = 1,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>()
                        }
                    })
                }
            };

            var previousPriceEpisodeStatuses = new List<PriceEpisodeStatusChange>
            {
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = "1234" , AcademicYear = "2324", CommitmentId = 1 },
                    AgreedPrice = 1000
                }
            };

            calc.DetermineStatus(2324, priceEpisode, earnings, previousPriceEpisodeStatuses).Should().Be(PriceEpisodeStatus.Updated);
        }

        [Test]
        public void Difference_In_Number_Of_DataLocks_Returns_Updated()
        {
            var calc = mocker.Create<PriceEpisodeStatusCalculator>();

            var priceEpisode = new PriceEpisode
            {
                Identifier = "1234",
                AgreedPrice = 1000,
            };

            var earnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {

                    Type = OnProgrammeEarningType.Learning,
                    Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                    {
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 1, Period = 1,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>
                            {
                                new DataLockFailure { ApprenticeshipId = 1, DataLockError = DataLockErrorCode.DLOCK_03, ApprenticeshipPriceEpisodeIds = new List<long>{ 1 } },
                                new DataLockFailure { ApprenticeshipId = 1, DataLockError = DataLockErrorCode.DLOCK_04, ApprenticeshipPriceEpisodeIds = new List<long>{ 1 } },
                            }
                        },
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 1, Period = 2,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>()
                        },
                    })
                }
            };

            var previousPriceEpisodeStatuses = new List<PriceEpisodeStatusChange>
            {
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = priceEpisode.Identifier , AcademicYear = DefaultAcademicYear.ToString(), CommitmentId = 1 },
                    AgreedPrice = priceEpisode.AgreedPrice, 
                    Errors = new LegacyDataLockEventError[]
                    {
                        new LegacyDataLockEventError
                        {
                            ErrorCode = "DLOCK_03", 
                            SystemDescription = "No matching record found in the employer digital account for the standard code"
                        }
                    },
                }
            };

            calc.DetermineStatus(DefaultAcademicYear, priceEpisode, earnings, previousPriceEpisodeStatuses).Should().Be(PriceEpisodeStatus.Updated);
        }

        [Test]
        public void Different_DataLocks_Returns_Updated()
        {
            var calc = mocker.Create<PriceEpisodeStatusCalculator>();

            var priceEpisode = new PriceEpisode
            {
                Identifier = "1234",
                AgreedPrice = 1000,
            };

            var earnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {

                    Type = OnProgrammeEarningType.Learning,
                    Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                    {
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 1, Period = 1,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>
                            {
                                new DataLockFailure { ApprenticeshipId = 1, DataLockError = DataLockErrorCode.DLOCK_04, ApprenticeshipPriceEpisodeIds = new List<long>{ 1 } },
                            }
                        },
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 1, Period = 2,
                            DataLockFailures = new List<DataLockFailure>()
                        },
                    })
                }
            };

            var previousPriceEpisodeStatuses = new List<PriceEpisodeStatusChange>
            {
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = priceEpisode.Identifier , AcademicYear = DefaultAcademicYear.ToString(), CommitmentId = 1 },
                    AgreedPrice = priceEpisode.AgreedPrice,
                    Errors = new LegacyDataLockEventError[]
                    {
                        new LegacyDataLockEventError
                        {
                            ErrorCode = DataLockErrorCode.DLOCK_03.ToString(),
                            SystemDescription = "No matching record found in the employer digital account for the standard code"
                        }
                    },
                }
            };

            calc.DetermineStatus(DefaultAcademicYear, priceEpisode, earnings, previousPriceEpisodeStatuses).Should().Be(PriceEpisodeStatus.Updated);
        }

        [Test]
        public void Different_Matched_Learners_Returns_Updated()
        {
            var calc = mocker.Create<PriceEpisodeStatusCalculator>();

            var priceEpisode = new PriceEpisode
            {
                Identifier = "1234",
                AgreedPrice = 1000,
            };

            var earnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {

                    Type = OnProgrammeEarningType.Learning,
                    Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                    {
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 1, Period = 1,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>
                            {
                                new DataLockFailure { ApprenticeshipId = 1, DataLockError = DataLockErrorCode.DLOCK_03, ApprenticeshipPriceEpisodeIds = new List<long>{ 1 } },
                            }
                        },
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 2, Period = 2,
                            DataLockFailures = new List<DataLockFailure>()
                        },
                    })
                }
            };

            var previousPriceEpisodeStatuses = new List<PriceEpisodeStatusChange>
            {
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = priceEpisode.Identifier , AcademicYear = DefaultAcademicYear.ToString(), CommitmentId = 2},
                    AgreedPrice = priceEpisode.AgreedPrice,
                    
    

                    Errors = new LegacyDataLockEventError[]
                    {
                        new LegacyDataLockEventError
                        {
                            ErrorCode = "DLOCK_03",
                            SystemDescription = "No matching record found in the employer digital account for the standard code"
                        }
                    },
                }

            };

            calc.DetermineStatus(DefaultAcademicYear, priceEpisode, earnings, previousPriceEpisodeStatuses).Should().Be(PriceEpisodeStatus.Updated);
        }

        [Test]
        public void Learners_Swapped_DataLocks_Returns_Updated()
        {
            var calc = mocker.Create<PriceEpisodeStatusCalculator>();

            var priceEpisode = new PriceEpisode
            {
                Identifier = "1234",
                AgreedPrice = 1000,
            };

            var earnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {

                    Type = OnProgrammeEarningType.Learning,
                    Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                    {
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 1, Period = 1,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>
                            {
                                new DataLockFailure { ApprenticeshipId = 1, DataLockError = DataLockErrorCode.DLOCK_03, ApprenticeshipPriceEpisodeIds = new List<long>{ 1 } },
                            }
                        },
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 2, Period = 2,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>
                            {
                                new DataLockFailure { ApprenticeshipId = 2, DataLockError = DataLockErrorCode.DLOCK_04, ApprenticeshipPriceEpisodeIds = new List<long>{ 1 } },
                            }
                        },
                    })
                }
            };

            var previousPriceEpisodeStatuses = new List<PriceEpisodeStatusChange>
            {
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = priceEpisode.Identifier , AcademicYear = DefaultAcademicYear.ToString(), CommitmentId = 1},
                    AgreedPrice = priceEpisode.AgreedPrice,
                    Errors = new LegacyDataLockEventError[]
                    {
                        new LegacyDataLockEventError
                        {
                            ErrorCode = "DLOCK_04",
                            SystemDescription = "No matching record found in the employer digital account for the standard code"
                        }
                    },
                },
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = priceEpisode.Identifier , AcademicYear = DefaultAcademicYear.ToString(), CommitmentId = 2},
                    AgreedPrice = priceEpisode.AgreedPrice,
                    Errors = new LegacyDataLockEventError[]
                    {
                        new LegacyDataLockEventError
                        {
                            ErrorCode = "DLOCK_03",
                            SystemDescription = "No matching record found in the employer digital account for the standard code"
                        }
                    },
                }

            };

            calc.DetermineStatus(DefaultAcademicYear, priceEpisode, earnings, previousPriceEpisodeStatuses).Should().Be(PriceEpisodeStatus.Updated);
        }

        [Test]
        public void Same_Learners_Same_DataLocks_Returns_NoChange()
        {
            var calc = mocker.Create<PriceEpisodeStatusCalculator>();

            var priceEpisode = new PriceEpisode
            {
                Identifier = "1234",
                AgreedPrice = 1000,
            };

            var earnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {

                    Type = OnProgrammeEarningType.Learning,
                    Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                    {
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 1, Period = 1,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>
                            {
                                new DataLockFailure { ApprenticeshipId = 1, DataLockError = DataLockErrorCode.DLOCK_03, ApprenticeshipPriceEpisodeIds = new List<long>{ 1 } },
                            }
                        },
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 2, Period = 2,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>
                            {
                                new DataLockFailure { ApprenticeshipId = 2, DataLockError = DataLockErrorCode.DLOCK_03, ApprenticeshipPriceEpisodeIds = new List<long>{ 1 } },
                            }
                        },
                    })
                }
            };

            var previousPriceEpisodeStatuses = new List<PriceEpisodeStatusChange>
            {
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = priceEpisode.Identifier , AcademicYear = DefaultAcademicYear.ToString(), CommitmentId = 1},
                    AgreedPrice = priceEpisode.AgreedPrice,
                    Errors = new LegacyDataLockEventError[]
                    {
                        new LegacyDataLockEventError
                        {
                            ErrorCode = "DLOCK_03",
                            SystemDescription = "No matching record found in the employer digital account for the standard code"
                        }
                    },
                },
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = priceEpisode.Identifier , AcademicYear = DefaultAcademicYear.ToString(), CommitmentId = 2},
                    AgreedPrice = priceEpisode.AgreedPrice,
                    Errors = new LegacyDataLockEventError[]
                    {
                        new LegacyDataLockEventError
                        {
                            ErrorCode = "DLOCK_03",
                            SystemDescription = "No matching record found in the employer digital account for the standard code"
                        }
                    },
                }

            };

            calc.DetermineStatus(DefaultAcademicYear, priceEpisode, earnings, previousPriceEpisodeStatuses).Should().Be(PriceEpisodeStatus.NoCHange);
        }

        [Test]
        public void Same_Learners_No_DataLocks_Returns_NoChange()
        {
            var calc = mocker.Create<PriceEpisodeStatusCalculator>();

            var priceEpisode = new PriceEpisode
            {
                Identifier = "1234",
                AgreedPrice = 1000,
            };

            var earnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {

                    Type = OnProgrammeEarningType.Learning,
                    Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                    {
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 1, Period = 1,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure> ()
                        },
                        new EarningPeriod
                        {
                            AccountId = 12, Amount = 100, ApprenticeshipId = 2, Period = 2,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>()
                        },
                    })
                }
            };

            var previousPriceEpisodeStatuses = new List<PriceEpisodeStatusChange>
            {
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = priceEpisode.Identifier , AcademicYear = DefaultAcademicYear.ToString(), CommitmentId = 1},
                    AgreedPrice = priceEpisode.AgreedPrice,
                    Errors = Array.Empty<LegacyDataLockEventError>()
                },
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = priceEpisode.Identifier , AcademicYear = DefaultAcademicYear.ToString(), CommitmentId = 2},
                    AgreedPrice = priceEpisode.AgreedPrice,
                    Errors = Array.Empty<LegacyDataLockEventError>()
                }
            };

            calc.DetermineStatus(DefaultAcademicYear, priceEpisode, earnings, previousPriceEpisodeStatuses).Should().Be(PriceEpisodeStatus.NoCHange);
        }


        [Test]
        public void No_Matched_Learners_In_New_Event_Reurns_Updated()
        {
            var calc = mocker.Create<PriceEpisodeStatusCalculator>();

            var priceEpisode = new PriceEpisode
            {
                Identifier = "1234",
                AgreedPrice = 1000,
            };

            var earnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {

                    Type = OnProgrammeEarningType.Learning,
                    Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                    {
                        new EarningPeriod
                        {
                            AccountId = null, Amount = 100, ApprenticeshipId = null, Period = 1,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>
                            {
                                new DataLockFailure { ApprenticeshipId = null, DataLockError = DataLockErrorCode.DLOCK_01, ApprenticeshipPriceEpisodeIds = new List<long>( ) },
                            }
                        },
                        new EarningPeriod
                        {
                            AccountId = null, Amount = 100, ApprenticeshipId = null, Period = 2,
                            PriceEpisodeIdentifier = priceEpisode.Identifier,
                            DataLockFailures = new List<DataLockFailure>()
                            {
                                new DataLockFailure { ApprenticeshipId = null, DataLockError = DataLockErrorCode.DLOCK_02, ApprenticeshipPriceEpisodeIds = new List<long>() },
                            }
                        },
                    })
                }
            };

            var previousPriceEpisodeStatuses = new List<PriceEpisodeStatusChange>
            {
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = priceEpisode.Identifier , AcademicYear = DefaultAcademicYear.ToString(), CommitmentId = 1},
                    AgreedPrice = priceEpisode.AgreedPrice,
                    Errors = Array.Empty<LegacyDataLockEventError>()
                },
                new PriceEpisodeStatusChange
                {
                    DataLock = new LegacyDataLockEvent{ PriceEpisodeIdentifier = priceEpisode.Identifier , AcademicYear = DefaultAcademicYear.ToString(), CommitmentId = 2},
                    AgreedPrice = priceEpisode.AgreedPrice,
                    Errors = Array.Empty<LegacyDataLockEventError>()
                }
            };

            calc.DetermineStatus(DefaultAcademicYear, priceEpisode, earnings, previousPriceEpisodeStatuses).Should().Be(PriceEpisodeStatus.Updated);
        }

    }
}