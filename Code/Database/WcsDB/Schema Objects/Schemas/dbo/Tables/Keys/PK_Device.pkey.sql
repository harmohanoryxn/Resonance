﻿ALTER TABLE [dbo].[Device]
    ADD CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED ([deviceId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

