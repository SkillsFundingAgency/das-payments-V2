Feature: Audit Funding Source Events
	In order to trace provider payments
	As a payments DevOps
	I want to the Payments V2 service to store all Funding Source Payment events


Scenario: CoFunded Funding Source Events Calculated
	Given the funding source service has calculated the following payments
	| Funding Source     | Amount | Transaction Type | Contarct Type |
	| CoInvestedSfa      | 90     | Learning         | Act2          |
	| CoInvestedEmployer | 10     | Learning         | Act2          |
	When the Audit Funding Source Service is notified of the calculated funding source payments
	Then the calculated funding source payments should be recorded in the funding source tables
