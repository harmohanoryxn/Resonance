ALTER TABLE [dbo].[Staging_Order]
	ADD CONSTRAINT [PK_Staging_Order]
	PRIMARY KEY (orderExternalSource, orderExternalId)