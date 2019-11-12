/*
This version of the query was constructed and used as part of R02. It should be used to ensure that the payments are consistent across each of the 
payments tables.

It needs work to tidy this up and to remove unnecessary columns. Its been checked in to make sure that it can be shared across the team.

Usage: Update the monthendjobid and other variables at the top of the script.
*/

declare @academicYear smallint = 1920
declare @collectionPeriod tinyint =2
declare @populateEarnings bit = 1
declare @monthendjobid int = 25680

select
                ukprns.Ukprn,
                           ukprns.lastJobId,
                TransactionTypes.TransactionType as [Transaction Type],
                           
                            isnull(Earnings.EarningsYTD, 0) - isnull(DatalockerrorsAudit.DataLockErrors, 0) - isnull(ActualPayments.ActualPaymentYTD, 0) - isnull(HeldBackCompletionPayments, 0) [Missing Actual Payments],
                           --isnull(Earnings.EarningsYTD, 0) - isnull(DataLockErrors.DataLockErrors, 0) - isnull(RequiredPaymentYTD, 0) - isnull(HeldBackCompletionPayments, 0) [Missing Required Payments],
                            isnull(Earnings.EarningsYTD, 0) - isnull(DatalockerrorsAudit.DataLockErrors, 0) - isnull(RequiredPaymentYTD, 0) - isnull(HeldBackCompletionPayments, 0) [Missing Required Payments (audit DL)],
                            isnull(Earnings.EarningLearnerCount, 0) as [Earnings Learners YTD (audit)],
                            isnull(Earnings.EarningsYTD, 0) as [Earnings YTD (audit)],
                            isnull(Earnings.EarningsACT1, 0) as [Earnings ACT1 (audit)],
                            isnull(Earnings.EarningsACT2, 0) as [Earnings ACT2 (audit)],
                            isnull(Earnings.NegativeEarningsYTD, 0) as [Negative Earnings (audit)],
                            isnull(Earnings.NegativeEarningsACT1, 0) as [Negative Earnings ACT1 (audit)],    
                            isnull(Earnings.NegativeEarningsACT2, 0) as [Negative Earnings ACT2 (audit)],
                     isnull(RequiredPayments.RequiredPaymentLearnerCount, 0) as [RequiredPaymentLearnerCount (audit)],
                        isnull(RequiredPayments.RequiredPaymentYTD, 0) as [Required Payments (audit)],
                       isnull(RequiredPayments.RequiredPaymentACT1, 0) as [Required Payments ACT1 (audit)],
                       isnull(RequiredPayments.RequiredPaymentACT2, 0) as [Required Payments ACT2 (audit)],
                            isnull(ActualPayments.ActualPaymentYTD, 0) as [Payments YTD],
                           isnull(ActualPayments.ActualPaymentACT1, 0) as [Payments ACT1],
                           isnull(ActualPayments.ActualPaymentACT2, 0) as [Payments ACT2],
                           --isnull(DataLocksEE.DataLockErrorLearnerCount, 0) as [DataLockError Learner Count New],
                           --isnull(DataLocksEE.DataLockErrors, 0) as [DataLock Errors New],
                     isnull([DataLockErrors].DataLockErrorLearnerCount, 0) as [DataLockError Learner Count],
                            isnull([DataLockErrors].DataLockErrors, 0) as [DataLock Errors],
                     isnull([DataLockErrorsAudit].DataLockErrorCount, 0) as [DataLockError Learner Count (audit)],
                       isnull([DataLockErrorsAudit].DataLockErrors, 0) as [DataLock Errors (audit)],

                            isnull([HeldBackCompletionPayments], 0) as [Held Back Completion (audit)],
                           isnull([HeldBackCompletionPaymentsAct1], 0) as [HBCP ACT1 (audit)],                isnull([HeldBackCompletionPaymentsAct2], 0) as [HBCP ACT2 (audit)],
                            isnull(PaymentsUsingLevy, 0) as [Payments using Levy],
                            isnull(PaymentsUsingCoInvestmentSFA, 0) as [PaymentsUsingCoInvestmentSFA],
                          isnull(PaymentsUsingCoInvestmentEmployer, 0) as [PaymentsUsingCoInvestmentEmployer],
                            isnull(PaymentsUsingFullyFundedSFA, 0) as [PaymentsUsingFullyFundedSFA],
                            isnull(PaymentsUsingTransfer, 0) as [PaymentsUsingTransfer]
        from (
                        select n as TransactionType from (values (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12),(13),(14),(15),(16)) v(n)
                ) as TransactionTypes
        
                cross join (
                        -- remove ukprns where there was no submissions and where there were failures
                                                SELECT 
                                                       j.DCJobid as LastJobId,
                                                       j.ukprn
                                                FROM 
                                                       payments2.job j 
                                                INNER JOIN
                                                       (
                                                       SELECT 
                                                              ukprn, 
                                                              MAX(ilrSubmissionTime) lastIlrSubmission
                                                       FROM 
                                                              payments2.job
                                                       WHERE 
                                                              AcademicYear = @academicYear
                                                       AND
                                                              CollectionPeriod <= @collectionPeriod
                                                       GROUP BY 
                                                              ukprn
                                                       ) lastSubmission
                                                ON 
                                                       lastSubmission.ukprn = j.ukprn
                                                AND 
                                                       lastsubmission.lastIlrSubmission = j.ilrSubmissionTime
                                                       --where j.ukprn = 10003915


                ) as ukprns

                -- Earnings from audit
                left join (
                        select
                                e.Ukprn,
                                TransactionType,
                                                       e.jobid,
                                count(distinct e.LearnerUln) [EarningLearnerCount],
                                sum(case when Amount > 0 then Amount end) [EarningsYTD],
                                sum(case when ContractType = 1 and Amount > 0 then Amount end) [EarningsACT1],
                                sum(case when ContractType = 2 and Amount > 0 then Amount end) [EarningsACT2],
                                sum(case when Amount < 0 then Amount end) [NegativeEarningsYTD],
                                sum(case when ContractType = 1 and Amount < 0 then Amount end) [NegativeEarningsACT1],
                                sum(case when ContractType = 2 and Amount < 0 then Amount end) [NegativeEarningsACT2]
                        from
                                Payments2.EarningEvent e with(nolock)
                                join Payments2.EarningEventPeriod p with(nolock) on p.EarningEventId = e.EventId 
                        where
                                e.AcademicYear = @academicYear
                                and e.CollectionPeriod = @collectionPeriod
                                and p.DeliveryPeriod <= @collectionPeriod
                                and @populateEarnings = 1
                        group by
                                e.Ukprn,
                                p.TransactionType,
                                                       e.jobid
                ) as Earnings
                on Earnings.TransactionType = TransactionTypes.TransactionType
                        and Earnings.Ukprn = ukprns.Ukprn 
                                         and Earnings.JobId = ukprns.lastJobId


                -- RequiredPayments from audit
                left join (
                        select 
                                ukprn,
                                RP.TransactionType,
                                                    count(distinct RP.LearnerUln) [RequiredPaymentLearnerCount],
                                sum(Amount) [RequiredPaymentYTD],
                                sum(case when ContractType = 1 then Amount end) [RequiredPaymentACT1],
                                sum(case when ContractType = 2 then Amount end) [RequiredPaymentACT2]
                        from 
                                [Payments2].[RequiredPaymentEvent] RP
                        where 
                                CollectionPeriod <= @collectionPeriod
                                and AcademicYear = @academicYear
                        group by 
                                ukprn,
                                TransactionType
        ) as RequiredPayments
                on RequiredPayments.TransactionType = TransactionTypes.TransactionType
                        and RequiredPayments.Ukprn = ukprns.Ukprn
                                         
                -- ActualPayments
                left join (
                        select 
                                TransactionType, 
                                Payments2.Payment.Ukprn,
                                count(distinct LearnerUln) [ActualPaymentLearnerCount],
                                sum(Amount) [ActualPaymentYTD],
                                sum(case when ContractType = 1 then Amount end) [ActualPaymentACT1],
                                sum(case when ContractType = 2 then Amount end) [ActualPaymentACT2]
                        from 
                                Payments2.Payment with(nolock) 
                        where 
                                AcademicYear = @academicYear
                                and CollectionPeriod <= @collectionPeriod
                        group by 
                                Payments2.Payment.Ukprn,
                                TransactionType
                ) as ActualPayments
                on ActualPayments.TransactionType = TransactionTypes.TransactionType
                        and ActualPayments.Ukprn = ukprns.Ukprn
                                         

                -- DataLockErrors
                left join (
                        select 
                                Payments2.DataLockFailure.Ukprn,
                                TransactionType, 
                                sum(Amount) [DataLockErrors],
                                count(distinct Payments2.DataLockFailure.LearnerUln) [DataLockErrorLearnerCount]
                        from 
                                Payments2.DataLockFailure with(nolock) 
                        where 
                                AcademicYear = @academicYear
                                and CollectionPeriod = @collectionPeriod
                                and DeliveryPeriod <= CollectionPeriod
                        group by 
                                Payments2.DataLockFailure.Ukprn,
                                TransactionType
                ) as [DataLockErrors]
                on [DataLockErrors].TransactionType = TransactionTypes.TransactionType
                        and [DataLockErrors].Ukprn = ukprns.Ukprn


                -- DataLockErrors from audit
                left join (
                        select
                                dle.Ukprn,
                                dlenpp.TransactionType,
                                                       dle.JobId,
                                sum(dlenpp.amount) as [DataLockErrors],
                                count(distinct dle.LearnerUln) as [DataLockErrorCount]
                        from [Payments2].[DataLockEvent] dle
                                join [Payments2].[DataLockEventNonPayablePeriod] dlenpp on dle.EventId = dlenpp.DataLockEventId
                        where 
                                dle.CollectionPeriod = @collectionPeriod
                                and dle.AcademicYear = @academicYear                            
                                and dlenpp.DeliveryPeriod <= @collectionPeriod --from message
                                --and dle.DataLockSourceId = 1 -- 1-ilr; 2-periodend
                                and dle.IsPayable = 0
                                and dlenpp.amount > 0
                        group by 
                                dle.Ukprn,
                                dlenpp.TransactionType,
                                                       dle.jobid
                ) as DataLockErrorsAudit
                on DataLockErrorsAudit.TransactionType = TransactionTypes.TransactionType
                        and DataLockErrorsAudit.Ukprn = ukprns.Ukprn
                                         and DataLockErrorsAudit.JobId = ukprns.lastJobId
                           ---- MK Datalocks
                           --left join (
    --                    select
    --                            e.Ukprn,
    --                            ep.TransactionType,
                           --                          e.JobId, 
                           --                          sum(case when  f.Id is not null then ep.Amount end) as DataLockErrors,
    --                            count(distinct f.LearnerUln) as [DataLockErrorLearnerCount]
    --                    from
    --                            Payments2.EarningEvent e with(nolock)
    --                            join Payments2.EarningEventPeriod ep with(nolock) on ep.EarningEventId = e.EventId
                                
    --                            left join Payments2.DataLockFailure f with(nolock) on f.EarningEventId = e.EventId
    --                                    and f.DeliveryPeriod = ep.DeliveryPeriod
    --                                    and f.TransactionType = ep.TransactionType
    --                    where
    --                             ep.Amount <> 0
    --                            and e.AcademicYear = @academicYear
    --                            and e.CollectionPeriod = @collectionPeriod
    --                            and ep.DeliveryPeriod <= @collectionPeriod
                                                       
    --                    group by
    --                            e.Ukprn,
    --                            ep.TransactionType,
                           --                          e.JobId
    --            ) as DataLocksEE
    --            on DataLocksEE.TransactionType = TransactionTypes.TransactionType
    --                    and DataLocksEE.Ukprn = ukprns.Ukprn
                           --     and DataLocksEE.JobId  = ukprns.lastJobId


                --HeldBackCompletionPayments from audit
                left join ( -- we're not recording HBCP yet so working out all TT2 earnings that were not paid and had no DL error
                        select
                                e.Ukprn,
                                ep.TransactionType,
                                                       e.JobId, 
                                sum(case when p.Id is null and f.Id is null then ep.Amount end) as HeldBackCompletionPayments,
                                sum(case when p.Id is null and f.Id is null and e.ContractType = 1 then ep.Amount end) as HeldBackCompletionPaymentsAct1,
                                sum(case when p.Id is null and f.Id is null and e.ContractType = 2 then ep.Amount end) as HeldBackCompletionPaymentsAct2
                        from
                                Payments2.EarningEvent e with(nolock)
                                join Payments2.EarningEventPeriod ep with(nolock) on ep.EarningEventId = e.EventId
                                left join Payments2.RequiredPaymentEvent p with(nolock) on p.EarningEventId = ep.EarningEventId
                                        and p.DeliveryPeriod = ep.DeliveryPeriod
                                        and p.TransactionType = ep.TransactionType
                                left join Payments2.DataLockFailure f with(nolock) on f.EarningEventId = e.EventId
                                        and f.DeliveryPeriod = ep.DeliveryPeriod
                                        and f.TransactionType = ep.TransactionType
                        where
                                ep.TransactionType = 2
                                and ep.Amount <> 0
                                and e.AcademicYear = @academicYear
                                and e.CollectionPeriod = @collectionPeriod
                                and ep.DeliveryPeriod <= @collectionPeriod
                        group by
                                e.Ukprn,
                                ep.TransactionType,
                                                       e.JobId
                ) as HeldBackCompletionPayments
                on HeldBackCompletionPayments.TransactionType = TransactionTypes.TransactionType
                        and HeldBackCompletionPayments.Ukprn = ukprns.Ukprn
                                         and HeldBackCompletionPayments.JobId  = ukprns.lastJobId

                --FundingSource
                left join (
                        select
                                fs.Ukprn,
                                fs.TransactionType,
                                                
                                sum(case when p.Id is not null and f.Id is null and fs.FundingSourceType = 1 then fs.Amount end) as PaymentsUsingLevy, --Levy accoutn 2=CoinvestSFA, 3-CoInvestEmp, 4-FullFunded, 5-Transfer,
                                sum(case when p.Id is not null and f.Id is null and fs.FundingSourceType = 2 then fs.Amount end) as PaymentsUsingCoInvestmentSFA,
                                sum(case when p.Id is not null and f.Id is null and fs.FundingSourceType = 3 then fs.Amount end) as PaymentsUsingCoInvestmentEmployer,
                                sum(case when p.Id is not null and f.Id is null and fs.FundingSourceType = 4 then fs.Amount end) as PaymentsUsingFullyFundedSFA,
                                sum(case when p.Id is not null and f.Id is null and fs.FundingSourceType = 5 then fs.Amount end) as PaymentsUsingTransfer
                      from
                                 Payments2.FundingSourceEvent fs with(nolock)
                                inner join Payments2.EarningEventPeriod ep with(nolock) on ep.EarningEventId = fs.EarningEventId  
                                                                    --and  ep.DeliveryPeriod <= fs.CollectionPeriod 
                                                                    and  ep.DeliveryPeriod = fs.DeliveryPeriod 
                                                                    and fs.TransactionType = ep.TransactionType
                                left join Payments2.RequiredPaymentEvent p with(nolock) on p.EarningEventId = ep.EarningEventId
                                        and p.DeliveryPeriod = ep.DeliveryPeriod
                                        and p.TransactionType = ep.TransactionType
                                left join Payments2.DataLockFailure f with(nolock) on f.EarningEventId = fs.EarningEventId
                                        and f.DeliveryPeriod = ep.DeliveryPeriod
                                        and f.TransactionType = ep.TransactionType
                        where
                                fs.Amount <> 0
                                and fs.AcademicYear = @academicYear
                                and fs.CollectionPeriod = @collectionPeriod
                                and fs.DeliveryPeriod <= @collectionPeriod
                        group by
                                fs.Ukprn,
                                fs.TransactionType
                                         
                ) as FundingSource
                on FundingSource.TransactionType = TransactionTypes.TransactionType
                        and FundingSource.Ukprn = ukprns.Ukprn
                                         --and FundingSource.JobId = @monthendjobid

--where TransactionTypes.TransactionType in (1,2,3)
order by 1,3

