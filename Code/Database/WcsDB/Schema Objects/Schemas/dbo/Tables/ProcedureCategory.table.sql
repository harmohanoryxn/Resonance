CREATE TABLE [dbo].[ProcedureCategory] (
    [procedureCategoryId] INT			  IDENTITY (1, 1) NOT NULL,
    [externalSourceId] int  NOT NULL,
    [externalId] nvarchar(50)  NOT NULL,
    [includeInMerge]      BIT             NOT NULL,
    [description]         NVARCHAR (200)  NOT NULL,
	CONSTRAINT UC_ProcedureCategory_External UNIQUE ([externalSourceId], [externalId])
);

