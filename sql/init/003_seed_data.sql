/*
    Water Information System
    003 - Seed demo data

    说明：
    - 演示账号与当前后端 DataSeeder 保持一致：
      admin / Admin@123
      viewer / Viewer@123
    - 密码使用 BCrypt 哈希，可直接登录系统
    - 若已经执行过 backend 自动 Seed，请不要重复执行本脚本
*/

USE [WaterInfoSystemDb];
GO

IF EXISTS (SELECT 1 FROM [dbo].[Users])
BEGIN
    PRINT N'Users table already contains data. Seed script skipped to avoid duplicates.';
    RETURN;
END;
GO

DECLARE @AdminUserId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @ViewerUserId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';

DECLARE @EastLakeReservoirId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333331';
DECLARE @WestHillReservoirId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333332';

DECLARE @SouthCreekRiverId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444441';
DECLARE @DragonBayRiverId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444442';

DECLARE @EastLakeWaterLevelStationId UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555551';
DECLARE @SouthCreekRainStationId UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555552';
DECLARE @DragonBayFlowStationId UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555553';
DECLARE @WestHillBackupStationId UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555554';

DECLARE @EastLakeData1 UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666611';
DECLARE @EastLakeData2 UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666612';
DECLARE @EastLakeData3 UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666613';
DECLARE @SouthCreekData1 UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666621';
DECLARE @SouthCreekData2 UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666622';
DECLARE @SouthCreekData3 UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666623';
DECLARE @DragonBayData1 UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666631';
DECLARE @DragonBayData2 UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666632';
DECLARE @DragonBayData3 UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666633';

DECLARE @Now DATETIME2(7) = SYSDATETIME();

INSERT INTO [dbo].[Users] ([Id], [Username], [PasswordHash], [FullName], [Role], [CreatedAt], [UpdatedAt])
VALUES
    (@AdminUserId, N'admin', N'$2a$11$xkoxQ0sij55oUCkZdaYJGOg/2jhIA9oDcc61gDk4RbSYOUmDm7gMa', N'张伟', N'Administrator', @Now, @Now),
    (@ViewerUserId, N'viewer', N'$2a$11$SFTbYY8b3jAcBKEp318o1e1gWihhJwkM3H42dOtmljh3WbEI5kuCi', N'李敏', N'User', @Now, @Now);

INSERT INTO [dbo].[Reservoirs] ([Id], [Name], [Location], [Capacity], [ManagementUnit], [Latitude], [Longitude], [Description], [CreatedAt], [UpdatedAt])
VALUES
    (@EastLakeReservoirId, N'东湖水库', N'江州市东湖新区', 1260.50, N'江州市水务局', 30.625000, 114.347000, N'承担城市供水与防汛调蓄任务。', @Now, @Now),
    (@WestHillReservoirId, N'西山水库', N'江州市西山镇', 980.20, N'西山区水利站', 30.576000, 114.287000, N'主要服务西山片区灌溉与生态补水。', @Now, @Now);

INSERT INTO [dbo].[Rivers] ([Id], [Name], [Length], [Basin], [Latitude], [Longitude], [Description], [CreatedAt], [UpdatedAt])
VALUES
    (@SouthCreekRiverId, N'南溪河', 68.50, N'南溪流域', 30.595000, 114.398000, N'区域内重点河道，汛期雨量变化明显。', @Now, @Now),
    (@DragonBayRiverId, N'龙湾河', 43.20, N'龙湾流域', 30.649000, 114.271000, N'承担沿线排涝与生态补水任务。', @Now, @Now);

INSERT INTO [dbo].[Stations] ([Id], [Name], [Type], [Longitude], [Latitude], [Status], [WarningThreshold], [CriticalThreshold], [Description], [LastActiveAt], [ReservoirId], [RiverId], [CreatedAt], [UpdatedAt])
VALUES
    (@EastLakeWaterLevelStationId, N'东湖1号水位站', N'WaterLevel', 114.349000, 30.626000, N'Online', 18.50, 20.00, N'东湖水库主坝前水位监测站。', DATEADD(MINUTE, -5, @Now), @EastLakeReservoirId, NULL, @Now, @Now),
    (@SouthCreekRainStationId, N'南溪河雨量站', N'Rainfall', 114.401000, 30.593000, N'Online', 50.00, 80.00, N'南溪河流域自动雨量监测站。', DATEADD(MINUTE, -12, @Now), NULL, @SouthCreekRiverId, @Now, @Now),
    (@DragonBayFlowStationId, N'龙湾流量站', N'Flow', 114.274000, 30.648000, N'Warning', 200.00, 300.00, N'龙湾河下游流量监测站。', DATEADD(MINUTE, -35, @Now), NULL, @DragonBayRiverId, @Now, @Now),
    (@WestHillBackupStationId, N'西山2号水位站', N'WaterLevel', 114.291000, 30.579000, N'Offline', 18.50, 20.00, N'西山水库备用站点，当前处于离线状态。', DATEADD(MINUTE, -48, @Now), @WestHillReservoirId, NULL, @Now, @Now);

