/*
This version of the query was constructed and used as part of R02. It should be used to ensure that the payments are consistent across each of the 
payments tables.

It needs work to tidy this up and to remove unnecessary columns. Its been checked in to make sure that it can be shared across the team.

Usage: Update the monthendjobid and other variables at the top of the script.
*/

declare @academicYear smallint = 1920
declare @collectionPeriod tinyint = 2
declare @populateEarnings bit = 1
declare @monthendjobid int = 53742

select
                ukprns.Ukprn,
				ukprns.lastJobId,
                TransactionTypes.TransactionType as [Transaction Type],

                isnull(Earnings.EarningsYTD, 0) - isnull(Datalockerrors.DataLockErrors, 0) - isnull(ActualPayments.ActualPaymentYTD, 0) - isnull(HeldBackCompletionPayments, 0) [Missing Actual Payments],
                isnull(Earnings.EarningsYTD, 0) - isnull(DataLockErrors.DataLockErrors, 0) - isnull(RequiredPaymentYTD, 0) - isnull(HeldBackCompletionPayments, 0) [Missing Required Payments],
                isnull(Earnings.EarningsYTD, 0) - isnull(DatalockerrorsAudit.DataLockErrors, 0) - isnull(RequiredPaymentYTD, 0) - isnull(HeldBackCompletionPayments, 0) [Missing Required Payments (audit DL)],
--                isnull(Earnings.EarningLearnerCount, 0) as [Earnings Learners YTD (audit)],
                isnull(Earnings.EarningsYTD, 0) as [Earnings YTD (audit)],
                isnull(Earnings.EarningsACT1, 0) as [Earnings ACT1 (audit)],
--                isnull(Earnings.EarningsACT2, 0) as [Earnings ACT2 (audit)],
--                --isnull(Earnings.NegativeEarningsYTD, 0) as [Negative Earnings (audit)],
--                --isnull(Earnings.NegativeEarningsACT1, 0) as [Negative Earnings ACT1 (audit)],
--                --isnull(Earnings.NegativeEarningsACT2, 0) as [Negative Earnings ACT2 (audit)],
--                isnull(RequiredPayments.RequiredPaymentLearnerCount, 0) as [RequiredPaymentLearnerCount (audit)],
--                isnull(RequiredPayments.RequiredPaymentYTD, 0) as [Required Payments (audit)],
                isnull(RequiredPayments.RequiredPaymentACT1, 0) as [Required Payments ACT1 (audit)],
--                isnull(RequiredPayments.RequiredPaymentACT2, 0) as [Required Payments ACT2 (audit)],
--                isnull(ActualPayments.ActualPaymentYTD, 0) as [Payments YTD],
                isnull(ActualPayments.ActualPaymentACT1, 0) as [Payments ACT1],
--                isnull(ActualPayments.ActualPaymentACT2, 0) as [Payments ACT2],
                isnull([DataLockErrors].DataLockErrorLearnerCount, 0) as [DataLockError Learner Count],
                isnull([DataLockErrors].DataLockErrors, 0) as [DataLock Errors],
                isnull([DataLockErrorsAudit].DataLockErrorCount, 0) as [DataLockError Learner Count (audit)],
                isnull([DataLockErrorsAudit].DataLockErrors, 0) as [DataLock Errors (audit)],

--                isnull([HeldBackCompletionPayments], 0) as [Held Back Completion (audit)],
                isnull([HeldBackCompletionPaymentsAct1], 0) as [HBCP ACT1 (audit)],
