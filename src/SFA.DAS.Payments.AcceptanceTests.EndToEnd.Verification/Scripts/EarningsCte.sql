;WITH RawEarnings
AS (
	-- this CTE is used as a subquery to calculate earnings and have a transaction type as a value in a field
	SELECT 
		[Amount]
		,[TransactionType]
		,[ACT]
		,[UKPRN]
		,[LearnRefNumber]
		,[PriceEpisodeIdentifier]
		,[Period]
	FROM (
		SELECT 
			[APEP].[PriceEpisodeOnProgPayment] [TransactionType1]
			,[APEP].[PriceEpisodeCompletionPayment] [TransactionType2]
			,[APEP].[PriceEpisodeBalancePayment] [TransactionType3]
			,[APEP].[PriceEpisodeFirstEmp1618Pay] [TransactionType4]
			,[APEP].[PriceEpisodeFirstProv1618Pay] [TransactionType5]
			,[APEP].[PriceEpisodeSecondEmp1618Pay] [TransactionType6]
			,[APEP].[PriceEpisodeSecondProv1618Pay] [TransactionType7]
			,[APEP].[PriceEpisodeApplic1618FrameworkUpliftOnProgPayment] [TransactionType8]
			,[APEP].[PriceEpisodeApplic1618FrameworkUpliftCompletionPayment] [TransactionType9]
			,[APEP].[PriceEpisodeApplic1618FrameworkUpliftBalancing] [TransactionType10]
			,[APEP].[PriceEpisodeFirstDisadvantagePayment] [TransactionType11]
			,[APEP].[PriceEpisodeSecondDisadvantagePayment] [TransactionType12]
			,CAST(NULL AS DECIMAL(12,5)) [TransactionType13]
			,CAST(NULL AS DECIMAL(12,5)) [TransactionType14]
			,[APEP].[PriceEpisodeLSFCash] [TransactionType15]
			,[APEP].[PriceEpisodeLearnerAdditionalPayment] [TransactionType16]
			,CASE WHEN [APE].[PriceEpisodeContractType] = 'Levy Contract' THEN 1 ELSE 2 END [ACT]
			,[APEP].[UKPRN]
			,[APEP].[LearnRefNumber]
			,[APEP].[PriceEpisodeIdentifier]
			,[APEP].[Period]
		FROM [Rulebase].[AEC_ApprenticeshipPriceEpisode_Period] APEP WITH (NOLOCK)
		INNER JOIN [Rulebase].[AEC_ApprenticeshipPriceEpisode] APE WITH (NOLOCK)
			ON [APEP].[UKPRN] = [APE].[UKPRN]
				AND [APEP].[LearnRefNumber] = [APE].[LearnRefNumber]
				AND [APEP].[PriceEpisodeIdentifier] = [APE].[PriceEpisodeIdentifier]
		WHERE [APEP].[Period] <= @collectionPeriod
        
		UNION ALL
        
		SELECT 
			NULL [TransactionType1]
			,NULL [TransactionType2]
			,NULL [TransactionType3]
			,NULL [TransactionType4]
			,NULL [TransactionType5]
			,NULL [TransactionType6]
			,NULL [TransactionType7]
			,NULL [TransactionType8]
			,NULL [TransactionType9]
			,NULL [TransactionType10]
			,NULL [TransactionType11]
			,NULL [TransactionType12]
			,[MathEngOnProgPayment] [TransactionType13]
			,[MathEngBalPayment] [TransactionType14]
			,[LearnSuppFundCash] [TransactionType15]
			,0 [TransactionType16]
			,CASE WHEN LDP.LearnDelContType = 'Levy Contract' THEN 1 ELSE 2 END [ACT]
			,[LDP].[UKPRN]
			,[LDP].[LearnRefNumber]
			,NULL [PriceEpisodeIdentifier]
			,[LDP].[Period]
		FROM [Rulebase].[AEC_LearningDelivery_Period] [LDP] WITH (NOLOCK)
		INNER JOIN [Rulebase].[AEC_LearningDelivery] [RLD] WITH (NOLOCK)
			ON [LDP].[UKPRN] = [RLD].[UKPRN]
				AND [LDP].[LearnRefNumber] = [RLD].[LearnRefNumber]
				AND [LDP].[AimSeqNumber] = [RLD].[AimSeqNumber]
		INNER JOIN .[Valid].[LearningDelivery] [LD] WITH (NOLOCK)
			ON [LD].[UKPRN] = [LDP].[UKPRN]
				AND [LD].[LearnRefNumber] = [LDP].[LearnRefNumber]
				AND [LD].[AimSeqNumber] = [RLD].[AimSeqNumber]
		WHERE (
				[MathEngOnProgPayment] > 0
				OR [MathEngBalPayment] > 0
				OR [LearnSuppFundCash] > 0
				)
			AND [LDP].[Period] <= @collectionPeriod
			AND [LD].[LearnAimRef] != 'ZPROG001'
		) AS PivotedEarnings
	UNPIVOT([Amount] FOR [TransactionType] IN (
				[TransactionType1]
				,[TransactionType2]
				,[TransactionType3]
				,[TransactionType4]
				,[TransactionType5]
				,[TransactionType6]
				,[TransactionType7]
				,[TransactionType8]
				,[TransactionType9]
				,[TransactionType10]
				,[TransactionType11]
				,[TransactionType12]
				,[TransactionType13]
				,[TransactionType14]
				,[TransactionType15]
				,[TransactionType16]
				)) AS [Earnings_Internal]
	)

,[Earnings] AS (
	SELECT 
		[Amount]
		,CAST(REPLACE([TransactionType], 'TransactionType', '') AS INT) [TransactionType]
		,[ACT] as [ContractType]
		,[Ukprn]
		,[LearnRefNumber]
		,[PriceEpisodeIdentifier]
		,[Period]
	FROM [RawEarnings]
	WHERE UKPRN IN (@ukprnlist)
)