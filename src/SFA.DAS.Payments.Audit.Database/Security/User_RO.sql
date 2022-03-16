CREATE USER [DASPaymentAuditROUser]
    WITH PASSWORD = N'$(AuditROUserPassword)';
GO
GRANT CONNECT TO [DASPaymentAuditROUser]
GO


