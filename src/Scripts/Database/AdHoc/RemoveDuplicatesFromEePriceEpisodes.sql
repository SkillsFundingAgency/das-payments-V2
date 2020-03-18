BEGIN TRANSACTION
;WITH cte
AS (
	SELECT eepe.Id
		, ROW_NUMBER() OVER (
			PARTITION BY eepe.[earningeventid]
			, eepe.[PriceEpisodeIdentifier]
			, eepe.[SfaContributionPercentage]
			, eepe.[TotalNegotiatedPrice1]
			, eepe.[TotalNegotiatedPrice2]
			, eepe.[TotalNegotiatedPrice3]
			, eepe.[TotalNegotiatedPrice4]
			, eepe.[StartDate]
			, eepe.[EffectiveTotalNegotiatedPriceStartDate]
			, eepe.[PlannedEndDate]
			, eepe.[ActualEndDate]
			, eepe.[NumberOfInstalments]
			, eepe.[InstalmentAmount]
			, eepe.[CompletionAmount]
			, eepe.[Completed]
			, eepe.[EmployerContribution]
			, eepe.[CompletionHoldBackExemptionCode]
			, eepe.[AgreedPrice]
			, eepe.[CourseStartDate] ORDER BY EARNINGEVENTID
			) AS RN
	FROM [Payments2].[EarningEventPriceEpisode] eepe
	INNER JOIN [Payments2].[EarningEvent] EE
	ON 
		EE.EventId = eepe.EarningEventId
	WHERE 
		EE.CollectionPeriod = 7
	AND 
		EE.AcademicYear = 1920
	)


--select test prior to deletion (mutally exclusive with the query below)

SELECT episodes.*
	, eppe.*
FROM CTE episodes
INNER JOIN payments2.EarningEventPriceEpisode eppe
	ON episodes.id = eppe.id
WHERE episodes.RN > 1
ORDER BY eppe.[earningeventid]
	, eppe.[PriceEpisodeIdentifier]
	, eppe.[SfaContributionPercentage]
	, eppe.[TotalNegotiatedPrice1]
	, eppe.[TotalNegotiatedPrice2]
	, eppe.[TotalNegotiatedPrice3]
	, eppe.[TotalNegotiatedPrice4]
	, eppe.[StartDate]
	, eppe.[EffectiveTotalNegotiatedPriceStartDate]
	, eppe.[PlannedEndDate]
	, eppe.[ActualEndDate]
	, eppe.[NumberOfInstalments]
	, eppe.[InstalmentAmount]
	, eppe.[CompletionAmount]
	, eppe.[Completed]
	, eppe.[EmployerContribution]
	, eppe.[CompletionHoldBackExemptionCode]
	, eppe.[AgreedPrice]
	, eppe.[CourseStartDate]

--Deletion

--DELETE
--FROM Payments2.EarningEventPriceEpisode
--WHERE id IN (
--		SELECT id
--		FROM cte
--		WHERE rn > 1
--		)

ROLLBACK TRANSACTION