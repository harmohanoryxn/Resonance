CREATE TABLE [dbo].[RfidDirection] (
    [rfidDirectionId] int IDENTITY(1,1) NOT NULL,
    [direction]        NVARCHAR (20) NOT NULL
    CONSTRAINT UC_RfidDirection_direction UNIQUE ([direction])
) ON [PRIMARY]
