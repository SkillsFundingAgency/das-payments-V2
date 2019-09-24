Feature: PV2-693 - Learners apprenticeship Stop Date is Changed by the Employer
	As an Employer,
	I want to be able to change the stop date applied to a learners apprenticeship from today's date or retrospectively
	So that  I no longer use my Levy funds to pay for the learner

Scenario Outline: PV2-693 - Learners apprenticeship Stop Date is Changed by the Employer
	Given the following employers
		| Identifier       | Legal Entity Name | Agreement Id |
		| Employer A       | Test Employer     | 12345        |

	And the following apprenticeships already exist with Employer Type "<Employer Type>"
		| Identifier       | Start date                   | Stopped On Date             | End date                  |Created On Date             |  Agreed On Date               | Learner   | Provider   | Employer   | Standard Code | Programme Type | Framework Code | Pathway Code | 
		| Apprenticeship A | 01/Aug/Current Academic Year | 25/Dec/Current Academic Year| 01/Aug/Next Academic Year |01/Aug/Current Academic Year|  01/Aug/Current Academic Year | Learner A | Provider A | Employer A | 20            | 25             | 593            | 1            | 
	
	And the existing apprenticeships have the following price episodes
		| Apprenticeship   | Agreed Price | Effective From               |
		| Apprenticeship A | 15000        | 01/Aug/Current Academic Year |

     And the apprenticeship stop date is changed as follows
		| Identifier       | Stopped On Date              | Status  |
		| Apprenticeship A | 01/Jan/Current Academic Year | Stopped |
	
	When the Approvals service notifies the Payments service that the apprenticeships stop date has changed 
	Then the Payments service should record the stopped apprenticeships
Examples:
	| Employer Type |
	| Non-Levy      |
	| Levy          |