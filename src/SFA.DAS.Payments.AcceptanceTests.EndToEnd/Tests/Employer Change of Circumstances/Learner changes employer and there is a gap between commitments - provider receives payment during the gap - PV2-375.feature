@ignore
Feature:Learner changes employer gap between commitments - provider receives payment during the gap - PV2-375
As a provider,
I want provider earnings and payments where learner changes employer and there is a gap between commitments - provider receives payment during the gap as they amend the ACT code and employment status code correctly, to be paid the correct amount
So that I am accurately paid my apprenticeship provision.
Scenario Outline: Learner changes employer and there is a gap between commitments - provider receives payment during the gap - PV2-375
Given the "employer 1" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 1>
And  the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
And the following commitments exist
    | Employer   | start date                   | end date                  | agreed price | status    | effective from               | effective to                 | stop effective from          |
    | employer 1 | 01/Aug/Current Academic Year | 04/Aug/Next Academic Year | 15000        | cancelled | 01/Aug/Current Academic Year | 02/Oct/Current Academic Year | 03/Oct/Current Academic Year |
    | employer 2 | 01/Nov/Current Academic Year | 04/Aug/Next Academic Year | 5625         | active    | 01/Nov/Current Academic Year |                              |                              |
And the provider is providing training for the following learners
	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
	| 03/Aug/Current Academic Year | 12 months        | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |
And price details as follows
    | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | SFA Contribution Percentage |
    | pe-1             | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | Act1          | 90%                         |
	| pe-2             | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | Act2          | 90%                         |
    | pe-3             | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          | 5000                    | 03/Nov/Current Academic Year           | 625                       | 03/Nov/Current Academic Year             | Act1          | 90%                         |
When the ILR file is submitted for the learners for collection period <Collection_Period>
Then the following learner earnings should be generated
	| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
	#p1
    | Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
    | Sep/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
	#p2
    | Oct/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
	#p3
    | Nov/Current Academic Year | 500          | 0          | 0         | pe-3                     |
    | Dec/Current Academic Year | 500          | 0          | 0         | pe-3                     |
    | Jan/Current Academic Year | 500          | 0          | 0         | pe-3                     |
    | Feb/Current Academic Year | 500          | 0          | 0         | pe-3                     |
    | Mar/Current Academic Year | 500          | 0          | 0         | pe-3                     |
    | Apr/Current Academic Year | 500          | 0          | 0         | pe-3                     |
    | May/Current Academic Year | 500          | 0          | 0         | pe-3                     |
    | Jun/Current Academic Year | 500          | 0          | 0         | pe-3                     |
    | Jul/Current Academic Year | 500          | 0          | 0         | pe-3                     |
And at month end only the following payments will be calculated
    | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
    | R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
    | R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
    | R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
    | R04/Current Academic Year | Nov/Current Academic Year | 500          | 0          | 0         |
    | R05/Current Academic Year | Dec/Current Academic Year | 500          | 0          | 0         |
And only the following provider payments will be recorded
    | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type | Employer    |
    | R01/Current Academic Year | Aug/Current Academic Year | 1000          | 0                         | Learning         | employer 1  |
    | R02/Current Academic Year | Sep/Current Academic Year | 1000          | 0                         | Learning         | employer 1  |
    | R03/Current Academic Year | Oct/Current Academic Year | 0             | 1000                      | Learning         | no employer |
    | R04/Current Academic Year | Nov/Current Academic Year | 500           | 0                         | Learning         | employer 2  |
    | R05/Current Academic Year | Dec/Current Academic Year | 500           | 0                         | Learning         | employer 2  |
And only the following provider payments will be generated
    | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type | Employer    |
    | R01/Current Academic Year | Aug/Current Academic Year | 1000          | 0                         | Learning         | employer 1  |
    | R02/Current Academic Year | Sep/Current Academic Year | 1000          | 0                         | Learning         | employer 1  |
    | R03/Current Academic Year | Oct/Current Academic Year | 0             | 1000                      | Learning         | no employer |
    | R04/Current Academic Year | Nov/Current Academic Year | 500           | 0                         | Learning         | employer 2  |
    | R05/Current Academic Year | Dec/Current Academic Year | 500           | 0                         | Learning         | employer 2  |

