Feature: Two levy learners but only enough levy to fund one learner- PV2-264

Scenario Outline: Two levy learners, levy available but for only one learner, both finished on time PV2-264
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
		| Identifier       | Learner ID | priority | start date                | end date                     | agreed price | Framework Code | Pathway Code | Programme Type | Agreed On Date            |
		| Apprenticeship 1 | learner a  | 1        | 01/Sep/Last Academic Year | 08/Sep/Current Academic Year | 7500         | 593            | 1            | 20             | 30/Aug/Last Academic Year |
		| Apprenticeship 2 | learner b  | 2        | 01/Sep/Last Academic Year | 08/Sep/Current Academic Year | 15000        | 593            | 1            | 20             | 31/Aug/Last Academic Year |
	
	And the provider previously submitted the following learner details
		| Learner ID | Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                | SFA Contribution Percentage |
		| learner a  | 01/Sep/Last Academic Year | 12 months        | 7500                 | 01/Sep/Last Academic Year           |                        |                                       |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |
		| learner b  | 01/Sep/Last Academic Year | 12 months        | 15000                | 01/Sep/Last Academic Year           |                        |                                       |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And price details as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 7500                 | 01/Sep/Last Academic Year           |                        | 01/Sep/Last Academic Year             | Act1          | 1                   | 90%                         |
        | pe-2             | 15000                | 01/Sep/Last Academic Year           |                        | 01/Sep/Last Academic Year             | Act1          | 1                   | 90%                         |
	And the following earnings had been generated for the learner
        | Learner ID | Delivery Period        | On-Programme | Completion | Balancing | Price Episode Identifier |
        | learner a  | Aug/Last Academic Year | 0            | 0          | 0         | pe-1                     |
        | learner a  | Sep/Last Academic Year | 500          | 0          | 0         | pe-1                     |
        | learner a  | Oct/Last Academic Year | 500          | 0          | 0         | pe-1                     |
        | learner a  | Nov/Last Academic Year | 500          | 0          | 0         | pe-1                     |
        | learner a  | Dec/Last Academic Year | 500          | 0          | 0         | pe-1                     |
        | learner a  | Jan/Last Academic Year | 500          | 0          | 0         | pe-1                     |
        | learner a  | Feb/Last Academic Year | 500          | 0          | 0         | pe-1                     |
        | learner a  | Mar/Last Academic Year | 500          | 0          | 0         | pe-1                     |
        | learner a  | Apr/Last Academic Year | 500          | 0          | 0         | pe-1                     |
        | learner a  | May/Last Academic Year | 500          | 0          | 0         | pe-1                     |
        | learner a  | Jun/Last Academic Year | 500          | 0          | 0         | pe-1                     |
        | learner a  | Jul/Last Academic Year | 500          | 0          | 0         | pe-1                     |
        | learner b  | Aug/Last Academic Year | 0            | 0          | 0         | pe-2                     |
        | learner b  | Sep/Last Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Oct/Last Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Nov/Last Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Dec/Last Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Jan/Last Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Feb/Last Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Mar/Last Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Apr/Last Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | May/Last Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Jun/Last Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Jul/Last Academic Year | 1000         | 0          | 0         | pe-2                     |
	# Levy Payments
    And the following provider payments had been generated
        | Learner ID | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | learner a  | R02/Last Academic Year | Sep/Last Academic Year | 450                    | 50                          | 0             | Learning         |
        | learner a  | R03/Last Academic Year | Oct/Last Academic Year | 0                      | 0                           | 500           | Learning         |
        | learner a  | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 500           | Learning         |
        | learner a  | R05/Last Academic Year | Dec/Last Academic Year | 0                      | 0                           | 500           | Learning         |
        | learner a  | R06/Last Academic Year | Jan/Last Academic Year | 0                      | 0                           | 500           | Learning         |
        | learner a  | R07/Last Academic Year | Feb/Last Academic Year | 0                      | 0                           | 500           | Learning         |
        | learner a  | R08/Last Academic Year | Mar/Last Academic Year | 0                      | 0                           | 500           | Learning         |
        | learner a  | R09/Last Academic Year | Apr/Last Academic Year | 0                      | 0                           | 500           | Learning         |
        | learner a  | R10/Last Academic Year | May/Last Academic Year | 0                      | 0                           | 500           | Learning         |
        | learner a  | R11/Last Academic Year | Jun/Last Academic Year | 0                      | 0                           | 500           | Learning         |
        | learner a  | R12/Last Academic Year | Jul/Last Academic Year | 0                      | 0                           | 500           | Learning         |
        | learner b  | R02/Last Academic Year | Sep/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R03/Last Academic Year | Oct/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R04/Last Academic Year | Nov/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R05/Last Academic Year | Dec/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R06/Last Academic Year | Jan/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R07/Last Academic Year | Feb/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R08/Last Academic Year | Mar/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R09/Last Academic Year | Apr/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R10/Last Academic Year | May/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R11/Last Academic Year | Jun/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R12/Last Academic Year | Jul/Last Academic Year | 900                    | 100                         | 0             | Learning         |
    But the Provider now changes the Learner details as follows
		| Learner ID | Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                | SFA Contribution Percentage |
		| learner a  | 01/Sep/Last Academic Year | 12 months        | 7500                 | 01/Sep/Last Academic Year           |                        |                                       | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |
		| learner b  | 01/Sep/Last Academic Year | 12 months        | 15000                | 01/Sep/Last Academic Year           |                        |                                       | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details as follows		
       | Learner ID  | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
       | learner a   | pe-1             | 7500                 | 01/Sep/Last Academic Year           |                        | 01/Sep/Last Academic Year             | Act1          | 1                   | 90%                         |
       | learner b   | pe-2             | 15000                | 01/Sep/Last Academic Year           |                        | 01/Sep/Last Academic Year             | Act1          | 1                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Learner ID | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| learner a  | Aug/Current Academic Year | 500          | 0          | 0         | pe-1                     |
		| learner a  | Sep/Current Academic Year | 0            | 1500       | 0         | pe-1                     |
		| learner a  | Oct/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Nov/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Dec/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Jan/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Feb/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Mar/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Apr/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | May/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Jun/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Jul/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner b  | Aug/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
		| learner b  | Sep/Current Academic Year | 0            | 3000       | 0         | pe-2                     |
		| learner b  | Oct/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| learner b  | Nov/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| learner b  | Dec/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| learner b  | Jan/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| learner b  | Feb/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| learner b  | Mar/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| learner b  | Apr/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| learner b  | May/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| learner b  | Jun/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| learner b  | Jul/Current Academic Year | 0            | 0          | 0         | pe-2                     |
    And at month end only the following payments will be calculated
        | Learner ID | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | learner a  | R01/Current Academic Year | Aug/Current Academic Year | 500          | 0          | 0         |
        | learner a  | R02/Current Academic Year | Sep/Current Academic Year | 0            | 1500       | 0         |
        | learner b  | R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
        | learner b  | R02/Current Academic Year | Sep/Current Academic Year | 0            | 3000       | 0         |
	And only the following provider payments will be recorded
        | Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | learner a  | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 500           | Learning         |
        | learner a  | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 1500          | Completion       |
        | learner b  | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R02/Current Academic Year | Sep/Current Academic Year | 2700                   | 300                         | 0             | Completion       |
	And only the following provider payments will be generated
        | Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | learner a  | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 500           | Learning         |
        | learner a  | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 1500          | Completion       |
        | learner b  | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R02/Current Academic Year | Sep/Current Academic Year | 2700                   | 300                         | 0             | Completion       |
Examples: 
        | Collection_Period         | Levy Balance |
        | R01/Current Academic Year | 500          |
        | R02/Current Academic Year | 1500         |
        | R03/Current Academic Year | 0            |