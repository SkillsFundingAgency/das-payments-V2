@ignore
Feature: One Levy learner changes employer,incentive earned in transfer month but ILR transfer is before the commitment changes_PV2-374
As a provider,
I want 1 learner aged 16-18, levy available, changes employer, earns incentive payment in the commitment transfer month - and the ILR transfer happens at an earlier point than the commitment changes, to be paid the correct amount
So that I am accurately paid my apprenticeship provision.
Scenario: One Levy learner changes employer,incentive earned in transfer month but ILR transfer is before the commitment changes_PV2-374
Given the "employer 1" levy account balance in collection period R03/Current Academic Year is 17000
And  the "employer 2" levy account balance in collection period R03/Current Academic Year is 17000
And the following commitments exist 
    | Identifier            | Employer   | start date                   | end date                  | agreed price | status  | effective from               | effective to                 | stop effective from          |
    | Apprentiiship 1       | employer 1 | 01/Aug/Current Academic Year | 28/Aug/Next Academic Year | 7500         | stopped | 01/Aug/Current Academic Year | 14/Nov/Current Academic Year | 15/Nov/Current Academic Year |
    | Apprentiiship 2       | employer 2 | 15/Nov/Current Academic Year | 28/Aug/Next Academic Year | 5625         | active  | 15/Nov/Current Academic Year |                              |                              |
And the provider previously submitted the following learner details
	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
	| 01/Aug/Current Academic Year | 12 months        | 6000                 | 06/Aug/Current Academic Year        | 1500                   | 06/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
And the following earnings had been generated for the learner
    | Delivery Period           | On-Programme | Completion | Balancing | First16To18EmployerIncentive | Second16To18EmployerIncentive | First16To18ProviderIncentive | Second16To18ProviderIncentive |
    | Aug/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
    | Sep/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
    | Oct/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
    | Nov/Current Academic Year | 500          | 0          | 0         | 500                          | 0                             | 500                          | 0                             |
    | Dec/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
    | Jan/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
    | Feb/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
    | Mar/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
    | Apr/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
    | May/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
    | Jun/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
    | Jul/Current Academic Year | 500          | 0          | 0         | 0                            | 500                           | 0                            | 500                           |
And the following provider payments had been generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Employer   |
    | R01/Current Academic Year | Aug/Current Academic Year | 500           | Learning         | employer 1 |
    | R02/Current Academic Year | Sep/Current Academic Year | 500           | Learning         | employer 1 |
    | R03/Current Academic Year | Oct/Current Academic Year | 500           | Learning         | employer 1 |
But the Provider now changes the Learner details as follows
	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
	| 01/Aug/Current Academic Year | 12 months        | 6000                 | 06/Aug/Current Academic Year        | 3000                   | 06/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |
And price details as follows
    | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage |
    | pe-1             | 6000                 | 06/Aug/Current Academic Year        | 1500                   | 06/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         |
    | pe-2             | 6000                 | 06/Aug/Current Academic Year        | 1500                   | 06/Aug/Current Academic Year          | 4000                    | 09/Nov/Current Academic Year           | 1625                      | 09/Nov/Current Academic Year             | 90%                         |
When the amended ILR file is re-submitted for the learners in collection period R04/Current Academic Year
Then the following learner earnings should be generated
	| Delivery Period           | On-Programme | Completion | Balancing | First16To18EmployerIncentive | Second16To18EmployerIncentive | First16To18ProviderIncentive | Second16To18ProviderIncentive | Price Episode Identifier |
	| Aug/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
	| Sep/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
	| Oct/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
	| Nov/Current Academic Year | 500          | 0          | 0         | 500                          | 0                             | 500                          | 0                             | pe-2                     |
	| Dec/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-2                     |
	| Jan/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-2                     |
	| Feb/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-2                     |
	| Mar/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-2                     |
	| Apr/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-2                     |
	| May/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-2                     |
	| Jun/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-2                     |
	| Jul/Current Academic Year | 500          | 0          | 0         | 0                            | 500                           | 0                            | 500                           | pe-2                     |
And Month end is triggered
And no provider payments will be recorded
And no provider payments will be generated

