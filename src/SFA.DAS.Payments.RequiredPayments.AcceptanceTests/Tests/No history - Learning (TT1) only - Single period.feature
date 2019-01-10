Feature: No history - Learning (TT1) only - Single period
R01 - First payment, No previous payments

Background:
	Given the current processing period is 1
	And a learner is undertaking a training with a training provider
	And the payments are for the current collection year
	And the SFA contribution percentage is 90%
	And the earning events component generates the following contract type 2 earnings:	
	| PriceEpisodeIdentifier | Delivery Period | TransactionType | Amount |
	| p1                     | 1               | Learning (TT1)  | 600    |

@Non-DAS
@Learning (TT1)
@NoHistory

Scenario: Contract Type 2 On Programme Learning payments
	When an earning event is received
	Then the required payments component will only generate contract type 2 required payments
	| PriceEpisodeIdentifier | Delivery Period | TransactionType | Amount |
	| p1                     | 1               | Learning (TT1)  | 600    |
