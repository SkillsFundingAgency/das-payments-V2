Feature: PV2-688 - New Apprenticeship Approved
	As an Employer
	I want tthe Payments Calc Data-Locks process to be made aware of newly approved commitments
	So that they can be matched to the ILR Learner and allow payments to be made for the learner

Scenario: PV2-688 - New Apprenticeship Approved
	Given the following employers
		| Identifier       | Legal Entity Name | Agreement Id |
		| Employer A       | Test Employer     | 12345        |
		| Sending Employer | Sender            | 54321        |
	And the following apprenticeships have been approved
		| Identifier       | Created On Date              | Agreed On Date               | Learner   | Provider   | Employer   | Sending Employer | Standard Code | Programme Type | Framework Code | Pathway Code | Start date                   | End date                     |
		| Apprenticeship A | 01/Aug/Current Academic Year | 01/Aug/Current Academic Year | Learner A | Provider A | Employer A | Sending Employer | 20            | 20             | 593            | 1            | 01/Aug/Current Academic Year | 01/Aug/Current Academic Year |
	And the apprenticeships have the following price episodes
		| Apprenticeship   | Agreed Price | Effective From               |
		| Apprenticeship A | 10000        | 01/Aug/Current Academic Year |
	When the Approvals service notifies the Payments service of the apprenticeships
	Then the Payments service should record the apprenticeships
