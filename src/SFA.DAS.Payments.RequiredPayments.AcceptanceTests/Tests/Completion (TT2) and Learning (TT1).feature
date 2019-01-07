Feature: Completion (TT2) and Learning (TT1)
R13 - First payment, no previous payments. Also Completion.

Background:
	Given the current processing period is 13
	And the payments are for the current collection year
	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000
	And the SFA contribution percentage is 90%
	And the earning events component generates the following contract type 2 earnings:	
	| PriceEpisodeIdentifier | Delivery Period | TransactionType  | Amount |
	| p1                     | 1               | Learning (TT1)   | 600    |
	| p1                     | 2               | Learning (TT1)   | 600    |
	| p1                     | 3               | Learning (TT1)   | 600    |
	| p1                     | 4               | Learning (TT1)   | 600    |
	| p1                     | 5               | Learning (TT1)   | 600    |
	| p1                     | 6               | Learning (TT1)   | 600    |
	| p1                     | 7               | Learning (TT1)   | 600    |
	| p1                     | 8               | Learning (TT1)   | 600    |
	| p1                     | 9               | Learning (TT1)   | 600    |
	| p1                     | 10              | Learning (TT1)   | 600    |
	| p1                     | 11              | Learning (TT1)   | 600    |
	| p1                     | 12              | Learning (TT1)   | 600    |
	| p1                     | 13              | Completion (TT2) | 1800   |

@Non-DAS
@Learning (TT1)
@Completion
@Historical_Payments

Scenario: Contract Type 2 On Programme Learning payments
	When an earning event is received
	Then the required payments component will only generate contract type 2 required payments
	| PriceEpisodeIdentifier | Delivery Period | TransactionType  | Amount |
	| p1                     | 1               | Learning (TT1)   | 600    |
	| p1                     | 2               | Learning (TT1)   | 600    |
	| p1                     | 3               | Learning (TT1)   | 600    |
	| p1                     | 4               | Learning (TT1)   | 600    |
	| p1                     | 5               | Learning (TT1)   | 600    |
	| p1                     | 6               | Learning (TT1)   | 600    |
	| p1                     | 7               | Learning (TT1)   | 600    |
	| p1                     | 8               | Learning (TT1)   | 600    |
	| p1                     | 9               | Learning (TT1)   | 600    |
	| p1                     | 10              | Learning (TT1)   | 600    |
	| p1                     | 11              | Learning (TT1)   | 600    |
	| p1                     | 12              | Learning (TT1)   | 600    |
	| p1                     | 13              | Completion (TT2) | 1800   |
