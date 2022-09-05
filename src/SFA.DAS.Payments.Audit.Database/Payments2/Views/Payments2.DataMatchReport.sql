CREATE VIEW [Payments2].[DataMatchReport]
AS

SELECT (CASE WHEN Dle.Datalocksourceid = 1 THEN 'ILR' ELSE 'DAS_PE' END) AS Collectiontype
     , Dle.Ukprn
     , Dle.Learnerreferencenumber
     , Dle.Learneruln
     , Dlenppf.Datalockfailureid
     , Ee.Learningaimsequencenumber
     , Dle.Collectionperiod
     , Dle.Academicyear
     , Dle.Ilrsubmissiondatetime
     , Dlenpp.Deliveryperiod
     , Dle.Datalocksourceid
     , Dle.Ispayable
     , Dle.Learningaimreference
     , Dle.Jobid
FROM Payments2.Datalockevent AS Dle WITH (NOLOCK)
INNER JOIN Payments2.Earningevent AS Ee WITH (NOLOCK) ON Dle.Earningeventid = Ee.Eventid
INNER JOIN Payments2.Datalockeventnonpayableperiod AS Dlenpp WITH (NOLOCK) ON Dle.Eventid = Dlenpp.Datalockeventid
INNER JOIN Payments2.Datalockeventnonpayableperiodfailures AS Dlenppf WITH (NOLOCK) ON Dlenpp.Datalockeventnonpayableperiodid = Dlenppf.Datalockeventnonpayableperiodid
INNER JOIN Payments2.Latestsuccessfuldatalockjobs Lsj WITH (NOLOCK) ON Dle.Jobid = Lsj.Dcjobid
WHERE Dle.Ispayable = 0
  AND Dle.Learningaimreference = 'ZPROG001'