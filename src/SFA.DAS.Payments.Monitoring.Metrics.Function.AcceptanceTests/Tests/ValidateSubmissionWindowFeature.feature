Feature: ValidateSubmissionWindowFeature

Scenario: Returns HttpStatus code 200 for Within  Tolerances Submission Window
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And Submission Percentage Is within  Tolerances
	When ValidateSubmissionWindow function is called
	Then HttpStatus code 200 with Json result is returned
	And The IsWithinTolerance is True in database

Scenario: Returns HttpStatus code 406 for results not within tolerance for submissions Window
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And Submission Percentage Is Not within  Tolerances
	When ValidateSubmissionWindow function is called
	Then HttpStatus code 406 without Json result is returned
	And The IsWithinTolerance is False in database

Scenario: Returns HttpStatus code 500 for invalid Submission Window
	Given SubmissionSumary Does Not Exists for CollectionPeriod 1 and AcademicYear 2021
	When ValidateSubmissionWindow function is called
	Then HttpStatus code 500 without Json result is returned
	And SubmissionsSumary is NOT Saved to database

Scenario: Uses default CollectionPeriodTolerances when configuration is missing and Submission within Tolerances
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And CollectionPeriodTolerances NOT configured
	And Submission Percentage Is within  Tolerances
	When ValidateSubmissionWindow function is called
	Then HttpStatus code 200 with Json result is returned
	And The IsWithinTolerance is True in database

Scenario: Uses default CollectionPeriodTolerances when configuration is missing and result is not within Tolerances
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And CollectionPeriodTolerances NOT configured
	And Submission Percentage Is Not within  Tolerances
	When ValidateSubmissionWindow function is called
	Then HttpStatus code 406 without Json result is returned
	And The IsWithinTolerance is False in database

Scenario: Uses CollectionPeriodTolerances configuration for upper and lower Tolerances and result is within Tolerances
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And CollectionPeriodTolerances are configured
	And Submission Percentage Is within  Tolerances
	When ValidateSubmissionWindow function is called
	Then HttpStatus code 200 with Json result is returned
	And The IsWithinTolerance is True in database

Scenario: Uses CollectionPeriodTolerances configuration for upper and lower Tolerances and result is not within Tolerances
	Given SubmissionSumary Exists for CollectionPeriod 1 and AcademicYear 2021
	And CollectionPeriodTolerances are configured
	And Submission Percentage Is Not within  Tolerances
	When ValidateSubmissionWindow function is called
	Then HttpStatus code 406 without Json result is returned
	And The IsWithinTolerance is False in database
