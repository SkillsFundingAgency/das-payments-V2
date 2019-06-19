﻿@supports_dc_e2e
Feature: Learner Change of Circumstances - Change to start date - PV2-282
	As a provider,
	I want a non-levy learner, where change to start date within calendar month, forward in month is paid the correct amount
	So that I am accurately paid my apprenticeship provision.

Scenario Outline: Change to start date within calendar month, forward in month PV2-282
	Given the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                               | SFA Contribution Percentage |
		| 05/Aug/Current Academic Year | 12 months        | 9000                 | 05/Aug/Current Academic Year        |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship Non-Levy Contract (procured) | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 600          | 0          | 0         |
        | Sep/Current Academic Year | 600          | 0          | 0         |
        | Oct/Current Academic Year | 600          | 0          | 0         |
        | Nov/Current Academic Year | 600          | 0          | 0         |
        | Dec/Current Academic Year | 600          | 0          | 0         |
        | Jan/Current Academic Year | 600          | 0          | 0         |
        | Feb/Current Academic Year | 600          | 0          | 0         |
        | Mar/Current Academic Year | 600          | 0          | 0         |
        | Apr/Current Academic Year | 600          | 0          | 0         |
        | May/Current Academic Year | 600          | 0          | 0         |
        | Jun/Current Academic Year | 600          | 0          | 0         |
        | Jul/Current Academic Year | 600          | 0          | 0         |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 540                    | 60                          | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 540                    | 60                          | Learning         |
    But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                               | SFA Contribution Percentage |
		| 06/Aug/Current Academic Year | 12 months        | 9000                 | 06/Aug/Current Academic Year        |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship Non-Levy Contract (procured) | 90%                         |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type | Aim Sequence Number |
		| pe-1             | 9000                 | 06/Aug/Current Academic Year        | 0                      | 01/Sep/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act2          | 1                   |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| Mar/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| Apr/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| Jun/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| Jul/Current Academic Year | 600          | 0          | 0         | pe-1                     |
    And only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R03/Current Academic Year | Oct/Current Academic Year | 600          | 0          | 0         |
        | R04/Current Academic Year | Nov/Current Academic Year | 600          | 0          | 0         |
        | R05/Current Academic Year | Dec/Current Academic Year | 600          | 0          | 0         |
        | R06/Current Academic Year | Jan/Current Academic Year | 600          | 0          | 0         |
        | R07/Current Academic Year | Feb/Current Academic Year | 600          | 0          | 0         |
        | R08/Current Academic Year | Mar/Current Academic Year | 600          | 0          | 0         |
        | R09/Current Academic Year | Apr/Current Academic Year | 600          | 0          | 0         |
        | R10/Current Academic Year | May/Current Academic Year | 600          | 0          | 0         |
        | R11/Current Academic Year | Jun/Current Academic Year | 600          | 0          | 0         |
        | R12/Current Academic Year | Jul/Current Academic Year | 600          | 0          | 0         |
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R03/Current Academic Year | Oct/Current Academic Year | 540                    | 60                          | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 540                    | 60                          | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 540                    | 60                          | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 540                    | 60                          | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 540                    | 60                          | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 540                    | 60                          | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 540                    | 60                          | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 540                    | 60                          | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 540                    | 60                          | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 540                    | 60                          | Learning         |
	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R03/Current Academic Year | Oct/Current Academic Year | 540                    | 60                          | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 540                    | 60                          | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 540                    | 60                          | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 540                    | 60                          | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 540                    | 60                          | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 540                    | 60                          | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 540                    | 60                          | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 540                    | 60                          | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 540                    | 60                          | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 540                    | 60                          | Learning         |
Examples: 
        | Collection_Period         |
        | R03/Current Academic Year |
        | R04/Current Academic Year |
        | R05/Current Academic Year |
        | R06/Current Academic Year |
        | R07/Current Academic Year |
        | R08/Current Academic Year |
        | R09/Current Academic Year |
        | R10/Current Academic Year |
        | R11/Current Academic Year |
        | R12/Current Academic Year |
