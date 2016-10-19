EXECUTE sp_addrolemember @rolename = N'db_datareader', @membername = N'IIS WCS Network Service';


GO
EXECUTE sp_addrolemember @rolename = N'db_datawriter', @membername = N'IIS WCS Network Service';

