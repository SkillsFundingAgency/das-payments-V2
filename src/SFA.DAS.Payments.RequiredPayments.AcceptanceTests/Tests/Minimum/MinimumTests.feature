Feature: Minimum Acceptance Tests

@learner_changes_contract_type
@Non-DAS
@apprenticeship_contract_type_2
@minimum_tests
Scenario: DPP_965_01 - Levy apprentice, provider edits contract type (ACT) in the ILR, previous on-programme and English/math payments are refunded and repaid according to latest contract type

	Given the current processing period is 3

	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref1      | 10000 | 10000 |

	And the following course information:
	| LearnRefNumber | Ukprn | ULN   | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate |
	| learnref1      | 10000 | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        |

	And the following contract type 1 on programme earnings for periods 1-12:
	| LearnRefNumber | Ukprn | Learning_1 |
	| learnref1      | 10000 | 600        |

	And the following historical contract type 2 payments exist:   
	| LearnRefNumber | Ukprn | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | Period | ULN   | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate | Learning_1 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 1      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |

	When a TOBY is received
	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | Period | ULN   | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate | Learning_1 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 1      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 1      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | -600       | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | -600       | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |

	And the payments due component will generate the following contract type 1 payable earnings:
	| LearnRefNumber | Ukprn | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | Period | ULN   | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate | Learning_1 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 1      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 3      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600        | 0            |             | 0                              | 0                              | 0                               | 0                               | 0                                  | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |
  

@learner_changes_contract_type
@Non-DAS
@apprenticeship_contract_type_2
@minimum_tests
Scenario: DPP_965_02 - Non-Levy apprentice, provider edits contract type (ACT) in the ILR, previous on-programme and English/math payments are refunded and repaid according to latest contract type		
	Given the current processing period is 3

	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref3      | 10000 | 10000 |

	And the following course information:
	| LearnRefNumber | Ukprn | ULN   | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate |
	| learnref3      | 10000 | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        |

	And the following contract type 2 on programme earnings for periods 1-12:

	| LearnRefNumber | Ukprn | Amount |
	| learnref3      | 10000 | 600    |

	And the following contract type 2 incentive earnings for periods 1-12:
	| LearnRefNumber | Ukprn | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 | 
	| learnref3      | 10000 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 

	And the following completion earnings:
	| LearnRefNumber | Ukprn | Amount |
	| learnref3      | 10000 | 1620   |  
	
	And the following historical payments exist:   
	| LearnRefNumber | Ukprn | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | Period | ULN   | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate | TransactionType01 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 | ApprenticeshipContractType |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 1      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 1                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 1                          |

   Then the payments due component will generate the following payable earnings:
	| LearnRefNumber | Ukprn | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | Period | ULN   | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate | TransactionType01 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 | ApprenticeshipContractType |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 1      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 1                          |
	| learnref3      | 10000 | 1            | p2                     | 06/08/2017       | 06/08/2017                   | 2      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 1                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 1      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | -600              | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 1                          |
	| learnref3      | 10000 | 1            | p2                     | 06/08/2017       | 06/08/2017                   | 2      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | -600              | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 1                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 1      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 3      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |

@additional_payments
@Non-DAS
@16-18_incentives
@completion
@minimum_tests
Scenario: AC3-Learner finishes on time, earns on-programme and completion payments. Assumes 12 month apprenticeship and learner completes after 10 months.
	Given the current processing period is 4

	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref3      | 10000 | 10000 |

	And the following course information:
	| LearnRefNumber | Ukprn | ULN   | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate |
	| learnref3      | 10000 | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        |

	And the following contract type 2 on programme earnings for periods 1-12:

	| LearnRefNumber | Ukprn | Amount |
	| learnref3      | 10000 | 540    |

	And the following contract type 2 incentive earnings for periods 1-12:
	| LearnRefNumber | Ukprn | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 |
	| learnref3      | 10000 | 0                              | 0                              | 0                               | 0                               | 120                                | 0                                 | 0                                 | 0                           | 0                            | 0                             | 0                           | 0                  |

	And the following completion earnings:
	| LearnRefNumber | Ukprn | Amount |
	| learnref3      | 10000 | 1620   |  
	
	And the following historical payments exist:   
	| LearnRefNumber | Ukprn | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | Period | ULN   | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate | TransactionType01 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 | ApprenticeshipContractType |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 1      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 540               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 120                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 540               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 120                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 3      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 540               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 120                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |

   Then the payments due component will generate the following payable earnings:
	| LearnRefNumber | Ukprn | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | Period | ULN   | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate | TransactionType01 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 | ApprenticeshipContractType |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 1      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 540               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 540               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 3      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 540               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 4      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 540               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |

