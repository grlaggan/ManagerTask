from dataclasses import dataclass

@dataclass
class TelegramMessage:
    name: str
    description: str
    tableName: str
    chatId: str
    minutes: int
    hours: int
    days: int