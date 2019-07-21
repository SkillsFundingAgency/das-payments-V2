
Feature: PV2-695 - Learners Apprenticeship Is Resumed After a Pause
	As an Employer,
	I want to be able to resume a learners apprenticeship if they are currently in a state of Pause
	So that I can resume paying for the learner from my Levy funds

Scenario: PV2-695 - Learners Apprenticeship Is Resumed After a Pause
	Given the following employers
		| Identifier       | Legal Entity Name | Agreement Id |
		| Employer A       | Test Employer     | 12345        |

	And the following apprenticeships already exist
		| Identifier       | Start date                   | Status | End date                  | Created On Date              | Agreed On Date               | Learner   | Provider   | Employer   | Standard Code | Programme Type | Framework Code | Pathway Code |
		| Apprenticeship A | 01/Aug/Current Academic Year | paused | 01/Aug/Next Academic Year | 01/Aug/Current Academic Year | 01/Aug/Current Academic Year | Learner A | Provider A | Employer A | 20            | 25             | 593            | 1            |
	
	And the existing apprenticeships have the following price episodes
		| Apprenticeship   | Agreed Price | Effective From               |
		| Apprenticeship A | 15000        | 01/Aug/Current Academic Year |

	And the existing apprenticeships has the following pause history
		| Apprenticeship   | Paused On Date               |
		| Apprenticeship A | 01/Dec/Current Academic Year |

     And the apprenticeship resumed date is changed as follows
		| Identifier       | Resumed On Date              | Status | 
		| Apprenticeship A | 01/Jan/Current Academic Year | Active | 
	
	When the Approvals service notifies the Payments service that the apprenticeships has been resumed
	Then the Payments service should record the resumed apprenticeships
	And the Payments service should record the paused apprenticeships history