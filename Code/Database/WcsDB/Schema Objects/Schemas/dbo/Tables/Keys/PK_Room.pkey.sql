﻿ALTER TABLE [dbo].[Room]
    ADD CONSTRAINT [PK_Room] PRIMARY KEY CLUSTERED ([roomId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

