import aiohttp
import datetime

from uuid import UUID
from dataclasses import asdict

from config.config import Config
from models.notifications.get_notifications_response import GetNotificationsResponse
from models.notifications.get_notification_by_id_response import GetNotificatinByIdResponse
from models.notifications.notification_dto import NotificationDto
from models.notifications.create_notification_request import CreateNotificationRequest

class NotificationsService:
    def __init__(self, config: Config):
        self.config = config
    
    async def get_notifications(self, chat_id: str, page: int = 1, offset: int = 5) -> GetNotificationsResponse:
        async with aiohttp.ClientSession(headers={"chatId": chat_id}) as session:
            async with session.get(self.config.notifications_endpoint
                                    + f"?page={page}&offset={offset}" if page and offset else "") as response:
                
                if response.status != 200:
                    raise ValueError(f"HTTP Error: {response.status} {response.reason}")
                
                try:
                    data = await response.json()
                    
                    notifications: list[NotificationDto] = []
                    for notification in data["notifications"]:
                        newNotificationDto = NotificationDto(
                            id=notification["id"],
                            name=notification["name"],
                            message=notification["message"],
                            notification_time=datetime.datetime.strptime(notification['notificationTime'], "%Y-%m-%dT%H:%M:%S.%fZ"),
                            created_at=datetime.datetime.strptime(notification['createdAt'], "%Y-%m-%dT%H:%M:%S.%fZ")
                        )
                        notifications.append(newNotificationDto)
                        
                    return GetNotificationsResponse(notifications=notifications, page=data["page"], offset=data["offset"], count_pages=data["countPages"])
                    
                except aiohttp.ContentTypeError:
                    raise ValueError("Response is not valid JSON or has wrong Content-Type")
        
    async def get_notification_by_id(self, id: UUID) -> GetNotificatinByIdResponse:
        async with aiohttp.ClientSession() as session:
            async with session.get(self.config.notifications_endpoint + f"/{id}") as response:
                if response.status != 200:
                    raise ValueError(f"HTTP Error: {response.status} {response.reason}")
                
                try:
                    data = await response.json()
                    
                    response = GetNotificatinByIdResponse(
                        data["name"],
                        data["message"],
                        data["notificationTime"],
                        data["createdAt"]
                    )
                    
                    return response
                except aiohttp.ContentTypeError:
                    raise ValueError("Response is not valid JSON or has wrong Content-Type")
    
    async def create_notification(self, chat_id: str, request: CreateNotificationRequest) -> UUID:
        async with aiohttp.ClientSession(headers={"chatId": chat_id}) as session:
            async with session.post(self.config.notifications_endpoint, json=asdict(request)) as response:
                if response.status != 200:
                    raise ValueError(f"HTTP Error: {response.status} {response.reason}")
                
                try:
                    data = await response.json()
                    result = data['id']
                    
                    return result
                except aiohttp.ContentTypeError:
                    raise ValueError("Response is not valid JSON or has wrong Content-Type")