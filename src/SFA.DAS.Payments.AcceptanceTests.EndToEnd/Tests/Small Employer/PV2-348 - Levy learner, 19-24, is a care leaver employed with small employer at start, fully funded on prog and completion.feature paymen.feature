Feature: Levy learner 19-24 is a care leaver employed with a small employer at start fully funded PV2-348
	As a provider,
	I want a levy learner aged 19-24 who is a care leaver, employed with a small employer at start, is fully funded for on programme and completion payments, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision - PV2-348

# for DC Integration
#    And the employment status in the ILR is:
#        | Employer    | Employment Status      | Employment Status Applies | Small Employer |
#        | employer 1  | in paid employment     | 05/08/2018                | SEM1           |
#	| LearnDelFAM |
#	| EEF4        |
Scenario Outline: Levy learner 19-24 is a care leaver employed with a small employer at start fully funded PV2-348
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
        | start date                | end date                     | agreed price |Framework Code | Pathway Code | Programme Type |
        | 06/Aug/Last Academic Year | 08/Aug/Current Academic Year | 7500         |593            | 1            | 20             |
	# 100% contribution for small employer, 19-24 learner due to EEF4 code
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 7500                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 100%                        |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | First16To18EmployerIncentive | First16To18ProviderIncentive | OnProgramme16To18FrameworkUplift |
        | Aug/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Sep/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Oct/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Nov/Last Academic Year | 500          | 0          | 0         | 500                          | 500                          | 120                              |
        | Dec/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Jan/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Feb/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Mar/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Apr/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | May/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Jun/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Jul/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type                 |
        | R01/Last Academic Year | Aug/Last Academic Year | 500                    | 0                         | Learning                         |
        | R02/Last Academic Year | Sep/Last Academic Year | 500                    | 0                         | Learning                         |
        | R03/Last Academic Year | Oct/Last Academic Year | 500                    | 0                         | Learning                         |
        | R04/Last Academic Year | Nov/Last Academic Year | 500                    | 0                         | Learning                         |
        | R05/Last Academic Year | Dec/Last Academic Year | 500                    | 0                         | Learning                         |
        | R06/Last Academic Year | Jan/Last Academic Year | 500                    | 0                         | Learning                         |
        | R07/Last Academic Year | Feb/Last Academic Year | 500                    | 0                         | Learning                         |
        | R08/Last Academic Year | Mar/Last Academic Year | 500                    | 0                         | Learning                         |
        | R09/Last Academic Year | Apr/Last Academic Year | 500                    | 0                         | Learning                         |
        | R10/Last Academic Year | May/Last Academic Year | 500                    | 0                         | Learning                         |
        | R11/Last Academic Year | Jun/Last Academic Year | 500                    | 0                         | Learning                         |
        | R12/Last Academic Year | Jul/Last Academic Year | 500                    | 0                         | Learning                         |
        | R01/Last Academic Year | Aug/Last Academic Year | 0                      | 120                       | OnProgramme16To18FrameworkUplift |
        | R02/Last Academic Year | Sep/Last Academic Year | 0                      | 120                       | OnProgramme16To18FrameworkUplift |
        | R03/Last Academic Year | Oct/Last Academic Year | 0                      | 120                       | OnProgramme16To18FrameworkUplift |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 120                       | OnProgramme16To18FrameworkUplift |
        | R05/Last Academic Year | Dec/Last Academic Year | 0                      | 120                       | OnProgramme16To18FrameworkUplift |
        | R06/Last Academic Year | Jan/Last Academic Year | 0                      | 120                       | OnProgramme16To18FrameworkUplift |
        | R07/Last Academic Year | Feb/Last Academic Year | 0                      | 120                       | OnProgramme16To18FrameworkUplift |
        | R08/Last Academic Year | Mar/Last Academic Year | 0                      | 120                       | OnProgramme16To18FrameworkUplift |
        | R09/Last Academic Year | Apr/Last Academic Year | 0                      | 120                       | OnProgramme16To18FrameworkUplift |
        | R10/Last Academic Year | May/Last Academic Year | 0                      | 120                       | OnProgramme16To18FrameworkUplift |
        | R11/Last Academic Year | Jun/Last Academic Year | 0                      | 120                       | OnProgramme16To18FrameworkUplift |
        | R12/Last Academic Year | Jul/Last Academic Year | 0                      | 120                       | OnProgramme16To18FrameworkUplift |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 500                       | First16To18EmployerIncentive     |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 500                       | First16To18ProviderIncentive     |
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 7500                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 100%                        |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 7500                 | 06/Aug/LAst Academic Year           | 0                      | 06/Aug/Last Academic Year             | 0                       |                                        | 0                         |                                          | Act1          | 1                   | 100%                        |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Second16To18EmployerIncentive | Second16To18ProviderIncentive | Completion16To18FrameworkUplift | Price Episode Identifier |
		| Aug/Current Academic Year | 0            | 1500       | 0         | 500                           | 500                           | 360                             | pe-1                     |
		| Sep/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Oct/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Nov/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Jan/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Feb/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Mar/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Apr/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| May/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Jun/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Jul/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
    And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Second16To18EmployerIncentive | Second16To18ProviderIncentive | Completion16To18FrameworkUplift |
		| R01/Current Academic Year | Aug/Current Academic Year | 0            | 1500       | 0         | 500                           | 500                           | 360                             |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type                |
		| R01/Current Academic Year | Aug/Current Academic Year | 1500                   | 0                         | Completion                      |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 500                       | Second16To18EmployerIncentive   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 500                       | Second16To18ProviderIncentive   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 360                       | Completion16To18FrameworkUplift |
	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type                |
		| R01/Current Academic Year | Aug/Current Academic Year | 1500                   | 0                         | Completion                      |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 500                       | Second16To18EmployerIncentive   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 500                       | Second16To18ProviderIncentive   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 360                       | Completion16To18FrameworkUplift |
