Feature: AuditDataCleanUp
Audit Data for Old or failed submissions should be deleted to improve system performance at Month End

Scenario: upon Audit Data Cleanup Schedule Function is executed, New TimedOut Submission Audit Data are NOT deleted
	Given a Provider has done two submissions, First Submission A and Second Submission B in collectionPeriod 2
	And Submission A has status Completed from collection period 2
	And Submission B has status TimedOut from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission A and Submission B Both NOT deleted

Scenario: upon Audit Data Cleanup Schedule Function is executed, Old TimedOut Submission Audit Data are NOT deleted
	Given a Provider has done two submissions, First Submission A and Second Submission B in collectionPeriod 2
	And Submission A has status TimedOut from collection period 2
	And Submission B has status Completed from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission A and Submission B Both NOT deleted

Scenario: upon Audit Data Cleanup Schedule Function is executed, All TimedOut Submission Audit Data are NOT deleted
	Given a Provider has done one submissions, Submission X in collection period 1
	And Now does two new submissions, First Submission A and Second Submission B in collection period 2
	And Submission X has status TimedOut from collection period 1
	And Submission A has status TimedOut from collection period 2
	And Submission B has status TimedOut from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission X is NOT deleted from collection period 1
	And Submission A is NOT deleted from collection period 2
	And Submission B is NOT deleted from collection period 2

Scenario: upon Audit Data Cleanup Schedule Function is executed, All DCJobSucceeded status null Submission for TimedOut Audit Data are NOT deleted
	Given a Provider has done one submissions, Submission X in collection period 1
	And Now does two new submissions, First Submission A and Second Submission B in collection period 2
	And Submission X has status TimedOut from collection period 1
	And Submission X has DCJobSucceeded Null from collection period 1
	And Submission A has status TimedOut from collection period 2
	And Submission A has DCJobSucceeded Null from collection period 2
	And Submission B has status TimedOut from collection period 2
	And Submission B has DCJobSucceeded Null from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission X is NOT deleted from collection period 1
	And Submission A is NOT deleted from collection period 2
	And Submission B is NOT deleted from collection period 2



Scenario: upon Audit Data Cleanup Schedule Function is executed, New DcTasksFailed Submission Audit Data are NOT deleted
	Given a Provider has done two submissions, First Submission A and Second Submission B in collectionPeriod 2
	And Submission A has status Completed from collection period 2
	And Submission B has status DcTasksFailed from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission A and Submission B Both NOT deleted

Scenario: upon Audit Data Cleanup Schedule Function is executed, Old DcTasksFailed Submission Audit Data are NOT deleted
	Given a Provider has done two submissions, First Submission A and Second Submission B in collectionPeriod 2
	And Submission A has status DcTasksFailed from collection period 2
	And Submission B has status Completed from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission A and Submission B Both NOT deleted

Scenario: upon Audit Data Cleanup Schedule Function is executed, All DcTasksFailed Submission Audit Data are NOT deleted
	Given a Provider has done one submissions, Submission X in collection period 1
	And Now does two new submissions, First Submission A and Second Submission B in collection period 2
	And Submission X has status DcTasksFailed from collection period 1
	And Submission A has status DcTasksFailed from collection period 2
	And Submission B has status DcTasksFailed from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission X is NOT deleted from collection period 1
	And Submission A is NOT deleted from collection period 2
	And Submission B is NOT deleted from collection period 2

Scenario: upon Audit Data Cleanup Schedule Function is executed, All DCJobSucceeded status null Submission for DcTasksFailed Audit Data are NOT deleted
	Given a Provider has done one submissions, Submission X in collection period 1
	And Now does two new submissions, First Submission A and Second Submission B in collection period 2
	And Submission X has status DcTasksFailed from collection period 1
	And Submission X has DCJobSucceeded Null from collection period 1
	And Submission A has status DcTasksFailed from collection period 2
	And Submission A has DCJobSucceeded Null from collection period 2
	And Submission B has status DcTasksFailed from collection period 2
	And Submission B has DCJobSucceeded Null from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission X is NOT deleted from collection period 1
	And Submission A is NOT deleted from collection period 2
	And Submission B is NOT deleted from collection period 2



Scenario: upon Audit Data Cleanup Schedule Function is executed, New InProgress Submission Audit Data are NOT deleted
	Given a Provider has done two submissions, First Submission A and Second Submission B in collectionPeriod 2
	And Submission A has status Completed from collection period 2
	And Submission B has status InProgress from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission A and Submission B Both NOT deleted

