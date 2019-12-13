
DECLARE @ukprnList table
(
	ukprn bigint 
)
--INSERT INTO @ukprnList --whitelist of suceeded ukprns if required
--VALUES
--(10000060),
--(10000061)

Declare @GivenUkprnCount INT = (SELECT COUNT(1) from @ukprnList)

;with allEarnings as
(
SELECT  ukprn,
ee.ContractType as [ApprenticeshipContractType],
(case when transactiontype = 1 then  Amount else 0 end) as [TT1],
(case when transactiontype = 2 then  Amount else 0 end) as [TT2],
(case when transactiontype = 3 then  Amount else 0 end) as [TT3],
(case when transactiontype = 4 then  Amount else 0 end) as [TT4],
(case when transactiontype = 5 then  Amount else 0 end) as [TT5],
(case when transactiontype = 6 then  Amount else 0 end) as [TT6],
(case when transactiontype = 7 then  Amount else 0 end) as [TT7],
(case when transactiontype = 8 then  Amount else 0 end) as [TT8],
(case when transactiontype = 9 then  Amount else 0 end) as [TT9],
(case when transactiontype = 10 then Amount else 0 end) as [TT10],
(case when transactiontype = 11 then Amount else 0 end) as [TT11],
(case when transactiontype = 12 then Amount else 0 end) as [TT12],
(case when transactiontype = 13 then Amount else 0 end) as [TT13],
(case when transactiontype = 14 then Amount else 0 end) as [TT14],
(case when transactiontype = 15 then Amount else 0 end) as [TT15],
(case when transactiontype = 16 then Amount else 0 end) as [TT16]

FROM Payments2.EarningEvent EE
JOIN Payments2.EarningEventPeriod EEP
    ON EEP.EarningEventId = EE.EventId
AND DeliveryPeriod <= @collectionperiod
AND Amount != 0
AND EE.CreationDate > @monthendStartTime
AND ((@GivenUkprnCount = 0) OR  (EE.ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
)


SELECT ukprn,  [ApprenticeshipContractType],
(	SUM(TT1) +
    SUM(TT2) +
    SUM(TT3) +
    SUM(TT4) +
    SUM(TT5) +
    SUM(TT6) +
    SUM(TT7) +
    SUM(TT8) +
    SUM(TT9) +
    SUM(TT10) +
    SUM(TT11) +
    SUM(TT12) +
    SUM(TT13) +
    SUM(TT14) +
    SUM(TT15) +
    SUM(TT16) ) as [AllTypes],
	SUM(TT1) [TT1], 
    SUM(TT2) [TT2],
    SUM(TT3) [TT3],
    SUM(TT4) [TT4],
    SUM(TT5) [TT5],
    SUM(TT6) [TT6],
    SUM(TT7) [TT7],
    SUM(TT8) [TT8],
    SUM(TT9) [TT9],
    SUM(TT10) [TT10],
    SUM(TT11) [TT11],
    SUM(TT12) [TT12],
    SUM(TT13) [TT13],
    SUM(TT14) [TT14],
    SUM(TT15) [TT15],
    SUM(TT16) [TT16]

FROM AllEarnings
GROUP BY [ApprenticeshipContractType], UKPRN
order by UKPRN,ApprenticeshipContractType



















