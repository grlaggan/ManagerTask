from dataclasses import dataclass

@dataclass
class CreateNotificationRequest:
    name: str
    message: str
    notificationTime: str