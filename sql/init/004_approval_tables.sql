/*
    Water Information System
    004 - Approval and Review tables

    审批申请、附件、审查结果、审查发现项
*/

USE [WaterInfoSystemDb];
GO

IF OBJECT_ID(N'dbo.ApprovalApplications', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ApprovalApplications]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [ApplicantName] NVARCHAR(100) NOT NULL,
        [ApplicantIdCard] NVARCHAR(50) NOT NULL,
        [EnterpriseName] NVARCHAR(200) NULL,
        [EnterpriseLicenseNo] NVARCHAR(100) NULL,
        [WaterIntakeLocation] NVARCHAR(200) NOT NULL,
        [WaterIntakePurpose] NVARCHAR(200) NOT NULL,
        [WaterIntakeAmount] DECIMAL(18, 2) NOT NULL,
        [ApplicationDate] DATETIME2(7) NOT NULL,
        [Status] NVARCHAR(30) NOT NULL DEFAULT 'Pending',
        [SubmittedByUserId] UNIQUEIDENTIFIER NOT NULL,
        [CreatedAt] DATETIME2(7) NOT NULL,
        [UpdatedAt] DATETIME2(7) NOT NULL,
        CONSTRAINT [FK_ApprovalApplications_Users]
            FOREIGN KEY ([SubmittedByUserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION
    );

    CREATE INDEX [IX_ApprovalApplications_SubmittedByUserId]
        ON [dbo].[ApprovalApplications]([SubmittedByUserId]);
    CREATE INDEX [IX_ApprovalApplications_Status]
        ON [dbo].[ApprovalApplications]([Status]);
END;
GO

IF OBJECT_ID(N'dbo.ApprovalAttachments', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ApprovalAttachments]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [ApplicationId] UNIQUEIDENTIFIER NOT NULL,
        [FileName] NVARCHAR(200) NOT NULL,
        [FileType] NVARCHAR(50) NOT NULL,
        [FilePath] NVARCHAR(500) NOT NULL,
        [AttachmentType] NVARCHAR(50) NOT NULL,
        [CreatedAt] DATETIME2(7) NOT NULL,
        CONSTRAINT [FK_ApprovalAttachments_Applications]
            FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[ApprovalApplications]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_ApprovalAttachments_ApplicationId]
        ON [dbo].[ApprovalAttachments]([ApplicationId]);
END;
GO

IF OBJECT_ID(N'dbo.ReviewResults', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ReviewResults]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [ApplicationId] UNIQUEIDENTIFIER NOT NULL,
        [IsPassed] BIT NOT NULL,
        [Summary] NVARCHAR(2000) NOT NULL,
        [ReviewedAt] DATETIME2(7) NOT NULL,
        [AgentVersion] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2(7) NOT NULL,
        [UpdatedAt] DATETIME2(7) NOT NULL,
        CONSTRAINT [FK_ReviewResults_Applications]
            FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[ApprovalApplications]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_ReviewResults_ApplicationId]
        ON [dbo].[ReviewResults]([ApplicationId]);
END;
GO

IF OBJECT_ID(N'dbo.ReviewFindings', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ReviewFindings]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [ReviewResultId] UNIQUEIDENTIFIER NOT NULL,
        [Category] NVARCHAR(50) NOT NULL,
        [Severity] NVARCHAR(20) NOT NULL,
        [Description] NVARCHAR(1000) NOT NULL,
        [Suggestion] NVARCHAR(1000) NULL,
        [CreatedAt] DATETIME2(7) NOT NULL,
        [UpdatedAt] DATETIME2(7) NOT NULL,
        CONSTRAINT [FK_ReviewFindings_ReviewResults]
            FOREIGN KEY ([ReviewResultId]) REFERENCES [dbo].[ReviewResults]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_ReviewFindings_ReviewResultId]
        ON [dbo].[ReviewFindings]([ReviewResultId]);
END;
GO

PRINT N'Approval tables are ready.';
GO
