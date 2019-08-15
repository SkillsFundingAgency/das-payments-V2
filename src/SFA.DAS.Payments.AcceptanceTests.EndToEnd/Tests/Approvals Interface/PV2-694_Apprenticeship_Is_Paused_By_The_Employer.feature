Feature:PV2-694 Learners apprenticeship is Paused by the Employer
	As an Employer,
	I want to be able to pause a learners apprenticeship from today's date or retrospectively
	So that I can temporarily stop making payments for the learner from my Levy

Scenario Outline: PV2-694 Learners apprenticeship is Paused by the Employer
	Given the following employers
		| Identifier       | Legal Entity Name | Agreement Id |
		| Employer A       | Test Employer     | 12345        |

	And the following apprenticeships already exist with Employer Type "<Employer Type>"
		| Identifier       | Created On Date              | Agreed On Date               | Learner   | Provider   | Employer   | Standard Code | Programme Type | Framework Code | Pathway Code | Start date                   | End date                  |
		| Apprenticeship A | 01/Aug/Current Academic Year | 01/Aug/Current Academic Year | Learner A | Provider A | Employer A | 20            | 25             | 593            | 1            | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year |
	
	And the existing apprenticeships have the following price episodes
		| Apprenticeship   | Agreed Price | Effective From               |
		| Apprenticeship A | 15000        | 01/Aug/Current Academic Year |

   And the apprenticeship is paused as follows
		| Identifier       | Paused On Date               | Status |
		| Apprenticeship A | 25/Dec/Current Academic Year | paused |
	
	When the Approvals service notifies the Payments service that the apprenticeships has been paused
	Then the Payments service should record the paused apprenticeships
	And the Payments service should record the paused apprenticeships history
Examples:
	| Employer Type |
	| Non-Levy      |
	| Levy          |