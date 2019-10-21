Feature:  16-18 Non-Levy apprentice adds SEM flag in 4 month PV2-1032
	As a provider,
	I want a 16-18 Non levy learner, where the small employer flag is added retrospectively in the ILR, and previous on-programme payments are refunded and repaid according to the latest small employer status
	So that I am accurately paid the apprenticeship amount by SFA
#Where a Small Employer flag SEM1 is added to an ILR then the learning is 100% SFA funded, so when an SEM1 flag is added retrospectively by the Provider to the ILR, 
#then a refund is applied for any previous payments and all payments are 100% SFA funded. 

Scenario Outline: 16-18 Non-Levy apprentice, provider retrospectively adds small employer flag in the ILR, previous on-programme payments are refunded and repaid according to latest small employer status
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
        |Framework Code | Pathway Code | Programme Type | agreed price | start date                | end date                     |
        |403            | 1            | 2             | 7500        | 06/Aug/Current Academic Year | 08/Aug/Next Academic Year |
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Current Academic Year | 12 months        | 7500                 | 06/Aug/Current Academic Year           | 0                      | 06/Aug/Current Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 403              | 1            | 2            | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                        |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | 
        | Aug/Current Academic Year | 500          | 0          | 0         | 
        | Sep/Current Academic Year | 500          | 0          | 0         | 
        | Oct/Current Academic Year | 500          | 0          | 0         | 
        | Nov/Current Academic Year | 500          | 0          | 0         | 
        | Dec/Current Academic Year | 500          | 0          | 0         | 
        | Jan/Current Academic Year | 500          | 0          | 0         | 
        | Feb/Current Academic Year | 500          | 0          | 0         | 
        | Mar/Current Academic Year | 500          | 0          | 0         | 
        | Apr/Current Academic Year | 500          | 0          | 0         | 
        | May/Current Academic Year | 500          | 0          | 0         | 
        | Jun/Current Academic Year | 500          | 0          | 0         | 
        | Jul/Current Academic Year | 500          | 0          | 0         | 
	 And the following provider payments had been generated
           | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
           | R01/Current Academic Year | Aug/Current Academic Year | 500           | Learning         |
           | R02/Current Academic Year | Sep/Current Academic Year | 500           | Learning         |
           | R03/Current Academic Year | Oct/Current Academic Year | 500           | Learning         |
         
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage | Small Employer |
		| 06/Aug/Current Academic Year | 12 months        | 7500                 | 06/Aug/Current Academic Year           | 0                      | 06/Aug/Current Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Levy Contract | 100%                        | SEM1           |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 7500                 | 06/Aug/Current Academic Year           | 0                      | 06/Aug/Current Academic Year             | 0                       |                                        | 0                         |                                          | Act1          | 1                   | 100%                        |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period        | On-Programme | Completion | Balancing | 
        | Aug/Current Academic Year | 500          | 0          | 0         | 
        | Sep/Current Academic Year | 500          | 0          | 0         | 
        | Oct/Current Academic Year | 500          | 0          | 0         | 
        | Nov/Current Academic Year | 500          | 0          | 0         | 
        | Dec/Current Academic Year | 500          | 0          | 0         | 
        | Jan/Current Academic Year | 500          | 0          | 0         | 
        | Feb/Current Academic Year | 500          | 0          | 0         | 
        | Mar/Current Academic Year | 500          | 0          | 0         | 
        | Apr/Current Academic Year | 500          | 0          | 0         | 
        | May/Current Academic Year | 500          | 0          | 0         | 
        | Jun/Current Academic Year | 500          | 0          | 0         | 
        | Jul/Current Academic Year | 500          | 0          | 0         | 
	And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period            | On Programme | Include Zero Expected Learning Payments | Transaction Type |
		| R04/Current Academic Year | Aug/Current Academic Year  | 0            | true                                    | Learning         |
		| R04/Current Academic Year | Sept/Current Academic Year | 0            | true                                    | Learning         |
		| R04/Current Academic Year | Oct/Current Academic Year  | 0            | true                                    | Learning         |
		| R04/Current Academic Year | Nov/Current Academic Year  | 500          |                                         | Learning         |
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period            | Levy Payments | SFA Co-Funded Payments | Transaction Type |
        | R04/Current Academic Year | Aug/Current Academic Year  | -500          |                        | Learning         |
        | R04/Current Academic Year | Aug/Current Academic Year  | 0             | 500                    | Learning         |
        | R04/Current Academic Year | Sept/Current Academic Year | -500          |                        | Learning         |
        | R04/Current Academic Year | Sept/Current Academic Year | 0             | 500                    | Learning         |
        | R04/Current Academic Year | Oct/Current Academic Year  | -500          |                        | Learning         |
        | R04/Current Academic Year | Oct/Current Academic Year  | 0             | 500                    | Learning         |
        | R04/Current Academic Year | Aug/Current Academic Year  | 0             | 500                    | Learning         |
     
	Examples:
        | Collection_Period         | Levy Balance |
        | R04/Current Academic Year | 7500         |
