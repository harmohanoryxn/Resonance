
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 12/20/2012 13:42:20
-- Generated from EDMX file: C:\Users\Cormac\Documents\TFS\WCS\Content\Code\Server\Cloudmaster.WCS.Services\WCSModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [WCS];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_AdmissionAdmissionStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Admission] DROP CONSTRAINT [FK_AdmissionAdmissionStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_AdmissionAdmissionType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Admission] DROP CONSTRAINT [FK_AdmissionAdmissionType];
GO
IF OBJECT_ID(N'[dbo].[FK_AdmissionAdmittingDoctorDoctor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Admission] DROP CONSTRAINT [FK_AdmissionAdmittingDoctorDoctor];
GO
IF OBJECT_ID(N'[dbo].[FK_AdmissionAttendingDoctorDoctor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Admission] DROP CONSTRAINT [FK_AdmissionAttendingDoctorDoctor];
GO
IF OBJECT_ID(N'[dbo].[FK_AdmissionBed]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Admission] DROP CONSTRAINT [FK_AdmissionBed];
GO
IF OBJECT_ID(N'[dbo].[FK_AdmissionLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Admission] DROP CONSTRAINT [FK_AdmissionLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_AdmissionOrder]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK_AdmissionOrder];
GO
IF OBJECT_ID(N'[dbo].[FK_AdmissionPrimaryCareDoctor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Admission] DROP CONSTRAINT [FK_AdmissionPrimaryCareDoctor];
GO
IF OBJECT_ID(N'[dbo].[FK_BedCleaningEventBed]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BedCleaningEvent] DROP CONSTRAINT [FK_BedCleaningEventBed];
GO
IF OBJECT_ID(N'[dbo].[FK_BedCleaningEventBedCleaningEventType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BedCleaningEvent] DROP CONSTRAINT [FK_BedCleaningEventBedCleaningEventType];
GO
IF OBJECT_ID(N'[dbo].[FK_BedNote]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Note] DROP CONSTRAINT [FK_BedNote];
GO
IF OBJECT_ID(N'[dbo].[FK_BedRoom]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Bed] DROP CONSTRAINT [FK_BedRoom];
GO
IF OBJECT_ID(N'[dbo].[FK_ConfigurationConfigurationLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ConfigurationLocation] DROP CONSTRAINT [FK_ConfigurationConfigurationLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_ConfigurationConfigurationType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Configuration] DROP CONSTRAINT [FK_ConfigurationConfigurationType];
GO
IF OBJECT_ID(N'[dbo].[FK_ConfigurationDeviceConfiguration]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DeviceConfiguration] DROP CONSTRAINT [FK_ConfigurationDeviceConfiguration];
GO
IF OBJECT_ID(N'[dbo].[FK_DeviceConnection]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Connection] DROP CONSTRAINT [FK_DeviceConnection];
GO
IF OBJECT_ID(N'[dbo].[FK_DeviceDeviceConfiguration]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DeviceConfiguration] DROP CONSTRAINT [FK_DeviceDeviceConfiguration];
GO
IF OBJECT_ID(N'[dbo].[FK_DevicePin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Pin] DROP CONSTRAINT [FK_DevicePin];
GO
IF OBJECT_ID(N'[dbo].[FK_ExternalSourceAdmission]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Admission] DROP CONSTRAINT [FK_ExternalSourceAdmission];
GO
IF OBJECT_ID(N'[dbo].[FK_ExternalSourceDoctor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Doctor] DROP CONSTRAINT [FK_ExternalSourceDoctor];
GO
IF OBJECT_ID(N'[dbo].[FK_ExternalSourceOrder]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK_ExternalSourceOrder];
GO
IF OBJECT_ID(N'[dbo].[FK_ExternalSourceProcedure]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Procedure] DROP CONSTRAINT [FK_ExternalSourceProcedure];
GO
IF OBJECT_ID(N'[dbo].[FK_ExternalSourceProcedureCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProcedureCategory] DROP CONSTRAINT [FK_ExternalSourceProcedureCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationConfigurationLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ConfigurationLocation] DROP CONSTRAINT [FK_LocationConfigurationLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationRoom]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Room] DROP CONSTRAINT [FK_LocationRoom];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationWCS_Device]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Device] DROP CONSTRAINT [FK_LocationWCS_Device];
GO
IF OBJECT_ID(N'[dbo].[FK_NotificationNotificationType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Notification] DROP CONSTRAINT [FK_NotificationNotificationType];
GO
IF OBJECT_ID(N'[dbo].[FK_NotificationRuleProcedure]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NotificationRule] DROP CONSTRAINT [FK_NotificationRuleProcedure];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK_OrderLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderNote]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Note] DROP CONSTRAINT [FK_OrderNote];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderNotification]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Notification] DROP CONSTRAINT [FK_OrderNotification];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderOrderingDoctor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK_OrderOrderingDoctor];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderProcedure]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK_OrderProcedure];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderStatusOrder]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK_OrderStatusOrder];
GO
IF OBJECT_ID(N'[dbo].[FK_PatientAdmission]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Admission] DROP CONSTRAINT [FK_PatientAdmission];
GO
IF OBJECT_ID(N'[dbo].[FK_PatientExternalSource]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Patient] DROP CONSTRAINT [FK_PatientExternalSource];
GO
IF OBJECT_ID(N'[dbo].[FK_ProcedureCategoryProcedure]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Procedure] DROP CONSTRAINT [FK_ProcedureCategoryProcedure];
GO
IF OBJECT_ID(N'[dbo].[FK_RfidDetectionPatient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RfidDetection] DROP CONSTRAINT [FK_RfidDetectionPatient];
GO
IF OBJECT_ID(N'[dbo].[FK_RfidDetectionRfidDetector]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RfidDetection] DROP CONSTRAINT [FK_RfidDetectionRfidDetector];
GO
IF OBJECT_ID(N'[dbo].[FK_RfidDetectionRfidDirection]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RfidDetection] DROP CONSTRAINT [FK_RfidDetectionRfidDirection];
GO
IF OBJECT_ID(N'[dbo].[FK_RfidDetectorExternalSource]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RfidDetector] DROP CONSTRAINT [FK_RfidDetectorExternalSource];
GO
IF OBJECT_ID(N'[dbo].[FK_RfidDetectorLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RfidDetector] DROP CONSTRAINT [FK_RfidDetectorLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_UpdateAdmission]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Update] DROP CONSTRAINT [FK_UpdateAdmission];
GO
IF OBJECT_ID(N'[dbo].[FK_UpdateBed]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Update] DROP CONSTRAINT [FK_UpdateBed];
GO
IF OBJECT_ID(N'[dbo].[FK_UpdateOrder]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Update] DROP CONSTRAINT [FK_UpdateOrder];
GO
IF OBJECT_ID(N'[dbo].[FK_WaitingAreaLocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Location] DROP CONSTRAINT [FK_WaitingAreaLocation];
GO
IF OBJECT_ID(N'[dbo].[FK_WaitingAreaRfidDetector]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RfidDetector] DROP CONSTRAINT [FK_WaitingAreaRfidDetector];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Admission]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Admission];
GO
IF OBJECT_ID(N'[dbo].[AdmissionStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdmissionStatus];
GO
IF OBJECT_ID(N'[dbo].[AdmissionType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdmissionType];
GO
IF OBJECT_ID(N'[dbo].[Bed]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Bed];
GO
IF OBJECT_ID(N'[dbo].[BedCleaningEvent]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BedCleaningEvent];
GO
IF OBJECT_ID(N'[dbo].[BedCleaningEventType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BedCleaningEventType];
GO
IF OBJECT_ID(N'[dbo].[Configuration]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configuration];
GO
IF OBJECT_ID(N'[dbo].[ConfigurationLocation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ConfigurationLocation];
GO
IF OBJECT_ID(N'[dbo].[ConfigurationType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ConfigurationType];
GO
IF OBJECT_ID(N'[dbo].[Connection]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Connection];
GO
IF OBJECT_ID(N'[dbo].[Device]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Device];
GO
IF OBJECT_ID(N'[dbo].[DeviceConfiguration]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeviceConfiguration];
GO
IF OBJECT_ID(N'[dbo].[Doctor]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Doctor];
GO
IF OBJECT_ID(N'[dbo].[ExternalSource]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExternalSource];
GO
IF OBJECT_ID(N'[dbo].[Location]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Location];
GO
IF OBJECT_ID(N'[dbo].[Note]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Note];
GO
IF OBJECT_ID(N'[dbo].[Notification]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Notification];
GO
IF OBJECT_ID(N'[dbo].[NotificationRule]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NotificationRule];
GO
IF OBJECT_ID(N'[dbo].[NotificationType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NotificationType];
GO
IF OBJECT_ID(N'[dbo].[Order]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Order];
GO
IF OBJECT_ID(N'[dbo].[OrderStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderStatus];
GO
IF OBJECT_ID(N'[dbo].[Patient]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Patient];
GO
IF OBJECT_ID(N'[dbo].[Pin]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Pin];
GO
IF OBJECT_ID(N'[dbo].[Procedure]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Procedure];
GO
IF OBJECT_ID(N'[dbo].[ProcedureCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProcedureCategory];
GO
IF OBJECT_ID(N'[dbo].[RfidDetection]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RfidDetection];
GO
IF OBJECT_ID(N'[dbo].[RfidDetector]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RfidDetector];
GO
IF OBJECT_ID(N'[dbo].[RfidDirection]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RfidDirection];
GO
IF OBJECT_ID(N'[dbo].[Room]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Room];
GO
IF OBJECT_ID(N'[dbo].[Update]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Update];
GO
IF OBJECT_ID(N'[dbo].[WaitingArea]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WaitingArea];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Admission'
CREATE TABLE [dbo].[Admission] (
    [admissionId] int IDENTITY(1,1) NOT NULL,
    [externalSourceId] int  NOT NULL,
    [externalId] nvarchar(50)  NOT NULL,
    [admitDateTime] datetime  NULL,
    [dischargeDateTime] datetime  NULL,
    [patientId] int  NOT NULL,
    [AdmissionType_admissionTypeId] int  NOT NULL,
    [AdmissionStatus_admissionStatusId] int  NOT NULL,
    [Location_locationId] int  NOT NULL,
    [Bed_bedId] int  NULL,
    [estimatedDischargeDateTime] datetime  NULL,
    [AdmittingDoctor_doctorId] int  NULL,
    [AttendingDoctor_doctorId] int  NULL,
    [PrimaryCareDoctor_doctorId] int  NULL
);
GO

-- Creating table 'AdmissionStatus'
CREATE TABLE [dbo].[AdmissionStatus] (
    [admissionStatusId] int IDENTITY(1,1) NOT NULL,
    [status] nvarchar(20)  NOT NULL
);
GO

-- Creating table 'AdmissionType'
CREATE TABLE [dbo].[AdmissionType] (
    [admissionTypeId] int IDENTITY(1,1) NOT NULL,
    [type] nvarchar(20)  NOT NULL
);
GO

-- Creating table 'Bed'
CREATE TABLE [dbo].[Bed] (
    [bedId] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(50)  NOT NULL,
    [Room_roomId] int  NULL
);
GO

-- Creating table 'BedCleaningEvent'
CREATE TABLE [dbo].[BedCleaningEvent] (
    [bedCleaningEventId] int IDENTITY(1,1) NOT NULL,
    [timestamp] datetime  NOT NULL,
    [Bed_bedId] int  NOT NULL,
    [bedCleaningEventTypeId] int  NOT NULL
);
GO

-- Creating table 'BedCleaningEventType'
CREATE TABLE [dbo].[BedCleaningEventType] (
    [bedCleaningEventTypeId] int IDENTITY(1,1) NOT NULL,
    [eventType] nvarchar(20)  NOT NULL
);
GO

-- Creating table 'Configuration'
CREATE TABLE [dbo].[Configuration] (
    [configurationId] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(50)  NOT NULL,
    [ConfigurationType_ConfigurationTypeId] int  NOT NULL
);
GO

-- Creating table 'ConfigurationLocation'
CREATE TABLE [dbo].[ConfigurationLocation] (
    [configurationLocationId] int IDENTITY(1,1) NOT NULL,
    [configurationId] int  NOT NULL,
    [locationId] int  NOT NULL,
    [isDefault] bit  NOT NULL
);
GO

-- Creating table 'ConfigurationType'
CREATE TABLE [dbo].[ConfigurationType] (
    [ConfigurationTypeId] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Connection'
CREATE TABLE [dbo].[Connection] (
    [connectionId] int IDENTITY(1,1) NOT NULL,
    [connectionTime] datetime  NOT NULL,
    [deviceId] int  NOT NULL
);
GO

-- Creating table 'Device'
CREATE TABLE [dbo].[Device] (
    [deviceId] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(50)  NOT NULL,
    [os] nvarchar(50)  NULL,
    [clientVersion] nvarchar(50)  NULL,
    [description] nvarchar(max)  NULL,
    [ipAddress] nvarchar(50)  NULL,
    [lastConnection] datetime  NULL,
    [locationId] int  NULL,
    [lockTimeout] int  NOT NULL,
    [configurationTimeout] int  NOT NULL
);
GO

-- Creating table 'DeviceConfiguration'
CREATE TABLE [dbo].[DeviceConfiguration] (
    [deviceId] int  NOT NULL,
    [shortcutKeyNo] int  NOT NULL,
    [configurationId] int  NOT NULL,
    [orderTimeout] int  NOT NULL,
    [presenceTimeout] int  NOT NULL,
    [rfidTimeout] int  NOT NULL,
    [cleaningBedDataTimeout] int  NOT NULL,
    [dischargeTimeout] int  NOT NULL,
    [admissionsTimeout] int  NOT NULL
);
GO

-- Creating table 'Doctor'
CREATE TABLE [dbo].[Doctor] (
    [doctorId] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(50)  NOT NULL,
    [externalSourceId] int  NOT NULL,
    [externalId] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'ExternalSource'
CREATE TABLE [dbo].[ExternalSource] (
    [externalSourceId] int IDENTITY(1,1) NOT NULL,
    [source] nvarchar(20)  NOT NULL
);
GO

-- Creating table 'Location'
CREATE TABLE [dbo].[Location] (
    [locationId] int IDENTITY(1,1) NOT NULL,
    [code] nvarchar(20)  NOT NULL,
    [name] nvarchar(50)  NOT NULL,
    [isEmergency] bit  NOT NULL,
    [contactInfo] nvarchar(20)  NULL,
    [WaitingArea_waitingAreaId] int  NULL,
    [includeInMerge] bit  NOT NULL
);
GO

-- Creating table 'Note'
CREATE TABLE [dbo].[Note] (
    [noteId] int IDENTITY(1,1) NOT NULL,
    [source] nvarchar(50)  NULL,
    [notes] nvarchar(max)  NULL,
    [dateCreated] datetime  NULL,
    [noteOrder] int  NOT NULL,
    [bedId] int  NULL,
    [orderId] int  NULL
);
GO

-- Creating table 'Notification'
CREATE TABLE [dbo].[Notification] (
    [notificationId] int IDENTITY(1,1) NOT NULL,
    [notificationTypeId] int  NOT NULL,
    [description] nvarchar(200)  NOT NULL,
    [priorToProcedureTime] int  NOT NULL,
    [isAcknowledgmentRequired] bit  NOT NULL,
    [acknowledged] bit  NOT NULL,
    [acknowledgedTime] datetime  NULL,
    [acknowledgedBy] nvarchar(20)  NULL,
    [notificationOrder] int  NULL,
    [orderId] int  NOT NULL,
    [durationMinutes] int  NOT NULL,
    [radiationRiskDurationMinutes] int  NOT NULL
);
GO

-- Creating table 'NotificationRule'
CREATE TABLE [dbo].[NotificationRule] (
    [notificationRuleId] int IDENTITY(1,1) NOT NULL,
    [description] nvarchar(200)  NOT NULL,
    [priorToProcedureTime] int  NOT NULL,
    [durationMinutes] int  NOT NULL,
    [isAcknowledgmentRequired] bit  NOT NULL,
    [Procedure_procedureId] int  NOT NULL,
    [radiationRiskDurationMinutes] int  NOT NULL
);
GO

-- Creating table 'NotificationType'
CREATE TABLE [dbo].[NotificationType] (
    [notificationTypeId] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Order'
CREATE TABLE [dbo].[Order] (
    [orderId] int IDENTITY(1,1) NOT NULL,
    [externalSourceId] int  NOT NULL,
    [externalId] nvarchar(50)  NOT NULL,
    [orderNumber] nvarchar(20)  NULL,
    [procedureTime] datetime  NULL,
    [orderStatusId] int  NOT NULL,
    [completedTime] datetime  NULL,
    [admissionId] int  NOT NULL,
    [clinicalIndicator] nvarchar(200)  NULL,
    [estimatedProcedureDuration] int  NULL,
    [Procedure_procedureId] int  NOT NULL,
    [Department_locationId] int  NOT NULL,
    [OrderingDoctor_doctorId] int  NULL,
    [isHidden] bit  NOT NULL,
    [acknowledged] bit  NOT NULL,
    [acknowledgedTime] datetime  NULL,
    [acknowledgedBy] nvarchar(20)  NULL
);
GO

-- Creating table 'OrderStatus'
CREATE TABLE [dbo].[OrderStatus] (
    [orderStatusId] int IDENTITY(1,1) NOT NULL,
    [status] nvarchar(20)  NOT NULL
);
GO

-- Creating table 'Patient'
CREATE TABLE [dbo].[Patient] (
    [patientId] int IDENTITY(1,1) NOT NULL,
    [externalSourceId] int  NOT NULL,
    [externalId] nvarchar(50)  NOT NULL,
    [givenName] nvarchar(50)  NULL,
    [surname] nvarchar(50)  NULL,
    [dob] datetime  NULL,
    [sex] nvarchar(20)  NULL,
    [isMrsaPositive] bit  NOT NULL,
    [isFallRisk] bit  NOT NULL,
    [isAssistanceRequired] bit  NOT NULL,
    [assistanceDescription] nvarchar(50)  NOT NULL,
    [hasLatexAllergy] bit  NOT NULL
);
GO

-- Creating table 'Pin'
CREATE TABLE [dbo].[Pin] (
    [pinId] int IDENTITY(1,1) NOT NULL,
    [Device_deviceId] int  NOT NULL,
    [pin] nvarchar(20)  NOT NULL
);
GO

-- Creating table 'Procedure'
CREATE TABLE [dbo].[Procedure] (
    [procedureId] int IDENTITY(1,1) NOT NULL,
    [externalSourceId] int  NOT NULL,
    [externalId] nvarchar(50)  NOT NULL,
    [code] nvarchar(20)  NOT NULL,
    [description] nvarchar(200)  NOT NULL,
    [durationMinutes] int  NULL,
    [ProcedureCategory_procedureCategoryId] int  NOT NULL
);
GO

-- Creating table 'ProcedureCategory'
CREATE TABLE [dbo].[ProcedureCategory] (
    [procedureCategoryId] int IDENTITY(1,1) NOT NULL,
    [externalSourceId] int  NOT NULL,
    [externalId] nvarchar(50)  NOT NULL,
    [includeInMerge] bit  NOT NULL,
    [description] nvarchar(200)  NOT NULL
);
GO

-- Creating table 'RfidDetection'
CREATE TABLE [dbo].[RfidDetection] (
    [rfidDetectionId] int IDENTITY(1,1) NOT NULL,
    [patientId] int  NOT NULL,
    [rfidDirectionId] int  NOT NULL,
    [rfidDetectorId] int  NOT NULL,
    [dateTimeDetected] datetime  NOT NULL
);
GO

-- Creating table 'RfidDetector'
CREATE TABLE [dbo].[RfidDetector] (
    [rfidDetectorId] int IDENTITY(1,1) NOT NULL,
    [externalSourceId] int  NOT NULL,
    [externalId] nvarchar(20)  NOT NULL,
    [Location_locationId] int  NULL,
    [WaitingArea_waitingAreaId] int  NULL
);
GO

-- Creating table 'RfidDirection'
CREATE TABLE [dbo].[RfidDirection] (
    [rfidDirectionId] int IDENTITY(1,1) NOT NULL,
    [direction] nvarchar(20)  NOT NULL
);
GO

-- Creating table 'Room'
CREATE TABLE [dbo].[Room] (
    [roomId] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(50)  NOT NULL,
    [Location_locationId] int  NOT NULL
);
GO

-- Creating table 'Update'
CREATE TABLE [dbo].[Update] (
    [updateId] int IDENTITY(1,1) NOT NULL,
    [type] nvarchar(50)  NULL,
    [source] nvarchar(50)  NULL,
    [value] nvarchar(200)  NULL,
    [dateCreated] datetime  NULL,
    [Order_orderId] int  NULL,
    [Bed_bedId] int  NULL,
    [Admission_admissionId] int  NULL
);
GO

-- Creating table 'WaitingArea'
CREATE TABLE [dbo].[WaitingArea] (
    [waitingAreaId] int IDENTITY(1,1) NOT NULL,
    [code] nvarchar(20)  NOT NULL,
    [name] nvarchar(50)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [admissionId] in table 'Admission'
ALTER TABLE [dbo].[Admission]
ADD CONSTRAINT [PK_Admission]
    PRIMARY KEY CLUSTERED ([admissionId] ASC);
GO

-- Creating primary key on [admissionStatusId] in table 'AdmissionStatus'
ALTER TABLE [dbo].[AdmissionStatus]
ADD CONSTRAINT [PK_AdmissionStatus]
    PRIMARY KEY CLUSTERED ([admissionStatusId] ASC);
GO

-- Creating primary key on [admissionTypeId] in table 'AdmissionType'
ALTER TABLE [dbo].[AdmissionType]
ADD CONSTRAINT [PK_AdmissionType]
    PRIMARY KEY CLUSTERED ([admissionTypeId] ASC);
GO

-- Creating primary key on [bedId] in table 'Bed'
ALTER TABLE [dbo].[Bed]
ADD CONSTRAINT [PK_Bed]
    PRIMARY KEY CLUSTERED ([bedId] ASC);
GO

-- Creating primary key on [bedCleaningEventId] in table 'BedCleaningEvent'
ALTER TABLE [dbo].[BedCleaningEvent]
ADD CONSTRAINT [PK_BedCleaningEvent]
    PRIMARY KEY CLUSTERED ([bedCleaningEventId] ASC);
GO

-- Creating primary key on [bedCleaningEventTypeId] in table 'BedCleaningEventType'
ALTER TABLE [dbo].[BedCleaningEventType]
ADD CONSTRAINT [PK_BedCleaningEventType]
    PRIMARY KEY CLUSTERED ([bedCleaningEventTypeId] ASC);
GO

-- Creating primary key on [configurationId] in table 'Configuration'
ALTER TABLE [dbo].[Configuration]
ADD CONSTRAINT [PK_Configuration]
    PRIMARY KEY CLUSTERED ([configurationId] ASC);
GO

-- Creating primary key on [configurationLocationId] in table 'ConfigurationLocation'
ALTER TABLE [dbo].[ConfigurationLocation]
ADD CONSTRAINT [PK_ConfigurationLocation]
    PRIMARY KEY CLUSTERED ([configurationLocationId] ASC);
GO

-- Creating primary key on [ConfigurationTypeId] in table 'ConfigurationType'
ALTER TABLE [dbo].[ConfigurationType]
ADD CONSTRAINT [PK_ConfigurationType]
    PRIMARY KEY CLUSTERED ([ConfigurationTypeId] ASC);
GO

-- Creating primary key on [connectionId] in table 'Connection'
ALTER TABLE [dbo].[Connection]
ADD CONSTRAINT [PK_Connection]
    PRIMARY KEY CLUSTERED ([connectionId] ASC);
GO

-- Creating primary key on [deviceId] in table 'Device'
ALTER TABLE [dbo].[Device]
ADD CONSTRAINT [PK_Device]
    PRIMARY KEY CLUSTERED ([deviceId] ASC);
GO

-- Creating primary key on [deviceId], [shortcutKeyNo] in table 'DeviceConfiguration'
ALTER TABLE [dbo].[DeviceConfiguration]
ADD CONSTRAINT [PK_DeviceConfiguration]
    PRIMARY KEY CLUSTERED ([deviceId], [shortcutKeyNo] ASC);
GO

-- Creating primary key on [doctorId] in table 'Doctor'
ALTER TABLE [dbo].[Doctor]
ADD CONSTRAINT [PK_Doctor]
    PRIMARY KEY CLUSTERED ([doctorId] ASC);
GO

-- Creating primary key on [externalSourceId] in table 'ExternalSource'
ALTER TABLE [dbo].[ExternalSource]
ADD CONSTRAINT [PK_ExternalSource]
    PRIMARY KEY CLUSTERED ([externalSourceId] ASC);
GO

-- Creating primary key on [locationId] in table 'Location'
ALTER TABLE [dbo].[Location]
ADD CONSTRAINT [PK_Location]
    PRIMARY KEY CLUSTERED ([locationId] ASC);
GO

-- Creating primary key on [noteId] in table 'Note'
ALTER TABLE [dbo].[Note]
ADD CONSTRAINT [PK_Note]
    PRIMARY KEY CLUSTERED ([noteId] ASC);
GO

-- Creating primary key on [notificationId] in table 'Notification'
ALTER TABLE [dbo].[Notification]
ADD CONSTRAINT [PK_Notification]
    PRIMARY KEY CLUSTERED ([notificationId] ASC);
GO

-- Creating primary key on [notificationRuleId] in table 'NotificationRule'
ALTER TABLE [dbo].[NotificationRule]
ADD CONSTRAINT [PK_NotificationRule]
    PRIMARY KEY CLUSTERED ([notificationRuleId] ASC);
GO

-- Creating primary key on [notificationTypeId] in table 'NotificationType'
ALTER TABLE [dbo].[NotificationType]
ADD CONSTRAINT [PK_NotificationType]
    PRIMARY KEY CLUSTERED ([notificationTypeId] ASC);
GO

-- Creating primary key on [orderId] in table 'Order'
ALTER TABLE [dbo].[Order]
ADD CONSTRAINT [PK_Order]
    PRIMARY KEY CLUSTERED ([orderId] ASC);
GO

-- Creating primary key on [orderStatusId] in table 'OrderStatus'
ALTER TABLE [dbo].[OrderStatus]
ADD CONSTRAINT [PK_OrderStatus]
    PRIMARY KEY CLUSTERED ([orderStatusId] ASC);
GO

-- Creating primary key on [patientId] in table 'Patient'
ALTER TABLE [dbo].[Patient]
ADD CONSTRAINT [PK_Patient]
    PRIMARY KEY CLUSTERED ([patientId] ASC);
GO

-- Creating primary key on [pinId] in table 'Pin'
ALTER TABLE [dbo].[Pin]
ADD CONSTRAINT [PK_Pin]
    PRIMARY KEY CLUSTERED ([pinId] ASC);
GO

-- Creating primary key on [procedureId] in table 'Procedure'
ALTER TABLE [dbo].[Procedure]
ADD CONSTRAINT [PK_Procedure]
    PRIMARY KEY CLUSTERED ([procedureId] ASC);
GO

-- Creating primary key on [procedureCategoryId] in table 'ProcedureCategory'
ALTER TABLE [dbo].[ProcedureCategory]
ADD CONSTRAINT [PK_ProcedureCategory]
    PRIMARY KEY CLUSTERED ([procedureCategoryId] ASC);
GO

-- Creating primary key on [rfidDetectionId] in table 'RfidDetection'
ALTER TABLE [dbo].[RfidDetection]
ADD CONSTRAINT [PK_RfidDetection]
    PRIMARY KEY CLUSTERED ([rfidDetectionId] ASC);
GO

-- Creating primary key on [rfidDetectorId] in table 'RfidDetector'
ALTER TABLE [dbo].[RfidDetector]
ADD CONSTRAINT [PK_RfidDetector]
    PRIMARY KEY CLUSTERED ([rfidDetectorId] ASC);
GO

-- Creating primary key on [rfidDirectionId] in table 'RfidDirection'
ALTER TABLE [dbo].[RfidDirection]
ADD CONSTRAINT [PK_RfidDirection]
    PRIMARY KEY CLUSTERED ([rfidDirectionId] ASC);
GO

-- Creating primary key on [roomId] in table 'Room'
ALTER TABLE [dbo].[Room]
ADD CONSTRAINT [PK_Room]
    PRIMARY KEY CLUSTERED ([roomId] ASC);
GO

-- Creating primary key on [updateId] in table 'Update'
ALTER TABLE [dbo].[Update]
ADD CONSTRAINT [PK_Update]
    PRIMARY KEY CLUSTERED ([updateId] ASC);
GO

-- Creating primary key on [waitingAreaId] in table 'WaitingArea'
ALTER TABLE [dbo].[WaitingArea]
ADD CONSTRAINT [PK_WaitingArea]
    PRIMARY KEY CLUSTERED ([waitingAreaId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [AdmissionStatus_admissionStatusId] in table 'Admission'
ALTER TABLE [dbo].[Admission]
ADD CONSTRAINT [FK_AdmissionAdmissionStatus]
    FOREIGN KEY ([AdmissionStatus_admissionStatusId])
    REFERENCES [dbo].[AdmissionStatus]
        ([admissionStatusId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AdmissionAdmissionStatus'
CREATE INDEX [IX_FK_AdmissionAdmissionStatus]
ON [dbo].[Admission]
    ([AdmissionStatus_admissionStatusId]);
GO

-- Creating foreign key on [AdmissionType_admissionTypeId] in table 'Admission'
ALTER TABLE [dbo].[Admission]
ADD CONSTRAINT [FK_AdmissionAdmissionType]
    FOREIGN KEY ([AdmissionType_admissionTypeId])
    REFERENCES [dbo].[AdmissionType]
        ([admissionTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AdmissionAdmissionType'
CREATE INDEX [IX_FK_AdmissionAdmissionType]
ON [dbo].[Admission]
    ([AdmissionType_admissionTypeId]);
GO

-- Creating foreign key on [Location_locationId] in table 'Admission'
ALTER TABLE [dbo].[Admission]
ADD CONSTRAINT [FK_AdmissionLocation]
    FOREIGN KEY ([Location_locationId])
    REFERENCES [dbo].[Location]
        ([locationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AdmissionLocation'
CREATE INDEX [IX_FK_AdmissionLocation]
ON [dbo].[Admission]
    ([Location_locationId]);
GO

-- Creating foreign key on [admissionId] in table 'Order'
ALTER TABLE [dbo].[Order]
ADD CONSTRAINT [FK_AdmissionOrder]
    FOREIGN KEY ([admissionId])
    REFERENCES [dbo].[Admission]
        ([admissionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AdmissionOrder'
CREATE INDEX [IX_FK_AdmissionOrder]
ON [dbo].[Order]
    ([admissionId]);
GO

-- Creating foreign key on [externalSourceId] in table 'Admission'
ALTER TABLE [dbo].[Admission]
ADD CONSTRAINT [FK_ExternalSourceAdmission]
    FOREIGN KEY ([externalSourceId])
    REFERENCES [dbo].[ExternalSource]
        ([externalSourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ExternalSourceAdmission'
CREATE INDEX [IX_FK_ExternalSourceAdmission]
ON [dbo].[Admission]
    ([externalSourceId]);
GO

-- Creating foreign key on [patientId] in table 'Admission'
ALTER TABLE [dbo].[Admission]
ADD CONSTRAINT [FK_PatientAdmission]
    FOREIGN KEY ([patientId])
    REFERENCES [dbo].[Patient]
        ([patientId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PatientAdmission'
CREATE INDEX [IX_FK_PatientAdmission]
ON [dbo].[Admission]
    ([patientId]);
GO

-- Creating foreign key on [Bed_bedId] in table 'BedCleaningEvent'
ALTER TABLE [dbo].[BedCleaningEvent]
ADD CONSTRAINT [FK_BedCleaningEventBed]
    FOREIGN KEY ([Bed_bedId])
    REFERENCES [dbo].[Bed]
        ([bedId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BedCleaningEventBed'
CREATE INDEX [IX_FK_BedCleaningEventBed]
ON [dbo].[BedCleaningEvent]
    ([Bed_bedId]);
GO

-- Creating foreign key on [bedId] in table 'Note'
ALTER TABLE [dbo].[Note]
ADD CONSTRAINT [FK_BedNote]
    FOREIGN KEY ([bedId])
    REFERENCES [dbo].[Bed]
        ([bedId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BedNote'
CREATE INDEX [IX_FK_BedNote]
ON [dbo].[Note]
    ([bedId]);
GO

-- Creating foreign key on [Room_roomId] in table 'Bed'
ALTER TABLE [dbo].[Bed]
ADD CONSTRAINT [FK_BedRoom]
    FOREIGN KEY ([Room_roomId])
    REFERENCES [dbo].[Room]
        ([roomId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BedRoom'
CREATE INDEX [IX_FK_BedRoom]
ON [dbo].[Bed]
    ([Room_roomId]);
GO

-- Creating foreign key on [Bed_bedId] in table 'Update'
ALTER TABLE [dbo].[Update]
ADD CONSTRAINT [FK_BedUpdate]
    FOREIGN KEY ([Bed_bedId])
    REFERENCES [dbo].[Bed]
        ([bedId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BedUpdate'
CREATE INDEX [IX_FK_BedUpdate]
ON [dbo].[Update]
    ([Bed_bedId]);
GO

-- Creating foreign key on [bedCleaningEventTypeId] in table 'BedCleaningEvent'
ALTER TABLE [dbo].[BedCleaningEvent]
ADD CONSTRAINT [FK_BedCleaningEventBedCleaningEventType]
    FOREIGN KEY ([bedCleaningEventTypeId])
    REFERENCES [dbo].[BedCleaningEventType]
        ([bedCleaningEventTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BedCleaningEventBedCleaningEventType'
CREATE INDEX [IX_FK_BedCleaningEventBedCleaningEventType]
ON [dbo].[BedCleaningEvent]
    ([bedCleaningEventTypeId]);
GO

-- Creating foreign key on [configurationId] in table 'ConfigurationLocation'
ALTER TABLE [dbo].[ConfigurationLocation]
ADD CONSTRAINT [FK_ConfigurationConfigurationLocation]
    FOREIGN KEY ([configurationId])
    REFERENCES [dbo].[Configuration]
        ([configurationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConfigurationConfigurationLocation'
CREATE INDEX [IX_FK_ConfigurationConfigurationLocation]
ON [dbo].[ConfigurationLocation]
    ([configurationId]);
GO

-- Creating foreign key on [ConfigurationType_ConfigurationTypeId] in table 'Configuration'
ALTER TABLE [dbo].[Configuration]
ADD CONSTRAINT [FK_ConfigurationConfigurationType]
    FOREIGN KEY ([ConfigurationType_ConfigurationTypeId])
    REFERENCES [dbo].[ConfigurationType]
        ([ConfigurationTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConfigurationConfigurationType'
CREATE INDEX [IX_FK_ConfigurationConfigurationType]
ON [dbo].[Configuration]
    ([ConfigurationType_ConfigurationTypeId]);
GO

-- Creating foreign key on [configurationId] in table 'DeviceConfiguration'
ALTER TABLE [dbo].[DeviceConfiguration]
ADD CONSTRAINT [FK_ConfigurationDeviceConfiguration]
    FOREIGN KEY ([configurationId])
    REFERENCES [dbo].[Configuration]
        ([configurationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ConfigurationDeviceConfiguration'
CREATE INDEX [IX_FK_ConfigurationDeviceConfiguration]
ON [dbo].[DeviceConfiguration]
    ([configurationId]);
GO

-- Creating foreign key on [locationId] in table 'ConfigurationLocation'
ALTER TABLE [dbo].[ConfigurationLocation]
ADD CONSTRAINT [FK_LocationConfigurationLocation]
    FOREIGN KEY ([locationId])
    REFERENCES [dbo].[Location]
        ([locationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LocationConfigurationLocation'
CREATE INDEX [IX_FK_LocationConfigurationLocation]
ON [dbo].[ConfigurationLocation]
    ([locationId]);
GO

-- Creating foreign key on [deviceId] in table 'Connection'
ALTER TABLE [dbo].[Connection]
ADD CONSTRAINT [FK_DeviceConnection]
    FOREIGN KEY ([deviceId])
    REFERENCES [dbo].[Device]
        ([deviceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DeviceConnection'
CREATE INDEX [IX_FK_DeviceConnection]
ON [dbo].[Connection]
    ([deviceId]);
GO

-- Creating foreign key on [deviceId] in table 'DeviceConfiguration'
ALTER TABLE [dbo].[DeviceConfiguration]
ADD CONSTRAINT [FK_DeviceDeviceConfiguration]
    FOREIGN KEY ([deviceId])
    REFERENCES [dbo].[Device]
        ([deviceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Device_deviceId] in table 'Pin'
ALTER TABLE [dbo].[Pin]
ADD CONSTRAINT [FK_DevicePin]
    FOREIGN KEY ([Device_deviceId])
    REFERENCES [dbo].[Device]
        ([deviceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DevicePin'
CREATE INDEX [IX_FK_DevicePin]
ON [dbo].[Pin]
    ([Device_deviceId]);
GO

-- Creating foreign key on [locationId] in table 'Device'
ALTER TABLE [dbo].[Device]
ADD CONSTRAINT [FK_LocationWCS_Device]
    FOREIGN KEY ([locationId])
    REFERENCES [dbo].[Location]
        ([locationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LocationWCS_Device'
CREATE INDEX [IX_FK_LocationWCS_Device]
ON [dbo].[Device]
    ([locationId]);
GO

-- Creating foreign key on [OrderingDoctor_doctorId] in table 'Order'
ALTER TABLE [dbo].[Order]
ADD CONSTRAINT [FK_OrderOrderingDoctor]
    FOREIGN KEY ([OrderingDoctor_doctorId])
    REFERENCES [dbo].[Doctor]
        ([doctorId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderOrderingDoctor'
CREATE INDEX [IX_FK_OrderOrderingDoctor]
ON [dbo].[Order]
    ([OrderingDoctor_doctorId]);
GO

-- Creating foreign key on [externalSourceId] in table 'Order'
ALTER TABLE [dbo].[Order]
ADD CONSTRAINT [FK_ExternalSourceOrder]
    FOREIGN KEY ([externalSourceId])
    REFERENCES [dbo].[ExternalSource]
        ([externalSourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ExternalSourceOrder'
CREATE INDEX [IX_FK_ExternalSourceOrder]
ON [dbo].[Order]
    ([externalSourceId]);
GO

-- Creating foreign key on [externalSourceId] in table 'Procedure'
ALTER TABLE [dbo].[Procedure]
ADD CONSTRAINT [FK_ExternalSourceProcedure]
    FOREIGN KEY ([externalSourceId])
    REFERENCES [dbo].[ExternalSource]
        ([externalSourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ExternalSourceProcedure'
CREATE INDEX [IX_FK_ExternalSourceProcedure]
ON [dbo].[Procedure]
    ([externalSourceId]);
GO

-- Creating foreign key on [externalSourceId] in table 'ProcedureCategory'
ALTER TABLE [dbo].[ProcedureCategory]
ADD CONSTRAINT [FK_ExternalSourceProcedureCategory]
    FOREIGN KEY ([externalSourceId])
    REFERENCES [dbo].[ExternalSource]
        ([externalSourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ExternalSourceProcedureCategory'
CREATE INDEX [IX_FK_ExternalSourceProcedureCategory]
ON [dbo].[ProcedureCategory]
    ([externalSourceId]);
GO

-- Creating foreign key on [externalSourceId] in table 'Patient'
ALTER TABLE [dbo].[Patient]
ADD CONSTRAINT [FK_PatientExternalSource]
    FOREIGN KEY ([externalSourceId])
    REFERENCES [dbo].[ExternalSource]
        ([externalSourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PatientExternalSource'
CREATE INDEX [IX_FK_PatientExternalSource]
ON [dbo].[Patient]
    ([externalSourceId]);
GO

-- Creating foreign key on [externalSourceId] in table 'RfidDetector'
ALTER TABLE [dbo].[RfidDetector]
ADD CONSTRAINT [FK_RfidDetectorExternalSource]
    FOREIGN KEY ([externalSourceId])
    REFERENCES [dbo].[ExternalSource]
        ([externalSourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RfidDetectorExternalSource'
CREATE INDEX [IX_FK_RfidDetectorExternalSource]
ON [dbo].[RfidDetector]
    ([externalSourceId]);
GO

-- Creating foreign key on [Location_locationId] in table 'Room'
ALTER TABLE [dbo].[Room]
ADD CONSTRAINT [FK_LocationRoom]
    FOREIGN KEY ([Location_locationId])
    REFERENCES [dbo].[Location]
        ([locationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LocationRoom'
CREATE INDEX [IX_FK_LocationRoom]
ON [dbo].[Room]
    ([Location_locationId]);
GO

-- Creating foreign key on [Department_locationId] in table 'Order'
ALTER TABLE [dbo].[Order]
ADD CONSTRAINT [FK_OrderLocation]
    FOREIGN KEY ([Department_locationId])
    REFERENCES [dbo].[Location]
        ([locationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderLocation'
CREATE INDEX [IX_FK_OrderLocation]
ON [dbo].[Order]
    ([Department_locationId]);
GO

-- Creating foreign key on [Location_locationId] in table 'RfidDetector'
ALTER TABLE [dbo].[RfidDetector]
ADD CONSTRAINT [FK_RfidDetectorLocation]
    FOREIGN KEY ([Location_locationId])
    REFERENCES [dbo].[Location]
        ([locationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RfidDetectorLocation'
CREATE INDEX [IX_FK_RfidDetectorLocation]
ON [dbo].[RfidDetector]
    ([Location_locationId]);
GO

-- Creating foreign key on [WaitingArea_waitingAreaId] in table 'Location'
ALTER TABLE [dbo].[Location]
ADD CONSTRAINT [FK_WaitingAreaLocation]
    FOREIGN KEY ([WaitingArea_waitingAreaId])
    REFERENCES [dbo].[WaitingArea]
        ([waitingAreaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WaitingAreaLocation'
CREATE INDEX [IX_FK_WaitingAreaLocation]
ON [dbo].[Location]
    ([WaitingArea_waitingAreaId]);
GO

-- Creating foreign key on [orderId] in table 'Note'
ALTER TABLE [dbo].[Note]
ADD CONSTRAINT [FK_OrderNote]
    FOREIGN KEY ([orderId])
    REFERENCES [dbo].[Order]
        ([orderId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderNote'
CREATE INDEX [IX_FK_OrderNote]
ON [dbo].[Note]
    ([orderId]);
GO

-- Creating foreign key on [notificationTypeId] in table 'Notification'
ALTER TABLE [dbo].[Notification]
ADD CONSTRAINT [FK_NotificationNotificationType]
    FOREIGN KEY ([notificationTypeId])
    REFERENCES [dbo].[NotificationType]
        ([notificationTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NotificationNotificationType'
CREATE INDEX [IX_FK_NotificationNotificationType]
ON [dbo].[Notification]
    ([notificationTypeId]);
GO

-- Creating foreign key on [orderId] in table 'Notification'
ALTER TABLE [dbo].[Notification]
ADD CONSTRAINT [FK_OrderNotification]
    FOREIGN KEY ([orderId])
    REFERENCES [dbo].[Order]
        ([orderId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderNotification'
CREATE INDEX [IX_FK_OrderNotification]
ON [dbo].[Notification]
    ([orderId]);
GO

-- Creating foreign key on [Procedure_procedureId] in table 'NotificationRule'
ALTER TABLE [dbo].[NotificationRule]
ADD CONSTRAINT [FK_NotificationRuleProcedure]
    FOREIGN KEY ([Procedure_procedureId])
    REFERENCES [dbo].[Procedure]
        ([procedureId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NotificationRuleProcedure'
CREATE INDEX [IX_FK_NotificationRuleProcedure]
ON [dbo].[NotificationRule]
    ([Procedure_procedureId]);
GO

-- Creating foreign key on [Procedure_procedureId] in table 'Order'
ALTER TABLE [dbo].[Order]
ADD CONSTRAINT [FK_OrderProcedure]
    FOREIGN KEY ([Procedure_procedureId])
    REFERENCES [dbo].[Procedure]
        ([procedureId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderProcedure'
CREATE INDEX [IX_FK_OrderProcedure]
ON [dbo].[Order]
    ([Procedure_procedureId]);
GO

-- Creating foreign key on [orderStatusId] in table 'Order'
ALTER TABLE [dbo].[Order]
ADD CONSTRAINT [FK_OrderStatusOrder]
    FOREIGN KEY ([orderStatusId])
    REFERENCES [dbo].[OrderStatus]
        ([orderStatusId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderStatusOrder'
CREATE INDEX [IX_FK_OrderStatusOrder]
ON [dbo].[Order]
    ([orderStatusId]);
GO

-- Creating foreign key on [Order_orderId] in table 'Update'
ALTER TABLE [dbo].[Update]
ADD CONSTRAINT [FK_UpdateOrder]
    FOREIGN KEY ([Order_orderId])
    REFERENCES [dbo].[Order]
        ([orderId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UpdateOrder'
CREATE INDEX [IX_FK_UpdateOrder]
ON [dbo].[Update]
    ([Order_orderId]);
GO

-- Creating foreign key on [patientId] in table 'RfidDetection'
ALTER TABLE [dbo].[RfidDetection]
ADD CONSTRAINT [FK_RfidDetectionPatient]
    FOREIGN KEY ([patientId])
    REFERENCES [dbo].[Patient]
        ([patientId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RfidDetectionPatient'
CREATE INDEX [IX_FK_RfidDetectionPatient]
ON [dbo].[RfidDetection]
    ([patientId]);
GO

-- Creating foreign key on [ProcedureCategory_procedureCategoryId] in table 'Procedure'
ALTER TABLE [dbo].[Procedure]
ADD CONSTRAINT [FK_ProcedureCategoryProcedure]
    FOREIGN KEY ([ProcedureCategory_procedureCategoryId])
    REFERENCES [dbo].[ProcedureCategory]
        ([procedureCategoryId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProcedureCategoryProcedure'
CREATE INDEX [IX_FK_ProcedureCategoryProcedure]
ON [dbo].[Procedure]
    ([ProcedureCategory_procedureCategoryId]);
GO

-- Creating foreign key on [rfidDetectorId] in table 'RfidDetection'
ALTER TABLE [dbo].[RfidDetection]
ADD CONSTRAINT [FK_RfidDetectionRfidDetector]
    FOREIGN KEY ([rfidDetectorId])
    REFERENCES [dbo].[RfidDetector]
        ([rfidDetectorId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RfidDetectionRfidDetector'
CREATE INDEX [IX_FK_RfidDetectionRfidDetector]
ON [dbo].[RfidDetection]
    ([rfidDetectorId]);
GO

-- Creating foreign key on [rfidDirectionId] in table 'RfidDetection'
ALTER TABLE [dbo].[RfidDetection]
ADD CONSTRAINT [FK_RfidDetectionRfidDirection]
    FOREIGN KEY ([rfidDirectionId])
    REFERENCES [dbo].[RfidDirection]
        ([rfidDirectionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RfidDetectionRfidDirection'
CREATE INDEX [IX_FK_RfidDetectionRfidDirection]
ON [dbo].[RfidDetection]
    ([rfidDirectionId]);
GO

-- Creating foreign key on [WaitingArea_waitingAreaId] in table 'RfidDetector'
ALTER TABLE [dbo].[RfidDetector]
ADD CONSTRAINT [FK_WaitingAreaRfidDetector]
    FOREIGN KEY ([WaitingArea_waitingAreaId])
    REFERENCES [dbo].[WaitingArea]
        ([waitingAreaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WaitingAreaRfidDetector'
CREATE INDEX [IX_FK_WaitingAreaRfidDetector]
ON [dbo].[RfidDetector]
    ([WaitingArea_waitingAreaId]);
GO

-- Creating foreign key on [AdmittingDoctor_doctorId] in table 'Admission'
ALTER TABLE [dbo].[Admission]
ADD CONSTRAINT [FK_AdmissionAdmittingDoctorDoctor]
    FOREIGN KEY ([AdmittingDoctor_doctorId])
    REFERENCES [dbo].[Doctor]
        ([doctorId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AdmissionAdmittingDoctorDoctor'
CREATE INDEX [IX_FK_AdmissionAdmittingDoctorDoctor]
ON [dbo].[Admission]
    ([AdmittingDoctor_doctorId]);
GO

-- Creating foreign key on [AttendingDoctor_doctorId] in table 'Admission'
ALTER TABLE [dbo].[Admission]
ADD CONSTRAINT [FK_AdmissionAttendingDoctorDoctor]
    FOREIGN KEY ([AttendingDoctor_doctorId])
    REFERENCES [dbo].[Doctor]
        ([doctorId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AdmissionAttendingDoctorDoctor'
CREATE INDEX [IX_FK_AdmissionAttendingDoctorDoctor]
ON [dbo].[Admission]
    ([AttendingDoctor_doctorId]);
GO

-- Creating foreign key on [PrimaryCareDoctor_doctorId] in table 'Admission'
ALTER TABLE [dbo].[Admission]
ADD CONSTRAINT [FK_AdmissionPrimaryCareDoctor]
    FOREIGN KEY ([PrimaryCareDoctor_doctorId])
    REFERENCES [dbo].[Doctor]
        ([doctorId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AdmissionPrimaryCareDoctor'
CREATE INDEX [IX_FK_AdmissionPrimaryCareDoctor]
ON [dbo].[Admission]
    ([PrimaryCareDoctor_doctorId]);
GO

-- Creating foreign key on [Bed_bedId] in table 'Admission'
ALTER TABLE [dbo].[Admission]
ADD CONSTRAINT [FK_AdmissionBed]
    FOREIGN KEY ([Bed_bedId])
    REFERENCES [dbo].[Bed]
        ([bedId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AdmissionBed'
CREATE INDEX [IX_FK_AdmissionBed]
ON [dbo].[Admission]
    ([Bed_bedId]);
GO

-- Creating foreign key on [externalSourceId] in table 'Doctor'
ALTER TABLE [dbo].[Doctor]
ADD CONSTRAINT [FK_ExternalSourceDoctor]
    FOREIGN KEY ([externalSourceId])
    REFERENCES [dbo].[ExternalSource]
        ([externalSourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ExternalSourceDoctor'
CREATE INDEX [IX_FK_ExternalSourceDoctor]
ON [dbo].[Doctor]
    ([externalSourceId]);
GO

-- Creating foreign key on [Admission_admissionId] in table 'Update'
ALTER TABLE [dbo].[Update]
ADD CONSTRAINT [FK_UpdateAdmission]
    FOREIGN KEY ([Admission_admissionId])
    REFERENCES [dbo].[Admission]
        ([admissionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UpdateAdmission'
CREATE INDEX [IX_FK_UpdateAdmission]
ON [dbo].[Update]
    ([Admission_admissionId]);
GO

-- Creating foreign key on [Bed_bedId] in table 'Update'
ALTER TABLE [dbo].[Update]
ADD CONSTRAINT [FK_UpdateBed]
    FOREIGN KEY ([Bed_bedId])
    REFERENCES [dbo].[Bed]
        ([bedId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UpdateBed'
CREATE INDEX [IX_FK_UpdateBed]
ON [dbo].[Update]
    ([Bed_bedId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------