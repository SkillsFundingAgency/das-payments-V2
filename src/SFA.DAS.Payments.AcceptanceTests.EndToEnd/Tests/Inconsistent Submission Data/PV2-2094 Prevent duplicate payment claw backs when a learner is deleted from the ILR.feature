Feature: PV2-2094-Prevent-duplicate-payment-claw-backs-when-a-learner-is-deleted-from-the-ILR
Scenario: PV2-2094 Prevent duplicate payment claw backs when a learner is deleted from the ILR

Given Commitment exists - which should this match, needs to match the commitments in FM36 in say was as alex spec
When an ILR file is submitted for period R05
And After Period-end Run 10 Payments are generated
When an ILR file is submitted for period R06
And After Period-end Run 6 Payments are generated
When an ILR file is submitted for period R08
And After Period-end Run 10 Payments are generated
When an ILR file is submitted for period R11
Then After final submission wrong Payments of 11 are generated