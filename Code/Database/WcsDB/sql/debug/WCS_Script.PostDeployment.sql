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