INSERT INTO [dbo].[MonitoringDatas] ([Id], [StationId], [DataType], [Value], [CollectedAt], [Remark], [CreatedAt], [UpdatedAt])
VALUES
    (@EastLakeData1, @EastLakeWaterLevelStationId, N'WaterLevel', 17.60, '2026-04-01T09:00:00', N'水位自动采集', '2026-04-01T09:00:00', '2026-04-01T09:00:00'),
    (@EastLakeData2, @EastLakeWaterLevelStationId, N'WaterLevel', 18.20, '2026-04-02T09:00:00', N'水位自动采集', '2026-04-02T09:00:00', '2026-04-02T09:00:00'),
    (@EastLakeData3, @EastLakeWaterLevelStationId, N'WaterLevel', 18.90, '2026-04-03T09:00:00', N'水位自动采集', '2026-04-03T09:00:00', '2026-04-03T09:00:00'),
    (@SouthCreekData1, @SouthCreekRainStationId, N'Rainfall', 32.00, '2026-04-01T10:00:00', N'雨量自动采集', '2026-04-01T10:00:00', '2026-04-01T10:00:00'),
    (@SouthCreekData2, @SouthCreekRainStationId, N'Rainfall', 58.00, '2026-04-02T10:00:00', N'雨量自动采集', '2026-04-02T10:00:00', '2026-04-02T10:00:00'),
    (@SouthCreekData3, @SouthCreekRainStationId, N'Rainfall', 86.00, '2026-04-03T10:00:00', N'雨量自动采集', '2026-04-03T10:00:00', '2026-04-03T10:00:00'),
    (@DragonBayData1, @DragonBayFlowStationId, N'Flow', 182.00, '2026-04-01T11:00:00', N'流量自动采集', '2026-04-01T11:00:00', '2026-04-01T11:00:00'),
    (@DragonBayData2, @DragonBayFlowStationId, N'Flow', 205.00, '2026-04-02T11:00:00', N'流量自动采集', '2026-04-02T11:00:00', '2026-04-02T11:00:00'),
    (@DragonBayData3, @DragonBayFlowStationId, N'Flow', 228.00, '2026-04-03T11:00:00', N'流量自动采集', '2026-04-03T11:00:00', '2026-04-03T11:00:00');

INSERT INTO [dbo].[AlarmRecords] ([Id], [StationId], [MonitoringDataId], [AlarmType], [Level], [Message], [Status], [TriggeredAt], [HandledAt], [HandledByUserId], [HandleRemark], [CreatedAt], [UpdatedAt])
VALUES
    ('77777777-7777-7777-7777-777777777711', @EastLakeWaterLevelStationId, @EastLakeData3, N'ThresholdExceeded', N'Warning', N'东湖1号水位站水位超过警戒值。', N'Pending', '2026-04-03T09:00:00', NULL, NULL, NULL, '2026-04-03T09:00:00', '2026-04-03T09:00:00'),
    ('77777777-7777-7777-7777-777777777712', @SouthCreekRainStationId, @SouthCreekData3, N'ThresholdExceeded', N'Critical', N'南溪河雨量站雨量超过严重阈值。', N'Processing', '2026-04-03T10:00:00', '2026-04-03T10:20:00', @AdminUserId, N'已通知值班人员加强巡查。', '2026-04-03T10:00:00', '2026-04-03T10:20:00'),
    ('77777777-7777-7777-7777-777777777713', @DragonBayFlowStationId, @DragonBayData3, N'ThresholdExceeded', N'Warning', N'龙湾流量站流量持续处于高位。', N'Pending', '2026-04-03T11:00:00', NULL, NULL, NULL, '2026-04-03T11:00:00', '2026-04-03T11:00:00'),
    ('77777777-7777-7777-7777-777777777714', @WestHillBackupStationId, NULL, N'StationOffline', N'Warning', N'站点离线超过30分钟。', N'Pending', DATEADD(MINUTE, -30, @Now), NULL, NULL, NULL, DATEADD(MINUTE, -30, @Now), DATEADD(MINUTE, -30, @Now));

PRINT N'Demo seed data inserted successfully.';
GO
