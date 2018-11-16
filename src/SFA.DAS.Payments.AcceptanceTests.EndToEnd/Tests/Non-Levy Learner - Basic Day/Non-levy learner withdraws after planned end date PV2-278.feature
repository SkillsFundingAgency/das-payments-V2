Feature: Non-levy learner provider retrospectively notifies a withdrawal - PV2-278


Scenario Outline: A non-levy learner withdraws after planned end date
   Given the provider previously submitted the following learner details
        | ULN       | Priority | Start Date                  | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type        | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     |
        | learner a | 1        | start of last academic year | 12 months        | 12000                | Aug/Last Academic Year              | 3000                   | Aug/Last Academic Year                |                 | continuing        | 90%                         | ContractWithEmployer | 1                   | ZPROG001      | 403            | 1            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |

    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing |
        | Aug/Last Academic Year | 1000         | 0          | 0         |
        | Sep/Last Academic Year | 1000         | 0          | 0         |
        | Oct/Last Academic Year | 1000         | 0          | 0         |
        | Nov/Last Academic Year | 1000         | 0          | 0         |
        | Dec/Last Academic Year | 1000         | 0          | 0         |
        | Jan/Last Academic Year | 1000         | 0          | 0         |
        | Feb/Last Academic Year | 1000         | 0          | 0         |
        | Mar/Last Academic Year | 1000         | 0          | 0         |
        | Apr/Last Academic Year | 1000         | 0          | 0         |
        | May/Last Academic Year | 1000         | 0          | 0         |
        | Jun/Last Academic Year | 1000         | 0          | 0         |
        | Jul/Last Academic Year | 1000         | 0          | 0         |
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments |
        | R01/Last Academic Year | Aug/Last Academic Year | 900                    | 100                         |
        | R02/Last Academic Year | Sep/Last Academic Year | 900                    | 100                         |
        | R03/Last Academic Year | Oct/Last Academic Year | 900                    | 100                         |
        | R04/Last Academic Year | Nov/Last Academic Year | 900                    | 100                         |
        | R05/Last Academic Year | Dec/Last Academic Year | 900                    | 100                         |
        | R06/Last Academic Year | Jan/Last Academic Year | 900                    | 100                         |
        | R07/Last Academic Year | Feb/Last Academic Year | 900                    | 100                         |
        | R08/Last Academic Year | Mar/Last Academic Year | 900                    | 100                         |
        | R09/Last Academic Year | Apr/Last Academic Year | 900                    | 100                         |
        | R10/Last Academic Year | May/Last Academic Year | 900                    | 100                         |
        | R11/Last Academic Year | Jun/Last Academic Year | 900                    | 100                         |
        | R12/Last Academic Year | Jul/Last Academic Year | 900                    | 100                         |
        

	But the Provider now changes the Learner details as follows
        | ULN       | Priority | Start Date             | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration   | Completion Status   | SFA Contribution Percentage | Contract Type        | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     |
        | learner a | 1        | start of academic year | 12 months        | 12000                | Aug/Current Academic Year           | 3000                   | Aug/Current Academic Year             | <Actual_Duration> | <Completion_Status> | 90%                         | ContractWithEmployer | 1                   | ZPROG001      | 403            | 1            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |
	
			
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
        | Mar/Current Academic Year | 0            | 0          | 0         |
        | Apr/Current Academic Year | 0            | 0          | 0         |
        | May/Current Academic Year | 0            | 0          | 0         |
        | Jun/Current Academic Year | 0            | 0          | 0         |
        | Jul/Current Academic Year | 0            | 0          | 0         |
    And no payments will be calculated
    And no provider payments will be generated

Examples: 
        | Collection_Period         | Delivery_Period           | Completion_Status | Actual_Duration |
        | R01/Current Academic Year | Aug/Current Academic Year | continuing        |                 |
        | R02/Current Academic Year | Sep/Current Academic Year | continuing        |                 |
        | R03/Current Academic Year | Oct/Current Academic Year | withdrawn         | 15 months       |
        | R04/Current Academic Year | Nov/Current Academic Year | withdrawn         | 15 months       |