CREATE TABLE [dbo].[Procedure] (
    [procedureId]                           INT            IDENTITY (1, 1) NOT NULL,
    [externalSourceId] int  NOT NULL,
    [externalId] nvarchar(50)  NOT NULL,
    [code]                                  NVARCHAR (20)  NOT NULL,
    [description]                           NVARCHAR (200) NOT NULL,
    [durationMinutes]                       INT            NULL,
    [ProcedureCategory_procedureCategoryId] INT            NOT NULL
	CONSTRAINT UC_Procedure UNIQUE ([ProcedureCategory_procedureCategoryId],[code])
	CONSTRAINT UC_Procedure_External UNIQUE ([externalSourceId],[externalId])
);

