Feature: ACT 2 On-Programme Payments
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

Scenario: Learner Payments
	Given the learner has some on-programme earnings
	When the earnings are sent to the required payments service
	Then the service should generate the required payments