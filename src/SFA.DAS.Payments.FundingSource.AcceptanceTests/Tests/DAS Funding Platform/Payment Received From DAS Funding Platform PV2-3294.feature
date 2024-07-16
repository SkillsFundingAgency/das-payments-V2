﻿Feature: Payment Received From DAS Funding Platform PV2-3294
A payment is generated by the DAS Funding Platform for consumption by Payments V2

Scenario: A calculate on programme payment message is received
	Given the current collection period is R1
	And the payments are for the current collection year
	And a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 100%
	When a calculate on programme payment command is received
	Then a funding source levy transaction is created for the calculated payment
	And a payment with a funding platform type of DasFundingPlatform is created for the calculated payment
