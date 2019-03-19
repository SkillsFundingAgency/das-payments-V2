Feature: 2 learners 2 employers 1 provider enough levy - PV2-267
	
#2 learners, 2 employers, 1 provider - enough levy
#       
#	   Given the employer 1 has a levy balance > agreed price for all months
#       And the employer 2 has a levy balance > agreed price for all months
#        
#		And the following commitments exist:
#            | Employer   | ULN       | priority | agreed price | start date | end date   |
#            | employer 1 | learner a | 1        | 7500         | 01/09/2018 | 08/09/2019 |
#            | employer 2 | learner b | 1        | 15000        | 01/09/2018 | 08/09/2019 |
#        
#		When an ILR file is submitted with the following data:
#            | ULN       | agreed price | learner type       | start date | planned end date | actual end date | completion status |
#            | learner a | 7500         | programme only DAS | 01/09/2018 | 08/09/2019       | 08/09/2019      | completed         |
#            | learner b | 15000        | programme only DAS | 01/09/2018 | 08/09/2019       | 08/09/2019      | completed         |
#        
#		Then the provider earnings and payments break down as follows:
#            | Type                            | 09/18 | 10/18 | 11/18 | ... | 08/19 | 09/19 | 10/19 |
#            | Provider Earned Total           | 1500  | 1500  | 1500  | ... | 1500  | 4500  | 0     |
#            | Provider Earned from SFA        | 1500  | 1500  | 1500  | ... | 1500  | 4500  | 0     |
#            | Provider Earned from Employer 1 | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#            | Provider Earned from Employer 2 | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#            | Provider Paid by SFA            | 0     | 1500  | 1500  | ... | 1500  | 1500  | 4500  |
#            | Payment due from Employer 1     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#            | Payment due from Employer 2     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#            | employer 1 Levy account debited | 0     | 500   | 500   | ... | 500   | 500   | 1500  |
#            | employer 2 Levy account debited | 0     | 1000  | 1000  | ... | 1000  | 1000  | 3000  |
#            | SFA Levy employer budget        | 1500  | 1500  | 1500  | ... | 1500  | 4500  | 0     |
#            | SFA Levy co-funding budget      | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#            | SFA non-Levy co-funding budget  | 0     | 0     | 0     | ... | 0     | 0     | 0     |

# levy balance enough for both employers
# Commitments line
# Levy Payments
# Multiple employers

