# 水利信息系统

一个面向水库、河道、站点监测与告警管理的水利信息系统课程项目，用于课程提交与答辩演示。

## 项目简介

- 项目名称：水利信息系统
- 项目性质：课程项目
- 项目说明：提供登录鉴权、基础信息管理、监测数据展示、告警处理、图表统计和地图展示能力

## 技术栈

- 后端：ASP.NET Core 8、EF Core、SQL Server
- 前端：Vue 3、TypeScript、Vite、Element Plus、Pinia
- 数据库：SQL Server
- 认证：JWT + BCrypt 密码哈希
- 接口文档：OpenAPI + Scalar

## 核心功能

- 登录与权限控制
- 水库、河道、站点管理
- 监测数据查询与趋势展示
- 告警记录查询与处理
- 统计图表展示
- 地图点位展示

## 项目结构

- `backend`：后端解决方案与接口实现
- `frontend`：前端项目
- `sql`：数据库初始化脚本与相关 SQL 文件
- `docs`：课程文档、说明书与过程资料

## 运行方式

### 1. 数据库初始化

项目当前后端使用 `EnsureCreated + DataSeeder` 初始化数据库，不依赖 EF Core Migration。

有两种方式可以初始化数据库：

1. 手动执行 SQL 脚本
   - `sql/init/001_create_database.sql`
   - `sql/init/002_create_tables.sql`
   - `sql/init/003_seed_data.sql`
2. 直接启动后端
   - 后端首次连接空数据库时，会自动创建表并写入演示数据

说明：

- 手动执行 SQL 脚本更适合提交和答辩时展示数据库结构
- 如果已经执行了 `003_seed_data.sql`，后端 `DataSeeder` 检测到 `Users` 表有数据后会跳过再次初始化
- 如果选择手动方式，请按 `001 -> 002 -> 003` 的顺序完整执行

### 环境配置

后端启动前需要配置 JWT 签名密钥。请在 `backend/src/WaterInfoSystem.API/` 目录下创建 `appsettings.Development.json` 文件：

```json
{
  "Jwt": {
    "SigningKey": "Your-Secret-Key-At-Least-32-Characters-Long!!"
  }
}
```

或者通过环境变量设置：`Jwt__SigningKey=Your-Secret-Key-At-Least-32-Characters-Long!!`

> 注意：`appsettings.Development.json` 已在 `.gitignore` 中，不会被提交到仓库。

### 2. 启动后端

推荐命令：

```powershell
cd backend/src/WaterInfoSystem.API
dotnet run --no-launch-profile --urls http://localhost:5000
```

默认数据库连接在 `backend/src/WaterInfoSystem.API/appsettings.json` 中配置，当前库名为 `WaterInfoSystemDb`。

### 3. 启动前端

```powershell
cd frontend/water-info-web
npm install
npm run dev
```

前端默认地址：

- `http://localhost:5173`

开发环境下前端代理默认转发到：

- `http://localhost:5000`

### 4. 使用启动脚本

仓库根目录提供 Windows 演示启动脚本：

```powershell
.\start.ps1
```

脚本会：

- 检查前后端路径是否存在
- 提示先确认 SQL Server 已启动
- 分别打开两个 PowerShell 窗口启动后端和前端
- 将后端固定到 `http://localhost:5000`，与前端开发代理保持一致

如果前端依赖已经安装完成，也可以使用：

```powershell
.\start.ps1 -SkipDependencyInstall
```

## 演示账号

如果使用项目默认演示数据，可直接使用以下账号：

- 管理员：`admin` / `Admin@123`
- 普通用户：`viewer` / `Viewer@123`

这些账号同时与：

- 后端 `DataSeeder`
- `sql/init/003_seed_data.sql`

保持一致。

## 接口文档

后端启动后可访问：

- OpenAPI JSON：`http://localhost:5000/openapi/v1.json`
- Scalar 文档页：`http://localhost:5000/scalar/v1`

## 说明

- 本次仓库收尾补充了 SQL 初始化脚本、根目录启动脚本和简短 README，便于最终提交与答辩演示
- 未扩展业务功能，未调整前后端主体结构


