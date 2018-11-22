Feature: One Non-levy learner withdraws after planned end date PV2-278
 
Background:
	Given the payments are for the current collection year
	And a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%
	

@NonLevy_BasicDay_Withdrawal
Scenario: pre - planned end date
	Given the current collection period is R01
	And planned course duration is 12 months
	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | TotalNegotiatedPrice | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 15000                | withdrawn	    |

	And the following contract type 2 On Programme earnings are provided:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| p2                     | 1				| Learning (TT1)  | 1000   |
	| p2                     | 2				| Learning (TT1)  | 0      |
	| p2                     | 3				| Learning (TT1)  | 0      |
	| p2                     | 4				| Learning (TT1)  | 0      |
	| p2                     | 5				| Learning (TT1)  | 0      |
	| p2                     | 6				| Learning (TT1)  | 0      |
	| p2                     | 7				| Learning (TT1)  | 0      |
	| p2                     | 8				| Learning (TT1)  | 0      |
	| p2                     | 9    			| Learning (TT1)  | 0      |
	| p2                     | 10				| Learning (TT1)  | 0      |
	| p2                     | 11				| Learning (TT1)  | 0      |
	| p2                     | 12				| Learning (TT1)  | 0      |

	When an earnings event is received
	Then the payments due component will generate the following contract type 2 payments due:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| p2                     | 1				| Learning (TT1)  | 1000   |

@NonLevy_BasicDay_Withdrawal
Scenario: post - planned end date
	Given the current collection period is R06
	And planned course duration is 12 months
	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | TotalNegotiatedPrice | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 15000                | withdrawn	    |

	And the following contract type 2 On Programme earnings are provided:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| p2                     | 1				| Learning (TT1)  | 1000   |
	| p2                     | 2				| Learning (TT1)  | 0      |
	| p2                     | 3				| Learning (TT1)  | 0      |
	| p2                     | 4				| Learning (TT1)  | 0      |
	| p2                     | 5				| Learning (TT1)  | 0      |
	| p2                     | 6				| Learning (TT1)  | 0      |
	| p2                     | 7				| Learning (TT1)  | 0      |
	| p2                     | 8				| Learning (TT1)  | 0      |
	| p2                     | 9    			| Learning (TT1)  | 0      |
	| p2                     | 10				| Learning (TT1)  | 0      |
	| p2                     | 11				| Learning (TT1)  | 0      |
	| p2                     | 12				| Learning (TT1)  | 0      |

	When an earnings event is received
	Then the payments due component will generate the following contract type 2 payments due:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| p2                     | 1				| Learning (TT1)  | 1000   |
	| p2                     | 2				| Learning (TT1)  | 0      |
	| p2                     | 3				| Learning (TT1)  | 0      |
	| p2                     | 4				| Learning (TT1)  | 0      |
	| p2                     | 5				| Learning (TT1)  | 0      |
	| p2                     | 6				| Learning (TT1)  | 0      |