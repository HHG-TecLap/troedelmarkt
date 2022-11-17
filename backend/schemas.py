from .ormmodels import Seller
from pydantic import BaseModel
from decimal import Decimal

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
    rate: Decimal|None

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
    name: str|None
    rate: Decimal|None

class ItemModel(BaseModel):
    sellerId: str
    price: Decimal

MalformedTransactionResponse = dict[str,tuple[list[str],list[str]]]