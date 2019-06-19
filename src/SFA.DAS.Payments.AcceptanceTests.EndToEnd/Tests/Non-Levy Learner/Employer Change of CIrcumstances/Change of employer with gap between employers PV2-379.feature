#@supports_dc_e2e
Feature: PV2-379 Non-levy learner changes employer and there is a gap - provider receives payment during the gap as they amend the ACT code and employment status code correctly
 # And the employment status in the ILR is
 #       | Employer   | Employment Status      | Employment Status Applies    | Small Employer |
 #       | employer 1 | in paid employment     | 03/Aug/Current Academic Year |                |
 #       |            | not in paid employment | 03/Oct/Current Academic Year |                |
 #       | employer 2 | in paid employment     | 03/Nov/Current Academic Year |                |

# SFA Contribution Percentage is moved to earnings table
# "price details as follows" has additional residual fields

Scenario Outline: Non-levy learner changes employer and there is a gap - provider receives payment during the gap PV2-379
	Given the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     |
		| 03/Aug/Current Academic Year | 12 months        | 15000                | 03/Aug/Current Academic Year        | 0                      | 03/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |
	# additional residual fields
	And price details as follows
		| Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| 1st price details | 15000                | 03/Aug/Current Academic Year        | 0                      | 03/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | Act2          | 1                   | 90%                         |
		| 2nd price details | 15000                | 03/Oct/Current Academic Year        | 0                      | 03/Oct/Current Academic Year          | 5625                    | 03/Oct/Current Academic Year           | 0                         | 03/Oct/Current Academic Year             | Act2          | 1                   | 100%                        |
		| 3rd price details | 15000                | 03/Nov/Current Academic Year        | 0                      | 03/Nov/Current Academic Year          | 5625                    | 03/Nov/Current Academic Year           | 0                         | 03/Nov/Current Academic Year             | Act2          | 1                   | 90%                         |
	# SFA Contribution Percentage is moved to earnings table
	When the ILR file is submitted for the learners for collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | SFA Contribution Percentage | Price Episode Identifier |
		# employer 1																					   
		| Aug/Current Academic Year | 1000         | 0          | 0         | 90%                         | 1st price details        |
		| Sep/Current Academic Year | 1000         | 0          | 0         | 90%                         | 1st price details        |
		# not in paid employment
		| Oct/Current Academic Year | 1000         | 0          | 0         | 100%                        | 2nd price details        |
		# employer 2
		| Nov/Current Academic Year | 500          | 0          | 0         | 90%                         | 3rd price details        |
		| Dec/Current Academic Year | 500          | 0          | 0         | 90%                         | 3rd price details        |
		| Jan/Current Academic Year | 500          | 0          | 0         | 90%                         | 3rd price details        |
		| Feb/Current Academic Year | 500          | 0          | 0         | 90%                         | 3rd price details        |
		| Mar/Current Academic Year | 500          | 0          | 0         | 90%                         | 3rd price details        |
		| Apr/Current Academic Year | 500          | 0          | 0         | 90%                         | 3rd price details        |
		| May/Current Academic Year | 500          | 0          | 0         | 90%                         | 3rd price details        |
		| Jun/Current Academic Year | 500          | 0          | 0         | 90%                         | 3rd price details        |
		| Jul/Current Academic Year | 500          | 0          | 0         | 90%                         | 3rd price details        |
	And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         | 1st price details        |
		| R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         | 1st price details        |
		| R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         | 2nd price details        |
		| R04/Current Academic Year | Nov/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
		| R05/Current Academic Year | Dec/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
		| R06/Current Academic Year | Jan/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
		| R07/Current Academic Year | Feb/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
		| R08/Current Academic Year | Mar/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
		| R09/Current Academic Year | Apr/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
		| R10/Current Academic Year | May/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
		| R11/Current Academic Year | Jun/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
		| R12/Current Academic Year | Jul/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
	And only the following provider payments will be recorded											 
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Employer    | Price Episode Identifier |
		| R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         | employer 1  | 1st price details        |
		| R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         | employer 1  | 1st price details        |
		# 100%
		| R03/Current Academic Year | Oct/Current Academic Year | 1000                   | 0                           | Learning         | no employer | 2nd price details        |
		| R04/Current Academic Year | Nov/Current Academic Year | 450                    | 50                          | Learning         | employer 2  | 2nd price details        |
		| R05/Current Academic Year | Dec/Current Academic Year | 450                    | 50                          | Learning         | employer 2  | 2nd price details        |
		| R06/Current Academic Year | Jan/Current Academic Year | 450                    | 50                          | Learning         | employer 2  | 2nd price details        |
		| R07/Current Academic Year | Feb/Current Academic Year | 450                    | 50                          | Learning         | employer 2  | 2nd price details        |
		| R08/Current Academic Year | Mar/Current Academic Year | 450                    | 50                          | Learning         | employer 2  | 2nd price details        |
		| R09/Current Academic Year | Apr/Current Academic Year | 450                    | 50                          | Learning         | employer 2  | 2nd price details        |
		| R10/Current Academic Year | May/Current Academic Year | 450                    | 50                          | Learning         | employer 2  | 2nd price details        |
		| R11/Current Academic Year | Jun/Current Academic Year | 450                    | 50                          | Learning         | employer 2  | 2nd price details        |
		| R12/Current Academic Year | Jul/Current Academic Year | 450                    | 50                          | Learning         | employer 2  | 2nd price details        |
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Employer    |
		| R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         | employer 1  |
		| R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         | employer 1  |
		| R03/Current Academic Year | Oct/Current Academic Year | 1000                   | 0                           | Learning         | no employer |
		| R04/Current Academic Year | Nov/Current Academic Year | 450                    | 50                          | Learning         | employer 2  |
		| R05/Current Academic Year | Dec/Current Academic Year | 450                    | 50                          | Learning         | employer 2  |
		| R06/Current Academic Year | Jan/Current Academic Year | 450                    | 50                          | Learning         | employer 2  |
		| R07/Current Academic Year | Feb/Current Academic Year | 450                    | 50                          | Learning         | employer 2  |
		| R08/Current Academic Year | Mar/Current Academic Year | 450                    | 50                          | Learning         | employer 2  |
		| R09/Current Academic Year | Apr/Current Academic Year | 450                    | 50                          | Learning         | employer 2  |
		| R10/Current Academic Year | May/Current Academic Year | 450                    | 50                          | Learning         | employer 2  |
		| R11/Current Academic Year | Jun/Current Academic Year | 450                    | 50                          | Learning         | employer 2  |
		| R12/Current Academic Year | Jul/Current Academic Year | 450                    | 50                          | Learning         | employer 2  |
	Examples:
		| Collection_Period         |
		| R01/Current Academic Year |
		| R02/Current Academic Year |
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