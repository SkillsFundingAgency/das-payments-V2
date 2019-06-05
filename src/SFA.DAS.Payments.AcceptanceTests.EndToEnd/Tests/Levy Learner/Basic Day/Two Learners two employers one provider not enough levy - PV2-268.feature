Feature: Two Learners two employers one provider not enough levy - PV2-268
	As a provider,
	I want 2 learners with 2 employers that have 1 provider and there is not enough levy to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.

Scenario Outline: 2 levy learners 2 employers 1 provider not enough levy PV2-268
	# levy balance is not enough for both employers
	Given the "employer 1" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 1>
	And  the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
	And the following commitments exist
		| Identifier     	| Employer   | Learner ID | priority | start date                | end date                     | agreed price |Framework Code | Pathway Code | Programme Type |
		| Apprenticeship 1	| employer 1 | learner a  | 1        | 01/Sep/Last Academic Year | 08/Sep/Current Academic Year | 7500         |593            | 1            | 20             |
		| Apprenticeship 2	| employer 2 | learner b  | 2        | 01/Sep/Last Academic Year | 08/Sep/Current Academic Year | 15000        |593            | 1            | 20             |
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
	And the following provider payments had been generated
		| Learner ID | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
		| learner a  | R02/Last Academic Year | Sep/Last Academic Year | 450                    | 50                          | 0             | Learning         |
		| learner a  | R03/Last Academic Year | Oct/Last Academic Year | 360                    | 40                          | 100           | Learning         |
		| learner a  | R04/Last Academic Year | Nov/Last Academic Year | 360                    | 40                          | 100           | Learning         |
		| learner a  | R05/Last Academic Year | Dec/Last Academic Year | 360                    | 40                          | 100           | Learning         |
		| learner a  | R06/Last Academic Year | Jan/Last Academic Year | 360                    | 40                          | 100           | Learning         |
		| learner a  | R07/Last Academic Year | Feb/Last Academic Year | 360                    | 40                          | 100           | Learning         |
		| learner a  | R08/Last Academic Year | Mar/Last Academic Year | 360                    | 40                          | 100           | Learning         |
		| learner a  | R09/Last Academic Year | Apr/Last Academic Year | 360                    | 40                          | 100           | Learning         |
		| learner a  | R10/Last Academic Year | May/Last Academic Year | 360                    | 40                          | 100           | Learning         |
		| learner a  | R11/Last Academic Year | Jun/Last Academic Year | 360                    | 40                          | 100           | Learning         |
		| learner a  | R12/Last Academic Year | Jul/Last Academic Year | 360                    | 40                          | 100           | Learning         |
		| learner b  | R02/Last Academic Year | Sep/Last Academic Year | 450                    | 50                          | 500           | Learning         |
		| learner b  | R03/Last Academic Year | Oct/Last Academic Year | 450                    | 50                          | 500           | Learning         |
		| learner b  | R04/Last Academic Year | Nov/Last Academic Year | 450                    | 50                          | 500           | Learning         |
		| learner b  | R05/Last Academic Year | Dec/Last Academic Year | 450                    | 50                          | 500           | Learning         |
		| learner b  | R06/Last Academic Year | Jan/Last Academic Year | 450                    | 50                          | 500           | Learning         |
		| learner b  | R07/Last Academic Year | Feb/Last Academic Year | 450                    | 50                          | 500           | Learning         |
		| learner b  | R08/Last Academic Year | Mar/Last Academic Year | 450                    | 50                          | 500           | Learning         |
		| learner b  | R09/Last Academic Year | Apr/Last Academic Year | 450                    | 50                          | 500           | Learning         |
		| learner b  | R10/Last Academic Year | May/Last Academic Year | 450                    | 50                          | 500           | Learning         |
		| learner b  | R11/Last Academic Year | Jun/Last Academic Year | 450                    | 50                          | 500           | Learning         |
		| learner b  | R12/Last Academic Year | Jul/Last Academic Year | 450                    | 50                          | 500           | Learning         |
	But the Provider now changes the Learner details as follows
		| Learner ID | Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                | SFA Contribution Percentage |
		| learner a  | 01/Sep/Last Academic Year | 12 months        | 7500                 | 01/Sep/Last Academic Year           |                        |                                       | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |
		| learner b  | 01/Sep/Last Academic Year | 12 months        | 15000                | 01/Sep/Last Academic Year           |                        |                                       | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 7500                 | 01/Sep/Last Academic Year           |                        | 01/Sep/Last Academic Year             | Act1          | 1                   | 90%                         |
        | pe-2             | 15000                | 01/Sep/Last Academic Year           |                        | 01/Sep/Last Academic Year             | Act1          | 1                   | 90%                         |
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
		| Employer   | Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
		| employer 1 | learner a  | R01/Current Academic Year | Aug/Current Academic Year | 225                    | 25                          | 250           | Learning         |
		| employer 1 | learner a  | R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | 500           | Completion       |
		| employer 2 | learner b  | R01/Current Academic Year | Aug/Current Academic Year | 450                    | 50                          | 500           | Learning         |
		| employer 2 | learner b  | R02/Current Academic Year | Sep/Current Academic Year | 1350                   | 150                         | 1500          | Completion       |
	And only the following provider payments will be generated
		| Employer   | Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
		| employer 1 | learner a  | R01/Current Academic Year | Aug/Current Academic Year | 225                    | 25                          | 250           | Learning         |
		| employer 1 | learner a  | R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | 500           | Completion       |
		| employer 2 | learner b  | R01/Current Academic Year | Aug/Current Academic Year | 450                    | 50                          | 500           | Learning         |
		| employer 2 | learner b  | R02/Current Academic Year | Sep/Current Academic Year | 1350                   | 150                         | 1500          | Completion       |
	Examples: 
		| Collection_Period         | Levy Balance for employer 1 | Levy Balance for employer 2 |
		| R01/Current Academic Year | 250                         | 500                         |
		| R02/Current Academic Year | 500                         | 1500                        |
		| R03/Current Academic Year | 0                           | 0                           |




