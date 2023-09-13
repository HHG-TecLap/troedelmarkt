from .ormmodels import Seller
from .calculations import *
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
    starting_fee: Decimal = Decimal(CONFIG["service"]["default_fee"])

    def to_sql(self):
        return Seller(
            id=self.id,
            name=self.name,
            rate=str(self.rate),
            starting_fee=str(self.starting_fee)
        )

class SellerServerModel(SellerClientModel):
    balance: Decimal
    revenue: Decimal
    provision: Decimal

    @staticmethod
    def from_sql(obj: Seller):
        balance = Decimal(obj.balance)
        starting_fee = Decimal(obj.starting_fee)
        rate = Decimal(obj.rate)
        return SellerServerModel(
            id=obj.id,
            name=obj.name,
            rate=obj.rate,
            balance=obj.balance,
            revenue=calculate_revenue(
                balance,
                starting_fee,
                rate
            ),
            provision=calculate_provision(
                balance,
                starting_fee,
                rate
            ),
            starting_fee=obj.starting_fee
        )

    def to_sql(self):
        return Seller(
            id=self.id,
            name=self.name,
            rate=str(self.rate),
            balance=str(self.balance),
            starting_fee=str(self.starting_fee)
        )
        pass

class SellerModifyModel(BaseModel):
    name: Optional[str] = None
    rate: Optional[Decimal] = None
    starting_fee: Optional[Decimal] = None

class ItemModel(BaseModel):
    sellerId: str
    price: Decimal

MalformedTransactionResponse = dict[str,tuple[list[str],list[str]]]