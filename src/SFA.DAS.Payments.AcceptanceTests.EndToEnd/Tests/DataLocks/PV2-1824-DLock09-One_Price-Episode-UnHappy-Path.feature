Feature: PV2-1824-DLock09-One_Price-Episode-UnHappy-Path
Scenario: PV2-1824-DLock09-One_Price-Episode-UnHappy-Path


Given there are 2 Commitments in DAS, Commitment A and Commitment B
And there is a single price episode in the ILR, PE 1
And the PE 1 in the ILR matches to both Commitments A and B, on ULN and UKPRN
And the start date in the PE 1 is before the start date for Commitment A
And the start date in the PE 1 is before the start date for Commitment B
When the Provider submits the single price episode PE 1 in the ILR
Then no match is found for a Commitment in DAS

And a DLOCK_09 is triggered
And there is a DLOCK_09 entry in the Data Match report for PE 1


#
#Given there are 2 price episodes in the ILR submitted for R02/current academic year, PE-1 and PE-2
#
#And end date of PE-1 and the start date of PE-2 occur in the same month
#
#And PE-1 in the ILR matches to both Commitments A and B, on ULN and UKPRN
#
#And the start date of PE-1 is before the start date for Commitment A
#
#And the start date of PE-1 is before the start date for Commitment B
#
#When the Provider submits the 2 price episodes in the ILR
#
#Then there is a DLOCK-09 triggered for PE-1 and no match in DAS
