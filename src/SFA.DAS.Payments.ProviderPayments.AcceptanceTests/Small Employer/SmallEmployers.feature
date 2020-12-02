Feature: non-DAS learner employed with a small employer, is fully funded for on programme and completion payments

Background:
	Given a learner is undertaking a training with a training provider
	And the payments are for the current collection year

@SmallEmployerNonDas
Scenario: AC1-Payment for a 16-18 non-DAS learner, small employer at start
	Given the SFA contribution percentage is 100%
	And the current collection period is R01
	And the funding source service generates the following contract type 2 payments:
	| Delivery Period | Transaction Type | Funding Source | Amount |
	| 1               | Learning (TT1)   | CoInvestedSfa  | 500    |
	When the funding source payments event are received
	Then the provider payments service will store the following payments:
	| Delivery Period | TransactionType | FundingSource | Amount |
	| 1               | Learning (TT1)  | CoInvestedSfa | 500    |

	   
Scenario: AC5- Payment for a 16-18 non-DAS learner, employer is not small
	Given the SFA contribution percentage is 90%
	And the current collection period is R01
	And the funding source service generates the following contract type 2 payments:
	| Delivery Period | Transaction Type | Funding Source       | Amount |
	| 1               | Learning (TT1)   | CoInvestedSfa       | 450    |
	| 1               | Learning (TT1)   | CoInvestedEmployer  | 50     |
	When the funding source payments event are received
	Then the provider payments service will store the following payments:
	| Delivery Period | TransactionType | FundingSource      | Amount |
	| 1               | Learning (TT1)  | CoInvestedSfa      | 450    |
	| 1               | Learning (TT1)  | CoInvestedEmployer | 50     |
