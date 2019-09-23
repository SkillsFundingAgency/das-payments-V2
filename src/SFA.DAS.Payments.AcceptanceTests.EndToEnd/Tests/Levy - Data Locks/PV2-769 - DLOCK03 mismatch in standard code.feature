Feature: PV2-769 - DLOCK03 mismatch in standard code
#
#Background: DLOCK_03 mismatch in standard code 
# 
#Scenario: ILR changes before second Commitment starts (i.e. there is only one existing Commitment in place)
#
#        Given the following commitments exist on 03/12/2018:
#            | commitment Id | version Id | ULN       | standard code | start date | end date   | agreed price |
#            | 1             |  1-001     | learner a | 51            | 03/08/2018 | 04/08/2019 | 15000        |
#        
#		When an ILR file is submitted on 03/12/2018 with the following data:
#            | ULN       | standard code | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#            | learner a | 51            | 03/08/2018 | 01/08/2019       | 31/10/2018      | withdrawn         | 12000                | 03/08/2018                          | 3000                   | 03/08/2018                            |
#            | learner a | 52            | 03/11/2018 | 01/08/2019       |                 | continuing        | 4500                 | 03/11/2018                          | 1125                   | 03/11/2018                            |
#        
#		Then the data lock status of the ILR in 03/12/2018 is:
#            | Payment type | 08/18               | 09/18               | 10/18               | 11/18 | 12/18 |
#            | On-program   | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-001 |       |       |
#        
#		And the provider earnings and payments break down as follows:
#            | Type                          | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 |
#            | Provider Earned Total         | 1000  | 1000  | 1000  | 500   | 500   |
#            | Provider Earned from SFA      | 1000  | 1000  | 1000  | 500   | 500   |
#            | Provider Paid by SFA          | 0     | 1000  | 1000  | 1000  | 0     |
#            | Levy account debited          | 0     | 1000  | 1000  | 1000  | 0     |
#            | SFA Levy employer budget      | 1000  | 1000  | 1000  | 0     | 0     |
#            | SFA Levy co-funding budget    | 0     | 0     | 0     | 0     | 0     |


Background: DLOCK_03 mismatch in standard code
	As a Provider,
	I want to be notified with a data lock when no matching record found in an employer digital account for the Standard Code
	So that I can correct the data mis-match between the Commitment and ILR PV2-769
 
Scenario: ILR changes before second Commitment starts (i.e. there is only one existing Commitment in place) produces DLOCK_03 PV2-769

	Given the employer levy account balance in collection period R01/Current Academic Year is 15000
	And the following commitments exist
		| Identifier       | standard code | programme type | pathway code | agreed price | start date | end date | status | effective from |
		| Apprenticeship a | 51            | 25             | 1            | 15000        | 03/Aug/Current Academic Year | 04/Aug/Current Academic Year | active | 03/Aug/Current Academic Year |
	And the following learners
        | Learner Reference Number | 
        | abc123                   | 
	And the following aims
		| Aim Type  | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Pathway Code | standard code | programme type | Funding Line Type                                  | Completion Status |
		| Programme | ZPROG001      | 03/Aug/Current Academic Year | 12 months        | 3 months        | 1                   | 1              | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | withdrawn         |
		| Programme | ZPROG001      | 02/Nov/Current Academic Year | 12 months        |                 | 2                   | 1              |52            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | continuing        |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Contract Type | SFA Contribution Percentage | Aim Sequence Number | Total Assessment Price |
		| pe-1             | 12000                | 03/Aug/Current Academic Year           | Act1          | 90%                         | 1                   | 3000                   |
		| pe-2             | 4500                 | 03/Nov/Current Academic Year           | Act1          | 90%                         | 2                   | 1125                   |
	When the amended ILR file is re-submitted for the learners in collection period R04/Current Academic Year

	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| Dec/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| Jan/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| Feb/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| Mar/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| Apr/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| May/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| Jun/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| Jul/Current Academic Year | 500          | 0          | 0         | pe-2                     |
	Then the following data lock failures were generated
        | Apprenticeship   | Delivery Period           | Transaction Type | standard code | programme type | pathway code | Error Code | Price Episode Identifier |
        | Apprenticeship a | Nov/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_03   | pe-2                     |
        | Apprenticeship a | Nov/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_07   | pe-2                     |
        | Apprenticeship a | Dec/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_03   | pe-2                     |
        | Apprenticeship a | Dec/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_07   | pe-2                     |
        | Apprenticeship a | Jan/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_03   | pe-2                     |
        | Apprenticeship a | Jan/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_07   | pe-2                     |
        | Apprenticeship a | Feb/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_03   | pe-2                     |
        | Apprenticeship a | Feb/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_07   | pe-2                     |
        | Apprenticeship a | Mar/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_03   | pe-2                     |
        | Apprenticeship a | Mar/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_07   | pe-2                     |
        | Apprenticeship a | Apr/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_03   | pe-2                     |
        | Apprenticeship a | Apr/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_07   | pe-2                     |
        | Apprenticeship a | May/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_03   | pe-2                     |
        | Apprenticeship a | May/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_07   | pe-2                     |
        | Apprenticeship a | Jun/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_03   | pe-2                     |
        | Apprenticeship a | Jun/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_07   | pe-2                     |
        | Apprenticeship a | Jul/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_03   | pe-2                     |
        | Apprenticeship a | Jul/Current Academic Year | Learning         | 52            | 25             | 1            | DLOCK_07   | pe-2                     |
	#
	#And only the following provider payments will be generated
	#	| Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type |
	#	| R01/Current Academic Year | Aug/Current Academic Year | 0             | 1000                      | Learning         |
	#	| R02/Current Academic Year | Sep/Current Academic Year | 1000          | 1000                      | Learning         |
	#	| R03/Current Academic Year | Oct/Current Academic Year | 1000          | 1000                      | Learning         |
	#	| R04/Current Academic Year | Nov/Current Academic Year | 1000          | 500                       | Learning         |
	#	| R05/Current Academic Year | Dec/Current Academic Year | 0             | 500                       | Learning         |
	#	| R06/Current Academic Year | Jan/Current Academic Year | 0             | 500                       | Learning         |
	#	| R07/Current Academic Year | Feb/Current Academic Year | 0             | 500                       | Learning         |
	#	| R08/Current Academic Year | Mar/Current Academic Year | 0             | 500                       | Learning         |
	#	| R09/Current Academic Year | Apr/Current Academic Year | 0             | 500                       | Learning         |
	#	| R10/Current Academic Year | May/Current Academic Year | 0             | 500                       | Learning         |
	#	| R11/Current Academic Year | Jun/Current Academic Year | 0             | 500                       | Learning         |
	#	| R12/Current Academic Year | Jul/Current Academic Year | 0             | 500                       | Learning         |

	#And only the following provider payments will be recorded
	#	| Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type                 |
	#	| R12/Current Academic Year | Aug/Current Academic Year | 666.66667     | 0                         | Learning                         |
	#	| R12/Current Academic Year | Sep/Current Academic Year | 666.66667     | 0                         | Learning                         |
		#And the provider earnings and payments break down as follows:
#            | Type                          | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 |
#            | Provider Earned Total         | 1000  | 1000  | 1000  | 500   | 500   |
#            | Provider Earned from SFA      | 1000  | 1000  | 1000  | 500   | 500   |
#            | Provider Paid by SFA          | 0     | 1000  | 1000  | 1000  | 0     |
#            | Levy account debited          | 0     | 1000  | 1000  | 1000  | 0     |
#            | SFA Levy employer budget      | 1000  | 1000  | 1000  | 0     | 0     |
#            | SFA Levy co-funding budget    | 0     | 0     | 0     | 0     | 0     |