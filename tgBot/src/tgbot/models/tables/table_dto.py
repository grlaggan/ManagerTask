from dataclasses import dataclass
import uuid

@dataclass
class TableDto:
    id: uuid.UUID
    name: str
    description: str
