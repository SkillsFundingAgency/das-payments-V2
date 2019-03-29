  #Scenario: Earnings and payments for a DAS learner, levy available, and there is a change to the Negotiated Cost which happens in the middle of the month
  #      Given the following commitments exist:
  #          | commitment Id | version Id | ULN       | start date | end date   | status | agreed price | effective from | effective to |
  #          | 1             | 1-001      | learner a | 01/08/2018 | 31/08/2019 | active | 15000        | 01/08/2018     | 31/10/2018   |
  #          | 1             | 1-002      | learner a | 01/08/2018 | 31/08/2019 | active | 9375         | 01/11/2018     |              |
  #      When an ILR file is submitted with the following data:
  #          | ULN       | start date | planned end date | actual end date | completion status | Total training price 1 | Total training price 1 effective date | Total assessment price 1 | Total assessment price 1 effective date | Total training price 2 | Total training price 2 effective date | Total assessment price 2 | Total assessment price 2 effective date |
  #          | learner a | 01/08/2018 | 04/08/2019       |                 | continuing        | 12000                  | 01/08/2018                            | 3000                     | 01/08/2018                              | 7500                   | 10/11/2018                            | 1875                     | 10/11/2018                              |
  #      #Then the data lock status will be as follows:
  #      #    | Payment type | 08/18               | 09/18               | 10/18               | 11/18               | 12/18               | ... | 07/19               |
  #      #    | On-program   | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-002 | commitment 1 v1-002 | ... | commitment 1 v1-002 | 
  #      Then the provider earnings and payments break down as follows:
  #          | Type                          | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | ... | 07/19 | 08/19 |
  #          | Provider Earned Total         | 1000  | 1000  | 1000  | 500   | 500   | ... | 500   | 0     |
  #          | Provider Earned from SFA      | 1000  | 1000  | 1000  | 500   | 500   | ... | 500   | 0     |
  #          | Provider Earned from Employer | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
  #          | Provider Paid by SFA          | 0     | 1000  | 1000  | 1000  | 500   | ... | 500   | 500   |
  #          | Payment due from Employer     | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
  #          | Levy account debited          | 0     | 1000  | 1000  | 1000  | 500   | ... | 500   | 500   |
  #          | SFA Levy employer budget      | 1000  | 1000  | 1000  | 500   | 500   | ... | 500   | 0     |
  #          | SFA Levy co-funding budget    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |

Feature: Levy learner, and there is a change to the negotiated price which happens during the month - PV2-295
	As a provider,
	I want a levy learner, changes to the agreed price during the month, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.

Scenario Outline: Levy learner changes to the agreed price during the month PV2-295
	Given the employer levy account balance in collection period R01/Current Academic Year is 15000
	# Additional fields
	# Do I need to move the second commitment after history?	
	And the following commitments exist
        | commitment Id | version Id | start date                   | end date                  | status | agreed price | effective from               | effective to                 |
        | 29500         | 1-001      | 01/Aug/Current Academic Year | 31/Aug/Next Academic Year | active | 15000        | 01/Aug/Current Academic Year | 31/Oct/Current Academic Year |
        | 29500         | 1-002      | 01/Aug/Current Academic Year | 31/Aug/Next Academic Year | active | 9375         | 01/Nov/Current Academic Year |                              |
	
	And the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Aug/Current Academic Year | 12 months        | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    
	And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 1000         | 0          | 0         |
        | Sep/Current Academic Year | 1000         | 0          | 0         |
        | Oct/Current Academic Year | 1000         | 0          | 0         |
        | Nov/Current Academic Year | 1000         | 0          | 0         |
        | Dec/Current Academic Year | 1000         | 0          | 0         |
        | Jan/Current Academic Year | 1000         | 0          | 0         |
        | Feb/Current Academic Year | 1000         | 0          | 0         |
        | Mar/Current Academic Year | 1000         | 0          | 0         |
        | Apr/Current Academic Year | 1000         | 0          | 0         |
        | May/Current Academic Year | 1000         | 0          | 0         |
        | Jun/Current Academic Year | 1000         | 0          | 0         |
        | Jul/Current Academic Year | 1000         | 0          | 0         |
    
	And the following provider payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |        
    
	But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Aug/Current Academic Year | 12 months        | 7500                 | 10/Nov/Current Academic Year        | 1875                   | 10/Nov/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    
	Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 1000         | 0          | 0         |
        | Sep/Current Academic Year | 1000         | 0          | 0         |
        | Oct/Current Academic Year | 1000         | 0          | 0         |
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
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R04/Current Academic Year | Nov/Current Academic Year | 500          | 0          | 0         |
        | R05/Current Academic Year | Dec/Current Academic Year | 500          | 0          | 0         |
        | R06/Current Academic Year | Jan/Current Academic Year | 500          | 0          | 0         |
        | R07/Current Academic Year | Feb/Current Academic Year | 500          | 0          | 0         |
        | R08/Current Academic Year | Mar/Current Academic Year | 500          | 0          | 0         |
        | R09/Current Academic Year | Apr/Current Academic Year | 500          | 0          | 0         |
        | R10/Current Academic Year | May/Current Academic Year | 500          | 0          | 0         |
        | R11/Current Academic Year | Jun/Current Academic Year | 500          | 0          | 0         |
        | R12/Current Academic Year | Jul/Current Academic Year | 500          | 0          | 0         |
    
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R04/Current Academic Year | Nov/Current Academic Year | 500           | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 500           | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 500           | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 500           | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 500           | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 500           | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 500           | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 500           | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 500           | Learning         |
	
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R04/Current Academic Year | Nov/Current Academic Year | 500           | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 500           | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 500           | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 500           | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 500           | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 500           | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 500           | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 500           | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 500           | Learning         |
Examples: 
        | Collection_Period         | 
        | R04/Current Academic Year | 
        | R05/Current Academic Year | 
        | R06/Current Academic Year | 
        | R07/Current Academic Year | 
        | R08/Current Academic Year | 
        | R09/Current Academic Year | 
        | R10/Current Academic Year | 
        | R11/Current Academic Year | 
        | R12/Current Academic Year | 