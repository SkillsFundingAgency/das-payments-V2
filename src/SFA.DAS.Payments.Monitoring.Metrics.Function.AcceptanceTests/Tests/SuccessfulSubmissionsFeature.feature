Feature: Successful Submissions Feature for A Single Provider

Scenario: Submission with failed dc job and successful das job yields no results
Given there is a submission from a failed dc job
And the job was successfully processed on das
When calling the function
Then the results are empty

Scenario: Submission with failed dc job and failed das job yields no results
Given there is a submission from a failed dc job
And the job failed processing on das
When calling the function
Then the results are empty

Scenario: Submission with successful dc job and failed das job yields no results
Given there is a submission from a successful dc job
And the job failed processing on das
When calling the function
Then the results are empty

Scenario: Submission with successful dc job and successful das job yields results
Given there is a submission from a successful dc job
And the job was successfully processed on das
When calling the function
Then there is a single instance of the test ukprn

Scenario: Multiple successful and failed submissions for a period
Given there have been 3 successful submissions for the period
And there have been 3 failed submissions for the period
When calling the function
Then there is a single instance of the test ukprn

Scenario: Multiple successful submissions and no failed submissions for a period
Given there have been 3 successful submissions for the period
When calling the function
Then there is a single instance of the test ukprn

Scenario: Multiple failed submissions and no successful submissions for a period
Given there have been 3 failed submissions for the period
When calling the function
Then the results are empty


