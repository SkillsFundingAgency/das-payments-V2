Feature: PV2-1195 - New Apprenticeship Approved
	As the DAS
	I want Approvals messages paused during month end processing and reprocessed after month end stops
	So that consistent data lock result can be achieved during month end processing

Scenario Outline: PV2-1195 - New Apprenticeship Approved
	Given the following employers
		| Identifier       | Legal Entity Name | Agreement Id |
		| Employer A       | Test Employer     | 11950        |
		| Sending Employer | Sender            | 11951        |
	And the following apprenticeships have been approved with Employer Type "<Employer Type>"
		| Identifier       | Created On Date              | Agreed On Date               | Learner   | Provider   | Employer   | Sending Employer | Standard Code | Programme Type | Framework Code | Pathway Code | Start date                   | End date                     |
		| Apprenticeship A | 01/Aug/Current Academic Year | 01/Aug/Current Academic Year | Learner A | Provider A | Employer A | Sending Employer | 20            | 20             | 593            | 1            | 01/Aug/Current Academic Year | 01/Aug/Current Academic Year |
	And the apprenticeships have the following price episodes
		| Apprenticeship   | Agreed Price | Effective From               |
		| Apprenticeship A | 10000        | 01/Aug/Current Academic Year |
	And Period end has started

	When the Approvals service notifies the Payments service of the apprenticeships

	Then the Payments service should record the apprenticeships only when period end stops
Examples:
	| Employer Type |
	| Non-Levy      |
	| Levy          |
