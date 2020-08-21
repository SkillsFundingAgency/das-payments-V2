using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Monitoring.Metrics.Model;


namespace SFA.DAS.Payments.Monitoring.Metrics.Data
{
    public interface IDcMetricsDataContext
    {
        Task<List<TransactionTypeAmounts>> GetEarnings(long ukprn, short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
        Task<List<ProviderTransactionTypeAmounts>> GetEarnings(short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
    }

    public class DcMetricsDataContext : DbContext, IDcMetricsDataContext
    {
        private static string BaseDcEarningsQuery = @"
            ;WITH 
            RawEarnings AS (
                SELECT
                    APEP.LearnRefNumber,
                    APEP.Ukprn,
                    APE.PriceEpisodeAimSeqNumber [AimSeqNumber],
                    APEP.PriceEpisodeIdentifier,
                    APE.EpisodeStartDate,
                    APE.EpisodeEffectiveTNPStartDate,
                    APEP.[Period],
                    L.ULN,
                    COALESCE(LD.ProgType, 0) [ProgrammeType],
                    COALESCE(LD.FworkCode, 0) [FrameworkCode],
                    COALESCE(LD.PwayCode, 0) [PathwayCode],
                    COALESCE(LD.StdCode, 0) [StandardCode],
                    COALESCE(APEP.PriceEpisodeSFAContribPct, 0) [SfaContributionPercentage],
                    APE.PriceEpisodeFundLineType [FundingLineType],
                    LD.LearnAimRef,
                    LD.LearnStartDate [LearningStartDate],
                    COALESCE(APEP.PriceEpisodeOnProgPayment, 0) [TransactionType01],
                    COALESCE(APEP.PriceEpisodeCompletionPayment, 0) [TransactionType02],
                    COALESCE(APEP.PriceEpisodeBalancePayment, 0) [TransactionType03],
                    COALESCE(APEP.PriceEpisodeFirstEmp1618Pay, 0) [TransactionType04],
                    COALESCE(APEP.PriceEpisodeFirstProv1618Pay, 0) [TransactionType05],
                    COALESCE(APEP.PriceEpisodeSecondEmp1618Pay, 0) [TransactionType06],
                    COALESCE(APEP.PriceEpisodeSecondProv1618Pay, 0) [TransactionType07],
                    COALESCE(APEP.PriceEpisodeApplic1618FrameworkUpliftOnProgPayment, 0) [TransactionType08],
                    COALESCE(APEP.PriceEpisodeApplic1618FrameworkUpliftCompletionPayment, 0) [TransactionType09],
                    COALESCE(APEP.PriceEpisodeApplic1618FrameworkUpliftBalancing, 0) [TransactionType10],
                    COALESCE(APEP.PriceEpisodeFirstDisadvantagePayment, 0) [TransactionType11],
                    COALESCE(APEP.PriceEpisodeSecondDisadvantagePayment, 0) [TransactionType12],
                    0 [TransactionType13],
                    0 [TransactionType14],
                    COALESCE(APEP.PriceEpisodeLSFCash, 0) [TransactionType15],
                    COALESCE([APEP].[PriceEpisodeLearnerAdditionalPayment], 0) [TransactionType16],
      	            CASE WHEN [APE].[PriceEpisodeContractType] = 'Levy Contract' THEN 1 WHEN [APE].[PriceEpisodeContractType] = 'Contract for services with the employer' THEN 1 WHEN [APE].[PriceEpisodeContractType] = 'None' THEN 0 WHEN [APE].[PriceEpisodeContractType] = 'Non-Levy Contract' THEN 2 WHEN [APE].[PriceEpisodeContractType] = 'Contract for services with the ESFA' THEN 2 ELSE -1 END [ApprenticeshipContractType],
                    PriceEpisodeTotalTNPPrice [TotalPrice],
                    0 [MathsAndEnglish]
                FROM Rulebase.AEC_ApprenticeshipPriceEpisode_Period APEP
                INNER JOIN Rulebase.AEC_ApprenticeshipPriceEpisode APE
                    on APEP.UKPRN = APE.UKPRN
                    and APEP.LearnRefNumber = APE.LearnRefNumber
                    and APEP.PriceEpisodeIdentifier = APE.PriceEpisodeIdentifier
                JOIN Valid.Learner L
                    on L.UKPRN = APEP.Ukprn
                    and L.LearnRefNumber = APEP.LearnRefNumber
                JOIN Valid.LearningDelivery LD
                    on LD.UKPRN = APEP.Ukprn
                    and LD.LearnRefNumber = APEP.LearnRefNumber
                    and LD.AimSeqNumber = APE.PriceEpisodeAimSeqNumber
                where (
                    APEP.PriceEpisodeOnProgPayment != 0
                    or APEP.PriceEpisodeCompletionPayment != 0
                    or APEP.PriceEpisodeBalancePayment != 0
                    or APEP.PriceEpisodeFirstEmp1618Pay != 0
                    or APEP.PriceEpisodeFirstProv1618Pay != 0
                    or APEP.PriceEpisodeSecondEmp1618Pay != 0
                    or APEP.PriceEpisodeSecondProv1618Pay != 0
                    or APEP.PriceEpisodeApplic1618FrameworkUpliftOnProgPayment != 0
                    or APEP.PriceEpisodeApplic1618FrameworkUpliftCompletionPayment != 0
                    or APEP.PriceEpisodeApplic1618FrameworkUpliftBalancing != 0
                    or APEP.PriceEpisodeFirstDisadvantagePayment != 0
                    or APEP.PriceEpisodeSecondDisadvantagePayment != 0
                    or APEP.PriceEpisodeLSFCash != 0
                    )
                    AND APEP.Period <= @collectionperiod
            )
            , RawEarningsMathsAndEnglish AS (
                select 
                    LDP.LearnRefNumber,
                    LDP.Ukprn,
                    LDP.AimSeqNumber,
                    NULL [PriceEpisodeIdentifier],
                    NULL [EpisodeStartDate],
                    NULL [EpisodeEffectiveTNPStartDate],
                    LDP.[Period],
                    L.ULN,
                    COALESCE(LD.ProgType, 0) [ProgrammeType],
                    COALESCE(LD.FworkCode, 0) [FrameworkCode],
                    COALESCE(LD.PwayCode, 0) [PathwayCode],
                    COALESCE(LD.StdCode, 0) [StandardCode],
                    COALESCE(LDP.[LearnDelSFAContribPct], 0) [SfaContributionPercentage],
                    LDP.FundLineType [FundingLineType],
                    LD.LearnAimRef,
                    LD.LearnStartDate [LearningStartDate],
                    0 [TransactionType01],
                    0 [TransactionType02],
                    0 [TransactionType03],
                    0 [TransactionType04],
                    0 [TransactionType05],
                    0 [TransactionType06],
                    0 [TransactionType07],
                    0 [TransactionType08],
                    0 [TransactionType09],
                    0 [TransactionType10],
                    0 [TransactionType11],
                    0 [TransactionType12],
                    COALESCE(MathEngOnProgPayment, 0) [TransactionType13],
                    COALESCE(MathEngBalPayment, 0) [TransactionType14],
                    COALESCE(LearnSuppFundCash, 0) [TransactionType15],
                    0 [TransactionType16],
			            CASE WHEN LDP.LearnDelContType = 'Levy Contract' THEN 1 WHEN LDP.LearnDelContType = 'Contract for services with the employer' THEN 1 WHEN LDP.LearnDelContType = 'None' THEN 0 WHEN LDP.LearnDelContType = 'Non-Levy Contract' THEN 2 WHEN LDP.LearnDelContType = 'Contract for services with the ESFA' THEN 2 ELSE -1 END [ApprenticeshipContractType],
                    0 [TotalPrice],
                    1 [MathsAndEnglish]
                FROM Rulebase.AEC_LearningDelivery_Period LDP
                INNER JOIN Valid.LearningDelivery LD
                    on LD.UKPRN = LDP.UKPRN
                    and LD.LearnRefNumber = LDP.LearnRefNumber
                    and LD.AimSeqNumber = LDP.AimSeqNumber
                JOIN Valid.Learner L
                    on L.UKPRN = LD.Ukprn
                    and L.LearnRefNumber = LD.LearnRefNumber
                where (
                    MathEngOnProgPayment != 0
                    or MathEngBalPayment != 0
                    or LearnSuppFundCash != 0
                    )
                    and LD.LearnAimRef != 'ZPROG001'
                    AND Period <= @collectionperiod
            )
            , AllEarnings AS (
                SELECT * 
                FROM RawEarnings
                UNION
                SELECT * 
                FROM RawEarningsMathsAndEnglish
            )

            ";


        private static string UkprnFilterSelect =
            @"SELECT Cast([ApprenticeshipContractType] as TinyInt) as ContractType,
                SUM(TransactionType01) [TransactionType1], 
                SUM(TransactionType02) [TransactionType2],
                SUM(TransactionType03) [TransactionType3],
                SUM(TransactionType04) [TransactionType4],
                SUM(TransactionType05) [TransactionType5],
                SUM(TransactionType06) [TransactionType6],
                SUM(TransactionType07) [TransactionType7],
                SUM(TransactionType08) [TransactionType8],
                SUM(TransactionType09) [TransactionType9],
                SUM(TransactionType10) [TransactionType10],
                SUM(TransactionType11) [TransactionType11],
                SUM(TransactionType12) [TransactionType12],
                SUM(TransactionType13) [TransactionType13],
                SUM(TransactionType14) [TransactionType14],
                SUM(TransactionType15) [TransactionType15],
                SUM(TransactionType16) [TransactionType16]

            FROM AllEarnings
            where ukprn =  @ukprn
            and ApprenticeshipContractType in (1,2)
                GROUP BY [ApprenticeshipContractType], UKPRN
                order by UKPRN,ApprenticeshipContractType";

        private static string UkprnGroupSelect =
            @"SELECT CAST(Ukprn as bigint) as Ukprn, Cast([ApprenticeshipContractType] as TinyInt) as ContractType,
                SUM(TransactionType01) [TransactionType1], 
                SUM(TransactionType02) [TransactionType2],
                SUM(TransactionType03) [TransactionType3],
                SUM(TransactionType04) [TransactionType4],
                SUM(TransactionType05) [TransactionType5],
                SUM(TransactionType06) [TransactionType6],
                SUM(TransactionType07) [TransactionType7],
                SUM(TransactionType08) [TransactionType8],
                SUM(TransactionType09) [TransactionType9],
                SUM(TransactionType10) [TransactionType10],
                SUM(TransactionType11) [TransactionType11],
                SUM(TransactionType12) [TransactionType12],
                SUM(TransactionType13) [TransactionType13],
                SUM(TransactionType14) [TransactionType14],
                SUM(TransactionType15) [TransactionType15],
                SUM(TransactionType16) [TransactionType16]

            FROM AllEarnings
            where ApprenticeshipContractType in (1,2)
                GROUP BY [ApprenticeshipContractType], UKPRN
                order by UKPRN,ApprenticeshipContractType";

        private readonly string connectionString;

        public DbQuery<TransactionTypeAmounts> Earnings { get; set; }

        public DbQuery<ProviderTransactionTypeAmounts> AllProviderEarnings { get; set; }

        public DcMetricsDataContext(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public async Task<List<TransactionTypeAmounts>> GetEarnings(long ukprn, short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            return await Earnings.FromSql(BaseDcEarningsQuery + UkprnFilterSelect , new SqlParameter("@ukprn", ukprn), new SqlParameter("@collectionperiod", collectionPeriod)).ToListAsync(cancellationToken);
        }

        public async Task<List<ProviderTransactionTypeAmounts>> GetEarnings(short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            return await AllProviderEarnings.FromSql(BaseDcEarningsQuery + UkprnGroupSelect ,  new SqlParameter("@collectionperiod", collectionPeriod)).ToListAsync(cancellationToken);
        }
    }
}