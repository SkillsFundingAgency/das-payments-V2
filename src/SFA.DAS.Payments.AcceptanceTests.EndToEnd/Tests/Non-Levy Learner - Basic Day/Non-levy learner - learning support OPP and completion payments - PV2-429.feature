Feature: Non-levy learner - learning support OPP and completion payments - PV2-429
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@mytag
Scenario Outline: Non-levy learner - learning support, OPP and completion payments PV2-429
 # For DC integration
 # learning support code | learning support date from | learning support date to     |
 # 1                     | 06/Aug/Last Academic Year  | 10/Aug/Current Academic Year |
	Given the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Sep/Last Academic Year | 12 months        | 15000                | 06/Sep/Last Academic Year           | 0                      | 06/Sep/Last Academic Year             |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | LearningSupport |
        | Aug/Last Academic Year | 0            | 0          | 0         | 0               |
        | Sep/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Oct/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Nov/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Dec/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Jan/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Feb/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Mar/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Apr/Last Academic Year | 1000         | 0          | 0         | 150             |
        | May/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Jun/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Jul/Last Academic Year | 1000         | 0          | 0         | 150             |
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type |
        | R02/Last Academic Year | Sep/Last Academic Year | 900                    | 100                         | 0                         | Learning         |
        | R03/Last Academic Year | Oct/Last Academic Year | 900                    | 100                         | 0                         | Learning         |
        | R04/Last Academic Year | Nov/Last Academic Year | 900                    | 100                         | 0                         | Learning         |
        | R05/Last Academic Year | Dec/Last Academic Year | 900                    | 100                         | 0                         | Learning         |
        | R06/Last Academic Year | Jan/Last Academic Year | 900                    | 100                         | 0                         | Learning         |
        | R07/Last Academic Year | Feb/Last Academic Year | 900                    | 100                         | 0                         | Learning         |
        | R08/Last Academic Year | Mar/Last Academic Year | 900                    | 100                         | 0                         | Learning         |
        | R09/Last Academic Year | Apr/Last Academic Year | 900                    | 100                         | 0                         | Learning         |
        | R10/Last Academic Year | May/Last Academic Year | 900                    | 100                         | 0                         | Learning         |
        | R11/Last Academic Year | Jun/Last Academic Year | 900                    | 100                         | 0                         | Learning         |
        | R12/Last Academic Year | Jul/Last Academic Year | 900                    | 100                         | 0                         | Learning         |
        | R02/Last Academic Year | Sep/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport  |
        | R03/Last Academic Year | Oct/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport  |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport  |
        | R05/Last Academic Year | Dec/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport  |
        | R06/Last Academic Year | Jan/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport  |
        | R07/Last Academic Year | Feb/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport  |
        | R08/Last Academic Year | Mar/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport  |
        | R09/Last Academic Year | Apr/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport  |
        | R10/Last Academic Year | May/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport  |
        | R11/Last Academic Year | Jun/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport  |
        | R12/Last Academic Year | Jul/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport  |
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Sep/Last Academic Year | 12 months        | 15000                | 06/Sep/Last Academic Year           | 0                      | 06/Sep/Last Academic Year             | 12 months       | completed         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | LearningSupport |
		| Aug/Current Academic Year | 1000         | 0          | 0         | 150             |
		| Sep/Current Academic Year | 0            | 3000       | 0         | 0               |
		| Oct/Current Academic Year | 0            | 0          | 0         | 0               |
		| Nov/Current Academic Year | 0            | 0          | 0         | 0               |
		| Dec/Current Academic Year | 0            | 0          | 0         | 0               |
		| Jan/Current Academic Year | 0            | 0          | 0         | 0               |
		| Feb/Current Academic Year | 0            | 0          | 0         | 0               |
		| Mar/Current Academic Year | 0            | 0          | 0         | 0               |
		| Apr/Current Academic Year | 0            | 0          | 0         | 0               |
		| May/Current Academic Year | 0            | 0          | 0         | 0               |
		| Jun/Current Academic Year | 0            | 0          | 0         | 0               |
		| Jul/Current Academic Year | 0            | 0          | 0         | 0               |
    And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport |
		| R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         | 150             |
		| R02/Current Academic Year | Sep/Current Academic Year | 0            | 3000       | 0         | 0               |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | 0                         | Learning         |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport  |
		| R02/Current Academic Year | Sep/Current Academic Year | 2700                   | 300                         | 0                         | Completion       |
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | 0                         | Learning         |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport  |
		| R02/Current Academic Year | Sep/Current Academic Year | 2700                   | 300                         | 0                         | Completion       |
	Examples:
        | Collection_Period         |
        | R01/Current Academic Year |
        | R02/Current Academic Year |