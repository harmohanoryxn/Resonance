/*
Deployment script for WCS
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "WCS"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\"

GO
:on error exit
GO
USE [master]
GO
IF (DB_ID(N'$(DatabaseName)') IS NOT NULL
    AND DATABASEPROPERTYEX(N'$(DatabaseName)','Status') <> N'ONLINE')
BEGIN
    RAISERROR(N'The state of the target database, %s, is not set to ONLINE. To deploy to this database, its state must be set to ONLINE.', 16, 127,N'$(DatabaseName)') WITH NOWAIT
    RETURN
END

GO
IF (DB_ID(N'$(DatabaseName)') IS NOT NULL) 
BEGIN
    ALTER DATABASE [$(DatabaseName)]
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$(DatabaseName)];
END

GO
PRINT N'Creating $(DatabaseName)...'
GO
CREATE DATABASE [$(DatabaseName)]
    ON 
    PRIMARY(NAME = [WCS], FILENAME = '$(DefaultDataPath)$(DatabaseName).mdf', FILEGROWTH = 1024 KB)
    LOG ON (NAME = [WCS_log], FILENAME = '$(DefaultLogPath)$(DatabaseName)_1.ldf', MAXSIZE = 2097152 MB, FILEGROWTH = 10 %) COLLATE Latin1_General_CI_AS
GO
EXECUTE sp_dbcmptlevel [$(DatabaseName)], 100;


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ANSI_NULLS ON,
                ANSI_PADDING ON,
                ANSI_WARNINGS ON,
                ARITHABORT ON,
                CONCAT_NULL_YIELDS_NULL ON,
                NUMERIC_ROUNDABORT OFF,
                QUOTED_IDENTIFIER ON,
                ANSI_NULL_DEFAULT ON,
                CURSOR_DEFAULT LOCAL,
                RECOVERY FULL,
                CURSOR_CLOSE_ON_COMMIT OFF,
                AUTO_CREATE_STATISTICS ON,
                AUTO_SHRINK OFF,
                AUTO_UPDATE_STATISTICS ON,
                RECURSIVE_TRIGGERS OFF 
            WITH ROLLBACK IMMEDIATE;
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_CLOSE OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ALLOW_SNAPSHOT_ISOLATION OFF;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET READ_COMMITTED_SNAPSHOT OFF;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_UPDATE_STATISTICS_ASYNC OFF,
                PAGE_VERIFY NONE,
                DATE_CORRELATION_OPTIMIZATION OFF,
                DISABLE_BROKER,
                PARAMETERIZATION SIMPLE,
                SUPPLEMENTAL_LOGGING OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'$(DatabaseName)')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [$(DatabaseName)]
    SET TRUSTWORTHY OFF,
        DB_CHAINING OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'The database settings cannot be modified. You must be a SysAdmin to apply these settings.';
    END


GO
IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'$(DatabaseName)')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [$(DatabaseName)]
    SET HONOR_BROKER_PRIORITY OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'The database settings cannot be modified. You must be a SysAdmin to apply these settings.';
    END


GO
USE [$(DatabaseName)]
GO
IF fulltextserviceproperty(N'IsFulltextInstalled') = 1
    EXECUTE sp_fulltext_database 'enable';


GO
/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

GO
PRINT N'Creating [IIS WCS Network Service]...';


GO
CREATE USER [IIS WCS Network Service] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE];


GO
PRINT N'Creating <unnamed>...';


GO
EXECUTE sp_addrolemember @rolename = N'db_datareader', @membername = N'IIS WCS Network Service';


GO
PRINT N'Creating <unnamed>...';


GO
EXECUTE sp_addrolemember @rolename = N'db_datawriter', @membername = N'IIS WCS Network Service';


GO
PRINT N'Creating [dbo].[Admission]...';


GO
CREATE TABLE [dbo].[Admission] (
    [admissionId]                       INT           IDENTITY (1, 1) NOT NULL,
    [externalSourceId]                  INT           NOT NULL,
    [externalId]                        NVARCHAR (50) NOT NULL,
    [admitDateTime]                     DATETIME      NULL,
    [estimatedDischargeDateTime]        DATETIME      NULL,
    [dischargeDateTime]                 DATETIME      NULL,
    [patientId]                         INT           NOT NULL,
    [PrimaryCareDoctor_doctorId]        INT           NULL,
    [AttendingDoctor_doctorId]          INT           NULL,
    [AdmittingDoctor_doctorId]          INT           NULL,
    [AdmissionType_admissionTypeId]     INT           NOT NULL,
    [AdmissionStatus_admissionStatusId] INT           NOT NULL,
    [Location_locationId]               INT           NOT NULL,
    [Bed_bedId]                         INT           NULL,
    CONSTRAINT [UC_Admission_External] UNIQUE NONCLUSTERED ([externalSourceId] ASC, [externalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_Admission...';


GO
ALTER TABLE [dbo].[Admission]
    ADD CONSTRAINT [PK_Admission] PRIMARY KEY CLUSTERED ([admissionId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Admission].[IX_Admission_admitDateTime]...';


GO
CREATE NONCLUSTERED INDEX [IX_Admission_admitDateTime]
    ON [dbo].[Admission]([admitDateTime] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);


GO
PRINT N'Creating [dbo].[Admission].[IX_Admission_dischargeDateTime]...';


GO
CREATE NONCLUSTERED INDEX [IX_Admission_dischargeDateTime]
    ON [dbo].[Admission]([dischargeDateTime] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);


GO
PRINT N'Creating [dbo].[Admission].[IX_FK_AdmissionAdmissionStatus]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_AdmissionAdmissionStatus]
    ON [dbo].[Admission]([AdmissionStatus_admissionStatusId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Admission].[IX_FK_AdmissionAdmissionType]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_AdmissionAdmissionType]
    ON [dbo].[Admission]([AdmissionType_admissionTypeId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);


GO
PRINT N'Creating [dbo].[Admission].[IX_FK_AdmissionBed]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_AdmissionBed]
    ON [dbo].[Admission]([Bed_bedId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);


GO
PRINT N'Creating [dbo].[Admission].[IX_FK_AdmissionLocation]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_AdmissionLocation]
    ON [dbo].[Admission]([Location_locationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Admission].[IX_FK_ExternalSourceAdmission]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_ExternalSourceAdmission]
    ON [dbo].[Admission]([externalSourceId] ASC, [externalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Admission].[IX_FK_PatientAdmission]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_PatientAdmission]
    ON [dbo].[Admission]([patientId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[AdmissionStatus]...';


GO
CREATE TABLE [dbo].[AdmissionStatus] (
    [admissionStatusId] INT           IDENTITY (1, 1) NOT NULL,
    [status]            NVARCHAR (20) NOT NULL,
    CONSTRAINT [UC_AdmissionStatus_status] UNIQUE NONCLUSTERED ([status] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_AdmissionStatus...';


GO
ALTER TABLE [dbo].[AdmissionStatus]
    ADD CONSTRAINT [PK_AdmissionStatus] PRIMARY KEY CLUSTERED ([admissionStatusId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[AdmissionType]...';


GO
CREATE TABLE [dbo].[AdmissionType] (
    [admissionTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [type]            NVARCHAR (20) NOT NULL,
    CONSTRAINT [UC_AdmissionType_type] UNIQUE NONCLUSTERED ([type] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_AdmissionType...';


GO
ALTER TABLE [dbo].[AdmissionType]
    ADD CONSTRAINT [PK_AdmissionType] PRIMARY KEY CLUSTERED ([admissionTypeId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Bed]...';


GO
CREATE TABLE [dbo].[Bed] (
    [bedId]       INT           IDENTITY (1, 1) NOT NULL,
    [name]        NVARCHAR (50) NOT NULL,
    [Room_roomId] INT           NULL
);


GO
PRINT N'Creating PK_Bed...';


GO
ALTER TABLE [dbo].[Bed]
    ADD CONSTRAINT [PK_Bed] PRIMARY KEY CLUSTERED ([bedId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Bed].[IX_FK_BedRoom]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_BedRoom]
    ON [dbo].[Bed]([Room_roomId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[BedCleaningEvent]...';


GO
CREATE TABLE [dbo].[BedCleaningEvent] (
    [bedCleaningEventId]     INT      IDENTITY (1, 1) NOT NULL,
    [timestamp]              DATETIME NOT NULL,
    [Bed_bedId]              INT      NOT NULL,
    [bedCleaningEventTypeId] INT      NOT NULL
);


GO
PRINT N'Creating PK_BedCleaningEvent...';


GO
ALTER TABLE [dbo].[BedCleaningEvent]
    ADD CONSTRAINT [PK_BedCleaningEvent] PRIMARY KEY CLUSTERED ([bedCleaningEventId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[BedCleaningEvent].[IX_BedCleaningEvent_timestamp]...';


GO
CREATE NONCLUSTERED INDEX [IX_BedCleaningEvent_timestamp]
    ON [dbo].[BedCleaningEvent]([timestamp] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);


GO
PRINT N'Creating [dbo].[BedCleaningEvent].[IX_FK_BedCleaningEventBed]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_BedCleaningEventBed]
    ON [dbo].[BedCleaningEvent]([Bed_bedId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[BedCleaningEvent].[IX_FK_BedCleaningEventType]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_BedCleaningEventType]
    ON [dbo].[BedCleaningEvent]([bedCleaningEventTypeId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);


GO
PRINT N'Creating [dbo].[BedCleaningEventType]...';


GO
CREATE TABLE [dbo].[BedCleaningEventType] (
    [bedCleaningEventTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [eventType]              NVARCHAR (20) NOT NULL,
    CONSTRAINT [UC_BedCleaningEventType_eventType] UNIQUE NONCLUSTERED ([eventType] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_BedCleaningEventType...';


GO
ALTER TABLE [dbo].[BedCleaningEventType]
    ADD CONSTRAINT [PK_BedCleaningEventType] PRIMARY KEY CLUSTERED ([bedCleaningEventTypeId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Configuration]...';


GO
CREATE TABLE [dbo].[Configuration] (
    [configurationId]                       INT           IDENTITY (1, 1) NOT NULL,
    [name]                                  NVARCHAR (50) NOT NULL,
    [ConfigurationType_ConfigurationTypeId] INT           NOT NULL
);


GO
PRINT N'Creating PK_Configuration...';


GO
ALTER TABLE [dbo].[Configuration]
    ADD CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED ([configurationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Configuration].[IX_FK_ConfigurationConfigurationType]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_ConfigurationConfigurationType]
    ON [dbo].[Configuration]([ConfigurationType_ConfigurationTypeId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[ConfigurationLocation]...';


GO
CREATE TABLE [dbo].[ConfigurationLocation] (
    [configurationLocationId] INT IDENTITY (1, 1) NOT NULL,
    [configurationId]         INT NOT NULL,
    [locationId]              INT NOT NULL,
    [isDefault]               BIT NOT NULL
);


GO
PRINT N'Creating PK_ConfigurationLocation...';


GO
ALTER TABLE [dbo].[ConfigurationLocation]
    ADD CONSTRAINT [PK_ConfigurationLocation] PRIMARY KEY CLUSTERED ([configurationLocationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[ConfigurationLocation].[IX_FK_ConfigurationConfigurationLocation]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_ConfigurationConfigurationLocation]
    ON [dbo].[ConfigurationLocation]([configurationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[ConfigurationLocation].[IX_FK_LocationConfigurationLocation]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_LocationConfigurationLocation]
    ON [dbo].[ConfigurationLocation]([locationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[ConfigurationType]...';


GO
CREATE TABLE [dbo].[ConfigurationType] (
    [ConfigurationTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [name]                NVARCHAR (50) NOT NULL
);


GO
PRINT N'Creating PK_ConfigurationType...';


GO
ALTER TABLE [dbo].[ConfigurationType]
    ADD CONSTRAINT [PK_ConfigurationType] PRIMARY KEY CLUSTERED ([ConfigurationTypeId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Connection]...';


GO
CREATE TABLE [dbo].[Connection] (
    [connectionId]   INT      IDENTITY (1, 1) NOT NULL,
    [connectionTime] DATETIME NOT NULL,
    [deviceId]       INT      NOT NULL
);


GO
PRINT N'Creating PK_Connection...';


GO
ALTER TABLE [dbo].[Connection]
    ADD CONSTRAINT [PK_Connection] PRIMARY KEY CLUSTERED ([connectionId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Connection].[IX_FK_DeviceConnection]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_DeviceConnection]
    ON [dbo].[Connection]([deviceId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Device]...';


GO
CREATE TABLE [dbo].[Device] (
    [deviceId]             INT            IDENTITY (1, 1) NOT NULL,
    [name]                 NVARCHAR (50)  NOT NULL,
    [description]          NVARCHAR (MAX) NOT NULL,
    [os]                   NVARCHAR (50)  NULL,
    [clientVersion]        NVARCHAR (50)  NULL,
    [ipAddress]            NVARCHAR (50)  NULL,
    [lastConnection]       DATETIME       NULL,
    [locationId]           INT            NULL,
    [lockTimeout]          INT            NOT NULL,
    [configurationTimeout] INT            NOT NULL,
    CONSTRAINT [UC_Device_name] UNIQUE NONCLUSTERED ([name] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_Device...';


GO
ALTER TABLE [dbo].[Device]
    ADD CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED ([deviceId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Device].[IX_FK_LocationWCS_Device]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_LocationWCS_Device]
    ON [dbo].[Device]([locationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[DeviceConfiguration]...';


GO
CREATE TABLE [dbo].[DeviceConfiguration] (
    [deviceId]               INT NOT NULL,
    [shortcutKeyNo]          INT NOT NULL,
    [configurationId]        INT NOT NULL,
    [cleaningBedDataTimeout] INT NOT NULL,
    [orderTimeout]           INT NOT NULL,
    [presenceTimeout]        INT NOT NULL,
    [rfidTimeout]            INT NOT NULL,
    [dischargeTimeout]       INT NOT NULL,
    [admissionsTimeout]      INT NOT NULL,
    CONSTRAINT [UC_Device_deviceIdshortcutKeyNo] UNIQUE NONCLUSTERED ([deviceId] ASC, [shortcutKeyNo] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_DeviceConfiguration...';


GO
ALTER TABLE [dbo].[DeviceConfiguration]
    ADD CONSTRAINT [PK_DeviceConfiguration] PRIMARY KEY CLUSTERED ([deviceId] ASC, [shortcutKeyNo] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[DeviceConfiguration].[IX_FK_ConfigurationDeviceConfiguration]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_ConfigurationDeviceConfiguration]
    ON [dbo].[DeviceConfiguration]([configurationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[DeviceConfiguration].[IX_FK_DeviceDeviceConfiguration]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_DeviceDeviceConfiguration]
    ON [dbo].[DeviceConfiguration]([deviceId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Doctor]...';


GO
CREATE TABLE [dbo].[Doctor] (
    [doctorId]         INT           IDENTITY (1, 1) NOT NULL,
    [externalSourceId] INT           NOT NULL,
    [externalId]       NVARCHAR (50) NOT NULL,
    [name]             NVARCHAR (50) NOT NULL,
    CONSTRAINT [UC_Doctor_External] UNIQUE NONCLUSTERED ([externalSourceId] ASC, [externalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_Doctor...';


GO
ALTER TABLE [dbo].[Doctor]
    ADD CONSTRAINT [PK_Doctor] PRIMARY KEY CLUSTERED ([doctorId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Doctor].[IX_Doctor_externalId]...';


GO
CREATE NONCLUSTERED INDEX [IX_Doctor_externalId]
    ON [dbo].[Doctor]([externalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);


GO
PRINT N'Creating [dbo].[Doctor].[IX_FK_ExternalSourceDoctor]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_ExternalSourceDoctor]
    ON [dbo].[Doctor]([externalSourceId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);


GO
PRINT N'Creating [dbo].[ExternalSource]...';


GO
CREATE TABLE [dbo].[ExternalSource] (
    [externalSourceId] INT           IDENTITY (1, 1) NOT NULL,
    [source]           NVARCHAR (20) NOT NULL,
    CONSTRAINT [UC_ExternalSource_source] UNIQUE NONCLUSTERED ([source] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_ExternalSource...';


GO
ALTER TABLE [dbo].[ExternalSource]
    ADD CONSTRAINT [PK_ExternalSource] PRIMARY KEY CLUSTERED ([externalSourceId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Location]...';


GO
CREATE TABLE [dbo].[Location] (
    [locationId]                INT           IDENTITY (1, 1) NOT NULL,
    [code]                      NVARCHAR (20) NOT NULL,
    [name]                      NVARCHAR (50) NOT NULL,
    [isEmergency]               BIT           NOT NULL,
    [contactInfo]               NVARCHAR (20) NULL,
    [includeInMerge]            BIT           NOT NULL,
    [WaitingArea_waitingAreaId] INT           NULL,
    CONSTRAINT [UC_Location_code] UNIQUE NONCLUSTERED ([code] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_Location...';


GO
ALTER TABLE [dbo].[Location]
    ADD CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED ([locationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Location].[IX_FK_WaitingAreaLocation]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_WaitingAreaLocation]
    ON [dbo].[Location]([WaitingArea_waitingAreaId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Log]...';


GO
CREATE TABLE [dbo].[Log] (
    [logId]        INT            IDENTITY (1, 1) NOT NULL,
    [Date]         DATETIME       NOT NULL,
    [ComputerName] NVARCHAR (50)  NOT NULL,
    [Thread]       NVARCHAR (50)  NOT NULL,
    [Level]        NVARCHAR (50)  NOT NULL,
    [Logger]       NVARCHAR (50)  NOT NULL,
    [Message]      NVARCHAR (MAX) NOT NULL,
    [Exception]    NVARCHAR (MAX) NULL
);


GO
PRINT N'Creating PK_Log...';


GO
ALTER TABLE [dbo].[Log]
    ADD CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([logId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Note]...';


GO
CREATE TABLE [dbo].[Note] (
    [noteId]      INT            IDENTITY (1, 1) NOT NULL,
    [source]      NVARCHAR (50)  NULL,
    [notes]       NVARCHAR (MAX) NULL,
    [dateCreated] DATETIME       NULL,
    [noteOrder]   INT            NOT NULL,
    [bedId]       INT            NULL,
    [orderId]     INT            NULL
);


GO
PRINT N'Creating PK_Note...';


GO
ALTER TABLE [dbo].[Note]
    ADD CONSTRAINT [PK_Note] PRIMARY KEY CLUSTERED ([noteId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Note].[IX_FK_BedNote]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_BedNote]
    ON [dbo].[Note]([bedId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Note].[IX_FK_OrderNote]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_OrderNote]
    ON [dbo].[Note]([orderId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Note].[IX_Note_dateCreated]...';


GO
CREATE NONCLUSTERED INDEX [IX_Note_dateCreated]
    ON [dbo].[Note]([dateCreated] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);


GO
PRINT N'Creating [dbo].[Notification]...';


GO
CREATE TABLE [dbo].[Notification] (
    [notificationId]               INT            IDENTITY (1, 1) NOT NULL,
    [notificationTypeId]           INT            NOT NULL,
    [description]                  NVARCHAR (200) NOT NULL,
    [priorToProcedureTime]         INT            NOT NULL,
    [isAcknowledgmentRequired]     BIT            NOT NULL,
    [acknowledged]                 BIT            NOT NULL,
    [acknowledgedTime]             DATETIME       NULL,
    [acknowledgedBy]               NVARCHAR (20)  NULL,
    [notificationOrder]            INT            NULL,
    [orderId]                      INT            NOT NULL,
    [durationMinutes]              INT            NOT NULL,
    [radiationRiskDurationMinutes] INT            NOT NULL
);


GO
PRINT N'Creating PK_Notification...';


GO
ALTER TABLE [dbo].[Notification]
    ADD CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED ([notificationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Notification].[IX_FK_NotificationNotificationType]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_NotificationNotificationType]
    ON [dbo].[Notification]([notificationTypeId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Notification].[IX_FK_OrderNotification]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_OrderNotification]
    ON [dbo].[Notification]([orderId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[NotificationRule]...';


GO
CREATE TABLE [dbo].[NotificationRule] (
    [notificationRuleId]           INT            IDENTITY (1, 1) NOT NULL,
    [description]                  NVARCHAR (200) NOT NULL,
    [priorToProcedureTime]         INT            NOT NULL,
    [durationMinutes]              INT            NOT NULL,
    [radiationRiskDurationMinutes] INT            NOT NULL,
    [isAcknowledgmentRequired]     BIT            NOT NULL,
    [Procedure_procedureId]        INT            NOT NULL
);


GO
PRINT N'Creating PK_NotificationRule...';


GO
ALTER TABLE [dbo].[NotificationRule]
    ADD CONSTRAINT [PK_NotificationRule] PRIMARY KEY CLUSTERED ([notificationRuleId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[NotificationRule].[IX_FK_NotificationRuleProcedure]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_NotificationRuleProcedure]
    ON [dbo].[NotificationRule]([Procedure_procedureId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[NotificationType]...';


GO
CREATE TABLE [dbo].[NotificationType] (
    [notificationTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [name]               NVARCHAR (50) NOT NULL
);


GO
PRINT N'Creating PK_NotificationType...';


GO
ALTER TABLE [dbo].[NotificationType]
    ADD CONSTRAINT [PK_NotificationType] PRIMARY KEY CLUSTERED ([notificationTypeId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Order]...';


GO
CREATE TABLE [dbo].[Order] (
    [orderId]                    INT            IDENTITY (1, 1) NOT NULL,
    [externalSourceId]           INT            NOT NULL,
    [externalId]                 NVARCHAR (50)  NOT NULL,
    [orderNumber]                NVARCHAR (20)  NULL,
    [procedureTime]              DATETIME       NULL,
    [orderStatusId]              INT            NOT NULL,
    [completedTime]              DATETIME       NULL,
    [admissionId]                INT            NOT NULL,
    [clinicalIndicator]          NVARCHAR (200) NULL,
    [estimatedProcedureDuration] INT            NULL,
    [Procedure_procedureId]      INT            NOT NULL,
    [Department_locationId]      INT            NOT NULL,
    [OrderingDoctor_doctorId]    INT            NULL,
    [isHidden]                   BIT            NOT NULL,
    [acknowledged]               BIT            NOT NULL,
    [acknowledgedTime]           DATETIME       NULL,
    [acknowledgedBy]             NVARCHAR (20)  NULL,
    [history]                    NVARCHAR (200) NULL,
    [diagnosis]                  NVARCHAR (200) NULL,
    [currentCardiologist]        NVARCHAR (200) NULL,
    [requiresSupervision]        BIT            NOT NULL,
    [requiresFootwear]           BIT            NOT NULL,
    [requiresMedicalRecords]     BIT            NOT NULL,
    CONSTRAINT [UC_Order_External] UNIQUE NONCLUSTERED ([externalSourceId] ASC, [externalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_Order...';


GO
ALTER TABLE [dbo].[Order]
    ADD CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([orderId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Order].[IX_FK_AdmissionOrder]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_AdmissionOrder]
    ON [dbo].[Order]([admissionId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Order].[IX_FK_ExternalSourceOrder]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_ExternalSourceOrder]
    ON [dbo].[Order]([externalSourceId] ASC, [externalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Order].[IX_FK_OrderLocation]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_OrderLocation]
    ON [dbo].[Order]([Department_locationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Order].[IX_FK_OrderOrderingDoctor]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_OrderOrderingDoctor]
    ON [dbo].[Order]([OrderingDoctor_doctorId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Order].[IX_FK_OrderProcedure]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_OrderProcedure]
    ON [dbo].[Order]([Procedure_procedureId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Order].[IX_FK_OrderStatusOrder]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_OrderStatusOrder]
    ON [dbo].[Order]([orderStatusId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Order].[IX_Order_procedureTime]...';


GO
CREATE NONCLUSTERED INDEX [IX_Order_procedureTime]
    ON [dbo].[Order]([procedureTime] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);


GO
PRINT N'Creating [dbo].[OrderStatus]...';


GO
CREATE TABLE [dbo].[OrderStatus] (
    [orderStatusId] INT           IDENTITY (1, 1) NOT NULL,
    [status]        NVARCHAR (20) NOT NULL,
    CONSTRAINT [UC_OrderStatus_status] UNIQUE NONCLUSTERED ([status] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_OrderStatus...';


GO
ALTER TABLE [dbo].[OrderStatus]
    ADD CONSTRAINT [PK_OrderStatus] PRIMARY KEY CLUSTERED ([orderStatusId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Patient]...';


GO
CREATE TABLE [dbo].[Patient] (
    [patientId]             INT           IDENTITY (1, 1) NOT NULL,
    [externalSourceId]      INT           NOT NULL,
    [externalId]            NVARCHAR (50) NOT NULL,
    [givenName]             NVARCHAR (50) NULL,
    [surname]               NVARCHAR (50) NULL,
    [dob]                   DATETIME      NULL,
    [sex]                   NVARCHAR (20) NULL,
    [isMrsaPositive]        BIT           NOT NULL,
    [isFallRisk]            BIT           NOT NULL,
    [isAssistanceRequired]  BIT           NOT NULL,
    [assistanceDescription] NVARCHAR (50) NOT NULL,
    [hasLatexAllergy]       BIT           NOT NULL,
    CONSTRAINT [UC_Patient_External] UNIQUE NONCLUSTERED ([externalSourceId] ASC, [externalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_Patient...';


GO
ALTER TABLE [dbo].[Patient]
    ADD CONSTRAINT [PK_Patient] PRIMARY KEY CLUSTERED ([patientId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Patient].[IX_FK_ExternalSourcePatient]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_ExternalSourcePatient]
    ON [dbo].[Patient]([externalSourceId] ASC, [externalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Pin]...';


GO
CREATE TABLE [dbo].[Pin] (
    [pinId]           INT           IDENTITY (1, 1) NOT NULL,
    [pin]             NVARCHAR (20) NOT NULL,
    [Device_deviceId] INT           NOT NULL
);


GO
PRINT N'Creating PK_Pin...';


GO
ALTER TABLE [dbo].[Pin]
    ADD CONSTRAINT [PK_Pin] PRIMARY KEY CLUSTERED ([pinId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Pin].[IX_FK_DevicePin]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_DevicePin]
    ON [dbo].[Pin]([Device_deviceId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Procedure]...';


GO
CREATE TABLE [dbo].[Procedure] (
    [procedureId]                           INT            IDENTITY (1, 1) NOT NULL,
    [externalSourceId]                      INT            NOT NULL,
    [externalId]                            NVARCHAR (50)  NOT NULL,
    [code]                                  NVARCHAR (20)  NOT NULL,
    [description]                           NVARCHAR (200) NOT NULL,
    [durationMinutes]                       INT            NULL,
    [ProcedureCategory_procedureCategoryId] INT            NOT NULL,
    CONSTRAINT [UC_Procedure] UNIQUE NONCLUSTERED ([ProcedureCategory_procedureCategoryId] ASC, [code] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF),
    CONSTRAINT [UC_Procedure_External] UNIQUE NONCLUSTERED ([externalSourceId] ASC, [externalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_Procedure...';


GO
ALTER TABLE [dbo].[Procedure]
    ADD CONSTRAINT [PK_Procedure] PRIMARY KEY CLUSTERED ([procedureId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Procedure].[IX_FK_ExternalSourceProcedure]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_ExternalSourceProcedure]
    ON [dbo].[Procedure]([externalSourceId] ASC, [externalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Procedure].[IX_FK_ProcedureCategoryProcedure]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_ProcedureCategoryProcedure]
    ON [dbo].[Procedure]([ProcedureCategory_procedureCategoryId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[ProcedureCategory]...';


GO
CREATE TABLE [dbo].[ProcedureCategory] (
    [procedureCategoryId] INT            IDENTITY (1, 1) NOT NULL,
    [externalSourceId]    INT            NOT NULL,
    [externalId]          NVARCHAR (50)  NOT NULL,
    [includeInMerge]      BIT            NOT NULL,
    [description]         NVARCHAR (200) NOT NULL,
    CONSTRAINT [UC_ProcedureCategory_External] UNIQUE NONCLUSTERED ([externalSourceId] ASC, [externalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_ProcedureCategory...';


GO
ALTER TABLE [dbo].[ProcedureCategory]
    ADD CONSTRAINT [PK_ProcedureCategory] PRIMARY KEY CLUSTERED ([procedureCategoryId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[ProcedureCategory].[IX_FK_ExternalSourceProcedureCategory]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_ExternalSourceProcedureCategory]
    ON [dbo].[ProcedureCategory]([externalSourceId] ASC, [externalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[RfidDetection]...';


GO
CREATE TABLE [dbo].[RfidDetection] (
    [rfidDetectionId]  INT      IDENTITY (1, 1) NOT NULL,
    [patientId]        INT      NOT NULL,
    [rfidDirectionId]  INT      NOT NULL,
    [rfidDetectorId]   INT      NOT NULL,
    [dateTimeDetected] DATETIME NOT NULL
) ON [PRIMARY];


GO
PRINT N'Creating PK_RfidDetection...';


GO
ALTER TABLE [dbo].[RfidDetection]
    ADD CONSTRAINT [PK_RfidDetection] PRIMARY KEY CLUSTERED ([rfidDetectionId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[RfidDetection].[IX_FK_PatientRfidDetection]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_PatientRfidDetection]
    ON [dbo].[RfidDetection]([patientId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[RfidDetection].[IX_FK_RfidDetectorRfidDetection]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_RfidDetectorRfidDetection]
    ON [dbo].[RfidDetection]([rfidDetectorId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[RfidDetection].[IX_FK_RfidDirectionRfidDetection]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_RfidDirectionRfidDetection]
    ON [dbo].[RfidDetection]([rfidDirectionId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[RfidDetection].[IX_RfidDetectionTimestamp]...';


GO
CREATE NONCLUSTERED INDEX [IX_RfidDetectionTimestamp]
    ON [dbo].[RfidDetection]([dateTimeDetected] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[RfidDetector]...';


GO
CREATE TABLE [dbo].[RfidDetector] (
    [rfidDetectorId]            INT           IDENTITY (1, 1) NOT NULL,
    [externalSourceId]          INT           NOT NULL,
    [externalId]                NVARCHAR (20) NOT NULL,
    [Location_locationId]       INT           NULL,
    [WaitingArea_waitingAreaId] INT           NULL,
    CONSTRAINT [UC_RfidDetector_External] UNIQUE NONCLUSTERED ([externalSourceId] ASC, [externalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_RfidDetector...';


GO
ALTER TABLE [dbo].[RfidDetector]
    ADD CONSTRAINT [PK_RfidDetector] PRIMARY KEY CLUSTERED ([rfidDetectorId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[RfidDetector].[IX_FK_RfidDetectorLocation]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_RfidDetectorLocation]
    ON [dbo].[RfidDetector]([Location_locationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);


GO
PRINT N'Creating [dbo].[RfidDetector].[IX_FK_WaitingAreaRfidDetector]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_WaitingAreaRfidDetector]
    ON [dbo].[RfidDetector]([WaitingArea_waitingAreaId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[RfidDirection]...';


GO
CREATE TABLE [dbo].[RfidDirection] (
    [rfidDirectionId] INT           IDENTITY (1, 1) NOT NULL,
    [direction]       NVARCHAR (20) NOT NULL,
    CONSTRAINT [UC_RfidDirection_direction] UNIQUE NONCLUSTERED ([direction] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
) ON [PRIMARY];


GO
PRINT N'Creating PK_RfidDirection...';


GO
ALTER TABLE [dbo].[RfidDirection]
    ADD CONSTRAINT [PK_RfidDirection] PRIMARY KEY CLUSTERED ([rfidDirectionId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Room]...';


GO
CREATE TABLE [dbo].[Room] (
    [roomId]              INT           IDENTITY (1, 1) NOT NULL,
    [name]                NVARCHAR (50) NOT NULL,
    [Location_locationId] INT           NOT NULL,
    CONSTRAINT [UC_Room] UNIQUE NONCLUSTERED ([name] ASC, [Location_locationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_Room...';


GO
ALTER TABLE [dbo].[Room]
    ADD CONSTRAINT [PK_Room] PRIMARY KEY CLUSTERED ([roomId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Room].[IX_FK_LocationRoom]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_LocationRoom]
    ON [dbo].[Room]([Location_locationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Staging_Admission]...';


GO
CREATE TABLE [dbo].[Staging_Admission] (
    [admissionExternalSource]     NVARCHAR (20) NOT NULL,
    [admissionExternalId]         NVARCHAR (20) NOT NULL,
    [admissionType]               NVARCHAR (20) NULL,
    [admissionStatus]             NVARCHAR (20) NULL,
    [admitDateTime]               DATETIME      NULL,
    [dischargeDateTime]           DATETIME      NULL,
    [patientExternalId]           NVARCHAR (20) NOT NULL,
    [patientGivenName]            NVARCHAR (50) NULL,
    [patientSurname]              NVARCHAR (50) NULL,
    [patientSex]                  NVARCHAR (20) NULL,
    [patientDOB]                  DATETIME      NULL,
    [isMrsaPositive]              BIT           NOT NULL,
    [isFallRisk]                  BIT           NOT NULL,
    [isAssistanceRequired]        BIT           NOT NULL,
    [assistanceDescription]       NVARCHAR (50) NULL,
    [hasLatexAllergy]             BIT           NOT NULL,
    [location]                    NVARCHAR (20) NULL,
    [room]                        NVARCHAR (50) NULL,
    [bed]                         NVARCHAR (50) NULL,
    [doctorExternalSource]        NVARCHAR (20) NOT NULL,
    [attendingDoctorExternalId]   NVARCHAR (50) NULL,
    [attendingDoctorName]         NVARCHAR (50) NULL,
    [admittingDoctorExternalId]   NVARCHAR (50) NULL,
    [admittingDoctorName]         NVARCHAR (50) NULL,
    [primaryCareDoctorExternalId] NVARCHAR (50) NULL,
    [primaryCareDoctorName]       NVARCHAR (50) NULL
);


GO
PRINT N'Creating PK_Staging_Admission...';


GO
ALTER TABLE [dbo].[Staging_Admission]
    ADD CONSTRAINT [PK_Staging_Admission] PRIMARY KEY CLUSTERED ([admissionExternalSource] ASC, [admissionExternalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Staging_Order]...';


GO
CREATE TABLE [dbo].[Staging_Order] (
    [orderExternalSource]         NVARCHAR (20)  NOT NULL,
    [orderExternalId]             NVARCHAR (20)  NOT NULL,
    [orderNumber]                 NVARCHAR (50)  NULL,
    [department]                  NVARCHAR (50)  NOT NULL,
    [procedureCategoryExternalId] NVARCHAR (20)  NULL,
    [procedureExternalId]         NVARCHAR (20)  NULL,
    [procedureCode]               NVARCHAR (20)  NULL,
    [procedureDescription]        NVARCHAR (200) NULL,
    [procedureTime]               DATETIME       NULL,
    [status]                      NVARCHAR (20)  NULL,
    [clinicalIndicators]          NVARCHAR (200) NULL,
    [completedDateTime]           DATETIME       NULL,
    [estimatedDuration]           INT            NULL,
    [admissionExternalSource]     NVARCHAR (20)  NOT NULL,
    [admissionExternalId]         NVARCHAR (20)  NULL,
    [doctorExternalSource]        NVARCHAR (20)  NOT NULL,
    [orderingDoctorExternalId]    NVARCHAR (50)  NULL,
    [orderingDoctorName]          NVARCHAR (50)  NULL,
    [isHidden]                    BIT            NOT NULL,
    [history]                     NVARCHAR (200) NULL,
    [diagnosis]                   NVARCHAR (200) NULL,
    [currentCardiologist]         NVARCHAR (200) NULL,
    [requiresSupervision]         BIT            NOT NULL,
    [requiresFootwear]            BIT            NOT NULL,
    [requiresMedicalRecords]      BIT            NOT NULL
);


GO
PRINT N'Creating PK_Staging_Order...';


GO
ALTER TABLE [dbo].[Staging_Order]
    ADD CONSTRAINT [PK_Staging_Order] PRIMARY KEY CLUSTERED ([orderExternalSource] ASC, [orderExternalId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Update]...';


GO
CREATE TABLE [dbo].[Update] (
    [updateId]              INT            IDENTITY (1, 1) NOT NULL,
    [type]                  NVARCHAR (50)  NULL,
    [source]                NVARCHAR (50)  NULL,
    [value]                 NVARCHAR (MAX) NULL,
    [dateCreated]           DATETIME       NULL,
    [Bed_bedId]             INT            NULL,
    [Order_orderId]         INT            NULL,
    [Admission_admissionId] INT            NULL
);


GO
PRINT N'Creating PK_Update...';


GO
ALTER TABLE [dbo].[Update]
    ADD CONSTRAINT [PK_Update] PRIMARY KEY CLUSTERED ([updateId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[Update].[IX_FK_UpdateAdmission]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_UpdateAdmission]
    ON [dbo].[Update]([Admission_admissionId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);


GO
PRINT N'Creating [dbo].[Update].[IX_FK_UpdateBed]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_UpdateBed]
    ON [dbo].[Update]([Bed_bedId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Update].[IX_FK_UpdateOrder]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_UpdateOrder]
    ON [dbo].[Update]([Order_orderId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[Update].[IX_Update_dateCreated]...';


GO
CREATE NONCLUSTERED INDEX [IX_Update_dateCreated]
    ON [dbo].[Update]([dateCreated] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);


GO
PRINT N'Creating [dbo].[WaitingArea]...';


GO
CREATE TABLE [dbo].[WaitingArea] (
    [waitingAreaId] INT           IDENTITY (1, 1) NOT NULL,
    [code]          NVARCHAR (20) NOT NULL,
    [name]          NVARCHAR (50) NOT NULL,
    CONSTRAINT [UC_WaitingArea_code] UNIQUE NONCLUSTERED ([code] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
);


GO
PRINT N'Creating PK_WaitingArea...';


GO
ALTER TABLE [dbo].[WaitingArea]
    ADD CONSTRAINT [PK_WaitingArea] PRIMARY KEY CLUSTERED ([waitingAreaId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating On column: requiresSupervision...';


GO
ALTER TABLE [dbo].[Order]
    ADD DEFAULT (0) FOR [requiresSupervision];


GO
PRINT N'Creating On column: requiresFootwear...';


GO
ALTER TABLE [dbo].[Order]
    ADD DEFAULT (0) FOR [requiresFootwear];


GO
PRINT N'Creating On column: requiresMedicalRecords...';


GO
ALTER TABLE [dbo].[Order]
    ADD DEFAULT (0) FOR [requiresMedicalRecords];


GO
PRINT N'Creating On column: requiresSupervision...';


GO
ALTER TABLE [dbo].[Staging_Order]
    ADD DEFAULT (0) FOR [requiresSupervision];


GO
PRINT N'Creating On column: requiresFootwear...';


GO
ALTER TABLE [dbo].[Staging_Order]
    ADD DEFAULT (0) FOR [requiresFootwear];


GO
PRINT N'Creating On column: requiresMedicalRecords...';


GO
ALTER TABLE [dbo].[Staging_Order]
    ADD DEFAULT (0) FOR [requiresMedicalRecords];


GO
PRINT N'Creating FK_AdmissionAdmissionStatus...';


GO
ALTER TABLE [dbo].[Admission] WITH NOCHECK
    ADD CONSTRAINT [FK_AdmissionAdmissionStatus] FOREIGN KEY ([AdmissionStatus_admissionStatusId]) REFERENCES [dbo].[AdmissionStatus] ([admissionStatusId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_AdmissionAdmissionType...';


GO
ALTER TABLE [dbo].[Admission] WITH NOCHECK
    ADD CONSTRAINT [FK_AdmissionAdmissionType] FOREIGN KEY ([AdmissionType_admissionTypeId]) REFERENCES [dbo].[AdmissionType] ([admissionTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_AdmissionAdmittingDoctorDoctor...';


GO
ALTER TABLE [dbo].[Admission] WITH NOCHECK
    ADD CONSTRAINT [FK_AdmissionAdmittingDoctorDoctor] FOREIGN KEY ([AdmittingDoctor_doctorId]) REFERENCES [dbo].[Doctor] ([doctorId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_AdmissionAttendingDoctorDoctor...';


GO
ALTER TABLE [dbo].[Admission] WITH NOCHECK
    ADD CONSTRAINT [FK_AdmissionAttendingDoctorDoctor] FOREIGN KEY ([AttendingDoctor_doctorId]) REFERENCES [dbo].[Doctor] ([doctorId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_AdmissionBed...';


GO
ALTER TABLE [dbo].[Admission] WITH NOCHECK
    ADD CONSTRAINT [FK_AdmissionBed] FOREIGN KEY ([Bed_bedId]) REFERENCES [dbo].[Bed] ([bedId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_AdmissionLocation...';


GO
ALTER TABLE [dbo].[Admission] WITH NOCHECK
    ADD CONSTRAINT [FK_AdmissionLocation] FOREIGN KEY ([Location_locationId]) REFERENCES [dbo].[Location] ([locationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_AdmissionPrimaryCareDoctor...';


GO
ALTER TABLE [dbo].[Admission] WITH NOCHECK
    ADD CONSTRAINT [FK_AdmissionPrimaryCareDoctor] FOREIGN KEY ([PrimaryCareDoctor_doctorId]) REFERENCES [dbo].[Doctor] ([doctorId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_ExternalSourceAdmission...';


GO
ALTER TABLE [dbo].[Admission] WITH NOCHECK
    ADD CONSTRAINT [FK_ExternalSourceAdmission] FOREIGN KEY ([externalSourceId]) REFERENCES [dbo].[ExternalSource] ([externalSourceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_PatientAdmission...';


GO
ALTER TABLE [dbo].[Admission] WITH NOCHECK
    ADD CONSTRAINT [FK_PatientAdmission] FOREIGN KEY ([patientId]) REFERENCES [dbo].[Patient] ([patientId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_BedRoom...';


GO
ALTER TABLE [dbo].[Bed] WITH NOCHECK
    ADD CONSTRAINT [FK_BedRoom] FOREIGN KEY ([Room_roomId]) REFERENCES [dbo].[Room] ([roomId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_BedCleaningEventBed...';


GO
ALTER TABLE [dbo].[BedCleaningEvent] WITH NOCHECK
    ADD CONSTRAINT [FK_BedCleaningEventBed] FOREIGN KEY ([Bed_bedId]) REFERENCES [dbo].[Bed] ([bedId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_BedCleaningEventBedCleaningEventType...';


GO
ALTER TABLE [dbo].[BedCleaningEvent] WITH NOCHECK
    ADD CONSTRAINT [FK_BedCleaningEventBedCleaningEventType] FOREIGN KEY ([bedCleaningEventTypeId]) REFERENCES [dbo].[BedCleaningEventType] ([bedCleaningEventTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_ConfigurationConfigurationType...';


GO
ALTER TABLE [dbo].[Configuration] WITH NOCHECK
    ADD CONSTRAINT [FK_ConfigurationConfigurationType] FOREIGN KEY ([ConfigurationType_ConfigurationTypeId]) REFERENCES [dbo].[ConfigurationType] ([ConfigurationTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_ConfigurationConfigurationLocation...';


GO
ALTER TABLE [dbo].[ConfigurationLocation] WITH NOCHECK
    ADD CONSTRAINT [FK_ConfigurationConfigurationLocation] FOREIGN KEY ([configurationId]) REFERENCES [dbo].[Configuration] ([configurationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_LocationConfigurationLocation...';


GO
ALTER TABLE [dbo].[ConfigurationLocation] WITH NOCHECK
    ADD CONSTRAINT [FK_LocationConfigurationLocation] FOREIGN KEY ([locationId]) REFERENCES [dbo].[Location] ([locationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_DeviceConnection...';


GO
ALTER TABLE [dbo].[Connection] WITH NOCHECK
    ADD CONSTRAINT [FK_DeviceConnection] FOREIGN KEY ([deviceId]) REFERENCES [dbo].[Device] ([deviceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_LocationWCS_Device...';


GO
ALTER TABLE [dbo].[Device] WITH NOCHECK
    ADD CONSTRAINT [FK_LocationWCS_Device] FOREIGN KEY ([locationId]) REFERENCES [dbo].[Location] ([locationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_ConfigurationDeviceConfiguration...';


GO
ALTER TABLE [dbo].[DeviceConfiguration] WITH NOCHECK
    ADD CONSTRAINT [FK_ConfigurationDeviceConfiguration] FOREIGN KEY ([configurationId]) REFERENCES [dbo].[Configuration] ([configurationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_DeviceDeviceConfiguration...';


GO
ALTER TABLE [dbo].[DeviceConfiguration] WITH NOCHECK
    ADD CONSTRAINT [FK_DeviceDeviceConfiguration] FOREIGN KEY ([deviceId]) REFERENCES [dbo].[Device] ([deviceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_ExternalSourceDoctor...';


GO
ALTER TABLE [dbo].[Doctor] WITH NOCHECK
    ADD CONSTRAINT [FK_ExternalSourceDoctor] FOREIGN KEY ([externalSourceId]) REFERENCES [dbo].[ExternalSource] ([externalSourceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_WaitingAreaLocation...';


GO
ALTER TABLE [dbo].[Location] WITH NOCHECK
    ADD CONSTRAINT [FK_WaitingAreaLocation] FOREIGN KEY ([WaitingArea_waitingAreaId]) REFERENCES [dbo].[WaitingArea] ([waitingAreaId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_BedNote...';


GO
ALTER TABLE [dbo].[Note] WITH NOCHECK
    ADD CONSTRAINT [FK_BedNote] FOREIGN KEY ([bedId]) REFERENCES [dbo].[Bed] ([bedId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_OrderNote...';


GO
ALTER TABLE [dbo].[Note] WITH NOCHECK
    ADD CONSTRAINT [FK_OrderNote] FOREIGN KEY ([orderId]) REFERENCES [dbo].[Order] ([orderId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_NotificationNotificationType...';


GO
ALTER TABLE [dbo].[Notification] WITH NOCHECK
    ADD CONSTRAINT [FK_NotificationNotificationType] FOREIGN KEY ([notificationTypeId]) REFERENCES [dbo].[NotificationType] ([notificationTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_OrderNotification...';


GO
ALTER TABLE [dbo].[Notification] WITH NOCHECK
    ADD CONSTRAINT [FK_OrderNotification] FOREIGN KEY ([orderId]) REFERENCES [dbo].[Order] ([orderId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_NotificationRuleProcedure...';


GO
ALTER TABLE [dbo].[NotificationRule] WITH NOCHECK
    ADD CONSTRAINT [FK_NotificationRuleProcedure] FOREIGN KEY ([Procedure_procedureId]) REFERENCES [dbo].[Procedure] ([procedureId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_AdmissionOrder...';


GO
ALTER TABLE [dbo].[Order] WITH NOCHECK
    ADD CONSTRAINT [FK_AdmissionOrder] FOREIGN KEY ([admissionId]) REFERENCES [dbo].[Admission] ([admissionId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_ExternalSourceOrder...';


GO
ALTER TABLE [dbo].[Order] WITH NOCHECK
    ADD CONSTRAINT [FK_ExternalSourceOrder] FOREIGN KEY ([externalSourceId]) REFERENCES [dbo].[ExternalSource] ([externalSourceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_OrderLocation...';


GO
ALTER TABLE [dbo].[Order] WITH NOCHECK
    ADD CONSTRAINT [FK_OrderLocation] FOREIGN KEY ([Department_locationId]) REFERENCES [dbo].[Location] ([locationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_OrderOrderingDoctor...';


GO
ALTER TABLE [dbo].[Order] WITH NOCHECK
    ADD CONSTRAINT [FK_OrderOrderingDoctor] FOREIGN KEY ([OrderingDoctor_doctorId]) REFERENCES [dbo].[Doctor] ([doctorId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_OrderProcedure...';


GO
ALTER TABLE [dbo].[Order] WITH NOCHECK
    ADD CONSTRAINT [FK_OrderProcedure] FOREIGN KEY ([Procedure_procedureId]) REFERENCES [dbo].[Procedure] ([procedureId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_OrderStatusOrder...';


GO
ALTER TABLE [dbo].[Order] WITH NOCHECK
    ADD CONSTRAINT [FK_OrderStatusOrder] FOREIGN KEY ([orderStatusId]) REFERENCES [dbo].[OrderStatus] ([orderStatusId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_PatientExternalSource...';


GO
ALTER TABLE [dbo].[Patient] WITH NOCHECK
    ADD CONSTRAINT [FK_PatientExternalSource] FOREIGN KEY ([externalSourceId]) REFERENCES [dbo].[ExternalSource] ([externalSourceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_DevicePin...';


GO
ALTER TABLE [dbo].[Pin] WITH NOCHECK
    ADD CONSTRAINT [FK_DevicePin] FOREIGN KEY ([Device_deviceId]) REFERENCES [dbo].[Device] ([deviceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_ExternalSourceProcedure...';


GO
ALTER TABLE [dbo].[Procedure] WITH NOCHECK
    ADD CONSTRAINT [FK_ExternalSourceProcedure] FOREIGN KEY ([externalSourceId]) REFERENCES [dbo].[ExternalSource] ([externalSourceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_ProcedureCategoryProcedure...';


GO
ALTER TABLE [dbo].[Procedure] WITH NOCHECK
    ADD CONSTRAINT [FK_ProcedureCategoryProcedure] FOREIGN KEY ([ProcedureCategory_procedureCategoryId]) REFERENCES [dbo].[ProcedureCategory] ([procedureCategoryId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_ExternalSourceProcedureCategory...';


GO
ALTER TABLE [dbo].[ProcedureCategory] WITH NOCHECK
    ADD CONSTRAINT [FK_ExternalSourceProcedureCategory] FOREIGN KEY ([externalSourceId]) REFERENCES [dbo].[ExternalSource] ([externalSourceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_RfidDetectionPatient...';


GO
ALTER TABLE [dbo].[RfidDetection] WITH NOCHECK
    ADD CONSTRAINT [FK_RfidDetectionPatient] FOREIGN KEY ([patientId]) REFERENCES [dbo].[Patient] ([patientId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_RfidDetectionRfidDetector...';


GO
ALTER TABLE [dbo].[RfidDetection] WITH NOCHECK
    ADD CONSTRAINT [FK_RfidDetectionRfidDetector] FOREIGN KEY ([rfidDetectorId]) REFERENCES [dbo].[RfidDetector] ([rfidDetectorId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_RfidDetectionRfidDirection...';


GO
ALTER TABLE [dbo].[RfidDetection] WITH NOCHECK
    ADD CONSTRAINT [FK_RfidDetectionRfidDirection] FOREIGN KEY ([rfidDirectionId]) REFERENCES [dbo].[RfidDirection] ([rfidDirectionId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_RfidDetectorExternalSource...';


GO
ALTER TABLE [dbo].[RfidDetector] WITH NOCHECK
    ADD CONSTRAINT [FK_RfidDetectorExternalSource] FOREIGN KEY ([externalSourceId]) REFERENCES [dbo].[ExternalSource] ([externalSourceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_RfidDetectorLocation...';


GO
ALTER TABLE [dbo].[RfidDetector] WITH NOCHECK
    ADD CONSTRAINT [FK_RfidDetectorLocation] FOREIGN KEY ([Location_locationId]) REFERENCES [dbo].[Location] ([locationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_WaitingAreaRfidDetector...';


GO
ALTER TABLE [dbo].[RfidDetector] WITH NOCHECK
    ADD CONSTRAINT [FK_WaitingAreaRfidDetector] FOREIGN KEY ([WaitingArea_waitingAreaId]) REFERENCES [dbo].[WaitingArea] ([waitingAreaId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_LocationRoom...';


GO
ALTER TABLE [dbo].[Room] WITH NOCHECK
    ADD CONSTRAINT [FK_LocationRoom] FOREIGN KEY ([Location_locationId]) REFERENCES [dbo].[Location] ([locationId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_UpdateAdmission...';


GO
ALTER TABLE [dbo].[Update] WITH NOCHECK
    ADD CONSTRAINT [FK_UpdateAdmission] FOREIGN KEY ([Admission_admissionId]) REFERENCES [dbo].[Admission] ([admissionId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_UpdateBed...';


GO
ALTER TABLE [dbo].[Update] WITH NOCHECK
    ADD CONSTRAINT [FK_UpdateBed] FOREIGN KEY ([Bed_bedId]) REFERENCES [dbo].[Bed] ([bedId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_UpdateOrder...';


GO
ALTER TABLE [dbo].[Update] WITH NOCHECK
    ADD CONSTRAINT [FK_UpdateOrder] FOREIGN KEY ([Order_orderId]) REFERENCES [dbo].[Order] ([orderId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating [dbo].[trgMakeOrderPhysioNotification]...';


GO
CREATE TRIGGER trgMakeOrderPhysioNotification ON dbo.[Order] 
FOR INSERT 
AS
INSERT INTO [dbo].[Notification]
           ([description]
		   ,[notificationTypeId]
           ,[priorToProcedureTime]
           ,[isAcknowledgmentRequired]
           ,[acknowledged]
           ,[acknowledgedTime]
           ,[acknowledgedBy]
           ,[notificationOrder]
           ,[orderId]
           ,[durationMinutes]
		   ,[radiationRiskDurationMinutes])
SELECT
	P.assistanceDescription
	, ( SELECT notificationTypeId FROM [NotificationType] WHERE [name] = 'Physio' )
	, 15
	, 1
	, 0
	, null
	, null
	, null
	, O.orderid
	, 15
	, 0
FROM
Inserted I
INNER JOIN [Order] O ON I.orderid = O.orderid
INNER JOIN [Admission] A ON O.admissionId = A.admissionId
INNER JOIN [Patient] P ON A.patientId = P.patientId
WHERE P.isAssistanceRequired = 1
GO
PRINT N'Creating [dbo].[trgMakeOrderPrepNotification]...';


GO
CREATE TRIGGER trgMakeOrderPrepNotification ON dbo.[Order] 
FOR INSERT 
AS
INSERT INTO [dbo].[Notification]
           ([description]
		   ,[notificationTypeId]
           ,[priorToProcedureTime]
           ,[isAcknowledgmentRequired]
           ,[acknowledged]
           ,[acknowledgedTime]
           ,[acknowledgedBy]
           ,[notificationOrder]
           ,[orderId]
           ,[durationMinutes]
		   ,[radiationRiskDurationMinutes])
SELECT
	NR.[description]
	, ( SELECT notificationTypeId FROM [NotificationType] WHERE [name] = 'Prep' )
	, priorToProcedureTime
	, isAcknowledgmentRequired
	, 0
	, null
	, null
	, null
	, O.orderid
	, NR.durationMinutes
	, NR.radiationRiskDurationMinutes
FROM notificationRule NR
INNER JOIN [order] O ON O.Procedure_procedureId = NR.Procedure_procedureId
INNER JOIN inserted i ON i.orderid = o.orderid
GO
PRINT N'Creating [dbo].[InsertUpdateAdmission]...';


GO
CREATE PROCEDURE [dbo].[InsertUpdateAdmission]
	@externalSourceId int
	,@externalId nvarchar(50)
	,@admitDateTime datetime
	,@dischargeDateTime datetime
	,@patientId int
	,@PrimaryCareDoctor_doctorId int
	,@AttendingDoctor_doctorId int
	,@AdmittingDoctor_doctorId int
	,@AdmissionType_admissionTypeId int
	,@AdmissionStatus_admissionStatusId int
	,@Location_locationId int
	,@Bed_bedId int
	,@source nvarchar(50)
AS

-- Create a temporary table variable to hold the output actions.
DECLARE @mergeOutput TABLE(Change VARCHAR(20), admissionId int);

MERGE [Admission] AS dest
USING
(
SELECT
	@externalSourceId
	,@externalId
	,@admitDateTime
	,@dischargeDateTime
	,@patientId
	,@PrimaryCareDoctor_doctorId
	,@AttendingDoctor_doctorId
	,@AdmittingDoctor_doctorId
	,@AdmissionType_admissionTypeId
	,@AdmissionStatus_admissionStatusId
	,@Location_locationId
	,@Bed_bedId
) AS source
(
	[externalSourceId]
	,[externalId]
	,[admitDateTime]
	,[dischargeDateTime]
	,[patientId]
	,[PrimaryCareDoctor_doctorId]
	,[AttendingDoctor_doctorId]
	,[AdmittingDoctor_doctorId]
	,[AdmissionType_admissionTypeId]
	,[AdmissionStatus_admissionStatusId]
	,[Location_locationId]
	,[Bed_bedId]
)
ON
(
	dest.externalSourceId = source.externalSourceId
	AND dest.externalId = source.externalId
)
WHEN MATCHED AND 
	dest.[admitDateTime] <> @admitDateTime
	OR dest.[dischargeDateTime] <> @dischargeDateTime
	OR dest.[patientId] <> @patientId
	OR dest.[PrimaryCareDoctor_doctorId] <> @PrimaryCareDoctor_doctorId
	OR dest.[AttendingDoctor_doctorId] <> @AttendingDoctor_doctorId
	OR dest.[AdmittingDoctor_doctorId] <> @AdmittingDoctor_doctorId
	OR dest.[AdmissionType_admissionTypeId] <> @AdmissionType_admissionTypeId
	OR dest.[AdmissionStatus_admissionStatusId] <> @AdmissionStatus_admissionStatusId
	OR dest.[Location_locationId] <> @Location_locationId
	OR dest.[Bed_bedId] <> @Bed_bedId
THEN 
UPDATE
	SET
		[admitDateTime] = @admitDateTime
		,[dischargeDateTime] = @dischargeDateTime
		,[patientId] = @patientId
		,[PrimaryCareDoctor_doctorId] = @PrimaryCareDoctor_doctorId
		,[AttendingDoctor_doctorId] = @AttendingDoctor_doctorId
		,[AdmittingDoctor_doctorId] = @AdmittingDoctor_doctorId
		,[AdmissionType_admissionTypeId] = @AdmissionType_admissionTypeId
		,[AdmissionStatus_admissionStatusId] = @AdmissionStatus_admissionStatusId
		,[Location_locationId] = @Location_locationId
		,[Bed_bedId] = @Bed_bedId
WHEN NOT MATCHED THEN	
	INSERT 
		(
		[externalSourceId]
		,[externalId]
		,[admitDateTime]
		,[dischargeDateTime]
		,[patientId]
		,[PrimaryCareDoctor_doctorId]
		,[AttendingDoctor_doctorId]
		,[AdmittingDoctor_doctorId]
		,[AdmissionType_admissionTypeId]
		,[AdmissionStatus_admissionStatusId]
		,[Location_locationId]
		,[Bed_bedId]
		)
     VALUES
		(
		@externalSourceId
		,@externalId
		,@admitDateTime
		,@dischargeDateTime
		,@patientId
		,@PrimaryCareDoctor_doctorId
		,@AttendingDoctor_doctorId
		,@AdmittingDoctor_doctorId
		,@AdmissionType_admissionTypeId
		,@AdmissionStatus_admissionStatusId
		,@Location_locationId
		,@Bed_bedId
		)
OUTPUT $action, inserted.admissionId INTO @mergeOutput;

INSERT INTO [Update]
	(
	[type]
	,[source]
	,[value]
	,[dateCreated]
	,[Bed_bedId]
	,[Admission_admissionId]
	)
SELECT 
	CASE Change
	WHEN 'INSERT' THEN 'Admission Imported'
	WHEN 'UPDATE' THEN 'Admission Updated'
	END
	,@source
	,@externalId
	,GETDATE()
	,@Bed_bedId
	,admissionId
FROM
	@mergeOutput

RETURN 0
GO
PRINT N'Creating [dbo].[InsertUpdateOrder]...';


GO
CREATE PROCEDURE [dbo].[InsertUpdateOrder]
	@externalSourceId int
	,@externalId nvarchar(50)
	,@orderNumber nvarchar(20)
	,@procedureTime datetime
	,@orderStatusId int
	,@completedTime datetime
	,@admissionId int
	,@clinicalIndicator nvarchar(200)
	,@estimatedProcedureDuration int
	,@Procedure_procedureId int
	,@Department_locationId int
	,@OrderingDoctor_doctorId int
	,@isHidden bit
	,@source nvarchar(50)
	,@history nvarchar(200)
	,@diagnosis nvarchar(200)
	,@currentCardiologist nvarchar(200)
	,@requiresSupervision bit
	,@requiresFootwear bit
	,@requiresMedicalRecords bit
AS

-- Create a temporary table variable to hold the output actions.
DECLARE @mergeOutput TABLE(Change VARCHAR(20), orderId int);

MERGE [Order] AS dest
USING
(
SELECT
	@externalSourceId
	,@externalId
	,@orderNumber
	,@procedureTime
	,@orderStatusId
	,@completedTime
	,@admissionId
	,@clinicalIndicator
	,@estimatedProcedureDuration
	,@Procedure_procedureId
	,@Department_locationId
	,@OrderingDoctor_doctorId
	,@isHidden
	,@history
	,@diagnosis
	,@currentCardiologist
	,@requiresSupervision
	,@requiresFootwear
	,@requiresMedicalRecords

) AS source
(
	[externalSourceId]
	,[externalId]
	,[orderNumber]
	,[procedureTime]
	,[orderStatusId]
	,[completedTime]
	,[admissionId]
	,[clinicalIndicator]
	,[estimatedProcedureDuration]
	,[Procedure_procedureId]
	,[Department_locationId]
	,[OrderingDoctor_doctorId]
	,[isHidden]
	,[history]
	,[diagnosis]
	,[currentCardiologist]
	,[requiresSupervision]
	,[requiresFootwear]
	,[requiresMedicalRecords]
)
ON
(
	dest.externalSourceId = source.externalSourceId
	AND dest.externalId = source.externalId
)
WHEN MATCHED AND
	dest.[orderStatusId] <> @orderStatusId
	OR dest.[completedTime] <> @completedTime
THEN 
UPDATE
	SET
		[orderStatusId] = @orderStatusId
		,[completedTime] = @completedTime
WHEN NOT MATCHED THEN
	INSERT 
		(
			[externalSourceId]
			,[externalId]
			,[orderNumber]
			,[procedureTime]
			,[orderStatusId]
			,[completedTime]
			,[admissionId]
			,[clinicalIndicator]
			,[estimatedProcedureDuration]
			,[Procedure_procedureId]
			,[Department_locationId]
			,[OrderingDoctor_doctorId]
			,[isHidden]
			,[acknowledged]
			,[acknowledgedTime]
			,[acknowledgedBy]
			,[history]
			,[diagnosis]
			,[currentCardiologist]
			,[requiresSupervision]
			,[requiresFootwear]
			,[requiresMedicalRecords]
		)
		VALUES
		(
			@externalSourceId
			,@externalId
			,@orderNumber
			,@procedureTime
			,@orderStatusId
			,@completedTime
			,@admissionId
			,@clinicalIndicator
			,@estimatedProcedureDuration
			,@Procedure_procedureId
			,@Department_locationId
			,@OrderingDoctor_doctorId
			,@isHidden
			,0
			,null
			,null
			,@history
			,@diagnosis
			,@currentCardiologist
			,@requiresSupervision
			,@requiresFootwear
			,@requiresMedicalRecords
		)
OUTPUT $action, inserted.orderId INTO @mergeOutput;

INSERT INTO [Update]
	(
	[type]
	,[source]
	,[value]
	,[dateCreated]
	,[Order_orderId]
	)
SELECT 
	CASE Change
	WHEN 'INSERT' THEN 'Order Imported'
	WHEN 'UPDATE' THEN 'Order Updated'
	END
	,@source
	,@externalId
	,GETDATE()
	,orderId
FROM
	@mergeOutput

RETURN 0
GO
-- Refactoring step to update target server with deployed transaction logs
CREATE TABLE  [dbo].[__RefactorLog] (OperationKey UNIQUEIDENTIFIER NOT NULL PRIMARY KEY)
GO
sp_addextendedproperty N'microsoft_database_tools_support', N'refactoring log', N'schema', N'dbo', N'table', N'__RefactorLog'
GO

GO
/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

SET NOCOUNT ON

PRINT 'Inserting Reference Data...'

/****** Object:  Table [dbo].[OrderStatus]    Script Date: 05/01/2012 01:13:41 ******/
SET IDENTITY_INSERT [dbo].[OrderStatus] ON
INSERT [dbo].[OrderStatus] ([orderStatusId], [status]) VALUES (1, N'InProgress')
INSERT [dbo].[OrderStatus] ([orderStatusId], [status]) VALUES (2, N'Completed')
INSERT [dbo].[OrderStatus] ([orderStatusId], [status]) VALUES (3, N'Cancelled')
SET IDENTITY_INSERT [dbo].[OrderStatus] OFF

