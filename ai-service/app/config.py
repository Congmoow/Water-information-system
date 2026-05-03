from pydantic_settings import BaseSettings
from pathlib import Path


class Settings(BaseSettings):
    APP_NAME: str = "涉水审批 AI 服务"
    APP_VERSION: str = "1.0.0"
    DEBUG: bool = True

    # Server
    HOST: str = "0.0.0.0"
    PORT: int = 8000

    # Embedding Model
    EMBEDDING_MODEL_NAME: str = "BAAI/bge-small-zh-v1.5"
    EMBEDDING_DEVICE: str = "cpu"

    # ChromaDB
    CHROMA_PERSIST_DIR: str = str(Path(__file__).parent.parent / "chroma_db")
    CHROMA_COLLECTION_NAME: str = "water_regulations"

    # Document Storage
    KNOWLEDGE_DOCS_DIR: str = str(Path(__file__).parent.parent / "knowledge_docs")
    UPLOAD_DIR: str = str(Path(__file__).parent.parent / "uploads")

    # LLM (for Agent)
    LLM_PROVIDER: str = "openai"  # openai / ollama
    LLM_MODEL: str = "gpt-4o-mini"
    LLM_API_KEY: str = ""
    LLM_BASE_URL: str = ""
    LLM_TEMPERATURE: float = 0.0

    # Text Splitting
    CHUNK_SIZE: int = 500
    CHUNK_OVERLAP: int = 50

    # .NET Backend
    DOTNET_BACKEND_URL: str = "http://localhost:5000"

    class Config:
        env_file = ".env"
        env_file_encoding = "utf-8"


settings = Settings()
