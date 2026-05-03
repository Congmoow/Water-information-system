"""知识库存储服务：基于 ChromaDB 的向量存储与语义检索。"""

from __future__ import annotations

from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain_chroma import Chroma
from langchain.schema import Document

from app.config import settings
from app.services.embedding_service import embedding_service
from app.services.document_parser import DocumentChunk


class KnowledgeStore:
    """管理 ChromaDB 向量数据库的读写与检索。"""

    _vectorstore: Chroma | None = None
    _text_splitter: RecursiveCharacterTextSplitter | None = None

    def initialize(self) -> None:
        if self._vectorstore is not None:
            return

        self._text_splitter = RecursiveCharacterTextSplitter(
            chunk_size=settings.CHUNK_SIZE,
            chunk_overlap=settings.CHUNK_OVERLAP,
            separators=["\n\n", "\n", "。", "；", "，", " ", ""],
        )

        self._vectorstore = Chroma(
            collection_name=settings.CHROMA_COLLECTION_NAME,
            embedding_function=embedding_service.embeddings,
            persist_directory=settings.CHROMA_PERSIST_DIR,
        )

    @property
    def vectorstore(self) -> Chroma:
        if self._vectorstore is None:
            self.initialize()
        return self._vectorstore  # type: ignore[return-value]

    def add_documents(self, chunks: list[DocumentChunk]) -> int:
        """将文档块分块后向量化存入 ChromaDB。返回实际存储的块数。"""
        docs = []
        for chunk in chunks:
            docs.append(Document(
                page_content=chunk.content,
                metadata={
                    "source": chunk.source,
                    **({"page": chunk.page} if chunk.page else {}),
                    **({"section": chunk.section} if chunk.section else {}),
                    **chunk.metadata,
                },
            ))

        # 二次分块
        split_docs = self._text_splitter.split_documents(docs)  # type: ignore[union-attr]

        if not split_docs:
            return 0

        self.vectorstore.add_documents(split_docs)
        return len(split_docs)

    def search(self, query: str, top_k: int = 5) -> list[dict]:
        """语义检索，返回相关文档片段。"""
        results = self.vectorstore.similarity_search_with_relevance_scores(
            query, k=top_k
        )

        return [
            {
                "content": doc.page_content,
                "source": doc.metadata.get("source", "未知"),
                "score": round(score, 4),
                "metadata": {k: v for k, v in doc.metadata.items() if k != "source"},
            }
            for doc, score in results
        ]

    def get_stats(self) -> dict:
        """获取知识库统计信息。"""
        collection = self.vectorstore._collection  # type: ignore[attr-defined]
        count = collection.count()
        return {
            "total_documents": count,
            "total_chunks": count,
            "collection_name": settings.CHROMA_COLLECTION_NAME,
        }


knowledge_store = KnowledgeStore()
