Review needed as
Feature: Break in Learning, leaner takes 2 months break

Background:
	
	Given the current processing period is 8


	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref1      | 10000 | 10000 |

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 04/08/2017        | 20/08/2018             | 20/11/2018    	       | BreakInLearning  |
	| 2            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 11/01/2018        | 06/10/2018             |				       | Continuing 	  |

	And the following contract type 2 on programme earnings for periods 1-3 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| p1                     | 04/08/2017       | 04/08/2017                   | 11250                | 750        |	

	And the following contract type 2 on programme earnings for periods 6-14 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| p2                     | 11/01/2018       | 11/11/2018                   | 11250                | 750        |	

@Non-DAS
@minimum_tests
@BreakInLearning
@review

Scenario Outline: Contract Type 2 On programme payments

	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1				      | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1				      | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1				      | 3      | 10000 | <transaction_type> | <amount> |

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 750    |


Scenario Outline: Contract Type 2 On programme payments changed price

	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p2				      | 6      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p2				      | 7      | 10000 | <transaction_type> | <amount> |

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p2                     | 6      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p2                     | 7      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p2                     | 8      | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 750    |
