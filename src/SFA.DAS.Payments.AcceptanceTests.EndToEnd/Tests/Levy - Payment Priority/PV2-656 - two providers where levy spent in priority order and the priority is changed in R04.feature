Feature: Two providers levy for full one provider - partial levy for other,employer changes payment priority for provider - PV2-656

As an Employer,
I want 2 Levy learners with different providers, where levy is spent in priority order and there is not enough levy to fund both learners, and employer changes provider priority in R04
So that the providers are accurately paid the apprenticeship amount by SFA - PV2-656

Scenario Outline: Two providers levy for full one provider - partial levy for other ,employer changes payment priority for provider - PV2-656

	Given the employer levy account balance in collection period <Collection_Period> is <Levy_Balance>
	# Commitment lines
	And the following commitments exist
		| Identifier       | Provider   | Learner ID | start date                   | end date                  | agreed price | Framework Code | Pathway Code | Programme Type |
		| Apprenticeship 1 | provider a | learner a  | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 7500         | 593            | 1            | 20             |
		| Apprenticeship 2 | provider b | learner b  | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 15000        | 593            | 1            | 20             |

	And the provider priority order is
        | Provider   | Priority | Collection_Period         |
        | provider a | 1        | R01/Current Academic Year |
        | provider b | 2        | R01/Current Academic Year |
        | provider a | 2        | R04/Current Academic Year |
        | provider b | 1        | R04/Current Academic Year |

	And the provider "provider a" is providing training for the following learners
		| Learner ID | Start Date                   | Planned Duration | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                | SFA Contribution Percentage |
		| learner a  | 01/Aug/Current Academic Year | 12 months        |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |

	And the provider "provider b" is providing training for the following learners
		| Learner ID | Start Date                   | Planned Duration | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                | SFA Contribution Percentage |
		| learner b  | 01/Aug/Current Academic Year | 12 months        |                 | continuing        | Act1          | 2                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |
	
	And price details as follows
		| Learner ID | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type | Aim Sequence Number |
		| learner a  | pe-1             | 7500                 | 01/Aug/Current Academic Year        |                        | 01/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act1          | 1                   |
		| learner b  | pe-2             | 15000                | 01/Aug/Current Academic Year        |                        | 01/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act1          | 2                   |


	When the ILR file is submitted for the learners for the collection period <Collection_Period> by "provider a"
	When the ILR file is submitted for the learners for the collection period <Collection_Period> by "provider b"

	Then the following learner earnings should be generated for "provider a"
		| Learner ID | Delivery Period           | On-Programme | Completion | Balancing |Price Episode Identifier |
		| learner a  | Aug/Current Academic Year | 500          | 0          | 0         |pe-1                     |
		| learner a  | Sep/Current Academic Year | 500          | 0          | 0         |pe-1                     |
		| learner a  | Oct/Current Academic Year | 500          | 0          | 0         |pe-1                     |
		| learner a  | Nov/Current Academic Year | 500          | 0          | 0         |pe-1                     |
		| learner a  | Dec/Current Academic Year | 500          | 0          | 0         |pe-1                     |
		| learner a  | Jan/Current Academic Year | 500          | 0          | 0         |pe-1                     |
		| learner a  | Feb/Current Academic Year | 500          | 0          | 0         |pe-1                     |
		| learner a  | Mar/Current Academic Year | 500          | 0          | 0         |pe-1                     |
		| learner a  | Apr/Current Academic Year | 500          | 0          | 0         |pe-1                     |
		| learner a  | May/Current Academic Year | 500          | 0          | 0         |pe-1                     |
		| learner a  | Jun/Current Academic Year | 500          | 0          | 0         |pe-1                     |
		| learner a  | Jul/Current Academic Year | 500          | 0          | 0         |pe-1                     |
	And the following learner earnings should be generated for "provider b"
		| Learner ID | Delivery Period           | On-Programme | Completion | Balancing |Price Episode Identifier |
		| learner b  | Aug/Current Academic Year | 1000         | 0          | 0         |pe-2                     |
		| learner b  | Sep/Current Academic Year | 1000         | 0          | 0         |pe-2                     |
		| learner b  | Oct/Current Academic Year | 1000         | 0          | 0         |pe-2                     |
		| learner b  | Nov/Current Academic Year | 1000         | 0          | 0         |pe-2                     |
		| learner b  | Dec/Current Academic Year | 1000         | 0          | 0         |pe-2                     |
		| learner b  | Jan/Current Academic Year | 1000         | 0          | 0         |pe-2                     |
		| learner b  | Feb/Current Academic Year | 1000         | 0          | 0         |pe-2                     |
		| learner b  | Mar/Current Academic Year | 1000         | 0          | 0         |pe-2                     |
		| learner b  | Apr/Current Academic Year | 1000         | 0          | 0         |pe-2                     |
		| learner b  | May/Current Academic Year | 1000         | 0          | 0         |pe-2                     |
		| learner b  | Jun/Current Academic Year | 1000         | 0          | 0         |pe-2                     |
		| learner b  | Jul/Current Academic Year | 1000         | 0          | 0         |pe-2                     |
	# Levy Payments    
  	 And at month end only the following payments will be calculated for "provider a"
		| Learner ID | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| learner a  | R01/Current Academic Year | Aug/Current Academic Year | 500          | 0          | 0         |
		| learner a  | R02/Current Academic Year | Sep/Current Academic Year | 500          | 0          | 0         |
		| learner a  | R03/Current Academic Year | Oct/Current Academic Year | 500          | 0          | 0         |
		| learner a  | R04/Current Academic Year | Nov/Current Academic Year | 500          | 0          | 0         |
 	 And at month end only the following payments will be calculated for "provider b"
		| Learner ID | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| learner b  | R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
		| learner b  | R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
		| learner b  | R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
		| learner b  | R04/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         |

	And Month end is triggered

	# Levy Payments
	And only the following "provider a" payments will be recorded
		| Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
		| learner a  | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 500           | Learning         |
		| learner a  | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 500           | Learning         |
		| learner a  | R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 500           | Learning         |
		| learner a  | R04/Current Academic Year | Nov/Current Academic Year | 225                    | 25                          | 250           | Learning         |

	And only the following "provider b" payments will be recorded
		| Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
		| learner b  | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
		| learner b  | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
		| learner b  | R03/Current Academic Year | Oct/Current Academic Year | 720                    | 80                          | 200           | Learning         |
		| learner b  | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
	
	And only the following "provider a" payments will be generated
		| Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
		| learner a  | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 500           | Learning         |
		| learner a  | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 500           | Learning         |
		| learner a  | R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 500           | Learning         |
		| learner a  | R04/Current Academic Year | Nov/Current Academic Year | 225                    | 25                          | 250           | Learning         |
		
	And only the following "provider b" payments will be generated
		| Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
		| learner b  | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
		| learner b  | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
		| learner b  | R03/Current Academic Year | Oct/Current Academic Year | 720                    | 80                          | 200           | Learning         |
		| learner b  | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 1000          | Learning         |  
