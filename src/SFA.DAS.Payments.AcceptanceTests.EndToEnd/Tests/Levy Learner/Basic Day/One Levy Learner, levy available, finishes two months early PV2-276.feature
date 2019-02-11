  #  Scenario: A DAS learner, levy available, learner finishes two months early
  #      Given levy balance > agreed price for all months
		#And the following commitments exist:
  #          | ULN       | priority | start date | end date   | agreed price |
  #          | learner a | 1        | 01/09/2017 | 08/09/2018 | 15000        |
  #      When an ILR file is submitted with the following data:
  #          | ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status |
  #          | learner a | programme only DAS | 15000        | 01/09/2017 | 08/09/2018       | 08/07/2018      | completed         |
  #      Then the provider earnings and payments break down as follows:
  #          | Type                       | 09/17 | 10/17 | 11/17 | ... | 07/18 | 08/18 |
  #          | Provider Earned Total      | 1000  | 1000  | 1000  | ... | 5000  | 0     |
  #          | Provider Earned from SFA   | 1000  | 1000  | 1000  | ... | 5000  | 0     |
  #          | Provider Paid by SFA       | 0     | 1000  | 1000  | ... | 1000  | 5000  |
  #          | Levy account debited       | 0     | 1000  | 1000  | ... | 1000  | 5000  |
  #          | SFA Levy employer budget   | 1000  | 1000  | 1000  | ... | 5000  | 0     |
  #          | SFA Levy co-funding budget | 0     | 0     | 0     | ... | 0     | 0     |
  #      And the transaction types for the payments are:
  #          | Transaction type | 10/17 | 11/17 | ... | 07/18 | 08/18 |
  #          | On-program       | 1000  | 1000  | ... | 1000  | 0     |
  #          | Completion       | 0     | 0     | ... | 0     | 3000  |
  #          | Balancing        | 0     | 0     | ... | 0     | 2000  |
			
# levy balance > agreed price for all months
# Commitments line
# Levy Payments
Feature: One Levy learner - levy available, finishes two months early PV2-276
	As a provider,
	I want a levy learner, where levy is available and the learner finishes two months early to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.
Scenario Outline: One levy learner, levy available, learner finishes two months early PV2-276
	# levy balance > agreed price for all months
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
        | start date                | end date                     | agreed price |
        | 01/Sep/Current Academic Year | 08/Sep/Next Academic Year | 15000        |
	And the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Sep/Current Academic Year | 12 months        | 15000                | 01/Sep/Current Academic Year        | 0                      | 01/Sep/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 0            | 0          | 0         |
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
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 1000          | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 1000          | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 1000          | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 1000          | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 1000          | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 1000          | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 1000          | Learning         |
    But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Sep/Current Academic Year | 12 months        | 15000                | 01/Sep/Current Academic Year        | 0                      | 01/Sep/Current Academic Year          | 10 months       | completed         | Act1          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 0            | 0          | 0         |
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
		| Jul/Current Academic Year | 0            | 3000       | 2000      |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R12/Current Academic Year | Jul/Current Academic Year | 0            | 0          | 2000      |
        | R12/Current Academic Year | Jul/Current Academic Year | 0            | 3000       | 0         |
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R12/Current Academic Year | Jul/Current Academic Year | 2000          | Balancing        |
        | R12/Current Academic Year | Jul/Current Academic Year | 3000          | Completion       |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R12/Current Academic Year | Jul/Current Academic Year | 2000          | Balancing        |
        | R12/Current Academic Year | Jul/Current Academic Year | 3000          | Completion       |
Examples: 
        | Collection_Period         | Levy Balance |
        | R12/Current Academic Year | 5500         |