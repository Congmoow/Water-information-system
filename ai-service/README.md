# 涉水审批 AI 服务

基于 FastAPI + LangChain + ChromaDB 的涉水审批合规审查 AI 服务。

## 功能

- **知识库管理**：文档解析（PDF/Word）→ 文本分块 → 向量化 → ChromaDB 存储
- **语义检索**：基于 RAG 的法规知识检索
- **MCP Server**：提供 `knowledge_search` 和 `check_completeness` 两个 MCP 工具
- **合规审查 Agent**：LangChain Agent 自动审查审批材料的合规性
- **OCR**：识别扫描件中的文字（PaddleOCR）

## 技术栈

| 组件 | 技术 |
|------|------|
| Web 框架 | FastAPI |
| AI 框架 | LangChain |
| 向量数据库 | ChromaDB |
| 嵌入模型 | BAAI/bge-small-zh-v1.5 |
| 文档解析 | python-docx / pdfplumber |
| OCR | PaddleOCR |
| MCP | MCP Python SDK |

## 目录结构

```
ai-service/
├── app/
│   ├── main.py              # FastAPI 入口
│   ├── config.py            # 配置管理
│   ├── routers/             # API 路由
│   ├── services/            # 业务服务
│   ├── models/              # 数据模型
│   └── mcp/                 # MCP Server
├── knowledge_docs/          # 知识库源文档
├── chroma_db/               # ChromaDB 持久化
├── uploads/                 # 上传文件
└── requirements.txt
```

## 快速开始

### 1. 环境准备

```bash
# 推荐使用 Python 3.10+
python -m venv venv
venv\Scripts\activate  # Windows
# source venv/bin/activate  # Linux/Mac

pip install -r requirements.txt
```

### 2. 配置

创建 `.env` 文件（可选）：

```env
EMBEDDING_MODEL_NAME=BAAI/bge-small-zh-v1.5
CHROMA_PERSIST_DIR=./chroma_db
LLM_PROVIDER=openai
LLM_API_KEY=your-api-key
```

### 3. 启动服务

```bash
# 开发模式
python -m uvicorn app.main:app --host 0.0.0.0 --port 8000 --reload

# 或使用入口文件
python -m app.main
```

### 4. 访问文档

- Swagger UI: http://localhost:8000/docs
- ReDoc: http://localhost:8000/redoc

## API 端点

### 知识库管理

| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/knowledge/upload` | 上传知识库文档 |
| POST | `/knowledge/search` | 语义检索 |
| GET | `/knowledge/stats` | 知识库统计 |

### 合规审查

| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/review/submit` | 提交审查 |
| GET | `/review/result/{id}` | 获取审查结果 |
| POST | `/review/check-completeness` | 检查材料完整性 |

### 健康检查

| 方法 | 端点 | 说明 |
|------|------|------|
| GET | `/health` | 服务健康状态 |

## MCP Server

MCP Server 可独立启动（stdio 模式）：

```bash
python -m app.mcp.server
```

提供的 MCP 工具：

- `knowledge_search`：检索水利法规知识库
- `check_completeness`：检查申请材料完整性
