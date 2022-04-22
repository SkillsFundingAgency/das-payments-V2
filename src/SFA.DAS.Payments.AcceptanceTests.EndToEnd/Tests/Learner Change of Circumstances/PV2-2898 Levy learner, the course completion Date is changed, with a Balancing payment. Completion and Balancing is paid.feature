Feature: Employer Stops PV2-2898 
Scenario Outline: Levy learner, the course completion Date is changed, with a Balancing payment. Completion and Balancing is paid
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following apprenticeships exist
		| framework code | programme type | pathway code | agreed price | start date                | end date                     | status  | effective from            | stop effective from          |
		| 593            | 20             | 1            | 17500        | 01/Aug/Last Academic Year | 01/Oct/Current Academic Year | stopped | 01/Aug/Last Academic Year | 15/Oct/Current Academic Year |		
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                | SFA Contribution Percentage |
		| 01/Aug/Last Academic Year | 14 months        | 17500                | 01/Aug/Last Academic Year           |                        | 01/Sep/Last Academic Year             | 14 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And price details are changed as follows        
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 17500                | 01/Aug/Last Academic Year           |                        | 01/Sep/Last Academic Year             | Act1          | 1                   | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
        | Aug/Last Academic Year    | 1000         | 0          | 0         | pe-1                     |
        | Sep/Last Academic Year    | 1000         | 0          | 0         | pe-1                     |
        | Oct/Last Academic Year    | 1000         | 0          | 0         | pe-1                     |
        | Nov/Last Academic Year    | 1000         | 0          | 0         | pe-1                     |
        | Dec/Last Academic Year    | 1000         | 0          | 0         | pe-1                     |
        | Jan/Last Academic Year    | 1000         | 0          | 0         | pe-1                     |
        | Feb/Last Academic Year    | 1000         | 0          | 0         | pe-1                     |
        | Mar/Last Academic Year    | 1000         | 0          | 0         | pe-1                     |
        | Apr/Last Academic Year    | 1000         | 0          | 0         | pe-1                     |
        | May/Last Academic Year    | 1000         | 0          | 0         | pe-1                     |
        | Jun/Last Academic Year    | 1000         | 0          | 0         | pe-1                     |
        | Jul/Last Academic Year    | 1000         | 0          | 0         | pe-1                     |
        | Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Sep/Current Academic Year | 1000         | 3500       | 0         | pe-1                     |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/Last Academic Year    | Aug/Last Academic Year    | 1000          | Learning         |
        | R02/Last Academic Year    | Sep/Last Academic Year    | 1000          | Learning         |
        | R03/Last Academic Year    | Oct/Last Academic Year    | 1000          | Learning         |
        | R04/Last Academic Year    | Nov/Last Academic Year    | 1000          | Learning         |
        | R05/Last Academic Year    | Dec/Last Academic Year    | 1000          | Learning         |
        | R06/Last Academic Year    | Jan/Last Academic Year    | 1000          | Learning         |
        | R07/Last Academic Year    | Feb/Last Academic Year    | 1000          | Learning         |
        | R08/Last Academic Year    | Mar/Last Academic Year    | 1000          | Learning         |
        | R09/Last Academic Year    | Apr/Last Academic Year    | 1000          | Learning         |
        | R10/Last Academic Year    | May/Last Academic Year    | 1000          | Learning         |
        | R11/Last Academic Year    | Jun/Last Academic Year    | 1000          | Learning         |
        | R12/Last Academic Year    | Jul/Last Academic Year    | 1000          | Learning         |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Balancing        |
        | R02/Current Academic Year | Sep/Current Academic Year | 3500          | Completion       |
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                | SFA Contribution Percentage |
		| 01/Aug/Last Academic Year | 14 months        | 17500                | 01/Aug/Current Academic Year        |                        |                                       | 14 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |
	
	And price details are changed as follows        
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-2             | 17500                | 01/Aug/Current Academic Year        |                        | 01/Oct/Current Academic Year          | Act1          | 1                   | 90%                         |
	
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
		| Sep/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| Oct/Current Academic Year | 0            | 3500       | 1000      | pe-2                     |
		| Nov/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| Jan/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| Feb/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| Mar/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| Apr/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| May/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| Jun/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| Jul/Current Academic Year | 0            | 0          | 0         | pe-2                     |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R03/Current Academic Year | Sep/Current Academic Year | 0            | 0          | -1000     |
        | R03/Current Academic Year | Sep/Current Academic Year | 0            | -3500      | 0         |
        | R03/Current Academic Year | Oct/Current Academic Year | 0            | 0          | 1000      |
        | R03/Current Academic Year | Oct/Current Academic Year | 0            | 3500       | 0         |
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R03/Current Academic Year | Sep/Current Academic Year | -1000         | Balancing        |
        | R03/Current Academic Year | Sep/Current Academic Year | -3500         | Completion       |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000          | Balancing        |
        | R03/Current Academic Year | Oct/Current Academic Year | 3500          | Completion       |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R03/Current Academic Year | Sep/Current Academic Year | -1000         | Balancing        |
        | R03/Current Academic Year | Sep/Current Academic Year | -3500         | Completion       |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000          | Balancing        |
        | R03/Current Academic Year | Oct/Current Academic Year | 3500          | Completion       |
Examples: 
        | Collection_Period         | Levy Balance |
        | R03/Current Academic Year | 5000         |
