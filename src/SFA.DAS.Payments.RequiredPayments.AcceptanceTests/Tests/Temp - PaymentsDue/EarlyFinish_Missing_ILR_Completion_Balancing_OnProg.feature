Feature: Provider earnings and payments where learner completes earlier than planned (3 months early) but missing previous submission

Background:

	Given the current processing period is 10

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the SFA contribution percentage is "90%"

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus	|
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 01/09/2017        | 08/09/2018             | 08/06/2018            | Completed			|

	And the following contract type 2 On Programme earnings for periods 1-9 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | TransactionType	| Amount	|
	| p1                     | 01/09/2017       | 01/09/2017                   | 15000                | Learning (TT1)	| 1000		|

	And the following contract type 2 On Programme earnings for period 10 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | TransactionType	| Amount	|
	| p1                     | 01/09/2017       | 01/09/2017                   | 15000                | Completion (TT2)| 3000      |
	| p1                     | 01/09/2017       | 01/09/2017                   | 15000                | Balancing (TT3) | 3000      |

	
@Non-DAS
@Completion (TT2)
@Balancing (TT3)
@FinishedEarly
@MissingSubmission

Scenario Outline: Contract Type 2 On Programme Learning payments

	When an earnings event is received

	Then the payments due component will generate the following contract type 2 Learning (TT1) payments due:
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

	Examples: 
	| transaction_type | amount |
	| Learning (TT1)   | 1000   |	
	
Scenario Outline: Contract Type 2 On Programme Completion payment

	When an earnings event is received

	Then the payments due component will generate the following contract type 2 Completion (TT2) payments due:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | <transaction_type> | <amount> |
	
	Examples: 
	| transaction_type | amount |
	| Completion (TT2) | 3000   |
	
	
Scenario Outline: Contract Type 2 On Programme Balancing payment

	When an earnings event is received

	Then the payments due component will generate the following contract type 2 Balancing (TT3) payments due:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period  | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 10      | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Balancing (TT3)  | 3000   |	
