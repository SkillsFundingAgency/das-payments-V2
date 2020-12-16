Feature: Month End
	As a Provider 
	I would like to receive payments calculated after the period closes

Background:
	Given a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%

Scenario: Provider payments after month end
	Given the current collection period is R02
	And the payments are for the current collection year
	And the funding source service generates the following contract type 2 payments:
	| Delivery Period | Transaction Type                   | Funding Source     | Amount |
	| 1               | Learning (TT1)                     | CoInvestedSfa      | 900    |
	| 1               | Completion (TT2)                   | CoInvestedSfa      | 2700   |
	| 1               | Balancing (TT3)                    | CoInvestedSfa      | 1800   |
	| 1               | Learning (TT1)                     | CoInvestedEmployer | 100    |
	| 1               | Completion (TT2)                   | CoInvestedEmployer | 300    |
	| 1               | Balancing (TT3)                    | CoInvestedEmployer | 1350   |
	| 1               | First16To18EmployerIncentive (TT4) | FullyFundedSfa     | 500    |
	When the period closes and month end processing begins
	And the funding source payments event are received
	Then the provider payments service will store the following payments:
	| Delivery Period | TransactionType                    | FundingSource      | Amount |
	| 1               | Learning (TT1)                     | CoInvestedSfa      | 900    |
	| 1               | Completion (TT2)                   | CoInvestedSfa      | 2700   |
	| 1               | Balancing (TT3)                    | CoInvestedSfa      | 1800   |
	| 1               | Learning (TT1)                     | CoInvestedEmployer | 100    |
	| 1               | Completion (TT2)                   | CoInvestedEmployer | 300    |
	| 1               | Balancing (TT3)                    | CoInvestedEmployer | 1350   |
	| 1               | First16To18EmployerIncentive (TT4) | FullyFundedSfa     | 500    |