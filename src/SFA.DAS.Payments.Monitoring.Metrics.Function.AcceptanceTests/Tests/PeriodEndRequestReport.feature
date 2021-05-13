Feature: PeriodEndRequestReportFeature

Scenario: Returns HttpStatus code 200 for Within Tolerance
	Given Payments within tolerance exist against DC Earnings for for CollectionPeriod 1 and AcademicYear 2021
	When PeriodEndRequestReport function is called
	Then HttpStatus code 200 with Json result is returned
	And The Period End Metrics IsWithinTolerance is True in database
