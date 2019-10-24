@basic_day
@supports_dc_e2e
Feature: One Levy learner - no levy available, finished on time PV2-262
	As a provider,
	I want a levy learner where no levy is available that finishes on time to be paid the balancing and completion payments,
	So that I am accurately paid my apprenticeship provision.
Scenario Outline: One levy learner, levy available, finished on time PV2-262
	# levy balance = 0 for all months
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	# Commitment line
	And the following commitments exist
        | start date                | end date                     | agreed price | Framework Code | Pathway Code | Programme Type |
        | 01/Sep/Last Academic Year | 08/Sep/Current Academic Year | 15000        | 593            | 1            | 20             |
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Sep/Last Academic Year | 12 months        | 15000                | 01/Sep/Last Academic Year           | 0                      | 01/Sep/Last Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract   | 90%                         |
    And price details as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 15000                | 01/Sep/Last Academic Year           |                        | 01/Sep/Last Academic Year             | Act1          | 1                   | 90%                         |
	And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | Price Episode Identifier |
        | Aug/Last Academic Year | 0            | 0          | 0         | pe-1                     |
        | Sep/Last Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Oct/Last Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Nov/Last Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Dec/Last Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Jan/Last Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Feb/Last Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Mar/Last Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Apr/Last Academic Year | 1000         | 0          | 0         | pe-1                     |
        | May/Last Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Jun/Last Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Jul/Last Academic Year | 1000         | 0          | 0         | pe-1                     |
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
		| 01/Sep/Last Academic Year | 12 months        | 15000                | 01/Sep/Last Academic Year           | 0                      | 01/Sep/Last Academic Year             | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract   | 90%                         |
	And price details as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 15000                | 01/Sep/Last Academic Year           |                        | 01/Sep/Last Academic Year             | Act1          | 1                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 0            | 3000       | 0         | pe-1                     |
		| Oct/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Mar/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Apr/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jun/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jul/Current Academic Year | 0            | 0          | 0         | pe-1                     |
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