#New name - learner_finishes_early_Balancing_Completion
#Old name - learner_finishes_early - non-DAS learner

Feature: Provider earnings and payments where learner completes earlier than planned

    #The earnings and payment rules for early completions are the same as for learners finishing on time, except that the completion payment is earned earlier.

Background:

	Given the current processing period is 13
	#Given the apprenticeship funding band maximum for each learner is 20000

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 01/09/2017        | 08/12/2018             | 08/09/2018            | Completed       |

	And the following contract type 2 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| p1                     | 01/09/2017       | 01/09/2017                   | 18750                | 1000       |

	And the following contract type 2 on programme earnings for period 13 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Completion_2 | Balancing_3 |
	| p1                     | 01/09/2017       | 01/09/2017                   | 18750                | 3750         | 3000        |

	
@Non-DAS
@minimum_tests
@completion
@balancing
@FinishingEarly

Scenario Outline: Contract Type 2 On programme payments
	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 7      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 8      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 9      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 10     | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 11     | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 12     | 10000 | <transaction_type> | <amount> |

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 7      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 8      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 9      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 10     | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 11     | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 12     | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 1000    |	
	
Scenario Outline: Contract Type 2 completion payment

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 13      | 10000 | <transaction_type> | <amount> |
	
	Examples: 
	| transaction_type | amount |
	| Completion_2     | 3750   |
	
	
Scenario Outline: Contract Type 2 balancing payment

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 13      | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Balancing_3      | 3000   |	
	

#----------------------------------------------------------------------------------------------------------------------------------------
#/*  Payments V1 for reference
#
#Feature: Provider earnings and payments where learner completes earlier than planned
#
#    The earnings and payment rules for early completions are the same as for learners finishing on time, except that the completion payment is earned earlier.
#
#    Background:
#        Given the apprenticeship funding band maximum for each learner is 20000

#    Scenario: A non-DAS learner, learner finishes early
#        When an ILR file is submitted with the following data:
#            | ULN       | learner type           | agreed price | start date | planned end date | actual end date | completion status |
#            | learner a | programme only non-DAS | 18750        | 01/09/2017 | 08/12/2018       | 08/09/2018      | completed         |
#        Then the provider earnings and payments break down as follows:
#            | Type                           | 09/17 | 10/17 | 11/17 | ... | 08/18 | 09/18 | 10/18 |
#            | Provider Earned Total          | 1000  | 1000  | 1000  | ... | 1000  | 6750  | 0     |
#            | Provider Earned from SFA       | 900   | 900   | 900   | ... | 900   | 6075  | 0     |
#            | Provider Earned from Employer  | 100   | 100   | 100   | ... | 100   | 675   | 0     |
#            | Provider Paid by SFA           | 0     | 900   | 900   | ... | 900   | 900   | 6075  |
#            | Payment due from Employer      | 0     | 100   | 100   | ... | 100   | 100   | 675   |
#            | Levy account debited           | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#            | SFA Levy employer budget       | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#            | SFA Levy co-funding budget     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#            | SFA non-Levy co-funding budget | 900   | 900   | 900   | ... | 900   | 6075  | 0     |
#        And the transaction types for the payments are:
#            | Payment type             | 10/17 | 11/17 | 12/17 | 01/18 | ... | 09/18 | 10/18 |
#            | On-program               | 900   | 900   | 900   | 900   | ... | 900   | 0     |
#            | Completion               | 0     | 0     | 0     | 0     | ... | 0     | 3375  |
#            | Balancing                | 0     | 0     | 0     | 0     | ... | 0     | 2700  |
#            | Employer 16-18 incentive | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#            | Provider 16-18 incentive | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#*/        
#----------------------------------------------------------------------------------------------------------------------------------------