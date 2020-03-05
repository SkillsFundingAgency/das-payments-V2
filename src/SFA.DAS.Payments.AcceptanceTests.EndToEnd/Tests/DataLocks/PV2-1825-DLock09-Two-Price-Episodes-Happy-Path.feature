Feature: PV2-1825-DLock09-Two-Price-Episodes-Happy-Path
Scenario: PV2-1825-DLock09-Two-Price-Episodes-Happy-Path

#Given there are 2 Commitments in DAS, A and B
#this one will be creating 2 commitments with no start dates (randomly genned ULN and UKPRNs to be accessible in later steps)

#Given there is an ILR with
Given there is an ILR for the collection period R02/current academic year with 2 price episodes, the end date of one occurs in the same month as the start date of the other
#maybe store the dates we are going to use on the context....... this should match the file so it's a bit brittle
#
And end date of PE-1 and the start date of PE-2 occur in the same month
##could do the logic mentioned in the previous step here, or nothing
#
And both PE-1 and PE-2 in the ILR matches to both Commitments A and B, on ULN and UKPRN
##update ULN and UKPRN to match the commitments
#
And the start date of PE-1 (2018-08-30) is after the start date for Commitment A (2018-08-01)
##so here we write the date of commitment A
#
And the start date of PE-1 (2018-08-30) is before the start date for Commitment B (2019-09-01)
##so here we write the date of commitment B
#

And the start date of PE-2 (2019-09-25) is after the start date for Commitment A (2018-08-01)

And the start date of PE-2 (2019-09-25) is on or after the start date for Commitment B (2019-09-01)

When the Provider submits the 2 price episodes in the ILR
##submit the FM36Global / Learner / whatever / DO the action / fire the message
#
Then there is a single match for PE-1 with Commitment A
##assert the result