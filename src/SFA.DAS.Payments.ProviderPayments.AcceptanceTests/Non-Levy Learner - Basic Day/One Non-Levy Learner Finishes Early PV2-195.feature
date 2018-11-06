Feature: One Non-Levy Learner Finishes Early PV2-195
Provider earnings and payments where learner completes earlier than planned

Background:
	Given a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%

@NonDas_BasicDay
Scenario: A non-DAS learner, learner finishes early
	Given the current collection period is R02
	And the payments are for the current collection year
	And the funding source service generates the following contract type 2 payments:
	| Delivery Period | Transaction Type | Funding Source      | Amount |
	| 1               | Learning (TT1)   | Co-InvestedSfa      | 900    |
	| 1               | Completion (TT2) | Co-InvestedSfa      | 2700   |
	| 1               | Balancing (TT3)  | Co-InvestedSfa      | 1800   |
	| 1               | Learning (TT1)   | Co-InvestedEmployer | 100    |
	| 1               | Completion (TT2) | Co-InvestedEmployer | 300    |
	| 1               | Balancing (TT3)  | Co-InvestedEmployer | 1350   |
	When the funding source payments event are received
	Then the provider payments service will store the following payments:
	| Delivery Period | TransactionType  | FundingSource       | Amount |
	| 1               | Learning (TT1)   | Co-InvestedSfa      | 900    |
	| 1               | Completion (TT2) | Co-InvestedSfa      | 2700   |
	| 1               | Balancing (TT3)  | Co-InvestedSfa      | 1800   |
	| 1               | Learning (TT1)   | Co-InvestedEmployer | 100    |
	| 1               | Completion (TT2) | Co-InvestedEmployer | 300    |
	| 1               | Balancing (TT3)  | Co-InvestedEmployer | 1350   |
	And at month end the provider payments service will publish the following payments
	| Delivery Period | TransactionType  | FundingSource       | Amount |
	| 1               | Learning (TT1)   | Co-InvestedSfa      | 900    |
	| 1               | Completion (TT2) | Co-InvestedSfa      | 2700   |
	| 1               | Balancing (TT3)  | Co-InvestedSfa      | 1800   |
	| 1               | Learning (TT1)   | Co-InvestedEmployer | 100    |
	| 1               | Completion (TT2) | Co-InvestedEmployer | 300    |
	| 1               | Balancing (TT3)  | Co-InvestedEmployer | 1350   |