from uuid import UUID
from dataclasses import dataclass

@dataclass
class UserDto:
    id: UUID 
    chat_id: int