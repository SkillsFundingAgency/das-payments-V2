

--DC Earnings
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
        COALESCE(APEP.PriceEpisodeESFAContribPct, 0) [SfaContributionPercentage],
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
        CASE WHEN APE.PriceEpisodeContractType = 'Contract for services with the employer' OR
                    APE.PriceEpisodeContractType = 'Levy Contract'
            THEN 1 ELSE 2 END [ApprenticeshipContractType],
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
        COALESCE(LDP.[LearnDelESFAContribPct], 0) [SfaContributionPercentage],
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
        CASE WHEN LDP.LearnDelContType = 'Contract for services with the employer' OR
                    LDP.LearnDelContType = 'Levy Contract'
            THEN 1 ELSE 2 END [ApprenticeshipContractType],
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

SELECT [ApprenticeshipContractType],
	SUM(TransactionType01) [TT1], 
    SUM(TransactionType02) [TT2],
    SUM(TransactionType03) [TT3],
    SUM(TransactionType04) [TT4],
    SUM(TransactionType05) [TT5],
    SUM(TransactionType06) [TT6],
    SUM(TransactionType07) [TT7],
    SUM(TransactionType08) [TT8],
    SUM(TransactionType09) [TT9],
    SUM(TransactionType10) [TT10],
    SUM(TransactionType11) [TT11],
    SUM(TransactionType12) [TT12],
    SUM(TransactionType13) [TT13],
    SUM(TransactionType14) [TT14],
    SUM(TransactionType15) [TT15],
    SUM(TransactionType16) [TT16]
FROM AllEarnings
where 
UKPRN IN (@ukprnlist)
GROUP BY [ApprenticeshipContractType]