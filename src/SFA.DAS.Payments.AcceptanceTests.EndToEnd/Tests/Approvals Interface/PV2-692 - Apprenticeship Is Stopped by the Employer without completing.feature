Feature: PV2-692 - Apprenticeship Is Stopped by the Employer without completing
	As an Employer
	I want to be able to stop a learners apprenticeship from today's date or retrospectively.
	So that I no longer use my Levy funds to pay for the learner

Scenario Outline: PV2-692 - Apprenticeship Is Stopped by the Employer without completing
	Given the following employers
		| Identifier       | Legal Entity Name | Agreement Id |
		| Employer A       | Test Employer     | 12345        |

	And the following apprenticeships already exist with Employer Type "<Employer Type>"
		| Identifier       | Created On Date              | Agreed On Date               | Learner   | Provider   | Employer   | Standard Code | Programme Type | Framework Code | Pathway Code | Start date                   | End date                  |
		| Apprenticeship A | 01/Aug/Current Academic Year | 01/Aug/Current Academic Year | Learner A | Provider A | Employer A | 20            | 25             | 593            | 1            | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year |
	
	And the existing apprenticeships have the following price episodes
		| Apprenticeship   | Agreed Price | Effective From               |
		| Apprenticeship A | 15000        | 01/Aug/Current Academic Year |

   And the apprenticeship is stopped as follows
		| Identifier       | Stopped On Date              | Status  |
		| Apprenticeship A | 25/Dec/Current Academic Year | Stopped |
	
	When the Approvals service notifies the Payments service that the apprenticeships has stopped
	Then the Payments service should record the stopped apprenticeships
Examples:
	| Employer Type |
	| Non-Levy      |
	| Levy          |