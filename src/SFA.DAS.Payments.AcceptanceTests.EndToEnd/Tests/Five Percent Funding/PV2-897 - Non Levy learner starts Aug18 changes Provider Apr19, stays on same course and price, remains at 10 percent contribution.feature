Feature: 5% Contribution from April 2019 PV2-897

Scenario Outline: Non Levy Learner, started learning before Apr19, changes Provider from Apr19 stays on the same course, remains on 10% contribution

Given "provider a" previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assesment Price | Total Assesment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Aug/Current Academic Year | 12 months        | 12000                | 06/Aug/Current Academic Year        | 3000                  | 06/Aug/Current Academic Year         | 8 months        | withdrawan        | Act2          | 1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
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
		| Apr/Current Academic Year | 0            | 0          | 0         |
		| May/Current Academic Year | 0            | 0          | 0         |
		| Jun/Current Academic Year | 0            | 0          | 0         |
		| Jul/Current Academic Year | 0            | 0          | 0         |
	And the following payments had been generated for "provider a"
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                          | Learning         |
		| R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                          | Learning         |
		| R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                          | Learning         |
		| R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                          | Learning         |
		| R05/Current Academic Year | Dec/Current Academic Year | 900                    | 100                          | Learning         |
		| R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                          | Learning         |
		| R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                          | Learning         |
		| R08/Current Academic Year | Mar/Current Academic Year | 900                    | 100                          | Learning         |
	But the Learner has now changed to "provider b" as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assesment Price | Total Assesment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 05/Apr/Current Academic Year | 4 months         | 3200                 | 05/Apr/Current Academic Year        | 800                     | 05/Apr/Current Academic Year         |                 | continuing        | Act2          | 1                   | ZPROG001      | 15            | 25            | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
		| pe-1             | 3200                 | 05/Apr/Current Academic Year        | 800                    | 05/Apr/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act2          |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
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
		| Apr/Current Academic Year | 800          | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 800          | 0          | 0         | pe-1                     |
		| Jun/Current Academic Year | 800          | 0          | 0         | pe-1                     |
		| Jul/Current Academic Year | 800          | 0          | 0         | pe-1                     |
	And only the following payments will be calculated
		| Provider   | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| provider b | R09/Current Academic Year | Apr/Current Academic Year | 800          | 0          | 0         |
		| provider b | R10/Current Academic Year | May/Current Academic Year | 800          | 0          | 0         |
		| provider b | R11/Current Academic Year | Jun/Current Academic Year | 800          | 0          | 0         |
		| provider b | R12/Current Academic Year | Jul/Current Academic Year | 800          | 0          | 0         |
	And only the following provider payments will be recorded
		| Provider   | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| provider b | R09/Current Academic Year | Apr/Current Academic Year | 720                    | 80                          | Learning         |
		| provider b | R10/Current Academic Year | May/Current Academic Year | 720                    | 80                          | Learning         |
		| provider b | R11/Current Academic Year | Jun/Current Academic Year | 720                    | 80                          | Learning         |
		| provider b | R12/Current Academic Year | Jul/Current Academic Year | 720                    | 80                          | Learning         |
	And at month end only the following provider payments will be generated
		| Provider   | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| provider b | R09/Current Academic Year | Apr/Current Academic Year | 720                    | 80                          | Learning         |
		| provider b | R10/Current Academic Year | May/Current Academic Year | 720                    | 80                          | Learning         |
		| provider b | R11/Current Academic Year | Jun/Current Academic Year | 720                    | 80                          | Learning         |
		| provider b | R12/Current Academic Year | Jul/Current Academic Year | 720                    | 80                          | Learning         |
		
	Examples:
		| Collection_Period         |
		| R09/Current Academic Year |
		| R10/Current Academic Year |
		| R11/Current Academic Year |
		| R12/Current Academic Year |