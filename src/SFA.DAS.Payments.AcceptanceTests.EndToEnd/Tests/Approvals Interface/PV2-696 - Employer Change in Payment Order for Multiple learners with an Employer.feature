Feature: PV2-696 - Employer Change in Payment Order for Multiple learners with an Employer
		As an Employer,
		I want to be able to make changes to the payment order for learners
		So that I can pay my learners in priority order

Scenario: PV2-696 - Employer Change in Payment Order for Multiple learners with an Employer
	Given the following employers
		| Identifier       | Legal Entity Name | Agreement Id |
		| Employer A       | Test Employer     | 12345        |

	And the employers provider priority order is as follows
        | Provider   | Priority |
        | provider a | 1        |
        | provider b | 2        |
        | provider c | 3        |
	And the Approvals service notifies the Payments service of Employer Provider Payment Priority Change
	And employers change provider priority order as follows
        | Provider   | Priority |
        | provider b | 1        |
        | provider c | 2        |
        | provider a | 3        |
	 When the Approvals service notifies the Payments service of changes to Employer Provider Payment Priority
	 Then the Payments service should record the Employer Provider Priority