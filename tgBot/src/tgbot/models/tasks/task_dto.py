import datetime
import uuid

from dataclasses import dataclass

from models.tables.table_dto import TableDto

@dataclass
class TaskDto:
    id: uuid.UUID
    name: str
    description: str
    created_at: datetime.datetime
    table: TableDto
    send_time: datetime.datetime
    status: int