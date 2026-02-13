from uuid import UUID
from dataclasses import dataclass

@dataclass
class GetTaskResponse:
    id: UUID
    name: str
    description: str
    table_name: str
    status: int