#Scenario: 2 learners, 2 employers, 1 provider - not enough levy

# Given the employer 1 has a levy balance of:
#            | 10/18 | 11/18 | 12/18 | ... | 09/19 | 10/19 |
#            | 0     | 100   | 100   | 100 | 250   | 500   |
# 
# And the employer 2 has a levy balance of:
#            | 10/18 | 11/18 | 12/18 | ... | 09/19 | 10/19 |
#            | 500   | 500   | 500   | 500 | 500   | 1500  |
#   
# And the following commitments exist:
#            | Employer   | ULN       | priority | agreed price | start date | end date   |
#            | employer 1 | learner a | 1        | 7500         | 01/09/2018 | 08/09/2019 |
#            | employer 2 | learner b | 1        | 15000        | 01/09/2018 | 08/09/2019 |
#        
#
#When an ILR file is submitted with the following data:
#            | ULN       | agreed price | learner type       | start date | planned end date | actual end date | completion status |
#            | learner a | 7500         | programme only DAS | 01/09/2018 | 08/09/2019       | 08/09/2019      | completed         |
#            | learner b | 15000        | programme only DAS | 01/09/2018 | 08/09/2019       | 08/09/2019      | completed         |
#			
#        
#Then the provider earnings and payments break down as follows:
#            | Type                            | 09/18 | 10/18 | 11/18 | ... | 08/19 | 09/19 | 10/19 |
#            | Provider Earned Total           | 1500  | 1500  | 1500  | ... | 1500  | 4500  | 0     |
#            | Provider Earned from SFA        | 1400  | 1410  | 1410  | ... | 1425  | 4250  | 0     |
#            | Provider Earned from Employer 1 | 50    | 40    | 40    | ... | 25    | 100   | 0     |
#            | Provider Earned from Employer 2 | 50    | 50    | 50    | ... | 50    | 150   | 0     |
#            | Provider Paid by SFA            | 0     | 1400  | 1410  | ... | 1410  | 1425  | 4250  |
#            | Payment due from Employer 1     | 0     | 50    | 40    | ... | 40    | 25    | 100   |
#            | Payment due from Employer 2     | 0     | 50    | 50    | ... | 50    | 50    | 150   |
#            | employer 1 Levy account debited | 0     | 0     | 100   | ... | 100   | 250   | 500   |
#            | employer 2 Levy account debited | 0     | 500   | 500   | ... | 500   | 500   | 1500  |
#            | SFA Levy employer budget        | 500   | 600   | 600   | ... | 750   | 2000  | 0     |
#            | SFA Levy co-funding budget      | 900   | 810   | 810   | ... | 675   | 2250  | 0     |
#            | SFA non-Levy co-funding budget  | 0     | 0     | 0     | ... | 0     | 0     | 0     |


# levy balance is not enough for both employers
# Commitments line
# Levy Payments
# Multiple employers