Scenario: upon Audit Data Cleanup Schedule Function is executed, Old InProgress Submission Audit Data are NOT deleted
	Given a Provider has done two submissions, First Submission A and Second Submission B in collectionPeriod 2
	And Submission A has status InProgress from collection period 2
	And Submission B has status Completed from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission A and Submission B Both NOT deleted

Scenario: upon Audit Data Cleanup Schedule Function is executed, All InProgress Submission Audit Data are NOT deleted
	Given a Provider has done one submissions, Submission X in collection period 1
	And Now does two new submissions, First Submission A and Second Submission B in collection period 2
	And Submission X has status InProgress from collection period 1
	And Submission A has status InProgress from collection period 2
	And Submission B has status InProgress from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission X is NOT deleted from collection period 1
	And Submission A is NOT deleted from collection period 2
	And Submission B is NOT deleted from collection period 2

Scenario: upon Audit Data Cleanup Schedule Function is executed, All DCJobSucceeded status null for InProgress Submission Audit Data are NOT deleted
	Given a Provider has done one submissions, Submission X in collection period 1
	And Now does two new submissions, First Submission A and Second Submission B in collection period 2
	And Submission X has status InProgress from collection period 1
	And Submission X has DCJobSucceeded Null from collection period 1
	And Submission A has status InProgress from collection period 2
	And Submission A has DCJobSucceeded Null from collection period 2
	And Submission B has status InProgress from collection period 2
	And Submission B has DCJobSucceeded Null from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission X is NOT deleted from collection period 1
	And Submission A is NOT deleted from collection period 2
	And Submission B is NOT deleted from collection period 2


Scenario: upon Audit Data Cleanup Schedule Function is executed, Old Completed Submission Audit Data is deleted
	Given a Provider has done two submissions, First Submission A and Second Submission B in collectionPeriod 2
	And Both Submission has status Completed in collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission A is deleted from collection period 2
	And Submission B is NOT deleted from collection period 2

Scenario: upon Audit Data Cleanup Schedule Function is executed, Old CompletedWithErrors Submission Audit Data is deleted
	Given a Provider has done two submissions, First Submission A and Second Submission B in collectionPeriod 2
	And Both Submission has status CompletedWithErrors in collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission A is deleted from collection period 2
	And Submission B is NOT deleted from collection period 2

Scenario: upon Audit Data Cleanup Schedule Function is executed, Old Completed Submission And DCJobSucceeded Audit Data is NOT deleted
	Given a Provider has done two submissions, First Submission A and Second Submission B in collectionPeriod 2
	And Both Submission has status Completed in collection period 2
	And Submission A has DCJobSucceeded true from collection period 2
	And Submission B has DCJobSucceeded false from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission A is NOT deleted from collection period 2
	And Submission B is NOT deleted from collection period 2

Scenario: upon Audit Data Cleanup Schedule Function is executed, Only Completed Submission Audit Data is deleted
	Given a Provider has done one submissions, Submission A in collection period 2
	And Now does two new submissions, First Submission B and Second Submission C in collection period 2
	And Now does two new submissions, First Submission D and Second Submission E in collection period 2
	And Submission A has status DcTasksFailed from collection period 2
	And Submission B has status TimedOut from collection period 2
	And Submission C has status CompletedWithErrors from collection period 2
	And Submission D has status Completed from collection period 2
	And Submission E has status InProgress from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission A is NOT deleted from collection period 2
	And Submission B is NOT deleted from collection period 2
	And Submission C is deleted from collection period 2
	And Submission D is NOT deleted from collection period 2
	And Submission E is NOT deleted from collection period 2

Scenario: upon Audit Data Cleanup Schedule Function is executed, No Submission Audit Data are deleted for multiple Periods
	Given a Provider has done one submissions, Submission A in collection period 1
	And Now does two new submissions, First Submission B and Second Submission C in collection period 2
	And Now does two new submissions, First Submission D and Second Submission E in collection period 2
	And Submission A has status CompletedWithErrors from collection period 1
	And Submission B has status TimedOut from collection period 2
	And Submission C has status DcTasksFailed from collection period 2
	And Submission D has status Completed from collection period 2
	And Submission E has status InProgress from collection period 2
	When Audit Data Cleanup Function is executed in collectionPeriod 2
	Then Submission A is NOT deleted from collection period 1
	And Submission B is NOT deleted from collection period 2
	And Submission C is NOT deleted from collection period 2
	And Submission D is NOT deleted from collection period 2
	And Submission E is NOT deleted from collection period 2