@additional_payments
@Non-DAS
@disadvantaged_postcode
@minimum_tests
Scenario:AC5- Payment for a non-DAS learner, lives in a disadvantaged postocde area - 1-10% most deprived, funding agreed within band maximum, UNDERTAKING APPRENTICESHIP FRAMEWORK The provider incentive for this postcode group is £600 split equally into 2 payments at 90 and 365 days. INELIGIBLE FOR APPRENTICESHIP STANDARDS
	Given the current processing period is 4

	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref3      | 10000 | 10000 |

	And the following course information:
	| LearnRefNumber | Ukprn | ULN   | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate |
	| learnref3      | 10000 | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        |

	And the following contract type 2 on programme earnings for periods 1-12:

	| LearnRefNumber | Ukprn | Amount |
	| learnref3      | 10000 | 600    |

	And the following contract type 2 incentive earnings for periods 1-12:
	| LearnRefNumber | Ukprn | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 |
	| learnref3      | 10000 | 0                 | 0                 | 0                 | 0                 | 120               | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 |

	And the following completion earnings:
	| LearnRefNumber | Ukprn | Amount |
	| learnref3      | 10000 | 1620   |  
	
	And the following historical payments exist:   
	| LearnRefNumber | Ukprn | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | Period | ULN   | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate | TransactionType01 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 | ApprenticeshipContractType |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 1      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 120                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 120                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 3      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 120                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |

   Then the payments due component will generate the following payable earnings:
	| LearnRefNumber | Ukprn | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | Period | ULN   | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate | TransactionType01 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 | ApprenticeshipContractType |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 1      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 3      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref3      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 4      | 10000 | 2             | 403           | 1           |              | 0.90000                   | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 600               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |

@additional_payments		     
@Non-DAS
@balancing
@minimum_tests
Scenario: 581-AC01-Non DAS learner finishes early, price equals the funding band maximum, earns balancing and completion framework uplift payments. Assumes 15 month apprenticeship and learner completes after 12 months.

@additional_payments		     
@Non-DAS
@16-18_incentives
@completion
@framework_uplift
@balancing
@minimum_tests
Scenario: 581-AC02-Non DAS learner finishes early, price lower than the funding band maximum, earns balancing and completion framework uplift payments. Assumes 15 month apprenticeship and learner completes after 12 months.

@additional_payments
@Non-DAS
@completion
@maths_and_english
@minimum_tests
Scenario:638-AC01 Non-DAS learner, takes an English qualification that has a planned end date that exceeds the actual end date of the programme aim

@additional_payments
@Non-DAS
@maths_and_english
@learning_support
@minimum_tests
Scenario:671-AC02 Non-DAS learner, levy available, is taking an English or maths qualification, has learning support and the negotiated price changes during the programme

@learner_finishes_early
@Non-DAS
@completion
@balancing
@minimum_tests
Scenario: A non-DAS learner, learner finishes early

@refunds
@Non-DAS
@price_change
@minimum_tests
Scenario:894-AC02 - non DAS standard learner, payments made then price is changed retrospectively from beginning
	Given the current processing period is 3

	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref1      | 10000 | 10000 |

	And the following course information:
	| LearnRefNumber | Ukprn | ULN   | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                     | LearnAimRef | LearningStartDate |
	| learnref1      | 10000 | 10000 | 1            | p1                     | 04/08/2017       | 04/08/2017                   | 25            | 403           | 1           |              | 1                         | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 04/08/2017        |

	And the following contract type 2 on programme earnings for periods 1-12:
	| LearnRefNumber | Ukprn | Amount |
	| learnref1      | 10000 | 1    |

	And the following historical payments exist:   
	| LearnRefNumber | Ukprn | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | Period | ULN   | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                     | LearnAimRef | LearningStartDate | TransactionType01 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 | ApprenticeshipContractType |
	| learnref1      | 10000 | 1            | p1                     | 04/08/2017       | 04/08/2017                   | 1      | 10000 | 25            | 403           | 1           |              | 1                         | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 04/08/2017        | 750               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref1      | 10000 | 1            | p1                     | 04/08/2017       | 04/08/2017                   | 2      | 10000 | 25            | 403           | 1           |              | 1                         | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 04/08/2017        | 750               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |

	 Then the payments due component will generate the following payable earnings:
	| LearnRefNumber | Ukprn | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | Period | ULN   | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                     | LearnAimRef | LearningStartDate | TransactionType01 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 | ApprenticeshipContractType |
	| learnref1      | 10000 | 1            | p1                     | 04/08/2017       | 04/08/2017                   | 1      | 10000 | 25            | 403           | 1           |              | 1                         | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 04/08/2017        | 750               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref1      | 10000 | 1            | p1                     | 04/08/2017       | 04/08/2017                   | 2      | 10000 | 25            | 403           | 1           |              | 1                         | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 04/08/2017        | 750               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref1      | 10000 | 1            | p2                     | 04/08/2017       | 04/08/2017                   | 1      | 10000 | 25            | 403           | 1           |              | 1                         | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 04/08/2017        | -749.33333        | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref1      | 10000 | 1            | p2                     | 04/08/2017       | 04/08/2017                   | 2      | 10000 | 25            | 403           | 1           |              | 1                         | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 04/08/2017        | -749.33333        | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref1      | 10000 | 1            | p2                     | 04/08/2017       | 04/08/2017                   | 1      | 10000 | 25            | 403           | 1           |              | 1                         | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 04/08/2017        | 0.66667           | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |

