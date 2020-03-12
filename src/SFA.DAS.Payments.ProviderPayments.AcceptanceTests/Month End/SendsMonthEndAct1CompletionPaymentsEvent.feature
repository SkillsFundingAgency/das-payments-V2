Feature: Month End Stop should send Act1 Completion payment events to Approvals service
	As a Provider 
	I would like to send a message to approvals when the month end stop event is received for any completion payments
	so that approval know that a learner has successfully completed their apprenticeship

Background:
	Given a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%

Scenario: upon month end stop, approvals are notified of any completion payments 
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
	