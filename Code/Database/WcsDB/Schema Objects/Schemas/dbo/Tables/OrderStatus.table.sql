CREATE TABLE [dbo].[OrderStatus] (
    [orderStatusId] INT           IDENTITY (1, 1) NOT NULL,
    [status]        NVARCHAR (20) NOT NULL
	CONSTRAINT UC_OrderStatus_status UNIQUE ([status])
);

