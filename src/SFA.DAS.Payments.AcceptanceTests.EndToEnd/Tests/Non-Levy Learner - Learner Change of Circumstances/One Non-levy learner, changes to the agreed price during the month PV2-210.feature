@supports_dc_e2e
Feature: Non-Levy learner, and there is a change to the agreed price which happens in the middle of the month PV2-210
As a provider,

I want a non-levy learner, changes to the agreed price during the month, to be paid the correct amount

So that I am accurately paid my apprenticeship provision.
Scenario Outline: Non-levy learner changes to the agreed price during the month PV2-210
	Given the following learners
		| Learner Reference Number |
		| abc123                   |
	And the following aims
		| Aim Type  | Start Date                   | Planned Duration | Actual Duration | Completion Status | Contract Type | Aim Reference | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type                               |
		| Programme | 01/Aug/Current Academic Year | 12 months        |                 | continuing        | Act2          | ZPROG001      | 1                   | 593            | 1            | 20             | 19+ Apprenticeship Non-Levy Contract (procured) |
  	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 15000                | 01/Aug/Current Academic Year        |                        |                                       | Act2          | 1                   | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 1000         | 0          | 0         |
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
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         |
	But aims details are changed as follows
		| Aim Type  | Start Date                   | Planned Duration | Actual Duration | Completion Status | Contract Type | Aim Reference | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type                               |
		| Programme | 01/Aug/Current Academic Year | 12 months        |                 | continuing        | Act2          | ZPROG001      | 1                   | 593            | 1            | 20             | 19+ Apprenticeship Non-Levy Contract (procured) | 
	And price details as follows
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | SFA Contribution Percentage | Contract Type | Aim Sequence Number |
        | pe-1             | 15000                | 01/Aug/Current Academic Year        |                        |                                       | 90%                         | Act2          | 1                   |
        | pe-2             | 9375                 | 10/Nov/Current Academic Year        |                        |                                       | 90%                         | Act2          | 1                   |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
        | Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Sep/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Oct/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Nov/Current Academic Year | 500          | 0          | 0         | pe-2                     |
        | Dec/Current Academic Year | 500          | 0          | 0         | pe-2                     |
        | Jan/Current Academic Year | 500          | 0          | 0         | pe-2                     |
        | Feb/Current Academic Year | 500          | 0          | 0         | pe-2                     |
        | Mar/Current Academic Year | 500          | 0          | 0         | pe-2                     |
        | Apr/Current Academic Year | 500          | 0          | 0         | pe-2                     |
        | May/Current Academic Year | 500          | 0          | 0         | pe-2                     |
        | Jun/Current Academic Year | 500          | 0          | 0         | pe-2                     |
        | Jul/Current Academic Year | 500          | 0          | 0         | pe-2                     |
    And only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R04/Current Academic Year | Nov/Current Academic Year | 500          | 0          | 0         |
        | R05/Current Academic Year | Dec/Current Academic Year | 500          | 0          | 0         |
        | R06/Current Academic Year | Jan/Current Academic Year | 500          | 0          | 0         |
        | R07/Current Academic Year | Feb/Current Academic Year | 500          | 0          | 0         |
        | R08/Current Academic Year | Mar/Current Academic Year | 500          | 0          | 0         |
        | R09/Current Academic Year | Apr/Current Academic Year | 500          | 0          | 0         |
        | R10/Current Academic Year | May/Current Academic Year | 500          | 0          | 0         |
        | R11/Current Academic Year | Jun/Current Academic Year | 500          | 0          | 0         |
        | R12/Current Academic Year | Jul/Current Academic Year | 500          | 0          | 0         |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R04/Current Academic Year | Nov/Current Academic Year | 450                    | 50                          | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 450                    | 50                          | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 450                    | 50                          | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 450                    | 50                          | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 450                    | 50                          | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 450                    | 50                          | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 450                    | 50                          | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 450                    | 50                          | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 450                    | 50                          | Learning         |
	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R04/Current Academic Year | Nov/Current Academic Year | 450                    | 50                          | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 450                    | 50                          | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 450                    | 50                          | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 450                    | 50                          | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 450                    | 50                          | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 450                    | 50                          | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 450                    | 50                          | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 450                    | 50                          | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 450                    | 50                          | Learning         |
Examples: 
        | Collection_Period         |
        | R04/Current Academic Year |
        | R05/Current Academic Year |
        | R06/Current Academic Year |
        | R07/Current Academic Year |
        | R08/Current Academic Year |
        | R09/Current Academic Year |
        | R10/Current Academic Year |
        | R11/Current Academic Year |
        | R12/Current Academic Year |