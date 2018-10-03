Feature: No history - ALL TT1 TT2 and TT3
#Provider earnings and payments where learner completes earlier than planned (3 months early) but missing previous submissions

Background:

	Given the current processing period is 10
	And the payments are for the current collection year
	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000
	And the SFA contribution percentage is 90%
	And the payments due component generates the following contract type 2 payments due:	
	| PriceEpisodeIdentifier | Period | TransactionType  | Amount |
	| p1                     | 1      | Learning (TT1)   | 1000   |
	| p1                     | 2      | Learning (TT1)   | 1000   |
	| p1                     | 3      | Learning (TT1)   | 1000   |
	| p1                     | 4      | Learning (TT1)   | 1000   |
	| p1                     | 5      | Learning (TT1)   | 1000   |
	| p1                     | 6      | Learning (TT1)   | 1000   |
	| p1                     | 7      | Learning (TT1)   | 1000   |
	| p1                     | 8      | Learning (TT1)   | 1000   |
	| p1                     | 9      | Learning (TT1)   | 1000   |
	| p1                     | 10     | Completion (TT2) | 3000   |
	| p1                     | 10     | Balancing (TT3)  | 3000   |
	
@Non-DAS
@Learning (TT1)
@Completion (TT2)
@Balancing (TT3)
@NoHistory

Scenario Outline: Contract Type 2 On Programme Learning payments
	
	When a payments due event is received

	Then the required payments component will generate the following contract type 2 Learning (TT1) payable earnings:
	| PriceEpisodeIdentifier | Period | TransactionType    | Amount   |
	| p1                     | 1      | <transaction_type> | <amount> |
	| p1                     | 2      | <transaction_type> | <amount> |
	| p1                     | 3      | <transaction_type> | <amount> |
	| p1                     | 4      | <transaction_type> | <amount> |
	| p1                     | 5      | <transaction_type> | <amount> |
	| p1                     | 6      | <transaction_type> | <amount> |
	| p1                     | 7      | <transaction_type> | <amount> |
	| p1                     | 8      | <transaction_type> | <amount> |
	| p1                     | 9      | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Learning (TT1)   | 1000   |	
	
Scenario Outline: Contract Type 2 On Programme Completion payment

	When a payments due event is received

	Then the required payments component will generate the following contract type 2 Completion (TT2) payable earnings:
	| PriceEpisodeIdentifier | Period | TransactionType    | Amount   |
	| p1                     | 10     | <transaction_type> | <amount> |
	
	Examples: 
	| transaction_type | amount |
	| Completion (TT2) | 3000   |
	
	
Scenario Outline: Contract Type 2 On Programme Balancing payment

	When a payments due event is received

	Then the required payments component will generate the following contract type 2 Balancing (TT3) payable earnings:
	| PriceEpisodeIdentifier | Period  | TransactionType    | Amount   |
	| p1                     | 10      | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Balancing (TT3)  | 3000   |	

