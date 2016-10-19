CREATE TABLE [dbo].[DeviceConfiguration] (
    [deviceId]        INT           NOT NULL,
    [shortcutKeyNo]   INT NOT NULL,
    [configurationId] INT           NOT NULL,
    [cleaningBedDataTimeout]     INT            NOT NULL,
    [orderTimeout]     INT          NOT NULL,
    [presenceTimeout]     INT       NOT NULL,
    [rfidTimeout]     INT           NOT NULL,
    [dischargeTimeout]     INT      NOT NULL,
    [admissionsTimeout]     INT     NOT NULL,
	CONSTRAINT UC_Device_deviceIdshortcutKeyNo UNIQUE ([deviceId], [shortcutKeyNo])
);

