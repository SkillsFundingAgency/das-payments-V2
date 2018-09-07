#New name - ContractTypeChangesFromAct2ToAct1
#Old name - DPP_965_01

Feature: Contract Type Changes From ACT2 To ACT1
	DPP_965_01 - Levy apprentice, provider edits contract type (ACT) in the ILR, previous on-programme and English/math payments are refunded and repaid according to latest contract type

Background:
	Given the current processing period is 3

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 20/08/2018             |                       | continuing       |

	And the following contract type 1 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| p2                     | 06/08/2017       | 06/08/2017                   | 9000                 | 600        | 
	
@Non-DAS
@minimum_tests
@learner_changes_contract_type
@apprenticeship_contract_type_changes
#@English_Maths

Scenario Outline: Contract Type 2 On programme payments

	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount    |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount>  |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount>  |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | -<amount> |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | -<amount> |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 600    |

@DAS
@minimum_tests
@learner_changes_contract_type
@apprenticeship_contract_type_changes
#@English_Maths

Scenario Outline: Contract Type 1 On programme payments

	When a TOBY is received

	Then the payments due component will generate the following contract type 1 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p2                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p2                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p2                     | 3      | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 600    |





#----------------------------------------------------
#Earlier approach - Following sections can be removed - Start
#----------------------------------------------------

@Non-DAS
@minimum_tests
@learner_changes_contract_type
@apprenticeship_contract_type_changes
#@English_Maths

Scenario: Contract Type 2 Payable Earnings

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | SfaContributionPercentage | Learning_1 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 |
	| learnref1      | 10000 | p1                     | 1      | 10000 | 0.90000                   | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | p1                     | 2      | 10000 | 0.90000                   | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | p1                     | 1      | 10000 | 0.90000                   | -600       | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | p1                     | 2      | 10000 | 0.90000                   | -600       | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |

@DAS
@minimum_tests
@learner_changes_contract_type
@apprenticeship_contract_type_changes
#@English_Maths

Scenario: Contract Type 1 Payable Earnings

	When a TOBY is received

	Then the payments due component will generate the following contract type 1 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | SfaContributionPercentage | Learning_1 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 |
	| learnref1      | 10000 | p2                     | 1      | 10000 | 0.90000                   | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | p2                     | 2      | 10000 | 0.90000                   | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | p2                     | 3      | 10000 | 0.90000                   | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
 
#----------------------------------------------------
#Earlier approach - Following sections can be removed - End
#----------------------------------------------------





#-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

# /* V1 - Original Test
#
# @learner_changes_contract_type
#Feature: Learner changes contract type
#
#Scenario: DPP_965_01 - Levy apprentice, provider edits contract type (ACT) in the ILR, previous on-programme and English/math payments are refunded and repaid according to latest contract type
#    Given The learner is programme only DAS
#	And levy balance > agreed price for all months
#	And the apprenticeship funding band maximum is 9000
#
#	And the following commitments exist:
#		| commitment Id | version Id | ULN       | start date | end date   | framework code | programme type | pathway code | agreed price | status | effective from | effective to |
#		| 1             | 1          | learner a | 01/08/2017 | 01/08/2018 | 403            | 2              | 1            | 9000         | Active | 01/08/2017     |              |
#        
#	When an ILR file is submitted for period R01 with the following data:
#        | ULN       | learner type           | agreed price | start date | planned end date | actual end date | completion status | aim type         | aim sequence number | aim rate | framework code | programme type | pathway code | contract type | contract type date from |
#        | learner a | programme only non-DAS | 9000         | 06/08/2017 | 20/08/2018       |                 | continuing        | programme        | 2                   |          | 403            | 2              | 1            | Non-DAS       | 06/08/2017              |
#        | learner a | programme only non-DAS |              | 06/08/2017 | 20/08/2018       |                 | continuing        | maths or english | 1                   | 471      | 403            | 2              | 1            |               |                         |
#        
#    And an ILR file is submitted for period R03 with the following data:
#        | ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | aim type         | aim sequence number | aim rate | framework code | programme type | pathway code | contract type | contract type date from |
#        | learner a | programme only DAS | 9000         | 06/08/2017 | 20/08/2018       |                 | continuing        | programme        | 2                   |          | 403            | 2              | 1            | DAS           | 06/08/2017              |
#        | learner a | programme only DAS |              | 06/08/2017 | 20/08/2018       |                 | continuing        | maths or english | 1                   | 471      | 403            | 2              | 1            |               |                         |
#
#	Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/17  | 09/17  | 10/17  | 11/17    |
#        | Provider Earned Total                   | 639.25 | 639.25 | 639.25 | 639.25   |
#        | Provider Earned from SFA                | 639.25 | 639.25 | 639.25 | 639.25   |
#        | Provider Earned from Employer           | 0      | 0      | 0      | 0        |
#        | Provider Paid by SFA                    | 0      | 579.25 | 579.25 | 1917.75  |
#        | Refund taken by SFA                     | 0      | 0      | 0      | -1158.50 |
#        | Payment due from Employer               | 0      | 60     | 60     | 0        |
#        | Refund due to employer                  | 0      | 0      | 0      | 120      |
#        | Levy account debited                    | 0      | 0      | 0      | 1800     |
#        | Levy account credited                   | 0      | 0      | 0      | 0        |
#        | SFA Levy employer budget                | 600    | 600    | 600    | 600      |
#        | SFA Levy co-funding budget              | 0      | 0      | 0      | 0        |
#        | SFA Levy additional payments budget     | 39.25  | 39.25  | 39.25  | 39.25    |
#        | SFA non-Levy co-funding budget          | 0      | 0      | 0      | 0        |
#        | SFA non-Levy additional payments budget | 0      | 0      | 0      | 0        |
#
#
# */


#-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------