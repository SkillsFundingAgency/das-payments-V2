#Review needed
Feature: Break in Learning, leaner takes 2 months break

Background:
	
	Given the current processing period is 8

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 01/09/2017        | 08/09/2018             | 31/10/2017    	       | planned break    |
	| 2            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 03/01/2018        | 08/11/2018             |				       | Continuing 	  |

	And the following contract type 2 on programme earnings for periods 1-3 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| p1                     | 01/09/2017       | 01/09/2017                   | 15000                | 1000       |	

	And the following contract type 2 on programme earnings for periods 6-14 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| p2                     | 03/01/2018       | 03/01/2018                   | 15000                | 1000       |	

@Non-DAS
@minimum_tests_additional
@BreakInLearning
@review

Scenario Outline: Contract Type 2 On programme payments

	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1				      | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1				      | 2      | 10000 | <transaction_type> | <amount> |

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 1000   |


Scenario Outline: Contract Type 2 On programme payments changed price

	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p2				      | 5      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p2				      | 6      | 10000 | <transaction_type> | <amount> |

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p2                     | 5      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p2                     | 6      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p2                     | 7      | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 1000   |

#
#    V1 - DAS test
#	
#    Scenario: Apprentice goes on a planned break midway through the learning episode and this is notified through the ILR
#        Given the following commitments exist on 03/12/2017:
#            | commitment Id | version Id | ULN       | start date | end date   | status | agreed price | effective from | effective to |
#            | 1             | 1-001      | learner a | 01/09/2017 | 08/09/2018 | active | 15000        | 01/09/2017     | 31/10/2017   |
#            | 1             | 1-002      | learner a | 01/09/2017 | 08/09/2018 | paused | 15000        | 01/11/2017     | 02/01/2018   |
#            | 1             | 1-003      | learner a | 01/09/2017 | 08/09/2018 | active | 15000        | 03/01/2018     |              |
#        When an ILR file is submitted on 03/12/2017 with the following data:
#            | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#            | learner a | 01/09/2017 | 08/09/2018       | 31/10/2017      | planned break     | 12000                | 01/09/2017                          | 3000                   | 01/09/2017                            |
#            | learner a | 03/01/2018 | 08/11/2018       |                 | continuing        | 12000                | 03/01/2018                          | 3000                   | 03/01/2018                            |
#        Then the provider earnings and payments break down as follows:
#            | Type                     | 09/17 | 10/17 | 11/17 | 12/17 | 01/18 | 02/18 | ... | 10/18 | 11/18 |
#            | Provider Earned from SFA | 1000  | 1000  | 0     | 0     | 1000  | 1000  | ... | 1000  | 0     |
#            | Provider Paid by SFA     | 0     | 1000  | 1000  | 0     | 0     | 1000  | ... | 1000  | 1000  |
#            | Levy account debited     | 0     | 1000  | 1000  | 0     | 0     | 1000  | ... | 1000  | 1000  |
#            | SFA Levy employer budget | 1000  | 1000  | 0     | 0     | 1000  | 1000  | ... | 1000  | 0     |