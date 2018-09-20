Feature: Completion (TT2) only - On time completion

#R13 - On time Completion with full history

Background:
	Given the current processing period is 13

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the SFA contribution percentage is "90%"

	And the payments due component generates the following contract type 2 payments due:	
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 4      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 5      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 6      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 7      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 8      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 9      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 11     | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 12     | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Completion (TT2)   | 1800   |

	And the following historical contract type 2 On Programme Learning payments exist:   

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning (TT1)     | 600    |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning (TT1)     | 600    |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 4      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 5      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 6      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 7      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 8      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 9      | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 11     | 10000 | Learning (TT1)		| 600    |
	| learnref1      | 10000 | p1                     | 12     | 10000 | Learning (TT1)		| 600    |

@Non-DAS
@Completion (TT2)
@Historical_Payments

Scenario Outline: Contract Type 2 no On Programme Learning payments

	When a payments due event is received

	Then the required payments component will not generate Learning (TT1) payable earnings


Scenario Outline: Contract Type 2 On Programme Completion payment

	When a payments due event is received

	Then the required payments component will generate the following contract type 2 Completion (TT2) payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 13     | 10000 | <transaction_type> | <amount> |
	
	Examples: 
	| transaction_type | amount |
	| Completion (TT2) | 1800   |


Scenario: Contract Type 2 no On Programme Balancing payment

	When a payments due event is received

	Then the required payments component will not generate any contract type 2 Balancing (TT3) payable earnings