/****** Object:  Table [dbo].[ConfigurationType]    Script Date: 05/01/2012 01:13:41 ******/
SET IDENTITY_INSERT [dbo].[ConfigurationType] ON
INSERT [dbo].[ConfigurationType] ([ConfigurationTypeId], [name]) VALUES (1, N'Ward')
INSERT [dbo].[ConfigurationType] ([ConfigurationTypeId], [name]) VALUES (2, N'Department')
INSERT [dbo].[ConfigurationType] ([ConfigurationTypeId], [name]) VALUES (3, N'Cleaning')
INSERT [dbo].[ConfigurationType] ([ConfigurationTypeId], [name]) VALUES (4, N'Physio')
INSERT [dbo].[ConfigurationType] ([ConfigurationTypeId], [name]) VALUES (5, N'Admissions')
INSERT [dbo].[ConfigurationType] ([ConfigurationTypeId], [name]) VALUES (6, N'Discharge')
SET IDENTITY_INSERT [dbo].[ConfigurationType] OFF

/****** Object:  Table [dbo].[BedCleaningEventType]    Script Date: 05/01/2012 01:13:41 ******/
SET IDENTITY_INSERT [dbo].[BedCleaningEventType] ON
INSERT [dbo].[BedCleaningEventType] ([bedCleaningEventTypeId], [eventType]) VALUES (1, N'BedCleaned')
INSERT [dbo].[BedCleaningEventType] ([bedCleaningEventTypeId], [eventType]) VALUES (2, N'BedMarkedAsDirty')
SET IDENTITY_INSERT [dbo].[BedCleaningEventType] OFF

