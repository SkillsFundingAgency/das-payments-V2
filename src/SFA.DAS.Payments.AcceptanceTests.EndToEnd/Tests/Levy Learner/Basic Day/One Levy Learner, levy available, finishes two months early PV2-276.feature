@basic_day
Feature: One Levy learner - levy available, finishes two months early PV2-276
	As a provider,
	I want a levy learner, where levy is available and the learner finishes two months early to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.
Scenario Outline: One levy learner, levy available, learner finishes two months early PV2-276
	# levy balance > agreed price for all months
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
        | start date                | end date                     | agreed price |Framework Code | Pathway Code | Programme Type |
        | 01/Sep/2025-26 Academic Year | 08/Sep/2026-27 Academic Year | 15000        |593            | 1            | 20             |
	And the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                | SFA Contribution Percentage |
		| 01/Sep/2025-26 Academic Year | 12 months        | 15000                | 01/Sep/2025-26 Academic Year        |                        |                                       |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And price details as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 15000                | 01/Sep/2025-26 Academic Year        |                        | 01/Sep/2025-26 Academic Year          | Act1          | 1                   | 90%                         |
	And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
        | Aug/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
        | Sep/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Oct/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Nov/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Dec/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Jan/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Feb/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Mar/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Apr/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
        | May/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Jun/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Jul/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R02/2025-26 Academic Year | Sep/2025-26 Academic Year | 1000          | Learning         |
        | R03/2025-26 Academic Year | Oct/2025-26 Academic Year | 1000          | Learning         |
        | R04/2025-26 Academic Year | Nov/2025-26 Academic Year | 1000          | Learning         |
        | R05/2025-26 Academic Year | Dec/2025-26 Academic Year | 1000          | Learning         |
        | R06/2025-26 Academic Year | Jan/2025-26 Academic Year | 1000          | Learning         |
        | R07/2025-26 Academic Year | Feb/2025-26 Academic Year | 1000          | Learning         |
        | R08/2025-26 Academic Year | Mar/2025-26 Academic Year | 1000          | Learning         |
        | R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 1000          | Learning         |
        | R10/2025-26 Academic Year | May/2025-26 Academic Year | 1000          | Learning         |
        | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 1000          | Learning         |
    But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                | SFA Contribution Percentage |
		| 01/Sep/2025-26 Academic Year | 12 months        | 15000                | 01/Sep/2025-26 Academic Year        |                        |                                       | 10 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 15000                | 01/Sep/2025-26 Academic Year        |                        | 01/Sep/2025-26 Academic Year          | Act1          | 1                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
		| Sep/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Oct/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Nov/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Dec/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jan/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Feb/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Mar/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Apr/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| May/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jun/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jul/2025-26 Academic Year | 0            | 3000       | 2000      | pe-1                     |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 0            | 0          | 2000      |
        | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 0            | 3000       | 0         |
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 2000          | Balancing        |
        | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 3000          | Completion       |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 2000          | Balancing        |
        | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 3000          | Completion       |
Examples: 
        | Collection_Period         | Levy Balance |
        | R12/2025-26 Academic Year | 5500         |