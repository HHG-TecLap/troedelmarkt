__all__ = ("Seller",)
from .database import Base
from sqlalchemy import Column, Text

class Seller(Base):
    __tablename__ = "sellers"

    id = Column(Text,nullable=False,primary_key=True)
    name = Column(Text,nullable=False)
    rate = Column(Text,nullable=True)
    balance = Column(Text,nullable=False,default=0)
