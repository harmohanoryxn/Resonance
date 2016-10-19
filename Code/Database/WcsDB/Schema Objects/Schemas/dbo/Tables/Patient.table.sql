CREATE TABLE [dbo].[Patient] (
    [patientId]         INT            IDENTITY (1, 1) NOT NULL,
    [externalSourceId] int  NOT NULL,
    [externalId] nvarchar(50)  NOT NULL,
    [givenName]         NVARCHAR (50)  NULL,
    [surname]           NVARCHAR (50)  NULL,
    [dob]               DATETIME       NULL,
    [sex]               NVARCHAR (20)  NULL,
    [isMrsaPositive]    BIT            NOT NULL,
    [isFallRisk]        BIT            NOT NULL,
    [isAssistanceRequired]        BIT            NOT NULL,
    [assistanceDescription]     NVARCHAR (50)  NOT NULL,
    [hasLatexAllergy]   BIT            NOT NULL
	CONSTRAINT UC_Patient_External UNIQUE ([externalSourceId], [externalId])
);

