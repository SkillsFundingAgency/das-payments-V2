Feature: PV2-1797-DataLock09-Single-Price-Episode-Happy-Path
Scenario: PV2-1797-DataLock09-Single-Price-Episode-Happy-Path-R02

Given there are 2 Commitments in DAS, Commitment A and Commitment B in collection period R02/current academic year 
And there is a single price episode in the ILR, PE-1
And the PE-1 in the ILR matches to both Commitments A and B, on ULN and UKPRN
And the start date in the PE-1 is on or after the start date for Commitment A
And the start date in the PE-1 is before the start date for Commitment B
When the Provider submits the single price episode PE-1 in the ILR
Then there is a single match for PE-1 with Commitment A
