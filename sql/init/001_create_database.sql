/*
    Water Information System
    001 - Create database

    用途：
    1. 为课程项目手动初始化 SQL Server 数据库
    2. 与 backend/src/WaterInfoSystem.API/appsettings.json 中的数据库名保持一致

    默认数据库名：WaterInfoSystemDb
*/

IF DB_ID(N'WaterInfoSystemDb') IS NULL
BEGIN
    CREATE DATABASE [WaterInfoSystemDb];
END;
GO

USE [WaterInfoSystemDb];
GO

PRINT N'Database [WaterInfoSystemDb] is ready.';
GO
