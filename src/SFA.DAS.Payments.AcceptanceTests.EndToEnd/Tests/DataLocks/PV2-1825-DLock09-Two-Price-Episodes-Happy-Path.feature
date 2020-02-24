Feature: PV2-1825-DLock09-Two-Price-Episodes-Happy-Path
Scenario: PV2-1825-DLock09-Two-Price-Episodes-Happy-Path
Given there are 2 Commitments in DAS, Commitment A with start date 2018-08-01 and Commitment B with start date 2018-10-01
#And there are 2 price episodes in the ILR, PE-1 and PE-2
#
#And end date of PE-1 and the start date of PE-2 occur in the same month
#And PE-1 in the ILR matches to both Commitments A and B, on ULN and UKPRN
#And the start date of PE-1 is after the start date for Commitment A
#And the start date of PE-1 is before the start date for Commitment B
#When the Provider submits the 2 price episodes in the ILR
#Then there is a single match for PE-1 with Commitment A