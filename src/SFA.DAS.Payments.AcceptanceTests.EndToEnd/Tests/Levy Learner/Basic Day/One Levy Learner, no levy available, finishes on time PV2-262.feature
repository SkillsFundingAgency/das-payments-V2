#1 Levy learner, no Levy available, finishes on time
#
#Notes: 
#This scenario is to accomodate when no Levy is available in the account - although the learner is regarded as ACT1 - Levy learner.
#
#The Commitment line includes an attribute called priority.  The employer can select which learner they want 
#to spend their Levy on in a priority order.  Where ULN is mentioned in the ILR line, learner a is used.  I understand 
#that this is for simplicity reasons- though this is open to change as required by the new implementation.
#
#
#Given levy balance = 0 for all months
#		
#And the following commitments exist:
#            | ULN       | priority | start date | end date   | agreed price |
#            | learner a | 1        | 01/09/2018 | 08/09/2019 | 15000        |
#        
#		When an ILR file is submitted with the following data:
#            | ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status |
#            | learner a | programme only DAS | 15000        | 01/09/2018 | 08/09/2019       | 08/09/2019      | completed         |
#        
#		Then the provider earnings and payments break down as follows:
#            | Type                          | 09/18 | 10/18 | 11/18 | ... | 08/19 | 09/19 | 10/19 |
#            | Provider Earned Total         | 1000  | 1000  | 1000  | ... | 1000  | 3000  | 0     |
#            | Provider Earned from SFA      | 900   | 900   | 900   | ... | 900   | 2700  | 0     |
#            | Provider Earned from Employer | 100   | 100   | 100   | ... | 100   | 300   | 0     |
#            | Provider Paid by SFA          | 0     | 900   | 900   | ... | 900   | 900   | 2700  |
#            | Payment due from Employer     | 0     | 100   | 100   | ... | 100   | 100   | 300   |
#            | Levy account debited          | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#            | SFA Levy employer budget      | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#            | SFA Levy co-funding budget    | 900   | 900   | 900   | ... | 900   | 2700  | 0     |
#            | SFA non-Levy co-funding budget| 0     | 0     | 0     | ... | 0     | 0     | 0     |
#
#
# New Given for levy balance
# New Commitments line
# New column - Levy Payments
Feature: One Levy learner - no levy available, finished on time PV2-262
	As a provider,
	I want a levy learner where no levy is available that finishes on time to be paid the balancing and completion payments,
	So that I am accurately paid my apprenticeship provision.
Scenario Outline: One levy learner, levy available, finished on time PV2-262
	# levy balance = 0 for all months
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	# Commitment line
	And the following commitments exist
        | start date                | end date                     | agreed price |
        | 01/Sep/Last Academic Year | 08/Sep/Current Academic Year | 15000        |
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Sep/Last Academic Year | 12 months        | 15000                | 01/Sep/Last Academic Year           | 0                      | 01/Sep/Last Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing |
        | Aug/Last Academic Year | 0            | 0          | 0         |
        | Sep/Last Academic Year | 1000         | 0          | 0         |
        | Oct/Last Academic Year | 1000         | 0          | 0         |
        | Nov/Last Academic Year | 1000         | 0          | 0         |
        | Dec/Last Academic Year | 1000         | 0          | 0         |
        | Jan/Last Academic Year | 1000         | 0          | 0         |
        | Feb/Last Academic Year | 1000         | 0          | 0         |
        | Mar/Last Academic Year | 1000         | 0          | 0         |
        | Apr/Last Academic Year | 1000         | 0          | 0         |
        | May/Last Academic Year | 1000         | 0          | 0         |
        | Jun/Last Academic Year | 1000         | 0          | 0         |
        | Jul/Last Academic Year | 1000         | 0          | 0         |
	# Levy Payments - 0 for all
	And the following provider payments had been generated
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R02/Last Academic Year | Sep/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | R03/Last Academic Year | Oct/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | R04/Last Academic Year | Nov/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | R05/Last Academic Year | Dec/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | R06/Last Academic Year | Jan/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | R07/Last Academic Year | Feb/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | R08/Last Academic Year | Mar/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | R09/Last Academic Year | Apr/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | R10/Last Academic Year | May/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | R11/Last Academic Year | Jun/Last Academic Year | 900                    | 100                         | 0             | Learning         |
        | R12/Last Academic Year | Jul/Last Academic Year | 900                    | 100                         | 0             | Learning         |
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Sep/Last Academic Year | 12 months        | 15000                | 01/Sep/Last Academic Year           | 0                      | 01/Sep/Last Academic Year             | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 1000         | 0          | 0         |
		| Sep/Current Academic Year | 0            | 3000       | 0         |
		| Oct/Current Academic Year | 0            | 0          | 0         |
		| Nov/Current Academic Year | 0            | 0          | 0         |
		| Dec/Current Academic Year | 0            | 0          | 0         |
		| Jan/Current Academic Year | 0            | 0          | 0         |
		| Feb/Current Academic Year | 0            | 0          | 0         |
		| Mar/Current Academic Year | 0            | 0          | 0         |
		| Apr/Current Academic Year | 0            | 0          | 0         |
		| May/Current Academic Year | 0            | 0          | 0         |
		| Jun/Current Academic Year | 0            | 0          | 0         |
		| Jul/Current Academic Year | 0            | 0          | 0         |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
        | R02/Current Academic Year | Sep/Current Academic Year | 0            | 3000       | 0         |
	# Levy Payments
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 2700                   | 300                         | 0             | Completion       |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 2700                   | 300                         | 0             | Completion       |
Examples: 
        | Collection_Period         | Levy Balance |
        | R01/Current Academic Year | 0            |
        | R02/Current Academic Year | 0            |
        | R03/Current Academic Year | 0            |