Examples: 
        | Collection_Period         | Levy_Balance |
        | R01/Current Academic Year | 1500         |
        | R02/Current Academic Year | 1500         |
        | R03/Current Academic Year | 700          |
        | R04/Current Academic Year | 1250         |

#Feature: Payment Priority
#
#Background: 2 providers, paid in priority order
#
#Scenario: Two Levy learners with different providers, levy is spent in priority order and there is not enough levy to fund both learners, and employer changes priority in R04
#
#       Given Two learners are programme only DAS 
#       And the apprenticeship funding band maximum for each learner is 17000
#        
#       And the employer's levy balance is:
#                | 08/18 | 09/18 | 10/18 | 11/18 | ...  | 07/19 | 08/19 |
#                | 1500  | 1500  | 700   | 1250  | 1250 | 1250  | 1250  |
#        
#	   And the provider priority order is:
#                | priority | provider |
#                | 1        | ABC 	  |
#                | 2        | DEF 	  |
#		
#       
#	   And the following commitments exist in period R01:
#                | priority | provider | start date | end date   | agreed price |
#                | 1        | ABC	  | 01/08/2018 | 28/08/2019 | 7500         |
#                | 2        | DEF	  | 01/08/2018 | 28/08/2019 | 15000        |
#                      
#       And an ILR file is submitted by provider ABC for collection period R01 with the following data:
#                | provider | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#                | ABC	   | 01/08/2018 | 28/08/2019       |                 | continuing        | 6000                 | 01/08/2018                          | 1500                   | 01/08/2018                            |
#
#       And an ILR file is submitted by provider DEF for collection period R01 with the following data:
#                | provider | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#                | DEF	   | 01/08/2018 | 28/08/2019       |                 | continuing        | 12000                | 01/08/2018                          | 3000                   | 01/08/2018                            |
#       
#
#
#       And the following commitments exist in R04:
#                | priority | provider | start date | end date   | agreed price |
#                | 2        | ABC	  | 01/08/2018 | 28/08/2019 | 7500         |
#                | 1        | DEF	  | 01/08/2018 | 28/08/2019 | 15000        |
#                      
#       When an ILR file is submitted by provider ABC for collection period R04 with the following data:
#                | priority | provider | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#                | 2        | ABC	  | 01/10/2018 | 28/08/2019       |                 | continuing        | 6000                 | 01/08/2018                          | 1500                   | 01/08/2018                            |
#
#       When an ILR file is submitted by provider DEF for collection period R04 with the following data:
#                | priority | provider | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#                | 1        | DEF	  | 01/10/2018 | 28/08/2019       |                 | continuing        | 12000                | 01/08/2018                          | 3000                   | 01/08/2018                            |
#
#      
#       Then the provider earnings and payments break down for provider ABC as follows:
#                | Type                           | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | ... | 07/19 | 08/19 |
#                | Provider Earned Total          | 500   | 500   | 500   | 500   | 500   | ... | 500   | 0     |
#                | Provider Earned from SFA       | 500   | 500   | 500   | 475   | 475   | ... | 475   | 0     |
#                | Provider Earned from Employer  | 0     | 0     | 0     | 25    | 25    | ... | 25    | 0     |
#                | Provider Paid by SFA           | 0     | 500   | 500   | 500   | 475   | ... | 475   | 475   |
#                | Payment due from Employer      | 0     | 0     | 0     | 0     | 25    | ... | 25    | 25    |
#                | Levy account debited           | 0     | 500   | 500   | 500   | 250   | ... | 250   | 250   |
#                | SFA Levy employer budget       | 500   | 500   | 500   | 250   | 250   | ... | 250   | 0     |
#                | SFA Levy co-funding budget     | 0     | 0     | 0     | 225   | 225   | ... | 225   | 0     |
#                | SFA non-Levy co-funding budget | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        
#       And the transaction types for the payments for provider ABC are:
#               | Payment type                   | 09/18 | 10/18 | 11/18 | ... | 07/19 | 08/19 |
#               | On-program                     | 500   | 500   | 500   | ... | 500   | 500   |
#               | Completion                     | 0     | 0     | 0     | ... | 0     | 0     |
#               | Balancing                      | 0     | 0     | 0     | ... | 0     | 0     |
#       
#       
#       And the provider earnings and payments break down for provider DEF as follows:
#                | Type                           | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | ... | 07/19 | 08/19 |
#                | Provider Earned Total          | 1000  | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#                | Provider Earned from SFA       | 1000  | 1000  | 920   | 1000  | 1000  | ... | 1000  | 0     |
#                | Provider Earned from Employer  | 0     | 0     | 80    | 0     | 0     | ... | 0     | 0     |
#                | Provider Paid by SFA           | 0     | 1000  | 1000  | 920   | 1000  | ... | 1000  | 1000  |
#                | Payment due from Employer      | 0     | 0     | 0     | 80    | 0     | ... | 0     | 0     |
#                | Levy account debited           | 0     | 1000  | 1000  | 200   | 1000  | ... | 1000  | 1000  |
#                | SFA Levy employer budget       | 1000  | 1000  | 200   | 1000  | 1000  | ... | 1000  | 0     |
#                | SFA Levy co-funding budget     | 0     | 0     | 720   | 0     | 0     | ... | 0     | 0     |
#                | SFA non-Levy co-funding budget | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        
#       And the transaction types for the payments for provider DEF are:
#               | Payment type                   | 09/18 | 10/18 | 11/18 | ... | 07/19 | 08/19 |
#               | On-program                     | 1000  | 1000  | 1000  | ... | 1000  | 1000  |
#               | Completion                     | 0     | 0     | 0     | ... | 0     | 0     |
#               | Balancing                      | 0     | 0     | 0     | ... | 0     | 0     |
#       
#       
#       And OBSOLETE - the provider earnings and payments break down as follows:
#                | Type                           | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | ... | 07/19 | 08/19 |
#                | Provider Earned Total          | 1500  | 1500  | 1500  | 1500  | 1500  | ... | 1500  | 0     |
#                | Provider Earned from SFA       | 1500  | 1500  | 1420  | 1475  | 1475  | ... | 1475  | 0     |
#                | Provider Earned from Employer  | 0     | 0     | 80    | 25    | 25    | ... | 25    | 0     |
#                | Provider Paid by SFA           | 0     | 1500  | 1500  | 1420  | 1475  | ... | 1475  | 1475  |
#                | Payment due from Employer      | 0     | 0     | 0     | 80    | 25    | ... | 25    | 25    |
#                | Levy account debited           | 0     | 1500  | 1500  | 700   | 1250  | ... | 1250  | 1250  |
#                | SFA Levy employer budget       | 1500  | 1500  | 700   | 1250  | 1250  | ... | 1250  | 0     |
#                | SFA Levy co-funding budget     | 0     | 0     | 720   | 225   | 225   | ... | 225   | 0     |
#                | SFA non-Levy co-funding budget | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |