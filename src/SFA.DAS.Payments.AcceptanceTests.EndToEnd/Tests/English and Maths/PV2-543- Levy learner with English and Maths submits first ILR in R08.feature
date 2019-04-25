#Feature:  Maths and English	
#
#Scenario: Levy learner, provider submits first ILR in R08 showing ACT1 from the start of learning. Correct Maths or English refunds are processed
#
#    Given levy balance > agreed price for all months
#	And the apprenticeship funding band maximum is 9000
#
#	And the following commitments exist:
#		| commitment Id | version Id | ULN       | start date | end date   | framework code | programme type | pathway code | agreed price | status | effective from | effective to |
#		| 1             | 1          | learner a | 01/08/2018 | 01/08/2019 | 403            | 2              | 1            | 9000         | Active | 01/08/2018     |              |
#        
#	When an ILR file is submitted for period R08 with the following data:
#        | ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | aim type         | aim sequence number | aim rate | framework code | programme type | pathway code | contract type | contract type date from |
#        | learner a | programme only DAS | 9000         | 06/08/2018 | 20/08/2019       |                 | continuing        | programme        | 2                   |          | 403            | 2              | 1            | DAS           | 06/08/2018              |
#        | learner a | programme only DAS |              | 06/08/2018 | 20/08/2019       |                 | continuing        | maths or english | 1                   | 471      | 403            | 2              | 1            |               |                         |
#        
#  
#	Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/18  | 09/18  | 10/18  | 11/18  | 12/18  | 01/19  | 02/19  | 03/19  | 04/19   | 05/19  |
#        | Provider Earned Total                   | 639.25 | 639.25 | 639.25 | 639.25 | 639.25 | 639.25 | 639.25 | 639.25 | 639.25  | 639.25 |
#        | Provider Earned from SFA                | 639.25 | 639.25 | 639.25 | 639.25 | 639.25 | 639.25 | 639.25 | 639.25 | 639.25  | 639.25 |
#        | Provider Earned from Employer           | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0       | 0      |
#        | Provider Paid by SFA                    | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 5114.00 | 639.25 |
#        | Refund taken by SFA                     | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0       | 0      |
#        | Payment due from Employer               | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0       | 0      |
#        | Refund due to employer                  | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0       | 0      |
#        | Levy account debited                    | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 4800    | 600    |
#        | Levy account credited                   | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0       | 0      |
#        | SFA Levy employer budget                | 600    | 600    | 600    | 600    | 600    | 600    | 600    | 600    | 600     | 600    |
#        | SFA Levy co-funding budget              | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0       | 0      |
#        | SFA Levy additional payments budget     | 39.25  | 39.25  | 39.25  | 39.25  | 39.25  | 39.25  | 39.25  | 39.25  | 39.25   | 39.25  |
#        | SFA non-Levy co-funding budget          | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0       | 0      |
#        | SFA non-Levy additional payments budget | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0      | 0       | 0      |
#		
#	And the transaction types for the payments are:
#		| Payment type                   | 09/18 | 10/18 | 11/18 | 12/18 | ... | 07/19 | 08/19 |
#		| On-program                     | 600   | 600   | 600   | 600   | ... | 600   | 600   |
#		| Completion                     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#		| Balancing                      | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | English and maths on programme | 39.25 | 39.25 | 39.25 | 39.25 | ... | 39.25 | 39.25 |
#		| English and maths Balancing    | 0     | 0     | 0     | 0     | ... | 0     | 0     |	


Feature: Levy learner with English & Maths submits first ILR in R08 - PV2-543
		As a provider,
		I want a Levy learner with English & Maths aim, where the, provider submits first ILR in R08 showing ACT1 from the start of learning and the correct Maths or English refunds are processed
		So that I am accurately paid my apprenticeship provision

Scenario Outline: Levy learner with English & Maths submits first ILR in R08 PV2-543
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
        | start date                   | end date                  | agreed price | status |
        | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 9000         | active |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status |
		| Programme        | ZPROG001      | 01/Aug/Current Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
		| Maths or English | 12345         | 01/Aug/Current Academic Year | 12 months        |                 | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
	And price details as follows	
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 9000                 | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          | Act1          | 1                   | 90%                         |
        |                  | 0                    | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          | Act1          | 2                   | 100%                        |
	When the ILR file is submitted for the learners for collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		#p1
		| Aug/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Sep/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Oct/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Nov/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Dec/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Jan/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Feb/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Mar/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Apr/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
		| May/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Jun/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Jul/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
		#p2
		| Aug/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
		| Sep/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
		| Oct/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
		| Nov/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
		| Dec/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
		| Jan/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
		| Feb/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
		| Mar/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
		| Apr/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
		| May/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
		| Jun/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
		| Jul/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
		| R08/Current Academic Year | Aug/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R08/Current Academic Year | Sep/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R08/Current Academic Year | Oct/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | R08/Current Academic Year | Nov/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | R08/Current Academic Year | Dec/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R08/Current Academic Year | Jan/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R08/Current Academic Year | Feb/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R08/Current Academic Year | Mar/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R09/Current Academic Year | Apr/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R10/Current Academic Year | May/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R11/Current Academic Year | Jun/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R12/Current Academic Year | Jul/Current Academic Year | 600          | 0          | 0         | 39.25                      |
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R08/Current Academic Year | Aug/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Sep/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Oct/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Nov/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Dec/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Jan/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Feb/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Mar/Current Academic Year | 600           | 0                         | Learning                   |
        | R09/Current Academic Year | Apr/Current Academic Year | 600           | 0                         | Learning                   |
        | R10/Current Academic Year | May/Current Academic Year | 600           | 0                         | Learning                   |
        | R11/Current Academic Year | Jun/Current Academic Year | 600           | 0                         | Learning                   |
        | R12/Current Academic Year | Jul/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Aug/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Sep/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Oct/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Nov/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Dec/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Jan/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Feb/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Mar/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R09/Current Academic Year | Apr/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R10/Current Academic Year | May/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R11/Current Academic Year | Jun/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Current Academic Year | Jul/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R08/Current Academic Year | Aug/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Sep/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Oct/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Nov/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Dec/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Jan/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Feb/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Mar/Current Academic Year | 600           | 0                         | Learning                   |
        | R09/Current Academic Year | Apr/Current Academic Year | 600           | 0                         | Learning                   |
        | R10/Current Academic Year | May/Current Academic Year | 600           | 0                         | Learning                   |
        | R11/Current Academic Year | Jun/Current Academic Year | 600           | 0                         | Learning                   |
        | R12/Current Academic Year | Jul/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Aug/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Sep/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Oct/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Nov/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Dec/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Jan/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Feb/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Mar/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R09/Current Academic Year | Apr/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R10/Current Academic Year | May/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R11/Current Academic Year | Jun/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Current Academic Year | Jul/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
Examples: 
        | Collection_Period         | Levy Balance |
        | R08/Current Academic Year | 9000         |
        | R09/Current Academic Year | 4200         |
        | R10/Current Academic Year | 3600         |
        | R11/Current Academic Year | 3000         |
        | R12/Current Academic Year | 2400         |