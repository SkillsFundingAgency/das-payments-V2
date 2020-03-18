Feature: Month End Stop should send Act1 Completion payment events to Approvals service
	As a Provider 
	I would like to send a message to approvals Team when the month end stop event is received for any completion payments
	so that approvals team know that a learner has successfully completed their apprenticeship


Scenario: upon month end stop, approvals are notified of any completion payments 
	Given the collection period is R02/Current Academic Year
	And the funding source service generates the following contract type payments
	| Contract Type | Collection Period         | Transaction Type                   | Amount |
	| Act1          | R02/Current Academic Year | Learning (TT1)                     | 1000   |
	| Act1          | R02/Current Academic Year | Completion (TT2)                   | 2000   |
	| Act1          | R02/Current Academic Year | Balancing (TT3)                    | 3000   |
	| Act2          | R02/Current Academic Year | Learning (TT1)                     | 4000   |
	| Act2          | R02/Current Academic Year | Completion (TT2)                   | 5000   |
	| Act2          | R02/Current Academic Year | Balancing (TT3)                    | 6000   |
	| Act2          | R02/Current Academic Year | First16To18EmployerIncentive (TT4) | 7000   |
	| Act1          | R03/Current Academic Year | Learning (TT1)                     | 1000   |
	| Act1          | R03/Current Academic Year | Completion (TT2)                   | 2000   |
	| Act1          | R03/Current Academic Year | Balancing (TT3)                    | 3000   |
	| Act2          | R03/Current Academic Year | Learning (TT1)                     | 4000   |
	| Act2          | R03/Current Academic Year | Completion (TT2)                   | 5000   |
	| Act2          | R03/Current Academic Year | Balancing (TT3)                    | 6000   |
	| Act2          | R03/Current Academic Year | First16To18EmployerIncentive (TT4) | 7000   |
	When month end stop event is received
	Then DAS approvals service should be notified of payments for learners with completion payments
	And only learners with completion payments should have events generated

	