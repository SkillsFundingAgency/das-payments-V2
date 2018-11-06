
GO
--ALTER ROLE [DataViewer] ADD MEMBER [User_DSCI];
GO
ALTER ROLE [DataViewer] ADD MEMBER [DASPaymentROUser];
GO
ALTER ROLE [DataProcessing] ADD MEMBER [DASPaymentRWUser];
GO

