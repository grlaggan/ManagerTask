import datetime

from uuid import UUID
from dataclasses import dataclass

@dataclass
class NotificationDto:
    id: UUID
    name: str
    message: str
    notification_time: datetime.datetime
    created_at: datetime.datetime