from fastapi import APIRouter, UploadFile, File, HTTPException

from app.models.schemas import (
    KnowledgeSearchRequest,
    KnowledgeSearchResponse,
    KnowledgeStatsResponse,
    SearchHit,
)
from app.services.document_parser import document_parser
from app.services.knowledge_store import knowledge_store

router = APIRouter()


@router.post("/upload")
async def upload_knowledge_document(file: UploadFile = File(...)):
    """上传知识库文档（PDF/Word），解析后向量化存入 ChromaDB。"""
    if not file.filename:
        raise HTTPException(status_code=400, detail="文件名不能为空")

    suffix = file.filename.rsplit(".", 1)[-1].lower()
    if suffix not in ("pdf", "docx", "doc"):
        raise HTTPException(status_code=400, detail="仅支持 PDF/Word 文档")

    content = await file.read()

    # 解析文档
    chunks = document_parser.parse_bytes(content, suffix, file.filename)

    # 存入向量数据库
    count = knowledge_store.add_documents(chunks)

    return {
        "message": f"文档 '{file.filename}' 已成功处理",
        "filename": file.filename,
        "chunks_count": count,
    }


@router.post("/search", response_model=KnowledgeSearchResponse)
async def search_knowledge(request: KnowledgeSearchRequest):
    """语义检索知识库。"""
    results = knowledge_store.search(request.query, top_k=request.top_k)

    hits = [
        SearchHit(
            content=r["content"],
            source=r["source"],
            score=r["score"],
            metadata=r.get("metadata", {}),
        )
        for r in results
    ]

    return KnowledgeSearchResponse(
        query=request.query,
        results=hits,
        total=len(hits),
    )


@router.get("/stats", response_model=KnowledgeStatsResponse)
async def get_knowledge_stats():
    """获取知识库统计信息。"""
    stats = knowledge_store.get_stats()
    return KnowledgeStatsResponse(**stats)