/****** Object:  Table [dbo].[AdmissionType]    Script Date: 05/01/2012 01:13:41 ******/
SET IDENTITY_INSERT [dbo].[AdmissionType] ON
INSERT [dbo].[AdmissionType] ([admissionTypeId], [type]) VALUES (1, N'In')
INSERT [dbo].[AdmissionType] ([admissionTypeId], [type]) VALUES (2, N'Out')
INSERT [dbo].[AdmissionType] ([admissionTypeId], [type]) VALUES (3, N'Day')
SET IDENTITY_INSERT [dbo].[AdmissionType] OFF

/****** Object:  Table [dbo].[AdmissionStatus]    Script Date: 05/01/2012 01:13:41 ******/
SET IDENTITY_INSERT [dbo].[AdmissionStatus] ON
INSERT [dbo].[AdmissionStatus] ([admissionStatusId], [status]) VALUES (1, N'Registered')
INSERT [dbo].[AdmissionStatus] ([admissionStatusId], [status]) VALUES (2, N'Admitted')
INSERT [dbo].[AdmissionStatus] ([admissionStatusId], [status]) VALUES (3, N'Discharged')
SET IDENTITY_INSERT [dbo].[AdmissionStatus] OFF

/****** Object:  Table [dbo].[RfidDirection]    Script Date: 05/01/2012 01:13:41 ******/
SET IDENTITY_INSERT [dbo].[RfidDirection] ON
INSERT [dbo].[RfidDirection]([rfidDirectionId], [direction]) VALUES (1, 'In')
INSERT [dbo].[RfidDirection]([rfidDirectionId], [direction]) VALUES (2, 'Out')
SET IDENTITY_INSERT [dbo].[RfidDirection] OFF

