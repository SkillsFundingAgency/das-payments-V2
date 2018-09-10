#New name - Additional_Payments_Eng_Planned_Date_Exceeds_Programme_aim_End_date
#Old name - AdditionalPayments_638-AC01

Feature: Additional payments english planned date exceeds programme aim end date
		638-AC01 Non-DAS learner, takes an English qualification that has a planned end date that exceeds the actual end date of the programme aim

Background:
	
	#Warning: Period 14 is for maths and english - currently not implented
	Given the current processing period is 14

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000


@Non-DAS
@minimum_tests
#@additional_payments
#@completion
#@Maths_English
#@Maths_English_FinshedLate
@partial

Scenario: Contract Type 2 no payment

	When MASH is received

	Then the payment source component will generate no contract type 2 coinvested payments


#----------------------------------------------------------------------------------------------------------------------------------------
#/*  Payments V1 for reference
#
#@mathsandenglishnondas
#@_minimum_acceptance_
#scenario:638-ac01 non-das learner, takes an english qualification that has a planned end date that exceeds the actual end date of the programme aim
#
#	when an ilr file is submitted with the following data:
#		| uln       | learner type           | aim type         | agreed price | aim rate | start date | planned end date | actual end date | completion status | 
#		| learner a | programme only non-das | programme        | 15000        |          | 06/08/2017 | 08/08/2018       | 08/08/2018      | completed         | 
#		| learner a | programme only non-das | maths or english |              | 471      | 06/08/2017 | 06/10/2018       | 06/10/2018      | completed         |
#	then the provider earnings and payments break down as follows:
#		| type                                    | 08/17   | 09/17   | 10/17   | ... | 05/18   | 06/18   | 07/18   | 08/18   | 09/18   | 10/18 | 11/18 |
#		| provider earned total                   | 1033.64 | 1033.64 | 1033.64 | ... | 1033.64 | 1033.64 | 1033.64 | 3033.64 | 33.64   | 0     | 0     |
#		| provider paid by sfa                    | 0       | 933.64  | 933.64  | ... | 933.64  | 933.64  | 933.64  | 933.64  | 2733.64 | 33.64 | 0     |
#		| payment due from employer               | 0       | 100     | 100     | ... | 100     | 100     | 100     | 100     | 300     | 0     | 0     |
#		| levy account debited                    | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0     | 0     |
#		| sfa levy employer budget                | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0     | 0     |
#		| sfa levy co-funding budget              | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0     | 0     |
#		| sfa non-levy co-funding budget          | 900     | 900     | 900     | ... | 900     | 900     | 900     | 2700    | 0       | 0     | 0     |
#		| sfa levy additional payments budget     | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0     | 0     |
#		| sfa non-levy additional payments budget | 33.64   | 33.64   | 33.64   | ... | 33.64   | 33.64   | 33.64   | 33.64   | 33.64   | 0     | 0     |
#    and the transaction types for the payments are:
#		| payment type                   | 09/17 | 10/17 | ... | 05/18 | 06/18 | 07/18 | 08/18 | 09/18 | 10/18 | 11/18 |
#		| on-program                     | 900   | 900   | ... | 900   | 900   | 900   | 900   | 0     | 0     | 0     |
#		| completion                     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 2700  | 0     | 0     |
#		| balancing                      | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#		| english and maths on programme | 33.64 | 33.64 | ... | 33.64 | 33.64 | 33.64 | 33.64 | 33.64 | 33.64 | 0     |
#		| english and maths balancing    | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     | 0     | 0     |

#----------------------------------------------------------------------------------------------------------------------------------------