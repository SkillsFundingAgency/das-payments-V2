@igonre
Feature: PV2-690 - Data Lock resolved for a learners with a Commitment in a state of approved and a data-lock recorded against it
	As a Provider/Employer,
	I want to resolve a data-lock associated with one or more of my learners.
	So that I can be paid for my learner (Provider) and can have my Levy funds debited (Employer)

Scenario: PV2-690 - Data Lock resolved for a learners with a Commitment in a state of approved and a data-lock recorded against it
	Given the following employers
		| Identifier       | Legal Entity Name | Agreement Id |
		| Employer A       | Test Employer     | 12345        |

	And the following apprenticeships already exist
		| Identifier       | Created On Date              | Agreed On Date               | Learner   | Provider   | Employer   | Standard Code | Programme Type | Framework Code | Pathway Code | Start date                   | End date                  |
		| Apprenticeship A | 01/Aug/Current Academic Year | 01/Aug/Current Academic Year | Learner A | Provider A | Employer A | 20            | 20             | 593            | 1            | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year |
	
	And the existing apprenticeships have the following price episodes
		| Apprenticeship   | Agreed Price | Effective From               |
		| Apprenticeship A | 15000        | 01/Aug/Current Academic Year |
		| Apprenticeship A | 10000        | 01/Sep/Current Academic Year |

   And the apprenticeships are changed as follows
		| Identifier       | Created On Date              | Agreed On Date               | Learner   | Provider   | Employer   | Standard Code | Programme Type | Framework Code | Pathway Code | Start date                   | End date                  |
		| Apprenticeship A | 01/Aug/Current Academic Year | 02/Aug/Current Academic Year | Learner A | Provider A | Employer A | 17            | 25             | 1              | 2            | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year |
	
	And the changed apprenticeships has the following price episodes
		| Apprenticeship   | Agreed Price | Effective From               | Effective To                 | 
		| Apprenticeship A | 10000        | 01/Sep/Current Academic Year | 30/Sep/Current Academic Year | 
		| Apprenticeship A | 20000        | 01/Oct/Current Academic Year |                              | 


	When the Approvals service notifies the Payments service of the apprenticeships datalock triage changes
	Then the Payments service should record the apprenticeships