/****** Object:  Table [dbo].[NotificationType]    Script Date: 05/01/2012 01:13:41 ******/
SET IDENTITY_INSERT [dbo].[NotificationType] ON
INSERT [dbo].[NotificationType]([notificationTypeId], [name]) VALUES (1, 'Prep')
INSERT [dbo].[NotificationType]([notificationTypeId], [name]) VALUES (2, 'Physio')
SET IDENTITY_INSERT [dbo].[NotificationType] OFF

SET NOCOUNT OFF


GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[Admission] WITH CHECK CHECK CONSTRAINT [FK_AdmissionAdmissionStatus];

ALTER TABLE [dbo].[Admission] WITH CHECK CHECK CONSTRAINT [FK_AdmissionAdmissionType];

ALTER TABLE [dbo].[Admission] WITH CHECK CHECK CONSTRAINT [FK_AdmissionAdmittingDoctorDoctor];

ALTER TABLE [dbo].[Admission] WITH CHECK CHECK CONSTRAINT [FK_AdmissionAttendingDoctorDoctor];

ALTER TABLE [dbo].[Admission] WITH CHECK CHECK CONSTRAINT [FK_AdmissionBed];

ALTER TABLE [dbo].[Admission] WITH CHECK CHECK CONSTRAINT [FK_AdmissionLocation];