Examples:
	| Collection_Period         | Levy Balance for employer 1 | Levy Balance for employer 2 |
	| R01/Current Academic Year | 15500                       | 6125                        |
	| R02/Current Academic Year | 14500                       | 6125                        |
	| R03/Current Academic Year | 13500                       | 6125                        |
	| R04/Current Academic Year | 13500                       | 6125                        |
	| R05/Current Academic Year | 13500                       | 5625                        |
#@LearnerChangesEmployerGapInCommitments
#AC1
		#Scenario:AC1- Provider earnings and payments where learner changes employer and there is a gap between commitments - provider receives payment during the gap as they amend the ACT code and employment status code correctly.
  #      Given The learner is programme only DAS
  #      And the apprenticeship funding band maximum is 17000
  #      And the employer 1 has a levy balance > agreed price for all months
  #      And the employer 2 has a levy balance > agreed price for all months
  #      And the learner changes employers
  #          | Employer    | Type    | ILR employment start date |   
  #          | employer 1  | DAS     | 03/08/2018                |
  #          | No employer | Non-DAS | 03/10/2018                |
  #          | employer 2  | DAS     | 03/11/2018                |
  #      And the following commitments exist:
  #          | commitment Id | version Id | Employer   | Provider   | ULN       | start date | end date   | agreed price | status    | effective from | effective to | stop effective from |
  #          | 1             | 1-001      | employer 1 | provider a | learner a | 01/08/2018 | 04/08/2019 | 15000        | Cancelled | 01/08/2018     | 02/10/2018   | 03/10/2018   |
  #          | 2             | 1-001      | employer 2 | provider a | learner a | 01/11/2018 | 04/08/2019 | 5625         | Active    | 01/11/2018     |              |              |
  #      When an ILR file is submitted with the following data:
  #          | ULN       | Provider   | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | Residual training price | Residual training price effective date | Residual assessment price | Residual assessment price effective date |
  #          | learner a | provider a | 03/08/2018 | 04/08/2019       |                 | continuing        | 12000                | 03/08/2018                          | 3000                   | 03/08/2018                            | 4500                    | 03/11/2018                             | 1125                      | 03/11/2018                               |
  #      And the Contract type in the ILR is:
  #          | contract type | date from  | date to    |
  #          | DAS           | 03/08/2018 | 02/10/2018 |
  #          | Non-DAS       | 03/10/2018 | 02/11/2018 |
  #          | DAS           | 03/11/2018 | 04/08/2019 |
  #      And the employment status in the ILR is:
  #          | Employer   | Employment Status      | Employment Status Applies |
  #          | employer 1 | in paid employment     | 02/08/2018                |
  #          |            | not in paid employment | 03/10/2018                |
  #          | employer 2 | in paid employment     | 03/11/2018                |         
  #   
  #      #Then the data lock status will be as follows:
  #      #    | Payment type | 08/18               | 09/18               | 10/18 | 11/18               | 12/18               |
  #      #    | On-program   | commitment 1 v1-001 | commitment 1 v1-001 |       | commitment 2 v1-001 | commitment 2 v1-001 |
  #      Then the provider earnings and payments break down as follows:
  #          | Type                            | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 |
  #          | Provider Earned Total           | 1000  | 1000  | 1000  | 500   | 500   |
  #          | Provider Paid by SFA            | 0     | 1000  | 1000  | 1000  | 500   |
  #          | employer 1 Levy account debited | 0     | 1000  | 1000  | 0     | 0     |
  #          | employer 2 Levy account debited | 0     | 0     | 0     | 0     | 500   |
  #          | SFA Levy employer budget        | 1000  | 1000  | 0     | 500   | 500   |
  #          | SFA Levy co-funding budget      | 0     | 0     | 0     | 0     | 0     |
  #          | SFA non-Levy co-funding budget  | 0     | 0     | 1000  | 0     | 0     |
