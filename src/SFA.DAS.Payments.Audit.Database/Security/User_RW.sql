CREATE USER [DASPaymentAuditRWUser]
    WITH PASSWORD = N'$(AuditRWUserPassword)';
GO
GRANT CONNECT TO [DASPaymentAuditRWUser]
GO


