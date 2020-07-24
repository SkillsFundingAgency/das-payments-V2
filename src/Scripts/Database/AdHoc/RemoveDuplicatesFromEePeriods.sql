BEGIN TRANSACTION
;WITH cte
AS (
	SELECT eep.Id
		, ROW_NUMBER() OVER (
			PARTITION BY earningeventid
			, priceEpisodeIdentifier
			, transactiontype
			, DeliveryPeriod
			, Amount ORDER BY EARNINGEVENTID
			) AS RN
	FROM [Payments2].[EarningEventPeriod] eep
	INNER JOIN [Payments2].[EarningEvent] EE
	ON 
		EE.EventId = EEP.EarningEventId
	WHERE 
		EE.CollectionPeriod = 7
	AND 
		EE.AcademicYear = 1920
	)


--select test prior to deletion (mutally exclusive with the query below)

SELECT earningPeriods.*
	, epp.*
FROM CTE earningPeriods
INNER JOIN payments2.earningeventperiod epp
	ON earningPeriods.id = epp.id
WHERE earningPeriods.RN > 1
ORDER BY epp.earningeventid
	, epp.priceEpisodeIdentifier
	, epp.transactiontype
	, epp.DeliveryPeriod
	, epp.Amount

--Deletion

--DELETE
--FROM Payments2.EarningEventPeriod
--WHERE id IN (
--		SELECT id
--		FROM cte
--		WHERE rn > 1
--		)



ROLLBACK TRANSACTION
