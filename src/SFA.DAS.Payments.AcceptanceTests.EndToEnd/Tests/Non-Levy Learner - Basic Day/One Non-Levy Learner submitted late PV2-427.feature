Feature: One Non-Levy Learner submitted late PV2-427
	As a provider,
	I want to ensure that when a non-levy learner is submitted for the first time in R13/R14 that payments are calculated correctly.
	So that I am accurately paid my apprenticeship provision.

Scenario: ILR submitted first time in R13 or R14 for a non-DAS learner ILR PV2-427
    Given the provider is providing training for the following learners
        | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                 | SFA Contribution Percentage |
        | 01/Sep/Current Academic Year | 12 months        | 15000                | 01/Sep/Current Academic Year        | 0                      | 01/Sep/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship Non-Levy Contract (procured)   | 90%                         |
	When the ILR file is submitted for the learners for collection period R13/Current Academic Year
	Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 0            | 0          | 0         |
        | Sep/Current Academic Year | 1000         | 0          | 0         |
        | Oct/Current Academic Year | 1000         | 0          | 0         |
        | Nov/Current Academic Year | 1000         | 0          | 0         |
        | Dec/Current Academic Year | 1000         | 0          | 0         |
        | Jan/Current Academic Year | 1000         | 0          | 0         |
        | Feb/Current Academic Year | 1000         | 0          | 0         |
        | Mar/Current Academic Year | 1000         | 0          | 0         |
        | Apr/Current Academic Year | 1000         | 0          | 0         |
        | May/Current Academic Year | 1000         | 0          | 0         |
        | Jun/Current Academic Year | 1000         | 0          | 0         |
        | Jul/Current Academic Year | 1000         | 0          | 0         |
    And only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R13/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
        | R13/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
        | R13/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         |
        | R13/Current Academic Year | Dec/Current Academic Year | 1000         | 0          | 0         |
        | R13/Current Academic Year | Jan/Current Academic Year | 1000         | 0          | 0         |
        | R13/Current Academic Year | Feb/Current Academic Year | 1000         | 0          | 0         |
        | R13/Current Academic Year | Mar/Current Academic Year | 1000         | 0          | 0         |
        | R13/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
        | R13/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
        | R13/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
        | R13/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R13/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Dec/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | May/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | Learning         |
	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R13/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Dec/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | May/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | Learning         |
        | R13/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | Learning         |
