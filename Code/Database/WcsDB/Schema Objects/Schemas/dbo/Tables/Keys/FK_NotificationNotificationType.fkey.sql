ALTER TABLE [dbo].[Notification]
    ADD CONSTRAINT [FK_NotificationNotificationType] FOREIGN KEY ([notificationTypeId]) REFERENCES [dbo].[NotificationType] ([notificationTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
