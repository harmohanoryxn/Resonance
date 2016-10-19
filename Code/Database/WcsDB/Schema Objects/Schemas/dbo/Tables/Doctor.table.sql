CREATE TABLE [dbo].[Doctor] (
    [doctorId] int IDENTITY(1,1) NOT NULL,
    [externalSourceId] int  NOT NULL,
    [externalId] nvarchar(50)  NOT NULL,
    [name] nvarchar(50)  NOT NULL
	CONSTRAINT UC_Doctor_External UNIQUE ([externalSourceId], [externalId])
);

