/*
    Water Information System
    002 - Create tables

    说明：
    - 表结构依据当前后端实体与 EF Core 配置整理
    - 当前项目将枚举按字符串持久化，因此以下字段使用 NVARCHAR 存储
*/

USE [WaterInfoSystemDb];
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Users]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [Username] NVARCHAR(50) NOT NULL,
        [PasswordHash] NVARCHAR(200) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Role] NVARCHAR(20) NOT NULL,
        [CreatedAt] DATETIME2(7) NOT NULL,
        [UpdatedAt] DATETIME2(7) NOT NULL
    );

    CREATE UNIQUE INDEX [IX_Users_Username] ON [dbo].[Users]([Username]);
END;
GO

IF OBJECT_ID(N'dbo.Reservoirs', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Reservoirs]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Location] NVARCHAR(150) NOT NULL,
        [Capacity] DECIMAL(18, 2) NOT NULL,
        [ManagementUnit] NVARCHAR(150) NOT NULL,
        [Latitude] DECIMAL(9, 6) NOT NULL,
        [Longitude] DECIMAL(9, 6) NOT NULL,
        [Description] NVARCHAR(500) NOT NULL,
        [CreatedAt] DATETIME2(7) NOT NULL,
        [UpdatedAt] DATETIME2(7) NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.Rivers', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Rivers]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Length] DECIMAL(18, 2) NOT NULL,
        [Basin] NVARCHAR(100) NOT NULL,
        [Latitude] DECIMAL(9, 6) NOT NULL,
        [Longitude] DECIMAL(9, 6) NOT NULL,
        [Description] NVARCHAR(500) NOT NULL,
        [CreatedAt] DATETIME2(7) NOT NULL,
        [UpdatedAt] DATETIME2(7) NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.Stations', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Stations]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Type] NVARCHAR(30) NOT NULL,
        [Longitude] DECIMAL(9, 6) NOT NULL,
        [Latitude] DECIMAL(9, 6) NOT NULL,
        [Status] NVARCHAR(30) NOT NULL,
        [WarningThreshold] DECIMAL(18, 2) NOT NULL,
        [CriticalThreshold] DECIMAL(18, 2) NOT NULL,
        [Description] NVARCHAR(500) NOT NULL,
        [LastActiveAt] DATETIME2(7) NULL,
        [ReservoirId] UNIQUEIDENTIFIER NULL,
        [RiverId] UNIQUEIDENTIFIER NULL,
        [CreatedAt] DATETIME2(7) NOT NULL,
        [UpdatedAt] DATETIME2(7) NOT NULL,
        CONSTRAINT [FK_Stations_Reservoirs_ReservoirId]
            FOREIGN KEY ([ReservoirId]) REFERENCES [dbo].[Reservoirs]([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Stations_Rivers_RiverId]
            FOREIGN KEY ([RiverId]) REFERENCES [dbo].[Rivers]([Id]) ON DELETE SET NULL
    );

    CREATE INDEX [IX_Stations_ReservoirId] ON [dbo].[Stations]([ReservoirId]);
    CREATE INDEX [IX_Stations_RiverId] ON [dbo].[Stations]([RiverId]);
END;
GO

IF OBJECT_ID(N'dbo.MonitoringDatas', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[MonitoringDatas]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [StationId] UNIQUEIDENTIFIER NOT NULL,
        [DataType] NVARCHAR(30) NOT NULL,
        [Value] DECIMAL(18, 2) NOT NULL,
        [CollectedAt] DATETIME2(7) NOT NULL,
        [Remark] NVARCHAR(300) NULL,
        [CreatedAt] DATETIME2(7) NOT NULL,
        [UpdatedAt] DATETIME2(7) NOT NULL,
        CONSTRAINT [FK_MonitoringDatas_Stations_StationId]
            FOREIGN KEY ([StationId]) REFERENCES [dbo].[Stations]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_MonitoringDatas_StationId_CollectedAt]
        ON [dbo].[MonitoringDatas]([StationId], [CollectedAt]);
END;
GO

IF OBJECT_ID(N'dbo.AlarmRecords', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AlarmRecords]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [StationId] UNIQUEIDENTIFIER NOT NULL,
        [MonitoringDataId] UNIQUEIDENTIFIER NULL,
        [AlarmType] NVARCHAR(30) NOT NULL,
        [Level] NVARCHAR(30) NOT NULL,
        [Message] NVARCHAR(300) NOT NULL,
        [Status] NVARCHAR(30) NOT NULL,
        [TriggeredAt] DATETIME2(7) NOT NULL,
        [HandledAt] DATETIME2(7) NULL,
        [HandledByUserId] UNIQUEIDENTIFIER NULL,
        [HandleRemark] NVARCHAR(300) NULL,
        [CreatedAt] DATETIME2(7) NOT NULL,
        [UpdatedAt] DATETIME2(7) NOT NULL,
        CONSTRAINT [FK_AlarmRecords_Stations_StationId]
            FOREIGN KEY ([StationId]) REFERENCES [dbo].[Stations]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AlarmRecords_MonitoringDatas_MonitoringDataId]
            FOREIGN KEY ([MonitoringDataId]) REFERENCES [dbo].[MonitoringDatas]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AlarmRecords_Users]
            FOREIGN KEY ([HandledByUserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );

    CREATE INDEX [IX_AlarmRecords_StationId] ON [dbo].[AlarmRecords]([StationId]);
    CREATE INDEX [IX_AlarmRecords_MonitoringDataId] ON [dbo].[AlarmRecords]([MonitoringDataId]);
    CREATE INDEX [IX_AlarmRecords_HandledByUserId] ON [dbo].[AlarmRecords]([HandledByUserId]);
END;
GO

PRINT N'Core tables are ready.';
GO