ALTER TABLE [dbo].[Admission] WITH CHECK CHECK CONSTRAINT [FK_AdmissionPrimaryCareDoctor];

ALTER TABLE [dbo].[Admission] WITH CHECK CHECK CONSTRAINT [FK_ExternalSourceAdmission];

ALTER TABLE [dbo].[Admission] WITH CHECK CHECK CONSTRAINT [FK_PatientAdmission];

ALTER TABLE [dbo].[Bed] WITH CHECK CHECK CONSTRAINT [FK_BedRoom];

ALTER TABLE [dbo].[BedCleaningEvent] WITH CHECK CHECK CONSTRAINT [FK_BedCleaningEventBed];

ALTER TABLE [dbo].[BedCleaningEvent] WITH CHECK CHECK CONSTRAINT [FK_BedCleaningEventBedCleaningEventType];

ALTER TABLE [dbo].[Configuration] WITH CHECK CHECK CONSTRAINT [FK_ConfigurationConfigurationType];

ALTER TABLE [dbo].[ConfigurationLocation] WITH CHECK CHECK CONSTRAINT [FK_ConfigurationConfigurationLocation];

ALTER TABLE [dbo].[ConfigurationLocation] WITH CHECK CHECK CONSTRAINT [FK_LocationConfigurationLocation];

