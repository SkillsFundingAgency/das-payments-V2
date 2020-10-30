Feature: ValidateSubmissionWindowFeature

Scenario: Returns HttpStatus code 200 for With In Tolerances Submission Window
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And Submission Percentage Is with in Tolerances
	When ValidateSubmissionWindow function is called for CollectionPeriod 1 and AcademicYear 2021
	Then HttpStatus code 200 with Json result is returned
	And IsWithinTolerance is True in database for CollectionPeriod 1 and AcademicYear 2021

Scenario: Returns HttpStatus code 406 for NOT With In Tolerances Submission Window
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And Submission Percentage NOT with in Tolerances
	When ValidateSubmissionWindow function is called for CollectionPeriod 1 and AcademicYear 2021
	Then HttpStatus code 406 with Json result is returned
	And IsWithinTolerance is False in database for CollectionPeriod 1 and AcademicYear 2021

Scenario: Returns HttpStatus code 500 for invalid Submission Window
	Given SubmissionSumary Does Not Exists for CollectionPeriod 1 and AcademicYear 2021
	When ValidateSubmissionWindow function is called for CollectionPeriod 1 and AcademicYear 2021
	Then HttpStatus code 500 with Json result is returned
	And SubmissionsSumary is NOT Saved to database for CollectionPeriod 1 and AcademicYear 2021

Scenario: Uses default CollectionPeriodTolerances when configuration is missing
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And CollectionPeriodTolerances NOT configured for CollectionPeriod 1 and AcademicYear 2021
	And Submission Percentage is with in Tolerances
	When ValidateSubmissionWindow function is called for CollectionPeriod 1 and AcademicYear 2021
	Then HttpStatus code 200 with Json result is returned
	And IsWithinTolerance is True in database for CollectionPeriod 1 and AcademicYear 2021

Scenario: Uses CollectionPeriodTolerances configuration for upper and lower Tolerances
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And CollectionPeriodTolerances are configured for CollectionPeriod 1 and AcademicYear 2021
	And Submission Percentage is with in Tolerances
	When ValidateSubmissionWindow function is called for CollectionPeriod 1 and AcademicYear 2021
	Then HttpStatus code 200 with Json result is returned
	And IsWithinTolerance is True in database for CollectionPeriod 1 and AcademicYear 2021

Scenario: Uses CollectionPeriodTolerances configuration for upper and lower Tolerances and result is outside of Tolerances
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And CollectionPeriodTolerances are configured for CollectionPeriod 1 and AcademicYear 2021
	And Submission Percentage NOT with in Tolerances
	When ValidateSubmissionWindow function is called for CollectionPeriod 1 and AcademicYear 2021
	Then HttpStatus code 406 with Json result is returned
	And IsWithinTolerance is False in database for CollectionPeriod 1 and AcademicYear 2021
