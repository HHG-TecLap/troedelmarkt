from sqlalchemy import create_engine
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker
from .config_reader import CONFIG

engine = create_engine(
    CONFIG["database"]["path"],
    connect_args={"check_same_thread": False},
)
session_factory = sessionmaker(
    engine,
    autoflush=False,
    autocommit=False
)
Base = declarative_base()