ALTER TABLE [dbo].[Connection] WITH CHECK CHECK CONSTRAINT [FK_DeviceConnection];

ALTER TABLE [dbo].[Device] WITH CHECK CHECK CONSTRAINT [FK_LocationWCS_Device];

ALTER TABLE [dbo].[DeviceConfiguration] WITH CHECK CHECK CONSTRAINT [FK_ConfigurationDeviceConfiguration];

ALTER TABLE [dbo].[DeviceConfiguration] WITH CHECK CHECK CONSTRAINT [FK_DeviceDeviceConfiguration];

ALTER TABLE [dbo].[Doctor] WITH CHECK CHECK CONSTRAINT [FK_ExternalSourceDoctor];

ALTER TABLE [dbo].[Location] WITH CHECK CHECK CONSTRAINT [FK_WaitingAreaLocation];

ALTER TABLE [dbo].[Note] WITH CHECK CHECK CONSTRAINT [FK_BedNote];

ALTER TABLE [dbo].[Note] WITH CHECK CHECK CONSTRAINT [FK_OrderNote];

ALTER TABLE [dbo].[Notification] WITH CHECK CHECK CONSTRAINT [FK_NotificationNotificationType];

ALTER TABLE [dbo].[Notification] WITH CHECK CHECK CONSTRAINT [FK_OrderNotification];

