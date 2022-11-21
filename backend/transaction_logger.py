from threading import Thread, Event, Lock
from typing import TypeVar, Generic
from collections.abc import Iterator
import xml.etree.ElementTree as ET
from .schemas import ItemModel
import os
from datetime import datetime
from dataclasses import dataclass, field
from time import time as timestamp
from atexit import register as exit_register

__all__ = (
    "TransactionLogger",
)

T = TypeVar('T')

def _add_increment_to_filename(filename: str, inc: int) -> str:
    if inc == 0: return filename
    name, ext = os.path.splitext(filename)
    return f"{name}({inc}){ext}"

def _write_tree(tree: ET.ElementTree, loc: str):
    ET.indent(tree,"\t")
    with open(loc,"wb") as file:
        tree.write(file,"utf-8")

class DynamicIterable(Generic[T]):
    def __init__(self) -> None:
        self._iteration_trigger = Event()
        self._buffer: list[T] = []
        self._buffer_lock = Lock()
        self._stop_flag: bool = False

    def __iter__(self) -> Iterator[T]:
        start = 0
        while not self._stop_flag:
            stop = len(self._buffer)
            yield from self._buffer[start:stop]
            start = stop
            self._iteration_trigger.wait()
            self._iteration_trigger.clear()
    
    def add_item(self, element: T) -> None:
        self._buffer.append(element)
        self._iteration_trigger.set()

@dataclass
class TransactionInfo:
    items: list[ItemModel]
    origin_ip: str
    ts: float = field(default_factory=timestamp())
    pass

class TransactionLogger(Thread):
    BASE_PATH = os.path.join(
        os.path.dirname(__file__),
        "./transactions"
    )

    def __init__(self):
        self._iterable: DynamicIterable[
            TransactionInfo
        ] = DynamicIterable()
        super().__init__(daemon=True)

    def add_transaction(
        self,
        *items: list[ItemModel], 
        origin_ip: str
    ) -> None:
        self._iterable.add_item(TransactionInfo(
            items,
            origin_ip,
            timestamp()
        ))

    def run(self):
        os.makedirs(self.BASE_PATH,exist_ok=True)
        working_file = os.path.join(
            self.BASE_PATH,
            f"{datetime.now().strftime('%YT%mT%d,%H-%M-%S')}.xml"
        )
        working_file_inc = 0
        while os.path.exists(_add_increment_to_filename(
                working_file,
                working_file_inc
        )):
            working_file_inc += 1
        working_file = _add_increment_to_filename(
            working_file,
            working_file_inc
        )

        tree = ET.ElementTree(ET.Element("Transactions"))
        root = tree.getroot()
        _write_tree(tree,working_file)

        for transaction in self._iterable:
            transaction_element = ET.Element(
                "Transaction",
                {
                    "origin" : transaction.origin_ip,
                    "time" : datetime.fromtimestamp(transaction.ts)\
                        .strftime("%H:%M:%S")
                }
            )
            for item in transaction.items:
                transaction_element.append(ET.Element(
                    "Item",
                    {
                        "sellerId" : item.sellerId,
                        "price" : str(item.price)
                    }
                ))
            root.append(transaction_element)
            _write_tree(tree,working_file)