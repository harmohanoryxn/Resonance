﻿ALTER TABLE [dbo].[NotificationRule]
    ADD CONSTRAINT [PK_NotificationRule] PRIMARY KEY CLUSTERED ([notificationRuleId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