ALTER TABLE [dbo].[NotificationRule] WITH CHECK CHECK CONSTRAINT [FK_NotificationRuleProcedure];

ALTER TABLE [dbo].[Order] WITH CHECK CHECK CONSTRAINT [FK_AdmissionOrder];

ALTER TABLE [dbo].[Order] WITH CHECK CHECK CONSTRAINT [FK_ExternalSourceOrder];

ALTER TABLE [dbo].[Order] WITH CHECK CHECK CONSTRAINT [FK_OrderLocation];

ALTER TABLE [dbo].[Order] WITH CHECK CHECK CONSTRAINT [FK_OrderOrderingDoctor];

ALTER TABLE [dbo].[Order] WITH CHECK CHECK CONSTRAINT [FK_OrderProcedure];

ALTER TABLE [dbo].[Order] WITH CHECK CHECK CONSTRAINT [FK_OrderStatusOrder];

ALTER TABLE [dbo].[Patient] WITH CHECK CHECK CONSTRAINT [FK_PatientExternalSource];

ALTER TABLE [dbo].[Pin] WITH CHECK CHECK CONSTRAINT [FK_DevicePin];

ALTER TABLE [dbo].[Procedure] WITH CHECK CHECK CONSTRAINT [FK_ExternalSourceProcedure];

