﻿Feature: TNP is higher than the maximum funding band, completed on time with full history
#Reduced on program funding as per band maximum - 600 instead of 1000. Also, reduced completion payment - 1800 instead of 3000

Background:
	Given the current processing period is 13
	And the apprenticeship funding band maximum is 9000

	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref1      | 10000 | 10000 |

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 20/08/2018             | 20/08/2018            | completion       |

	And the following contract type 2 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| p1                     | 06/08/2017       | 06/08/2017                   | 15000                | 600        |

	And the following contract type 2 on programme earnings for period 13 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Completion_2 |
	| p1                     | 06/08/2017       | 06/08/2017                   | 15000                | 1800         |

@Non-DAS
@Completion
@funding_band
@capping

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
	| Learning_1       | 600    |

Scenario Outline: Contract Type 2 completion payment

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 13     | 10000 | <transaction_type> | <amount> |
	
	Examples: 
	| transaction_type | amount |
	| Completion_2     | 1800   |