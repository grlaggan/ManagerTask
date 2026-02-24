from datetime import datetime
from dataclasses import dataclass

@dataclass
class GetNotificatinByIdResponse:
    name: str
    message: str
    notification_time: datetime
    created_at: datetime