@small_employers
@Non-DAS
@completion
@16-18_incentives
@framework_uplift
@minimum_tests
Scenario:AC2- 1 learner aged 19-24, non-DAS, with an Education Health Care (EHC) plan, In paid employment with a small employer at start, is fully funded for on programme and completion payments
	# Need an equivalent test for period 13 (1819-R01) to handle framework uplift completion payment
	Given the current processing period is 4

	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref1      | 10000 | 10000 |

	And the following course information:
	| LearnRefNumber | Ukprn | ULN   | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate |
	| learnref1      | 10000 | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2             | 403           | 1           |              | 1                         | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        |

	And the following contract type 2 on programme earnings for periods 1-12:
	| LearnRefNumber | Ukprn | Amount |
	| learnref1      | 10000 | 500    |

	And the following contract type 2 incentive earnings for period 4:
	| LearnRefNumber | Ukprn | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 |
	| learnref1      | 10000 | 500               | 500               | 0                 | 0                 | 120               | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 |

	And the following contract type 2 incentive earnings for period 13:
	| LearnRefNumber | Ukprn | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 |
	| learnref1      | 10000 | 500               | 500               | 0                 | 0                 | 0                 | 360               | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 |

	And the following completion earnings for period 13:
	| LearnRefNumber | Ukprn | Amount |
	| learnref1      | 10000 | 1500   |

	And the following historical payments exist:   
	| LearnRefNumber | Ukprn | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | Period | ULN   | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate | TransactionType01 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 | ApprenticeshipContractType |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 1      | 10000 | 2             | 403           | 1           |              | 1                         | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 500               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2      | 10000 | 2             | 403           | 1           |              | 1                         | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 500               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 3      | 10000 | 2             | 403           | 1           |              | 1                         | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 500               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |

	# once incentive payments have been implemented, this will also produce the output from the incentives
   Then the payments due component will generate the following payable earnings:
	| LearnRefNumber | Ukprn | AimSeqNumber | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | Period | ULN   | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | SfaContributionPercentage | FundingLineType                                                       | LearnAimRef | LearningStartDate | TransactionType01 | Completion_2 | Balancing_3 | First16To18EmployerIncentive_4 | First16To18ProviderIncentive_5 | Second16To18EmployerIncentive_6 | Second16To18ProviderIncentive_7 | OnProgramme16To18FrameworkUplift_8 | Completion16To18FrameworkUplift_9 | Balancing16To18FrameworkUplift_10 | FirstDisadvantagePayment_11 | SecondDisadvantagePayment_12 | OnProgrammeMathsAndEnglish_13 | BalancingMathsAndEnglish_14 | LearningSupport_15 | ApprenticeshipContractType |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 1      | 10000 | 2             | 403           | 1           |              | 1                         | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 500               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 2      | 10000 | 2             | 403           | 1           |              | 1                         | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 500               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 3      | 10000 | 2             | 403           | 1           |              | 1                         | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 500               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |
	| learnref1      | 10000 | 1            | p1                     | 06/08/2017       | 06/08/2017                   | 4      | 10000 | 2             | 403           | 1           |              | 1                         | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 500               | 0                 |                   | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 0                 | 2                          |