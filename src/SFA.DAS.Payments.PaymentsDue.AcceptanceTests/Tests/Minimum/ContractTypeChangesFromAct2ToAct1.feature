Feature: Contract Type Changes From ACT2 To ACT1
	DPP_965_01 - Levy apprentice, provider edits contract type (ACT) in the ILR, previous on-programme and English/math payments are refunded and repaid according to latest contract type
Background:
	Given the current processing period is 3

	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref1      | 10000 | 10000 |

	And the following course information:
	| LearnRefNumber | Ukprn | ULN   | AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate |
	| learnref1      | 10000 | 10000 | 1            | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        |

	And the following historical contract type 2 payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | SfaContributionPercentage | Learning_1 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 |
	| learnref1      | 10000 | p1                     | 1      | 10000 | 0.90000                   | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | p1                     | 2      | 10000 | 0.90000                   | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |

	And the following contract type 1 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| learnref1      | 10000 | p2                     | 06/08/2017       | 06/08/2017                   | 9000                 | 600        |
	
@learner_changes_contract_type
@Non-DAS
@apprenticeship_contract_type_changes
@minimum_tests
Scenario: Contract Type 2 Payable Earnings
	
	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | SfaContributionPercentage | Learning_1 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 |
	| learnref1      | 10000 | p1                     | 1      | 10000 | 0.90000                   | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | p1                     | 2      | 10000 | 0.90000                   | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | p1                     | 1      | 10000 | 0.90000                   | -600       | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | p1                     | 2      | 10000 | 0.90000                   | -600       | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |

@learner_changes_contract_type
@Non-DAS
@apprenticeship_contract_type_changes
@minimum_tests
Scenario: Contract Type 1 Payable Earnings

	When a TOBY is received

	Then the payments due component will generate the following contract type 1 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | SfaContributionPercentage | Learning_1 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 |
	| learnref1      | 10000 | p2                     | 1      | 10000 | 0.90000                   | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | p2                     | 2      | 10000 | 0.90000                   | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | p2                     | 3      | 10000 | 0.90000                   | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
 