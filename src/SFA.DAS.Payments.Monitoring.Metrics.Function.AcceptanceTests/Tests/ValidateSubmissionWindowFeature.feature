Feature: ValidateSubmissionWindowFeature

Scenario: Returns HttpStatus code 200 for Within  Tolerances Submission Window
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And Submission Percentage Is within  Tolerances
	When ValidateSubmissionWindow function is called for CollectionPeriod 1 and AcademicYear 2021
	Then HttpStatus code 200 with Json result is returned
	And The IsWithinTolerance is True in database for CollectionPeriod 1 and AcademicYear 2021

Scenario: Returns HttpStatus code 406 for results not within tolerance for submissions Window
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And Submission Percentage is Not within  Tolerances
	When ValidateSubmissionWindow function is called for CollectionPeriod 1 and AcademicYear 2021
	Then HttpStatus code 406 with Json result is returned
	And The IsWithinTolerance is False in database for CollectionPeriod 1 and AcademicYear 2021

Scenario: Returns HttpStatus code 500 for invalid Submission Window
	Given SubmissionSumary Does Not Exists for CollectionPeriod 1 and AcademicYear 2021
	When ValidateSubmissionWindow function is called for CollectionPeriod 1 and AcademicYear 2021
	Then HttpStatus code 500 with Json result is returned
	And SubmissionsSumary is NOT Saved to database for CollectionPeriod 1 and AcademicYear 2021

Scenario: Uses default CollectionPeriodTolerances when configuration is missing
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And CollectionPeriodTolerances NOT configured for CollectionPeriod 1 and AcademicYear 2021
	And Submission Percentage is within  Tolerances
	When ValidateSubmissionWindow function is called for CollectionPeriod 1 and AcademicYear 2021
	Then HttpStatus code 200 with Json result is returned
	And The IsWithinTolerance is True in database for CollectionPeriod 1 and AcademicYear 2021

Scenario: Uses CollectionPeriodTolerances configuration for upper and lower Tolerances
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And CollectionPeriodTolerances are configured for CollectionPeriod 1 and AcademicYear 2021
	And Submission Percentage is within  Tolerances
	When ValidateSubmissionWindow function is called for CollectionPeriod 1 and AcademicYear 2021
	Then HttpStatus code 200 with Json result is returned
	And The IsWithinTolerance is True in database for CollectionPeriod 1 and AcademicYear 2021

Scenario: Uses CollectionPeriodTolerances configuration for upper and lower Tolerances and result is outside of Tolerances
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And CollectionPeriodTolerances are configured for CollectionPeriod 1 and AcademicYear 2021
	And Submission Percentage is Not within  Tolerances
	When ValidateSubmissionWindow function is called for CollectionPeriod 1 and AcademicYear 2021
	Then HttpStatus code 406 with Json result is returned
	And The IsWithinTolerance is False in database for CollectionPeriod 1 and AcademicYear 2021
