﻿CREATE USER [DASPaymentROUser]
    WITH PASSWORD = N'$(ROUserPassword)';
GO
GRANT CONNECT TO [DASPaymentROUser]
GO


