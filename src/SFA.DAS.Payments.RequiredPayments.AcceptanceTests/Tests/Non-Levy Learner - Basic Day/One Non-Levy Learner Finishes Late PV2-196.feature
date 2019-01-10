Feature: One Non-Levy Learner Finishes Late PV2-196
Provider earnings and payments where learner completes later than planned

Background:
	Given a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%
	And the payments are for the current collection year

@NonDas_BasicDay
@finishes_late

Scenario: A non-DAS learner, learner finishes late
	Given the current collection period is R05

	And the earning events component generates the following contract type 2 earnings:	
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1				| Learning (TT1)	| 1000		|
	| p2                     | 5				| Completion (TT2)	| 3000		|

	And the following historical contract type 2 payments exist:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1				| Learning (TT1)	| 1000		|

	When an earning event is received
	Then the required payments component will only generate contract type 2 required payments
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 5				| Completion (TT2)	| 3000		|

@NonDas_BasicDay
@finishes_late
@withdrawal

# This scenario cannot be implemented as there is no input data. It is left here commented out for Pawan to remove when he has combined component tests into E2E ones.
# 
#Scenario: A non-DAS learner, learner withdraws after planned end date
#	Given the current collection period is R05
#
#	And the earning events component generated no contract type 2 earnings
#
#	And the following historical contract type 2 payments exist:
#	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
#	| p2                     | 1				| Learning (TT1)	| 1000		|
#
#	When an earning event is received
#	Then the required payments component will not generate any contract type 2 payable earnings

Scenario: A non-DAS learner, learner finishes late - no history
	Given the current collection period is R05

	And the earning events component generates the following contract type 2 earnings:	
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1				| Learning (TT1)	| 1000		|
	| p2                     | 5				| Completion (TT2)	| 3000		|

	When an earning event is received
	Then the required payments component will only generate contract type 2 required payments
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1				| Learning (TT1)	| 1000		|
	| p2                     | 5				| Completion (TT2)	| 3000		|