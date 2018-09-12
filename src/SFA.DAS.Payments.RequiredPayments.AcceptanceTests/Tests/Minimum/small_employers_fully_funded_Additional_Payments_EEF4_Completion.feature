#New name - small_employers_fully_funded_Additional_Payments_EEF4_Completion
#Old name - small_employers - AC3

Feature: Small Employers fully funded additional payments EEF4 completion
		 AC3- 1 learner aged 19-24, non-DAS, is a care leaver, In paid employment with a small employer at start, is fully funded for on programme and completion payments
#Note: care leavers are flagged on the ILR through EEF code = 4*
#Given the apprenticeship funding band maximum is 9000

Background: 

	Given the current processing period is 13

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 08/08/2018             | 08/08/2018            | Completed       |

	#Dave to consider this additional requirement for small employer tests 
	And the employment status in the ILR is:
    | Employer    | Employment Status      | Employment Status Applies | Small Employer |
    | EmployerRef1| in paid employment     | 05/08/2017                | SEM1           |

	#And the EEF (Eligibility for Enhanced Funding) code is:
	#| Employer    | LearnDelFAM |
	#| EmployerRef1| EEF4         |

	And the following contract type 2 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| p1                     | 06/08/2017       | 06/08/2017                   | 7500                 | 500        |

	And the following contract type 2 on programme earnings for period 13 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 | Completion_2 |
	| p1                     | 06/08/2017       | 06/08/2017                   | 7500                 | 500        | 1500         |
	
@Non-DAS
@minimum_tests
@small_employers
@completion
@fully_funded
#@enhanced_funding
#@16-18 incentive
#@Framework_uplift -- will require funding band
@partial

Scenario Outline: Contract Type 2 On programme payments

	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1       | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 7      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 8      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 9      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 10     | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 11     | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 12     | 10000 | <transaction_type> | <amount> |

	When an earning event is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1       | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 7      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 8      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 9      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 10     | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 11     | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 12     | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 500    |	
	
Scenario Outline: Contract Type 2 completion payment

	When an earning event is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period  | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 13      | 10000 | <transaction_type> | <amount> |
	
	Examples: 
	| transaction_type | amount |
	| Completion_2     | 1500   |
	

#----------------------------------------------------------------------------------------------------------------------------------------
# Payments V1 - for reference
#
#@SmallEmployerNonDas
#Scenario:AC3- 1 learner aged 19-24, non-DAS, is a care leaver, In paid employment with a small employer at start, is fully funded for on programme and completion payments
#
##Note: care leavers are flagged on the ILR through EEF code = 4*
#	Given the apprenticeship funding band maximum is 9000
#    When an ILR file is submitted with the following data:
#        | ULN       | learner type                 | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code | Employment Status  | Employment Status Applies | Employer Id | Small Employer | LearnDelFAM |
#        | learner a | 19-24 programme only non-DAS | 7500         | 06/08/2017 | 08/08/2018       | 08/08/2018      | completed         | 403            | 2              | 1            | In paid employment | 05/08/2017                | 12345678    | SEM1           | EEF4        |
#	And the employment status in the ILR is:
#        | Employer    | Employment Status      | Employment Status Applies | Small Employer |
#        | employer 1  | in paid employment     | 05/08/2017                | SEM1           |
#    Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/17 | 09/17 | 10/17 | 11/17 | 12/17 | ... | 07/18 | 08/18 | 09/18 |
#        | Provider Earned Total                   | 620   | 620   | 620   | 1620  | 620   | ... | 620   | 2860  | 0     |
#        | Provider Earned from SFA                | 620   | 620   | 620   | 1620  | 620   | ... | 620   | 2860  | 0     |
#        | Provider Earned from Employer           | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider Paid by SFA                    | 0     | 620   | 620   | 620   | 1620  | ... | 620   | 620   | 2860  |
#        | Payment due from Employer               | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Levy account debited                    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA Levy employer budget                | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA non-Levy co-funding budget          | 500   | 500   | 500   | 500   | 500   | ... | 500   | 1500  | 0     |
#        | SFA non-Levy additional payments budget | 120   | 120   | 120   | 1120  | 120   | ... | 120   | 1360  | 0     |
#
#    And the transaction types for the payments are:
#        | Payment type                 | 09/17 | 10/17 | 11/17 | 12/17 | ... | 08/18 | 09/18 |
#        | On-program                   | 500   | 500   | 500   | 500   | ... | 500   | 0     |
#        | Completion                   | 0     | 0     | 0     | 0     | ... | 0     | 1500  |
#        | Balancing                    | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 500   | ... | 0     | 500   |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 500   | ... | 0     | 500   |
#        | Framework uplift on-program  | 120   | 120   | 120   | 120   | ... | 120   | 0     |
#        | Framework uplift completion  | 0     | 0     | 0     | 0     | ... | 0     | 360   |
#        | Framework uplift balancing   | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Provider disadvantage uplift | 0     | 0     | 0     | 0     | ..  | 0     | 0     |
#----------------------------------------------------------------------------------------------------------------------------------------