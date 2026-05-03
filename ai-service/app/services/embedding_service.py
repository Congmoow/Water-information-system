"""嵌入模型服务：加载 HuggingFace 嵌入模型，提供文本向量化能力。"""

from __future__ import annotations

from langchain_huggingface import HuggingFaceEmbeddings

from app.config import settings


class EmbeddingService:
    """管理嵌入模型的生命周期。"""

    _embeddings: HuggingFaceEmbeddings | None = None

    def initialize(self) -> None:
        if self._embeddings is not None:
            return

        self._embeddings = HuggingFaceEmbeddings(
            model_name=settings.EMBEDDING_MODEL_NAME,
            model_kwargs={"device": settings.EMBEDDING_DEVICE},
            encode_kwargs={"normalize_embeddings": True},
        )

    @property
    def embeddings(self) -> HuggingFaceEmbeddings:
        if self._embeddings is None:
            self.initialize()
        return self._embeddings  # type: ignore[return-value]


embedding_service = EmbeddingService()
