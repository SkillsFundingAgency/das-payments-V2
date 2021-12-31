Feature: Levy learner changes employer negotiated total cost and ILR is submitted late - PV2-367
		As a provider,
		I want earnings and payments for a levy learner, levy available, where a total cost changes during the programme and ILR is submitted late, to be paid the correct amount
		So that I am accurately paid my apprenticeship provision

Scenario Outline: Levy learner changes employer negotiated total cost and ILR is submitted late - PV2-367
	Given the "employer 1" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 1>
	And  the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
	And the following commitments exist
        | Identifier       | Employer   | start date                   | end date                     | agreed price | status  | effective from               | effective to                 | stop effective from          | Standard Code | Programme Type |
        | Apprenticeship 1 | employer 1 | 01/Apr/last Academic Year    | 01/Mar/Current Academic Year | 4200         | stopped | 01/Apr/last Academic Year    | 01/Feb/Current Academic Year | 01/Feb/Current Academic Year | 51            | 25             |
        | Apprenticeship 2 | employer 2 | 01/Feb/Current Academic Year | 01/Mar/Current Academic Year | 1446         | active  | 01/Feb/Current Academic Year |                              |                              | 51            | 25             |
    And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 07/Apr/last Academic Year | 12 months        | 12000                | 07/Apr/last Academic Year           | 3000                   | 07/Apr/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details as follows
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
        | 1st price details | 4200                 | 07/Apr/last Academic Year           | 0                      | 07/Apr/last Academic Year             | 0                       |                                        | 0                         |                                          | 90%                         | Act1          |
	And the following earnings had been generated for the learner
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 280          | 0          | 0         | 1st price details        |
		| Sep/Current Academic Year | 280          | 0          | 0         | 1st price details        |
		| Oct/Current Academic Year | 280          | 0          | 0         | 1st price details        |
		| Nov/Current Academic Year | 280          | 0          | 0         | 1st price details        |
		| Dec/Current Academic Year | 280          | 0          | 0         | 1st price details        |
		| Jan/Current Academic Year | 280          | 0          | 0         | 1st price details        |
		| Feb/Current Academic Year | 280          | 0          | 0         | 1st price details        |
		| Mar/Current Academic Year | 280          | 0          | 0         | 1st price details        |
		| Apr/Current Academic Year | 0            | 0          | 0         | 1st price details        |
		| May/Current Academic Year | 0            | 0          | 0         | 1st price details        |
		| Jun/Current Academic Year | 0            | 0          | 0         | 1st price details        |
		| Jul/Current Academic Year | 0            | 0          | 0         | 1st price details        |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Employer   | Price Episode Identifier |
        | R01/Current Academic Year | Aug/Current Academic Year | 280           | Learning         | employer 1 | 1st price details        |
        | R02/Current Academic Year | Sep/Current Academic Year | 280           | Learning         | employer 1 | 1st price details        |
        | R03/Current Academic Year | Oct/Current Academic Year | 280           | Learning         | employer 1 | 1st price details        |
        | R04/Current Academic Year | Nov/Current Academic Year | 280           | Learning         | employer 1 | 1st price details        |
        | R05/Current Academic Year | Dec/Current Academic Year | 280           | Learning         | employer 1 | 1st price details        |
        | R06/Current Academic Year | Jan/Current Academic Year | 280           | Learning         | employer 1 | 1st price details        |
        | R07/Current Academic Year | Feb/Current Academic Year | 280           | Learning         | employer 1 | 1st price details        |
        | R08/Current Academic Year | Mar/Current Academic Year | 280           | Learning         | employer 1 | 1st price details        | 
    But the Provider now changes the Learner details as follows                  
        | Employer id | Start Date                | Planned Duration | Actual Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
        | employer 1  | 07/Apr/last Academic Year | 12 months        | 6 months        | 12000                | 07/Apr/last Academic Year           | 3000                   | 07/Apr/Current Academic Year          |                 | withdrawn         | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
        | employer 2  | 07/Apr/last Academic Year | 2 months         | 2 months        | 12000                | 07/Apr/last Academic Year           | 3000                   | 07/Apr/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And price details as follows
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
        | 1st price details | 4200                 | 07/Apr/last Academic Year           | 0                      | 07/Apr/last Academic Year             | 0                       |                                        | 0                         |                                          | 90%                         | Act1          |
        | 2nd price details | 0                    | 01/Feb/Current Academic Year        | 0                      | 01/Feb/Current Academic Year          | 606                     | 01/Feb/Current Academic Year           | 840                       | 01/Feb/Current Academic Year             | 90%                         | Act1          |
    When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
        | Aug/Current Academic Year | 280          | 0          | 0         | 1st price details        |
        | Sep/Current Academic Year | 280          | 0          | 0         | 1st price details        |
        | Oct/Current Academic Year | 280          | 0          | 0         | 1st price details        |
        | Nov/Current Academic Year | 280          | 0          | 0         | 1st price details        |
        | Dec/Current Academic Year | 280          | 0          | 0         | 1st price details        |
        | Jan/Current Academic Year | 280          | 0          | 0         | 1st price details        |
        | Feb/Current Academic Year | 579          | 0          | 0         | 2nd price details        |
        | Mar/Current Academic Year | 579          | 0          | 0         | 2nd price details        |
        | Apr/Current Academic Year | 0            | 0          | 0         | 2nd price details        |
        | May/Current Academic Year | 0            | 0          | 0         | 2nd price details        |
        | Jun/Current Academic Year | 0            | 0          | 0         | 2nd price details        |
        | Jul/Current Academic Year | 0            | 0          | 0         | 2nd price details        |
        
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R04/Current Academic Year | Aug/Current Academic Year | 280          | 0          | 0         |
        | R04/Current Academic Year | Sep/Current Academic Year | 280          | 0          | 0         |
        | R04/Current Academic Year | Oct/Current Academic Year | 280          | 0          | 0         |
        | R04/Current Academic Year | Nov/Current Academic Year | 280          | 0          | 0         |
        | R05/Current Academic Year | Dec/Current Academic Year | 280          | 0          | 0         |
        | R06/Current Academic Year | Jan/Current Academic Year | 280          | 0          | 0         |
#incorrectly generated payments        
        | R09/Current Academic Year | Feb/Current Academic Year | 299          | 0          | 0         |
        | R09/Current Academic Year | Mar/Current Academic Year | 299          | 0          | 0         | 
#Actual Expected Payments
        #| R07/Current Academic Year | Feb/Current Academic Year | 578.40       | 0          | 0         | 2nd price details        |
        #| R08/Current Academic Year | Mar/Current Academic Year | 578.40       | 0          | 0         | 2nd price details        |
        #| R09/Current Academic Year | Feb/Current Academic Year | -280         | 0          | 0         | 1st price details        |
        #| R09/Current Academic Year | Mar/Current Academic Year | -280         | 0          | 0         | 1st price details        |
                                                                                                          
Examples: 
        | Collection_Period         | Levy Balance for employer 1 | Levy Balance for employer 2 |
        | R09/Current Academic Year | 999999                      | 999999                      |
        