Examples: 
        | Collection_Period         | Levy Balance |
        | R01/Current Academic Year | 7500         |
        | R02/Current Academic Year | 7500         |



#@SmallEmployerDas   
#1 learner aged 19-24, Levy, is a care leaver, employed with a small employer at start, is fully funded for on programme and completion payments
#    Given levy balance > agreed price for all months
#	And the apprenticeship funding band maximum is 9000
#    And the following commitments exist:
#        | ULN       | framework code | programme type | pathway code | agreed price | start date | end date   |
#        | learner a | 403            | 2              | 1            | 7500         | 06/08/2018 | 08/08/2019 |
#    When an ILR file is submitted with the following data:
#        | ULN       | learner type             | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code | Employment Status  | Employment Status Applies | Employer Id | Small Employer | LearnDelFAM |
#        | learner a | 19-24 programme only DAS | 7500         | 06/08/2018 | 08/08/2019       | 08/08/2019      | completed         | 403            | 2              | 1            | In paid employment | 05/08/2018                | 12345678    | SEM1           | EEF4        |
#	And the employment status in the ILR is:
#        | Employer    | Employment Status      | Employment Status Applies | Small Employer |
#        | employer 1  | in paid employment     | 05/08/2018                | SEM1           |
#    Then the provider earnings and payments break down as follows:
#        | Type                                | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | ... | 07/19 | 08/19 | 09/19 |
#        | Provider Earned Total               | 620   | 620   | 620   | 1620  | 620   | ... | 620   | 2860  | 0     |
#        | Provider Earned from SFA            | 620   | 620   | 620   | 1620  | 620   | ... | 620   | 2860  | 0     |
#        | Provider Earned from Employer       | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider Paid by SFA                | 0     | 620   | 620   | 620   | 1620  | ... | 620   | 620   | 2860  |
#        | Payment due from Employer           | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Levy account debited                | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA Levy employer budget            | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA Levy co-funding budget          | 500   | 500   | 500   | 500   | 500   | ... | 500   | 1500  | 0     |
#        | SFA Levy additional payments budget | 120   | 120   | 120   | 1120  | 120   | ... | 120   | 1360  | 0     |
#
#    And the transaction types for the payments are:
#        | Payment type                 | 09/18 | 10/17 | 11/17 | 12/17 | ... | 08/18 | 09/18 |
#        | On-program                   | 500   | 500   | 500   | 500   | ... | 500   | 0     |
#        | Completion                   | 0     | 0     | 0     | 0     | ... | 0     | 1500  |
#        | Balancing                    | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 500   | ... | 0     | 500   |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 500   | ... | 0     | 500   |
#        | Framework uplift on-program  | 120   | 120   | 120   | 120   | ... | 120   | 0     |
#        | Framework uplift completion  | 0     | 0     | 0     | 0     | ... | 0     | 360   |
#        | Framework uplift balancing   | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Provider disadvantage uplift | 0     | 0     | 0     | 0     | ..  | 0     | 0     |