--                isnull([HeldBackCompletionPaymentsAct2], 0) as [HBCP ACT2 (audit)],
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
						select ukprn, max(dcjobid) lastJobId
                        from Payments2.Job j1
                        where AcademicYear = @academicYear
                                and CollectionPeriod = @collectionPeriod
                             
						/*and dcjobid  in (
53742
,53725
,53724
,53723
,53722
,53721
,53720
,53719
,53718
,53717
,53716
,53715
,53714
,53713
,53712
,53711
,53710
,53709
,53708
,53707
,53706
,53705
,53704
,53703
,53702
,53701
,53700
,53699
,53698
,53697
,53696
,53695
,53694
,53693
,53692
,53691
,53690
,53689
,53688
,53687
,53686
,53685
,53684
,53683
,53682
,53681
,53680
,53679
,53678
,53677
,53676
,53675
,53674
,53673
,53672
,53671
,53670
,53669
,53668
,53667
,53666
,53665
,53664
,53663
,53662
,53661
,53660
,53659
,53658
,53657
,53656
,53655
,53654
,53653
,53652
,53651
,53650
,53649
,53648
,53647
,53646
,53645
,53644
,53643
,53642
,53641
,53640
,53639
,53638
,53637
,53636
,53635
,53634
,53633
,53632
,53631
,53630
,53629
,53628
,53627
,53626
,53625
,53624
,53623
,53622
,53621
,53620
,53619
,53618
,53617
,53616
,53615
,53614
,53613
,53612
,53611
,53610
,53609
,53608
,53607
,53606
,53605
,53604
,53603
,53602
,53601
,53600
,53599
,53598
,53597
,53596
,53595
,53594
,53593
,53592
,53591
,53590
,53589
,53588
,53587
,53586
,53585
,53584
,53583
,53582
,53581
,53580
,53579
,53578
,53577
,53576
,53575
,53574
,53573
,53572
,53571
,53570
,53569
,53568
,53567
,53566
,53565
,53564
,53563
,53562
,53561
,53560
,53559
,53558
,53557
,53556
,53555
,53554
,53553
,53552
,53551
,53550
,53549
,53548
,53547
,53546
,53545
,53544
,53543
,53542
,53541
,53540
,53539
,53538
,53537
,53536
,53535
,53534
,53533
,53532
,53531
,53530
,53529
,53528
,53527
,53526
,53525
,53524
,53523
,53522
,53521
,53520
,53519
,53518
,53517
,53516
,53515
,53514
,53513
,53512
,53511
,53510
,53509
,53508
,53507
,53506
,53505
,53504
,53503
,53502
,53501
,53500
,53499
,53498
,53497
,53496
,53495
,53494
,53493
,53492
,53491
,53490
,53489
,53488
,53487
,53486
,53485
,53484
,53483
,53482
,53481
,53480
,53479
,53478
,53477
,53476
,53475
,53474
,53473
,53472
,53471
,53470
,53469
,53468
,53467
,53466
,53465
,53464
,53463
,53462
,53461
,53460
,53459
,53458
,53457
,53456
,53455
,53454
,53453
,53452
,53451
,53450
,53449
,53448
,53447
,53446
,53445
,53444
,53443
,53442
,53441
,53440
,53439
,53438
,53437
,53436
,53435
,53434
,53433
,53432
,53431
,53430
,53429
,53428
,53427
,53426
,53425
,53424
,53423
,53422
,53421
,53420
,53419
,53418
,53417
,53416
,53415
,53414
,53413
,53412
,53411
,53410
,53409
,53408
,53407
,53406
,53405
,53404
,53403
,53402
,53401
,53400
,53399
,53398
,53397
,53396
,53395
,53394
,53393
,53392
,53391
,53390
,53389
,53388
,53387
,53386
,53385
,53384
,53383
,53382
,53381
,53380
,53379
,53378
,53377
,53376
,53375
,53374
,53373
,53372
,53371
,53370
,53369
,53368
,53367
,53366
,53365
,53364
,53363
,53362
,53361
,53360
,53359
,53358
,53357
,53356
,53355
,53354
,53353
,53352
,53351
,53350
,53349
,53348
,53347
,53346
,53345
,53344
,53343
,53342
,53341
,53340
,53339
,53338
,53337
,53336
,53335
,53334
,53333
,53332
,53331
,53330
,53329
,53328
,53327
,53326
,53325
,53324
,53323
,53322
,53321
,53320
,53319
,53318
,53317
,53316
,53315
,53314
,53313
,53312
,53311
,53310
,53309
,53308
,53307
,53306
,53305
,53304
,53303
,53302
,53301
,53300
,53299
,53298
,53297
,53296
,53295
,53294
,53293
,53292
,53291
,53290
,53289
,53288
,53287
,53286
,53285
,53284
,53283
,53282
,53281
,53280
,53279
,53278
,53277
,53276
,53275
,53274
,53273
,53272
,53271
,53270
,53269
,53268
,53267
,53266
,53265
,53264
,53263
,53262
,53261
,53260
,53259
,53258
,53257
,53256
,53255
,53254
,53253
,53252
,53251
,53250
,53249
,53248
,53247
,53246
,53245
,53244
,53243
,53242
,53241
,53240
,53239
,53238
,53237
,53236
,53235
,53234
,53233
,53232
,53231
,53230
,53229
,53228
,53227
,53226
,53225
,53224
,53223
,53222
,53221
,53220
,53219
,53218
,53217
,53216
,53215
,53214
,53213
,53212
,53211
,53210
,53209
,53208
,53207
,53206
,53205
,53204
,53203
,53202
,53201
,53200
,53199
,53198
,53197
,53196
,53195
,53194
,53193
,53192
,53191
,53190
,53189
,53188
,53187
,53186
,53185
,53184
,53183
,53182
,53181
,53180
,53179
,53178
,53177
,53176
,53175
,53174
,53173
,53172
,53171
,53170
,53169
,53168
,53167
,53166
,53165
,53164
,53163
,53162
,53161
,53160
,53159
,53158
,53157
,53156
,53155
,53154
,53153
,53152
,53151
,53150
,53149
,53148
,53147
,53146
,53145
,53144
,53143
,53142
,53141
,53140
,53139
,53138
,53137
,53136
,53135
,53134
,53133
,53132
,53131
,53130
,53129
,53128
,53127
,53126
,53125
,53124
,53123
,53122
,53121
,53120
,53119
,53118
,53117
,53116
,53115
,53114
,53113
,53112
,53111
,53110
,53109
,53108
,53107
,53106
,53105
,53104
,53103
,53102
,53101
,53100
,53099
,53098
,53097
,53096
,53095
,53094
,53093
,53092
,53091
,53090
,53089
,53088
,53087
,53086
,53085
,53084
,53083
,53082
,53081
,53080
,53079
,53078
,53077
,53076
,53075
,53074
,53073
,53072
,53071
,53070
,53069
,53068
,53067
,53066
,53065
,53064
,53063
,53062
,53061
,53060
,53059
,53058
,53057
,53056
,53055
,53054
,53053
,53052
,53051
,53050
,53049
,53048
,53047
,53046
,53045
,53044
,53043
,53042
,53041
,53040
,53039
,53038
,53037
,53036
,53035
,53034
,53033
,53032
,53031
,53030
,53029
,53028
,53027
,53026
,53025
,53024
,53023
,53022
,53021
,53020
,53019
,53018
,53017
,53016
,53015
,53014
,53013
,53012
,53011
,53010
,53009
,53008
,53007
,53006
,53005
,53004
,53003
,53002
,53001
,53000
,52999
,52998
,52997
,52996
,52995
,52994
,52993
,52992
,52991
,52990
,52989
,52988
,52987
,52986
,52985
,52984
,52983
,52982
,52981
,52980
,52979
,52978
,52977
,52976
,52975
,52974
,52973
,52972
,52971
,52970
,52969
,52968
,52967
,52966
,52965
,52964
,52963
,52962
,52961
,52960
,52959
,52958
,52957
,52956
,52955
,52954
,52953
,52952
,52951
,52950
,52949
,52948
,52947
,52946
,52945
,52944
,52943
,52942
,52941
,52940
,52939
,52938
,52937
,52936
,52935
,52934
,52933
,52932
,52931
,52930
,52929
,52928
,52927
,52926
,52925
,52924
,52923
,52922
,52921
,52920
,52919
,52918
,52917
,52916
,52915
,52914
,52913
,52912
,52911
,52910
,52909
,52908
,52907
,52906
,52905
,52904
,52903
,52902
,52901
,52900
,52899
,52898
,52897
,52896
,52895
,52894
,52893
,52892
,52891
,52890
,52889
,52888
,52887
,52886
,52885
,52884
,52883
,52882
,52881
,52880
,52879
,52878
,52877
,52876
,52875
,52874
,52873
,52872
,52871
,52870
,52869
,52868
,52867
,52866
,52865
,52864
,52863
,52862
,52861
,52860
,52859
,52858
,52857
,52856
,52855
,52854
,52853
,52852
,52851
,52850
,52849
,52848
,52847
,52846
,52845
,52844
,52843
,52842
,52841
,52840
,52839
,52838
,52837
,52836
,52835
,52834
,52833
,52832
,52831
,52830
,52829
,52828
,52827
,52826
,52825
,52824
,52823
,52822
,52821
,52820
,52819
,52818
,52817
,52816
,52815
,52814
,52813
,52812
,52811
,52810
,52809
,52808
,52807
,52806
,52805
,52804
,52803
,52802
,52801
,52800
,52799
,52798
,52797
,52796
,52795
,52794
,52793
,52792
,52791
,52790
,52789
,52788
,52787
,52786
,52785
,52784
,52783
,52782
,52781
,52780
,52779
,52778
,52777
,52776
,52775
,52774
,52773
,52772
,52771
,52770
,52769
,52768
,52767
,52766
,52765
,52764
,52763
,52762
,52761
,52760
,52759
,52758
,52757
,52756
,52755
,52754
,52753
,52752
,52751
,52750
,52749
,52748
,52747
,52746
,52745
,52744
,52743
,52742
,52741
,52740
,52739
,52738
,52737
,52736
,52735
,52734
,52733
,52732
,52731
,52730
,52729
,52728
,52727
,52726
										)*/
							group by ukprn
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
                               -- and e.CreationDate between @StartTime and @EndTime
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
								RP.Jobid,
                                count(distinct RP.LearnerUln) [RequiredPaymentLearnerCount],
                                sum(Amount) [RequiredPaymentYTD],
                                sum(case when ContractType = 1 then Amount end) [RequiredPaymentACT1],
                                sum(case when ContractType = 2 then Amount end) [RequiredPaymentACT2]
                        from 
                                [Payments2].[RequiredPaymentEvent] RP
                        where 
                                CollectionPeriod = @collectionPeriod
                                and AcademicYear = @academicYear
                               -- and RP.[CreationDate] between @StartTime and @EndTime
                        group by 
                                ukprn,
                                TransactionType,
								rp.JobId
        ) as RequiredPayments
                on RequiredPayments.TransactionType = TransactionTypes.TransactionType
                        and RequiredPayments.Ukprn = ukprns.Ukprn
						and RequiredPayments.JobId = ukprns.lastJobId

                -- ActualPayments
                left join (
                        select 
                                TransactionType, 
                                Payments2.Payment.Ukprn,
								payments2.payment.jobid,
                                count(distinct LearnerUln) [ActualPaymentLearnerCount],
                                sum(Amount) [ActualPaymentYTD],
                                sum(case when ContractType = 1 then Amount end) [ActualPaymentACT1],
                                sum(case when ContractType = 2 then Amount end) [ActualPaymentACT2]
                        from 
                                Payments2.Payment with(nolock) 
                        where 
                                AcademicYear = @academicYear
                                and CollectionPeriod <= @collectionPeriod
								
                                --and Payments2.Payment.CreationDate between @StartTime and @EndTime
                        group by 
                                Payments2.Payment.Ukprn,
                                TransactionType,
								Payments2.Payment.JobId
                ) as ActualPayments
                on ActualPayments.TransactionType = TransactionTypes.TransactionType
                        and ActualPayments.Ukprn = ukprns.Ukprn
						and ActualPayments.JobId = @monthendjobid

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
                                --and Payments2.DataLockFailure.CreationDate between @StartTime and @EndTime
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
                                --and dle.[CreationDate] between @StartTime and @EndTime
                                and dlenpp.amount > 0
                        group by 
                                dle.Ukprn,
                                dlenpp.TransactionType,
								dle.jobid
                ) as DataLockErrorsAudit
                on DataLockErrorsAudit.TransactionType = TransactionTypes.TransactionType
                        and DataLockErrorsAudit.Ukprn = ukprns.Ukprn
						and DataLockErrorsAudit.JobId = ukprns.lastJobId


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
								fs.JobId,
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
								--and fs.[CreationDate] between @StartTime and @EndTime
                                and fs.AcademicYear = @academicYear
                                and fs.CollectionPeriod = @collectionPeriod
                                and fs.DeliveryPeriod <= @collectionPeriod
                        group by
                                fs.Ukprn,
                                fs.TransactionType,
								fs.jobid
                ) as FundingSource
                on FundingSource.TransactionType = TransactionTypes.TransactionType
                        and FundingSource.Ukprn = ukprns.Ukprn
						and FundingSource.JobId = @monthendjobid

where TransactionTypes.TransactionType in (1,2,3)
order by 1,3

