Feature: One Non-Levy Learner Withdraws After Qualifying Period PV2-277

@EndToEnd

Scenario Outline: A non-levy learner withdraws after qualifying period
	Given the provider is providing trainging for the following learners
		| LearnerId | Priority | Start Date             | Planned Duration | Total Training Price | Total Assesment Price | Actual Duration | Programme Type | Completion Status | SFA Contribution Percentage |
		| learner a | 1        | start of academic year | 12 months        | 12000                | 3000                  | 4 months        | 25             | withdrawn         | 90%                         |
    When the ILR file is submitted for the learners for collection period <collection_period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 1000         | 0          | 0         |
		| Sep/Current Academic Year | 1000         | 0          | 0         |
		| Oct/Current Academic Year | 1000         | 0          | 0         |
		| Nov/Current Academic Year | 1000         | 0          | 0         |
		| Dec/Current Academic Year | 0            | 0          | 0         |
		| Jan/Current Academic Year | 0            | 0          | 0         |
		| Feb/Current Academic Year | 0            | 0          | 0         |
		| Mar/Current Academic Year | 0            | 0          | 0         |
		| Apr/Current Academic Year | 0            | 0          | 0         |
		| May/Current Academic Year | 0            | 0          | 0         |
		| Jun/Current Academic Year | 0            | 0          | 0         |
		| Jul/Current Academic Year | 0            | 0          | 0         |
    And the following payments will be calculated
		| Collection Period			| Delivery Period           | On-Programme | Completion | Balancing |
		| R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
		| R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
		| R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
		| R04/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         |
	And at month end the following provider payments will be generated
		| Collection Period			| Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments |
		| R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         |
		| R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         |
		| R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         |
		| R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         |

	Examples:
		| collection_period			|
		| R01/Current Academic Year |
		| R02/Current Academic Year |
		| R03/Current Academic Year |
		| R04/Current Academic Year |