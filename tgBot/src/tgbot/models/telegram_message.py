from dataclasses import dataclass

@dataclass
class TelegramMessage:
    name: str
    description: str
    tableName: str
    minutes: int
    hours: int
    days: int