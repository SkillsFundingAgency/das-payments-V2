Feature: PeriodEndRequestReportFeature

Scenario: Returns HttpStatus code 200 for Within Tolerance
	Given Payments and DC Earnings within tolerance exist for CollectionPeriod 1 and AcademicYear 2021
	When PeriodEndRequestReport function is called
	Then HttpStatus code 200 with Json result is returned
	And IsWithinTolerance is True in database
