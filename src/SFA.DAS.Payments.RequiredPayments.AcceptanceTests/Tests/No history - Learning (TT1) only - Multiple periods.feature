Feature: No history - Learning (TT1) only - Multiple periods
R02 - First payment including previous month

Background:
	Given the current processing period is 2
	And the payments are for the current collection year
	And the SFA contribution percentage is 90%
	And the payments due component generates the following contract type 2 payments due:	
	| PriceEpisodeIdentifier | Delivery Period | TransactionType | Amount |
	| p1                     | 1               | Learning (TT1)  | 600    |
	| p1                     | 2               | Learning (TT1)  | 600    |

@Non-DAS
@Learning (TT1)
@NoHistory

Scenario: Contract Type 2 On Programme Learning payments
	When a payments due event is received
	Then the required payments component will only generate contract type 2 required payments
	| PriceEpisodeIdentifier | Delivery Period | TransactionType | Amount |
	| p1                     | 1               | Learning (TT1)  | 600    |
	| p1                     | 2               | Learning (TT1)  | 600    |