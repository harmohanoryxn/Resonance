ALTER TABLE [dbo].[Notification]
    ADD CONSTRAINT [FK_OrderNotification] FOREIGN KEY ([orderId]) REFERENCES [dbo].[Order] ([orderId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

