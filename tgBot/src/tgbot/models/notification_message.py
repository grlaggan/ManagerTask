from dataclasses import dataclass

@dataclass
class NotificationMessage:
    name: str
    message: str
    chatId: str
    minutes: int
    hours: int
    days: int