Feature: Remain with same Employer - PV2-325
	As a provider,
	I want a non-levy learner, that changes provider but remains with the same employer, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.

Scenario Outline: Non-Levy learner changes provider but remains with the same employer PV2-325
	# Restart indicator will be required for DC
	Given "provider a" previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assesment Price | Total Assesment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Aug/Current Academic Year | 12 months        | 7500                 | 06/Aug/Current Academic Year        | 0                     | 06/Aug/Current Academic Year         | 7 months        | withdrawan        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
	And the following earnings had been generated for the learner
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 500          | 0          | 0         |
		| Sep/Current Academic Year | 500          | 0          | 0         |
		| Oct/Current Academic Year | 500          | 0          | 0         |
		| Nov/Current Academic Year | 500          | 0          | 0         |
		| Dec/Current Academic Year | 500          | 0          | 0         |
		| Jan/Current Academic Year | 500          | 0          | 0         |
		| Feb/Current Academic Year | 500          | 0          | 0         |
		| Mar/Current Academic Year | 0            | 0          | 0         |
		| Apr/Current Academic Year | 0            | 0          | 0         |
		| May/Current Academic Year | 0            | 0          | 0         |
		| Jun/Current Academic Year | 0            | 0          | 0         |
		| Jul/Current Academic Year | 0            | 0          | 0         |
	And the following payments had been generated for "provider a"
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 450                    | 50                          | Learning         |
		| R02/Current Academic Year | Sep/Current Academic Year | 450                    | 50                          | Learning         |
		| R03/Current Academic Year | Oct/Current Academic Year | 450                    | 50                          | Learning         |
		| R04/Current Academic Year | Nov/Current Academic Year | 450                    | 50                          | Learning         |
		| R05/Current Academic Year | Dec/Current Academic Year | 450                    | 50                          | Learning         |
		| R06/Current Academic Year | Jan/Current Academic Year | 450                    | 50                          | Learning         |
		| R07/Current Academic Year | Feb/Current Academic Year | 450                    | 50                          | Learning         |
	But the Learner has now changed to "provider b" as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assesment Price | Total Assesment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 05/Mar/Current Academic Year | 5 months         | 3500                 | 05/Mar/Current Academic Year        | 0                     | 05/Mar/Current Academic Year         |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 0            | 0          | 0         |
		| Sep/Current Academic Year | 0            | 0          | 0         |
		| Oct/Current Academic Year | 0            | 0          | 0         |
		| Nov/Current Academic Year | 0            | 0          | 0         |
		| Dec/Current Academic Year | 0            | 0          | 0         |
		| Jan/Current Academic Year | 0            | 0          | 0         |
		| Feb/Current Academic Year | 0            | 0          | 0         |
		| Mar/Current Academic Year | 720          | 0          | 0         |
		| Apr/Current Academic Year | 720          | 0          | 0         |
		| May/Current Academic Year | 720          | 0          | 0         |
		| Jun/Current Academic Year | 720          | 0          | 0         |
		| Jul/Current Academic Year | 720          | 0          | 0         |
	And only the following payments will be calculated
		| Provider   | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| provider b | R08/Current Academic Year | Mar/Current Academic Year | 720          | 0          | 0         |
		| provider b | R09/Current Academic Year | Apr/Current Academic Year | 720          | 0          | 0         |
		| provider b | R10/Current Academic Year | May/Current Academic Year | 720          | 0          | 0         |
		| provider b | R11/Current Academic Year | Jun/Current Academic Year | 720          | 0          | 0         |
		| provider b | R12/Current Academic Year | Jul/Current Academic Year | 720          | 0          | 0         |
	And only the following provider payments will be recorded
		| Provider   | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| provider b | R08/Current Academic Year | Mar/Current Academic Year | 648                    | 72                          | Learning         |
		| provider b | R09/Current Academic Year | Apr/Current Academic Year | 648                    | 72                          | Learning         |
		| provider b | R10/Current Academic Year | May/Current Academic Year | 648                    | 72                          | Learning         |
		| provider b | R11/Current Academic Year | Jun/Current Academic Year | 648                    | 72                          | Learning         |
		| provider b | R12/Current Academic Year | Jul/Current Academic Year | 648                    | 72                          | Learning         |
	And at month end only the following provider payments will be generated
		| Provider   | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| provider b | R08/Current Academic Year | Mar/Current Academic Year | 648                    | 72                          | Learning         |
		| provider b | R09/Current Academic Year | Apr/Current Academic Year | 648                    | 72                          | Learning         |
		| provider b | R10/Current Academic Year | May/Current Academic Year | 648                    | 72                          | Learning         |
		| provider b | R11/Current Academic Year | Jun/Current Academic Year | 648                    | 72                          | Learning         |
		| provider b | R12/Current Academic Year | Jul/Current Academic Year | 648                    | 72                          | Learning         |
	Examples:
		| Collection_Period         |
		| R08/Current Academic Year |
		| R09/Current Academic Year |
		| R10/Current Academic Year |
		| R11/Current Academic Year |
		| R12/Current Academic Year |