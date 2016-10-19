CREATE TABLE [dbo].[RfidDetection](
	[rfidDetectionId] [int] IDENTITY(1,1) NOT NULL,
	[patientId] [int] NOT NULL,
	[rfidDirectionId] [int] NOT NULL,
	[rfidDetectorId] [int] NOT NULL,
	[dateTimeDetected] [datetime] NOT NULL
) ON [PRIMARY]