from dataclasses import dataclass

from models.notifications.notification_dto import NotificationDto

@dataclass
class GetNotificationsResponse:
    notifications: list[NotificationDto]
    page: int
    offset: int
    count_pages: int