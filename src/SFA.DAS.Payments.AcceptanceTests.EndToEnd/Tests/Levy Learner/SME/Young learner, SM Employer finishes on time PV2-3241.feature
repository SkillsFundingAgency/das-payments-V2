@basic_day
@supports_dc_e2e
Feature: Young learner - SM Employer finishes on time PV2-3241
	As an SME
    When I employ a young learner (aged under 21) on an apprenticeship who has started after 1st April 2024
    Then I should pay no co-investment contribution toward the apprenticeship training costs

# TODO: Simplify the step definitions to make it easier understand the business goal
#TODO: AS it stands this scenario probably only works when running in 23/24. TODO: refactor to work regardless of current year
Scenario Outline: Young learner, SM Employer finishes on time PV2-3241
	# levy balance = 0 for all months
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	# Commitment line
	And the following commitments exist
        | start date                   | end date                  | agreed price | Standard Code | Programme Type |  Employer Type |
        | 01/Apr/Current Academic Year | 08/Mar/Next Academic Year | 15000        | 50            | 25             |  Levy          |
    And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage | Age At Start |
		| 01/Apr/Current Academic Year | 12 months        | 12000                | 01/Apr/Current Academic Year        | 3000                   | 01/Apr/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 50            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 95%                         | 17           |
	# TODO: Simplify the step definitions to make it easier understand the business goal
	And price details as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 12000                | 01/Apr/Current Academic Year        | 3000                   | 01/Apr/Current Academic Year          | Act1          | 1                   | 95%                         |
	When the ILR file is submitted for the learners for collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Mar/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Apr/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jun/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jul/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
        | R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
        | R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
        | R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |
	# Levy Payments
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R09/Current Academic Year | Apr/Current Academic Year | 1000                   | 0                           | 0             | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 1000                   | 0                           | 0             | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 1000                   | 0                           | 0             | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 1000                   | 0                           | 0             | Learning         |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R09/Current Academic Year | Apr/Current Academic Year | 1000                   | 0                           | 0             | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 1000                   | 0                           | 0             | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 1000                   | 0                           | 0             | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 1000                   | 0                           | 0             | Learning         |
Examples: 
        | Collection_Period         | Levy Balance |
        | R09/Current Academic Year | 0            |
        | R10/Current Academic Year | 0            |
        | R11/Current Academic Year | 0            |
        | R12/Current Academic Year | 0            |