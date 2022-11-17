from .ormmodels import Seller
from .schemas import SellerClientModel, SellerModifyModel
from sqlalchemy.orm import Session
from sqlalchemy import select
from sqlite3 import IntegrityError

__all__ = (
    "get_all_sellers",
    "get_seller",
    "new_seller",
    "remove_seller",
    "update_seller"
)

def get_all_sellers(session: Session) -> list[Seller]:
    return session.execute(select(Seller)).scalars().all()
    pass

def get_seller(session: Session,seller_id: str) -> Seller|None:
    """Returns a Seller instance or None if not found"""
    return session.get(Seller,seller_id)

def new_seller(session: Session, schema: SellerClientModel) -> Seller:
    """Creates a new seller and returns the newly created instance"""
    seller = schema.to_sql()
    try:
        session.add(seller)
    except IntegrityError as e:
        raise RuntimeError("A trader with the specified ID already exists") from e
    session.commit()
    return seller
    pass

def remove_seller(session: Session, seller: Seller):
    session.delete(seller)
    session.commit()
    pass

def update_seller(
    session: Session, 
    seller_id: str, 
    schema: SellerModifyModel
) -> Seller:
    seller = get_seller(session,seller_id)
    if seller is None:
        raise KeyError

    seller.name = schema.name
    seller.rate = str(schema.rate) if schema.rate is not None else None

    session.commit()
    return seller