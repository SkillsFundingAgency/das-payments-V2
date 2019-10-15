WITH RawEarnings AS (
	SELECT
		[APEP].[LearnRefNumber] [LearnerReferenceNumber]
		,[APEP].[Ukprn]
		,[APEP].[PriceEpisodeIdentifier]
		,[APE].[EpisodeStartDate]
		,[APE].[EpisodeEffectiveTNPStartDate]
		,[APEP].[Period] [DeliveryPeriod]   
		,[L].[ULN] [LearnerUln]
		,COALESCE([LD].[ProgType], 0) [LearningAimProgrammeType]
		,COALESCE([LD].[FworkCode], 0) [LearningAimFrameworkCode]
		,COALESCE([LD].[PwayCode], 0) [LearningAimPathwayCode]
		,COALESCE([LD].[StdCode], 0) [LearningAimStandardCode]
		,COALESCE([APEP].[PriceEpisodeSFAContribPct], 0) [SfaContributionPercentage]
		,[APE].[PriceEpisodeFundLineType] [LearningAimFundingLineType]
		,[LD].[LearnAimRef] [LearningAimReference]
		,[LD].[LearnStartDate] [LearningStartDate]
		,COALESCE([APEP].[PriceEpisodeOnProgPayment], 0) [TransactionType01]
		,COALESCE([APEP].[PriceEpisodeCompletionPayment], 0) [TransactionType02]
		,COALESCE([APEP].[PriceEpisodeBalancePayment], 0) [TransactionType03]
		,COALESCE([APEP].[PriceEpisodeFirstEmp1618Pay], 0) [TransactionType04]
		,COALESCE([APEP].[PriceEpisodeFirstProv1618Pay], 0) [TransactionType05]
		,COALESCE([APEP].[PriceEpisodeSecondEmp1618Pay], 0) [TransactionType06]
		,COALESCE([APEP].[PriceEpisodeSecondProv1618Pay], 0) [TransactionType07]
		,COALESCE([APEP].[PriceEpisodeApplic1618FrameworkUpliftOnProgPayment], 0) [TransactionType08]
		,COALESCE([APEP].[PriceEpisodeApplic1618FrameworkUpliftCompletionPayment], 0) [TransactionType09]
		,COALESCE([APEP].[PriceEpisodeApplic1618FrameworkUpliftBalancing], 0) [TransactionType10]
		,COALESCE([APEP].[PriceEpisodeFirstDisadvantagePayment], 0) [TransactionType11]
		,COALESCE([APEP].[PriceEpisodeSecondDisadvantagePayment], 0) [TransactionType12]
		,0 [TransactionType13]
		,0 [TransactionType14]
		,COALESCE([APEP].[PriceEpisodeLSFCash], 0) [TransactionType15]
		,COALESCE([APEP].[PriceEpisodeLearnerAdditionalPayment], 0) [TransactionType16]
		,CASE WHEN [APE].[PriceEpisodeContractType] = 'Levy Contract' THEN 1
				WHEN [APE].[PriceEpisodeContractType] = 'Contract for services with the employer' THEN 1
				WHEN [APE].[PriceEpisodeContractType] = 'None' THEN 0
				WHEN [APE].[PriceEpisodeContractType] = 'Non-Levy Contract' THEN 2
				WHEN [APE].[PriceEpisodeContractType] = 'Contract for services with the ESFA' THEN 2
				ELSE -1 END [ContractType]
		,[APE].[PriceEpisodeFirstAdditionalPaymentThresholdDate] [FirstIncentiveCensusDate]
		,[APE].[PriceEpisodeSecondAdditionalPaymentThresholdDate] [SecondIncentiveCensusDate]
		,[APE].[PriceEpisodeLearnerAdditionalPaymentThresholdDate] [LearnerAdditionalPaymentsDate]
		,[APE].[PriceEpisodeTotalTnpPrice] [AgreedPrice]
		,[APE].[PriceEpisodeActualEndDate] [EndDate]
		,[APE].[PriceEpisodeCumulativePMRs] [CumulativePmrs]
		,[APE].[PriceEpisodeCompExemCode] [ExemptionCodeForCompletionHoldback]
	FROM [Rulebase].[AEC_ApprenticeshipPriceEpisode_Period] [APEP]
	INNER JOIN [Rulebase].[AEC_ApprenticeshipPriceEpisode] [APE]
		ON [APEP].[UKPRN] = [APE].[UKPRN]
		AND [APEP].[LearnRefNumber] = [APE].[LearnRefNumber]
		AND [APEP].[PriceEpisodeIdentifier] = [APE].[PriceEpisodeIdentifier]
	JOIN [Valid].[Learner] L
		ON [L].[UKPRN] = [APEP].[Ukprn]
		AND [L].[LearnRefNumber] = [APEP].[LearnRefNumber]
	JOIN [Valid].[LearningDelivery] LD
		ON [LD].[UKPRN] = [APEP].[Ukprn]
		AND [LD].[LearnRefNumber] = [APEP].[LearnRefNumber]
		AND [LD].[AimSeqNumber] = [APE].[PriceEpisodeAimSeqNumber]
	WHERE (
		[APEP].[PriceEpisodeOnProgPayment] != 0
		OR [APEP].[PriceEpisodeCompletionPayment] != 0
		OR [APEP].[PriceEpisodeBalancePayment] != 0
		OR [APEP].[PriceEpisodeFirstEmp1618Pay] != 0
		OR [APEP].[PriceEpisodeFirstProv1618Pay] != 0
		OR [APEP].[PriceEpisodeSecondEmp1618Pay] != 0
		OR [APEP].[PriceEpisodeSecondProv1618Pay] != 0
		OR [APEP].[PriceEpisodeApplic1618FrameworkUpliftOnProgPayment] != 0
		OR [APEP].[PriceEpisodeApplic1618FrameworkUpliftCompletionPayment] != 0
		OR [APEP].[PriceEpisodeApplic1618FrameworkUpliftBalancing] != 0
		OR [APEP].[PriceEpisodeFirstDisadvantagePayment] != 0
		OR [APEP].[PriceEpisodeSecondDisadvantagePayment] != 0
		OR [APEP].[PriceEpisodeLSFCash] != 0
		OR [APEP].[PriceEpisodeLearnerAdditionalPayment] != 0
		OR [APEP].[Period] = 1
		)
	
	UNION

	select
		[LDP].[LearnRefNumber] [LearnerReferenceNumber]
		,[LDP].[Ukprn]
		,NULL [PriceEpisodeIdentifier]
		,NULL [EpisodeStartDate]
		,NULL [EpisodeEffectiveTNPStartDate]
		,[LDP].[Period] [DeliveryPeriod]  
		,[L].[ULN] [LearnerUln]
		,COALESCE([LD].[ProgType], 0) [LearningAimProgrammeType] 
		,COALESCE([LD].[FworkCode], 0) [LearningAimFrameworkCode]
		,COALESCE([LD].[PwayCode], 0) [LearningAimPathwayCode]
		,COALESCE([LD].[StdCode], 0) [LearningAimStandardCode]
		,COALESCE([LDP].[LearnDelSFAContribPct], 0) [SfaContributionPercentage]
		,[LDP].[FundLineType] [LearningAimFundingLineType]
		,[LD].[LearnAimRef] [LearningAimReference]
		,[LD].[LearnStartDate] [LearningStartDate]
		,0 [TransactionType01]
		,0 [TransactionType02]
		,0 [TransactionType03]
		,0 [TransactionType04]
		,0 [TransactionType05]
		,0 [TransactionType06]
		,0 [TransactionType07]
		,0 [TransactionType08]
		,0 [TransactionType09]
		,0 [TransactionType10]
		,0 [TransactionType11]
		,0 [TransactionType12]
		,COALESCE([MathEngOnProgPayment], 0) [TransactionType13]
		,COALESCE([MathEngBalPayment], 0) [TransactionType14]
		,COALESCE([LearnSuppFundCash], 0) [TransactionType15]
		,0 [TransactionType16]
		,CASE WHEN [LDP].[LearnDelContType] = 'Levy Contract' THEN 1
				WHEN [LDP].[LearnDelContType] = 'Contract for services with the employer' THEN 1
				WHEN [LDP].[LearnDelContType] = 'None' THEN 0
				WHEN [LDP].[LearnDelContType] = 'Non-Levy Contract' THEN 2
				WHEN [LDP].[LearnDelContType] = 'Contract for services with the ESFA' THEN 2
				ELSE -1 END [ContractType]
		,NULL [FirstIncentiveCensusDate]
		,NULL [SecondIncentiveCensusDate]
		,NULL [LearnerAdditionalPaymentsDate]
		,NULL [AgreedPrice]
		,NULL [EndDate]
		,NULL [CumulativePmrs]
		,NULL [ExemptionCodeForCompletionHoldback]
	FROM [Rulebase].[AEC_LearningDelivery_Period] LDP
	INNER JOIN [Valid].[LearningDelivery] LD
		ON [LD].[UKPRN] = [LDP].[UKPRN]
		AND [LD].[LearnRefNumber] = [LDP].[LearnRefNumber]
		AND [LD].[AimSeqNumber] = [LDP].[AimSeqNumber]
	JOIN [Valid].[Learner] L
		ON [L].[UKPRN] = [LD].[Ukprn]
		AND [L].[LearnRefNumber] = [LD].[LearnRefNumber]
	WHERE (
		MathEngOnProgPayment != 0
		OR MathEngBalPayment != 0
		OR LearnSuppFundCash != 0
		)
		AND LD.LearnAimRef != 'ZPROG001'
)

