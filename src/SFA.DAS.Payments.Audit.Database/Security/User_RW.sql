﻿CREATE USER [DASPaymentRWUser]
    WITH PASSWORD = N'$(RWUserPassword)';
GO
GRANT CONNECT TO [DASPaymentRWUser]
GO


