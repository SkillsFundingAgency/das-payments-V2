Feature: PV2-689 - Update Existing Apprenticeship
	As an Employer
	I want the Payments Calc Data-Locks process to be made aware f updated commitments
	So that they can be matched to the ILR Learner and allow payments to be made for the learner

Scenario: PV2-689 - Update Existing Apprenticeship
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

   And the apprenticeships are changed has follows
		| Identifier       | Created On Date              | Agreed On Date               | Learner   | Provider   | Employer   | Standard Code | Programme Type | Framework Code | Pathway Code | Start date                   | End date                  |
		| Apprenticeship A | 01/Aug/Current Academic Year | 02/Aug/Current Academic Year | Learner A | Provider A | Employer A | 17            | 25             | 1              | 2            | 01/Aug/Current Academic Year | 01/Sep/Next Academic Year |
	
	And the changed apprenticeships has the following price episodes
		| Apprenticeship   | Agreed Price | Effective From               | Effective To                 | 
		| Apprenticeship A | 10000        | 01/Sep/Current Academic Year | 30/Sep/Current Academic Year | 
		| Apprenticeship A | 20000        | 01/Oct/Current Academic Year |                              | 


	When the Approvals service notifies the Payments service of the apprenticeships changes
	Then the Payments service should record the apprenticeships