Feature: AuditDataCleanUp
Audit Data for Old or failed submissions should be deleted to improve system performance at Month End

Background:

Scenario: upon Audit Data Cleanup Schedule Function is executed, New Failed Submission Audit Data is deleted
	Given a Provider has done two submissions, First Submission A and Second Submission B
	And Submission B has status DcTasksFailed from collection period 1
	When Audit Data Cleanup Function is executed
	Then Submission B is deleted

Scenario: upon Audit Data Cleanup Schedule Function is executed, old superseded Submission Audit Data are deleted
	Given a Provider has done two submissions, First Submission A and Second Submission B
	And Submission A has status DcTasksFailed from collection period 1
	When Audit Data Cleanup Function is executed
	Then Submission A is deleted

Scenario: upon Audit Data Cleanup Schedule Function is executed, all Failed Submission Audit Data are deleted
	Given a Provider has done two submissions, First Submission A and Second Submission B
	And Both Submission has status DcTasksFailed in collection period 1
	When Audit Data Cleanup Function is executed
	Then Submission A and Submission B Both deleted

Scenario: upon Audit Data Cleanup Schedule Function is executed, Old Failed Submission Audit Data is deleted
	Given a Provider has done two submissions, First Submission A and Second Submission B
	And Both Submission has status Completed in collection period 1
	When Audit Data Cleanup Function is executed
	Then Submission A is deleted

Scenario: upon Audit Data Cleanup Schedule Function is executed, Completed Submission from Previous collection period is NOT deleted
	Given a Provider has done one submissions, Submission X in collection period 1
	And Now does two new submissions, First Submission A and Second Submission B in collection period 2
	And Submission X has status Completed from collection period 1
	And Submission A has status DcTasksFailed from collection period 2
	And Submission B has status Completed from collection period 2
	When Audit Data Cleanup Function is executed
	Then Submission A is deleted
	And Submission X is NOT deleted from collection period 1
	And Submission B is NOT deleted from collection period 2

Scenario: upon Audit Data Cleanup Schedule Function is executed, New InProgress Submission Audit Data are NOT deleted
	Given a Provider has done two submissions, First Submission A and Second Submission B
	And Submission A has status Completed from collection period 1
	And Submission B has status InProgress from collection period 1
	When Audit Data Cleanup Function is executed
	Then Submission A and Submission B Both NOT deleted
