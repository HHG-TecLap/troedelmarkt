from .ormmodels import Seller
from .config_reader import CONFIG
from pydantic import BaseModel
from decimal import Decimal
from typing import Optional

class BaseModel(BaseModel):
    class Config:
        json_encoders = {
            Decimal : str
        }

__all__ = (
    "SellerClientModel",
    "SellerServerModel",
    "SellerModifyModel",
    "ItemModel",
    "MalformedTransactionResponse"
)

class SellerClientModel(BaseModel):
    id: str
    name: str
    rate: Decimal = Decimal(CONFIG["service"]["default_rate"])

    def to_sql(self):
        return Seller(
            id=self.id,
            name=self.name,
            rate=str(self.rate) if self.rate is not None else None,
        )

class SellerServerModel(SellerClientModel):
    balance: Decimal
    revenue: Decimal
    provision: Decimal

    @staticmethod
    def from_sql(obj: Seller):
        balance = Decimal(obj.balance)
        if obj.rate is None:
            rate = None
        else:
            rate = Decimal(obj.rate)
        return SellerServerModel(
            id=obj.id,
            name=obj.name,
            rate=obj.rate,
            balance=obj.balance,
            revenue=balance*(1-rate),
            provision=balance*rate
        )

    def to_sql(self):
        return Seller(
            id=self.id,
            name=self.name,
            rate=str(self.rate) if self.rate is not None else None,
            balance=str(self.balance)
        )
        pass

class SellerModifyModel(BaseModel):
    name: Optional[str] = None
    rate: Optional[Decimal] = None

class ItemModel(BaseModel):
    sellerId: str
    price: Decimal

MalformedTransactionResponse = dict[str,tuple[list[str],list[str]]]