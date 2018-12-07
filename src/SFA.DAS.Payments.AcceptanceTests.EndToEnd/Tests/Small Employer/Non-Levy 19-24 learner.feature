Feature: Non-Levy 19-24 learner
	As a provider,
	I want a non-levy learner, 1 learner aged 19-24, employed with a small employer at start, is co-funded for on programme and completion payments (this apprentice does not have a Education Health Care plan and is not a care leaver), to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.

Scenario: Non-levy learner 19-24 not a care leaver or with EHC plan employed with a small employer is co-funded PV2-330
#AC4- 1 learner aged 19-24, employed with a small employer at start, is co-funded for on programme and completion payments (this apprentice does not have a Education Health Care plan and is not a care leaver)
# And the employment status in the ILR is
#       | Employer   | Employment Status  | Employment Status Applies | Small Employer |
#       | employer 1 | in paid employment | 06/Aug/Last Academic Year | SEM1           |
	Given the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 7500                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             |                 | continuing        | Act2          | 1                   | ZPROG001      | 403            | 1            | 25             | 19-24 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing |
        | Aug/Last Academic Year | 500          | 0          | 0         |
        | Sep/Last Academic Year | 500          | 0          | 0         |
        | Oct/Last Academic Year | 500          | 0          | 0         |
        | Nov/Last Academic Year | 500          | 0          | 0         |
        | Dec/Last Academic Year | 500          | 0          | 0         |
        | Jan/Last Academic Year | 500          | 0          | 0         |
        | Feb/Last Academic Year | 500          | 0          | 0         |
        | Mar/Last Academic Year | 500          | 0          | 0         |
        | Apr/Last Academic Year | 500          | 0          | 0         |
        | May/Last Academic Year | 500          | 0          | 0         |
        | Jun/Last Academic Year | 500          | 0          | 0         |
        | Jul/Last Academic Year | 500          | 0          | 0         |
	# 90% funding even though is a small employer - 19-24 non-levy learner not a care leaver or with EHC plan 
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R01/Last Academic Year | Jul/Last Academic Year | 450                    | 50                          | Learning         |
        | R02/Last Academic Year | Aug/Last Academic Year | 450                    | 50                          | Learning         |
        | R03/Last Academic Year | Sep/Last Academic Year | 450                    | 50                          | Learning         |
        | R04/Last Academic Year | Oct/Last Academic Year | 450                    | 50                          | Learning         |
        | R05/Last Academic Year | Nov/Last Academic Year | 450                    | 50                          | Learning         |
        | R06/Last Academic Year | Dec/Last Academic Year | 450                    | 50                          | Learning         |
        | R07/Last Academic Year | Jan/Last Academic Year | 450                    | 50                          | Learning         |
        | R08/Last Academic Year | Feb/Last Academic Year | 450                    | 50                          | Learning         |
        | R09/Last Academic Year | Mar/Last Academic Year | 450                    | 50                          | Learning         |
        | R10/Last Academic Year | Apr/Last Academic Year | 450                    | 50                          | Learning         |
        | R11/Last Academic Year | May/Last Academic Year | 450                    | 50                          | Learning         |
        | R12/Last Academic Year | Jun/Last Academic Year | 450                    | 50                          | Learning         |
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 7500                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | 12 months       | completed         | Act2          | 1                   | ZPROG001      | 403            | 1            | 25             | 19-24 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period R01/Current Academic Year
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 0            | 1500       | 0         |
		| Sep/Current Academic Year | 0            | 0          | 0         |
		| Oct/Current Academic Year | 0            | 0          | 0         |
		| Nov/Current Academic Year | 0            | 0          | 0         |
		| Dec/Current Academic Year | 0            | 0          | 0         |
		| Jan/Current Academic Year | 0            | 0          | 0         |
		| Feb/Current Academic Year | 0            | 0          | 0         |
		| Mar/Current Academic Year | 0            | 0          | 0         |
		| Apr/Current Academic Year | 0            | 0          | 0         |
		| May/Current Academic Year | 0            | 0          | 0         |
		| Jun/Current Academic Year | 0            | 0          | 0         |
		| Jul/Current Academic Year | 0            | 0          | 0         |
    And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R01/Current Academic Year | Aug/Current Academic Year | 0            | 1500       | 0         |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 1350                   | 150                         | Completion       |
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 1350                   | 150                         | Completion       |
