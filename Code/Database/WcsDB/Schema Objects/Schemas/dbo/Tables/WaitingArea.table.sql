CREATE TABLE [dbo].[WaitingArea] (
    [waitingAreaId] int IDENTITY(1,1) NOT NULL,
    [code] nvarchar(20)  NOT NULL,
    [name] nvarchar(50)  NOT NULL
	CONSTRAINT UC_WaitingArea_code UNIQUE ([code])
)
