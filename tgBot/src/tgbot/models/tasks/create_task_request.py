from uuid import UUID
from dataclasses import dataclass
from datetime import datetime

@dataclass
class CreateTaskRequest:
    name: str
    description: str
    tableName: str
    sendTime: str