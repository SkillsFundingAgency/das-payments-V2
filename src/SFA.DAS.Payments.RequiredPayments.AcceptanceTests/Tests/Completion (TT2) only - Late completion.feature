Feature: Completion (TT2) only - Late completion
R14 - Delayed Completion, No OnProgram payment

Background:
	Given the current processing period is 14
	And the payments are for the current collection year
	And the SFA contribution percentage is 90%
	And the payments due component generates the following contract type 2 payments due:	
	| PriceEpisodeIdentifier | Delivery Period | TransactionType  | Amount |
	| p1                     | 1      | Learning (TT1)   | 600    |
	| p1                     | 2      | Learning (TT1)   | 600    |
	| p1                     | 3      | Learning (TT1)   | 600    |
	| p1                     | 4      | Learning (TT1)   | 600    |
	| p1                     | 5      | Learning (TT1)   | 600    |
	| p1                     | 6      | Learning (TT1)   | 600    |
	| p1                     | 7      | Learning (TT1)   | 600    |
	| p1                     | 8      | Learning (TT1)   | 600    |
	| p1                     | 9      | Learning (TT1)   | 600    |
	| p1                     | 10     | Learning (TT1)   | 600    |
	| p1                     | 11     | Learning (TT1)   | 600    |
	| p1                     | 12     | Learning (TT1)   | 600    |
	| p1                     | 14     | Completion (TT2) | 3000   |

	And the following historical contract type 2 payments exist:
	| PriceEpisodeIdentifier | Delivery Period | TransactionType | Amount |
	| p1                     | 1               | Learning (TT1)  | 600    |
	| p1                     | 2               | Learning (TT1)  | 600    |
	| p1                     | 3               | Learning (TT1)  | 600    |
	| p1                     | 4               | Learning (TT1)  | 600    |
	| p1                     | 5               | Learning (TT1)  | 600    |
	| p1                     | 6               | Learning (TT1)  | 600    |
	| p1                     | 7               | Learning (TT1)  | 600    |
	| p1                     | 8               | Learning (TT1)  | 600    |
	| p1                     | 9               | Learning (TT1)  | 600    |
	| p1                     | 10              | Learning (TT1)  | 600    |
	| p1                     | 11              | Learning (TT1)  | 600    |
	| p1                     | 12              | Learning (TT1)  | 600    |

@Non-DAS
@Completion (TT2)
@Historical_Payments

Scenario: Contract Type 2 no On Programme payments
	When a payments due event is received
	Then the required payments component will only generate contract type 2 required payments
	| PriceEpisodeIdentifier | Delivery Period | TransactionType  | Amount |
	| p1                     | 14              | Completion (TT2) | 3000   |
