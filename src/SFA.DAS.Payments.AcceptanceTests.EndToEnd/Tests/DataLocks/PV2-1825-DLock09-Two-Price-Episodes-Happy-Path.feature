Feature: PV2-1825-DLock09-Two-Price-Episodes-Happy-Path
Scenario: PV2-1825-DLock09-Two-Price-Episodes-Happy-Path

Given there is an ILR for the collection period R02/current academic year with 2 price episodes, the end date of one occurs in the same month as the start date of the other
And end date of PE-1 and the start date of PE-2 occur in the same month
And both PE-1 and PE-2 in the ILR matches to both Commitments A and B, on ULN and UKPRN
And the start date in the PE-1 is on or after the start date for Commitment A
And the start date in the PE-1 is before the start date for Commitment B
And the start date in the PE-2 is on or after the start date for Commitment A
And the start date in the PE-2 is on or after the start date for Commitment B
And the course in PE-1 matches the course in Commitment A
And the course in PE-2 matches the course in Commitment B

When the Provider submits the 2 price episodes in the ILR

Then there is a single match for PE-1 with Commitment A
And there is a single match for PE-2 with Commitment B
And the course in PE-1 does not match the course for Commitment B
And the course in PE-2 does not match the course for Commitment A