ALTER TABLE [dbo].[Procedure] WITH CHECK CHECK CONSTRAINT [FK_ProcedureCategoryProcedure];

ALTER TABLE [dbo].[ProcedureCategory] WITH CHECK CHECK CONSTRAINT [FK_ExternalSourceProcedureCategory];

ALTER TABLE [dbo].[RfidDetection] WITH CHECK CHECK CONSTRAINT [FK_RfidDetectionPatient];

ALTER TABLE [dbo].[RfidDetection] WITH CHECK CHECK CONSTRAINT [FK_RfidDetectionRfidDetector];

ALTER TABLE [dbo].[RfidDetection] WITH CHECK CHECK CONSTRAINT [FK_RfidDetectionRfidDirection];

ALTER TABLE [dbo].[RfidDetector] WITH CHECK CHECK CONSTRAINT [FK_RfidDetectorExternalSource];

ALTER TABLE [dbo].[RfidDetector] WITH CHECK CHECK CONSTRAINT [FK_RfidDetectorLocation];

ALTER TABLE [dbo].[RfidDetector] WITH CHECK CHECK CONSTRAINT [FK_WaitingAreaRfidDetector];

ALTER TABLE [dbo].[Room] WITH CHECK CHECK CONSTRAINT [FK_LocationRoom];

ALTER TABLE [dbo].[Update] WITH CHECK CHECK CONSTRAINT [FK_UpdateAdmission];

ALTER TABLE [dbo].[Update] WITH CHECK CHECK CONSTRAINT [FK_UpdateBed];

ALTER TABLE [dbo].[Update] WITH CHECK CHECK CONSTRAINT [FK_UpdateOrder];


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        DECLARE @VarDecimalSupported AS BIT;
        SELECT @VarDecimalSupported = 0;
        IF ((ServerProperty(N'EngineEdition') = 3)
            AND (((@@microsoftversion / power(2, 24) = 9)
                  AND (@@microsoftversion & 0xffff >= 3024))
                 OR ((@@microsoftversion / power(2, 24) = 10)
                     AND (@@microsoftversion & 0xffff >= 1600))))
            SELECT @VarDecimalSupported = 1;
        IF (@VarDecimalSupported > 0)
            BEGIN
                EXECUTE sp_db_vardecimal_storage_format N'$(DatabaseName)', 'ON';
            END
    END


GO