Scenario Outline: 2 levy learners 2 employers 1 provider and enough levy PV2-267
	# levy balance is enough for both employers
	Given the "employer 1" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 1>
	And  the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
	And the following commitments exist
        | Employer   | Learner ID | priority | start date                | end date                     | agreed price |
        | employer 1 | learner a  | 1        | 01/Sep/Last Academic Year | 08/Sep/Current Academic Year | 7500         |
        | employer 2 | learner b  | 2        | 01/Sep/Last Academic Year | 08/Sep/Current Academic Year | 15000        |
	And the provider previously submitted the following learner details
		| Learner ID | Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| learner a  | 01/Sep/Last Academic Year | 12 months        | 7500                 | 01/Sep/Last Academic Year           | 0                      | 01/Sep/Last Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
		| learner b  | 01/Sep/Last Academic Year | 12 months        | 15000                | 01/Sep/Last Academic Year           | 0                      | 01/Sep/Last Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And the following earnings had been generated for the learner
        | Learner ID | Delivery Period        | On-Programme | Completion | Balancing |
        | learner a  | Aug/Last Academic Year | 0            | 0          | 0         |
        | learner a  | Sep/Last Academic Year | 500          | 0          | 0         |
        | learner a  | Oct/Last Academic Year | 500          | 0          | 0         |
        | learner a  | Nov/Last Academic Year | 500          | 0          | 0         |
        | learner a  | Dec/Last Academic Year | 500          | 0          | 0         |
        | learner a  | Jan/Last Academic Year | 500          | 0          | 0         |
        | learner a  | Feb/Last Academic Year | 500          | 0          | 0         |
        | learner a  | Mar/Last Academic Year | 500          | 0          | 0         |
        | learner a  | Apr/Last Academic Year | 500          | 0          | 0         |
        | learner a  | May/Last Academic Year | 500          | 0          | 0         |
        | learner a  | Jun/Last Academic Year | 500          | 0          | 0         |
        | learner a  | Jul/Last Academic Year | 500          | 0          | 0         |
        | learner b  | Aug/Last Academic Year | 0            | 0          | 0         |
        | learner b  | Sep/Last Academic Year | 1000         | 0          | 0         |
        | learner b  | Oct/Last Academic Year | 1000         | 0          | 0         |
        | learner b  | Nov/Last Academic Year | 1000         | 0          | 0         |
        | learner b  | Dec/Last Academic Year | 1000         | 0          | 0         |
        | learner b  | Jan/Last Academic Year | 1000         | 0          | 0         |
        | learner b  | Feb/Last Academic Year | 1000         | 0          | 0         |
        | learner b  | Mar/Last Academic Year | 1000         | 0          | 0         |
        | learner b  | Apr/Last Academic Year | 1000         | 0          | 0         |
        | learner b  | May/Last Academic Year | 1000         | 0          | 0         |
        | learner b  | Jun/Last Academic Year | 1000         | 0          | 0         |
        | learner b  | Jul/Last Academic Year | 1000         | 0          | 0         |
    And the following provider payments had been generated
        | Learner ID | Collection Period      | Delivery Period        | Levy Payments | Transaction Type | Employer   |
        | learner a  | R02/Last Academic Year | Sep/Last Academic Year | 500           | Learning         | employer 1 |
        | learner a  | R03/Last Academic Year | Oct/Last Academic Year | 500           | Learning         | employer 1 |
        | learner a  | R04/Last Academic Year | Nov/Last Academic Year | 500           | Learning         | employer 1 |
        | learner a  | R05/Last Academic Year | Dec/Last Academic Year | 500           | Learning         | employer 1 |
        | learner a  | R06/Last Academic Year | Jan/Last Academic Year | 500           | Learning         | employer 1 |
        | learner a  | R07/Last Academic Year | Feb/Last Academic Year | 500           | Learning         | employer 1 |
        | learner a  | R08/Last Academic Year | Mar/Last Academic Year | 500           | Learning         | employer 1 |
        | learner a  | R09/Last Academic Year | Apr/Last Academic Year | 500           | Learning         | employer 1 |
        | learner a  | R10/Last Academic Year | May/Last Academic Year | 500           | Learning         | employer 1 |
        | learner a  | R11/Last Academic Year | Jun/Last Academic Year | 500           | Learning         | employer 1 |
        | learner a  | R12/Last Academic Year | Jul/Last Academic Year | 500           | Learning         | employer 1 |
        | learner b  | R02/Last Academic Year | Sep/Last Academic Year | 1000          | Learning         | employer 2 |
        | learner b  | R03/Last Academic Year | Oct/Last Academic Year | 1000          | Learning         | employer 2 |
        | learner b  | R04/Last Academic Year | Nov/Last Academic Year | 1000          | Learning         | employer 2 |
        | learner b  | R05/Last Academic Year | Dec/Last Academic Year | 1000          | Learning         | employer 2 |
        | learner b  | R06/Last Academic Year | Jan/Last Academic Year | 1000          | Learning         | employer 2 |
        | learner b  | R07/Last Academic Year | Feb/Last Academic Year | 1000          | Learning         | employer 2 |
        | learner b  | R08/Last Academic Year | Mar/Last Academic Year | 1000          | Learning         | employer 2 |
        | learner b  | R09/Last Academic Year | Apr/Last Academic Year | 1000          | Learning         | employer 2 |
        | learner b  | R10/Last Academic Year | May/Last Academic Year | 1000          | Learning         | employer 2 |
        | learner b  | R11/Last Academic Year | Jun/Last Academic Year | 1000          | Learning         | employer 2 |
        | learner b  | R12/Last Academic Year | Jul/Last Academic Year | 1000          | Learning         | employer 2 |
    But the Provider now changes the Learner details as follows
		| Learner ID | Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| learner a  | 01/Sep/Last Academic Year | 12 months        | 7500                 | 01/Sep/Last Academic Year           | 0                      | 01/Sep/Last Academic Year             | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
		| learner b  | 01/Sep/Last Academic Year | 12 months        | 15000                | 01/Sep/Last Academic Year           | 0                      | 01/Sep/Last Academic Year             | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Learner ID | Delivery Period           | On-Programme | Completion | Balancing |
		| learner a  | Aug/Current Academic Year | 500          | 0          | 0         |
		| learner a  | Sep/Current Academic Year | 0            | 1500       | 0         |
		| learner a  | Oct/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Nov/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Dec/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Jan/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Feb/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Mar/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Apr/Current Academic Year | 0            | 0          | 0         |
		| learner a  | May/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Jun/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Jul/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Aug/Current Academic Year | 1000         | 0          | 0         |
		| learner b  | Sep/Current Academic Year | 0            | 3000       | 0         |
		| learner b  | Oct/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Nov/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Dec/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Jan/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Feb/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Mar/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Apr/Current Academic Year | 0            | 0          | 0         |
		| learner b  | May/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Jun/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Jul/Current Academic Year | 0            | 0          | 0         |
    And at month end only the following payments will be calculated
        | Learner ID | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | learner a  | R01/Current Academic Year | Aug/Current Academic Year | 500          | 0          | 0         |
        | learner a  | R02/Current Academic Year | Sep/Current Academic Year | 0            | 1500       | 0         |
        | learner b  | R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
        | learner b  | R02/Current Academic Year | Sep/Current Academic Year | 0            | 3000       | 0         |
   And only the following provider payments will be recorded
        | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Employer   |
        | learner a  | R01/Current Academic Year | Aug/Current Academic Year | 500           | Learning         | employer 1 |
        | learner a  | R02/Current Academic Year | Sep/Current Academic Year | 1500          | Completion       | employer 1 |
        | learner b  | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         | employer 2 |
        | learner b  | R02/Current Academic Year | Sep/Current Academic Year | 3000          | Completion       | employer 2 |
	And only the following provider payments will be generated
        | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Employer   |
        | learner a  | R01/Current Academic Year | Aug/Current Academic Year | 500           | Learning         | employer 1 |
        | learner a  | R02/Current Academic Year | Sep/Current Academic Year | 1500          | Completion       | employer 1 |
        | learner b  | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         | employer 2 |
        | learner b  | R02/Current Academic Year | Sep/Current Academic Year | 3000          | Completion       | employer 2 |
	
Examples: 
        | Collection_Period         | Levy Balance for employer 1 | Levy Balance for employer 2 |
        | R01/Current Academic Year | 2500                        | 4500                        |
        | R02/Current Academic Year | 2000                        | 3500                        |