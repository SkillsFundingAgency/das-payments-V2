Feature: Learning (TT1) only - Single period
#R12 - Final OnProgram payment

Background:
	Given the current processing period is 12
	And the payments are for the current collection year
	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000
	And the SFA contribution percentage is 90%
	And the payments due component generates the following contract type 2 payments due:
	| PriceEpisodeIdentifier | Period | TransactionType | Amount |
	| p1                     | 1      | Learning (TT1)  | 600    |
	| p1                     | 2      | Learning (TT1)  | 600    |
	| p1                     | 3      | Learning (TT1)  | 600    |
	| p1                     | 4      | Learning (TT1)  | 600    |
	| p1                     | 5      | Learning (TT1)  | 600    |
	| p1                     | 6      | Learning (TT1)  | 600    |
	| p1                     | 7      | Learning (TT1)  | 600    |
	| p1                     | 8      | Learning (TT1)  | 600    |
	| p1                     | 9      | Learning (TT1)  | 600    |
	| p1                     | 10     | Learning (TT1)  | 600    |
	| p1                     | 11     | Learning (TT1)  | 600    |
	| p1                     | 12     | Learning (TT1)  | 600    |

	And the following historical contract type 2 On Programme Learning payments exist:
	| PriceEpisodeIdentifier | Period | TransactionType | Amount |
	| p1                     | 1      | Learning (TT1)  | 600    |
	| p1                     | 2      | Learning (TT1)  | 600    |
	| p1                     | 3      | Learning (TT1)  | 600    |
	| p1                     | 4      | Learning (TT1)  | 600    |
	| p1                     | 5      | Learning (TT1)  | 600    |
	| p1                     | 6      | Learning (TT1)  | 600    |
	| p1                     | 7      | Learning (TT1)  | 600    |
	| p1                     | 8      | Learning (TT1)  | 600    |
	| p1                     | 9      | Learning (TT1)  | 600    |
	| p1                     | 10     | Learning (TT1)  | 600    |
	| p1                     | 11     | Learning (TT1)  | 600    |

@Non-DAS
@Learning (TT1)
@Historical_Payments

Scenario Outline: Contract Type 2 On Programme Learning payments
	When a payments due event is received
	Then the required payments component will generate the following contract type 2 Learning (TT1) payable earnings:
	| PriceEpisodeIdentifier | Period | TransactionType    | Amount   |
	| p1                     | 12     | <transaction_type> | <amount> |
	
	Examples: 
	| transaction_type | amount |
	| Learning (TT1)   | 600    |


Scenario: Contract Type 2 no On Programme Completion payment
	When a payments due event is received
	Then the required payments component will not generate any contract type 2 Completion (TT2) payable earnings


Scenario: Contract Type 2 no On Programme Balancing payment
	When a payments due event is received
	Then the required payments component will not generate any contract type 2 Balancing (TT3) payable earnings