#Scenario: 1 learner aged 16-18, levy available, changes employer, earns incentive payment in the commitment transfer month - and the ILR transfer happens at an earlier point than the commitment changes 
## The incentives are not paid for november as there is a failing datalock for november and the month is ignored
# 
#        Given The learner is programme only DAS
#        And the employer 1 has a levy balance > agreed price for all months
#        And the employer 2 has a levy balance > agreed price for all months
#        And the learner changes employers
#            | Employer   | Type | ILR employment start date |
#            | employer 1 | DAS  | 06/08/2018                |
#            | employer 2 | DAS  | 09/11/2018                |
#        And the following commitments exist:
#            | Employer   | commitment Id | version Id | Provider   | ULN       | start date | end date   | agreed price | status    | effective from | effective to | stop effective from |
#            | employer 1 | 1             | 1-001      | provider a | learner a | 01/08/2018 | 01/08/2019 | 7500         | cancelled | 01/08/2018     | 14/11/2018   | 15/11/2018   |
#            | employer 2 | 2             | 1-001      | provider a | learner a | 15/11/2018 | 01/08/2019 | 5625         | active    | 15/11/2018     |              |              |
#       
#        When an ILR file is submitted with the following data:
#            | Provider   | learner type             | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | Residual training price | Residual training price effective date | Residual assessment price | Residual assessment price effective date |
#            | provider a | 16-18 programme only DAS | learner a | 06/08/2018 | 08/08/2019       |                 | continuing        | 6000                 | 06/08/2018                          | 1500                   | 06/08/2018                            | 4000                    | 09/11/2018                             | 1625                      | 09/11/2018                               |
#    
#        #Then the data lock status will be as follows:
#        #    | Payment type             | 08/18               | 09/18               | 10/18               | 11/18               | 12/18 | 01/19 |
#        #    | On-program               | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-001 |                     |       |       |
#        #    | Employer 16-18 incentive |                     |                     |                     | commitment 1 v1-001 |       |       |
#        #    | Provider 16-18 incentive |                     |                     |                     | commitment 1 v1-001 |       |       |
#        
#        Then the earnings and payments break down for provider a is as follows:
#            | Type                                | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | 01/19 |
#            | Provider Earned Total               | 500   | 500   | 500   | 1500  | 500   | 500   |
#            | Provider Earned from SFA            | 500   | 500   | 500   | 0     | 0     | 0     |
#            | Provider Earned from Employer 1     | 0     | 0     | 0     | 0     | 0     | 0     |
#            | Provider Earned from Employer 2     | 0     | 0     | 0     | 0     | 0     | 0     |
#            | Provider Paid by SFA                | 0     | 500   | 500   | 500   | 0     | 0     |
#            | Payment due from employer 1         | 0     | 0     | 0     | 0     | 0     | 0     |
#            | Payment due from employer 2         | 0     | 0     | 0     | 0     | 0     | 0     |
#            | Employer 1 Levy account debited     | 0     | 500   | 500   | 500   | 0     | 0     |
#            | Employer 2 Levy account debited     | 0     | 0     | 0     | 0     | 0     | 0     |
#            | SFA Levy employer budget            | 500   | 500   | 500   | 0     | 0     | 0     |
#            | SFA Levy co-funding budget          | 0     | 0     | 0     | 0     | 0     | 0     |
#            | SFA non-Levy co-funding budget      | 0     | 0     | 0     | 0     | 0     | 0     |
#            | SFA Levy additional payments budget | 0     | 0     | 0     | 0     | 0     | 0     |
#            
#         And the transaction types for the payments for provider a are:
#            | Payment type               | 09/18 | 10/18 | 11/18 | 12/18 | 01/19 |
#            | On-program                 | 500   | 500   | 500   | 0     | 0     |
#            | Completion                 | 0     | 0     | 0     | 0     | 0     |
#            | Balancing                  | 0     | 0     | 0     | 0     | 0     |
#            | Employer 1 16-18 incentive | 0     | 0     | 0     | 0     | 0     |
#            | Employer 2 16-18 incentive | 0     | 0     | 0     | 0     | 0     |
#            | Provider 16-18 incentive   | 0     | 0     | 0     | 0     | 0     |

# For DC integration
#        And the learner changes employers
#            | Employer   | Type | ILR employment start date |
#            | employer 1 | DAS  | 01/08/2018                |
#            | employer 2 | DAS  | 15/11/2018                |