SELECT SUM(TransactionType01) +
	SUM(TransactionType02) +
	SUM(TransactionType03) +
	SUM(TransactionType04) +
	SUM(TransactionType05) +
	SUM(TransactionType06) +
	SUM(TransactionType07) +
	SUM(TransactionType08) +
	SUM(TransactionType09) +
	SUM(TransactionType10) +
	SUM(TransactionType11) +
	SUM(TransactionType12) +
	SUM(TransactionType13) +
	SUM(TransactionType14) +
	SUM(TransactionType15) +
	SUM(TransactionType16) [Total], ContractType, SfaContributionPercentage
FROM RawEarnings

WHERE DeliveryPeriod IN (1,2)
AND [EpisodeStartDate] >= '2019-08-01'
AND [EpisodeStartDate] < '2020-08-01'
AND (
	TransactionType01 != 0 OR
	TransactionType02 != 0 OR
	TransactionType03 != 0 OR
	TransactionType04 != 0 OR
	TransactionType05 != 0 OR
	TransactionType06 != 0 OR
	TransactionType07 != 0 OR
	TransactionType08 != 0 OR
	TransactionType09 != 0 OR
	TransactionType10 != 0 OR
	TransactionType11 != 0 OR
	TransactionType12 != 0 OR
	TransactionType13 != 0 OR
	TransactionType14 != 0 OR
	TransactionType15 != 0 OR
	TransactionType16 != 0 
)

GROUP BY ContractType, SfaContributionPercentage
--Order by UKPRN, learneruln, DeliveryPeriod

