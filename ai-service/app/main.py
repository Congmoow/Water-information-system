from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from app.config import settings
from app.routers import health, knowledge, review

app = FastAPI(
    title=settings.APP_NAME,
    version=settings.APP_VERSION,
    description="涉水审批智能审核系统 - AI 服务（知识库、MCP、Agent）",
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(health.router, tags=["健康检查"])
app.include_router(knowledge.router, prefix="/knowledge", tags=["知识库管理"])
app.include_router(review.router, prefix="/review", tags=["合规审查"])


@app.on_event("startup")
async def startup():
    """初始化服务：加载嵌入模型、连接 ChromaDB。"""
    from app.services.embedding_service import embedding_service
    from app.services.knowledge_store import knowledge_store

    embedding_service.initialize()
    knowledge_store.initialize()


if __name__ == "__main__":
    import uvicorn
    uvicorn.run("app.main:app", host=settings.HOST, port=settings.PORT, reload=settings.DEBUG)
