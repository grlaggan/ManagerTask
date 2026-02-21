import uuid

from aiogram import Router, F
from aiogram.types import Message, CallbackQuery
from aiogram.utils.keyboard import InlineKeyboardBuilder

from config.config import Config
from services.notifications_service import NotificationsService
from services.health_check_service import HealthCheckService
from keyboards.keyboards import notifications_keyboard
from models.notifications.get_notifications_response import GetNotificationsResponse
from models.notifications.get_notification_by_id_response import GetNotificatinByIdResponse

router = Router()

config: Config = Config(".env.tgBot")
notification_service = NotificationsService(config=config)
health_check_service = HealthCheckService(config)

@router.callback_query(F.data.startswith("notification_"))
async def get_notificatin_by_id_command(callback: CallbackQuery) -> None:
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await callback.message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    notification_id = callback.data.split('_')[1]
    response: GetNotificatinByIdResponse = await notification_service.get_notification_by_id(uuid.UUID(notification_id))
    
    await callback.message.answer(text=f"Название: {response.name}\nОписание: {response.message}")

@router.message(F.text == "Уведомления")
async def process_notifications_command(message: Message) -> None:
    await message.answer(text="Выберите операцию", reply_markup=notifications_keyboard)

@router.message(F.text == "Получить уведомления")
async def get_notification_command(message: Message) -> None:
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    builder = InlineKeyboardBuilder()
    
    response: GetNotificationsResponse = await notification_service.get_notifications()
    
    for notification in response.notifications:
        builder.button(text=f"Name: {notification.name}", callback_data=f"notification_{notification.id}")
    
    builder.adjust(1)
    
    if response.count_pages > 0:
        another_builder = InlineKeyboardBuilder()
        
        count_pages = 6 if response.count_pages > 6 else response.count_pages
        for page in range(1, count_pages + 1):
            another_builder.button(text=f"{page}", callback_data=f"notifications_page:{page}")
        
        another_builder.button(text=">>", callback_data=f"next_notifications_page:{2}")
        builder.attach(another_builder)
    
    await message.answer("Here's list of notifications", reply_markup=builder.as_markup())

@router.callback_query(F.data.startswith("next_notifications_page:"))
async def get_notification_next_page_command(callback: CallbackQuery) -> None:
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await callback.message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    count_page = int(callback.data.split(':')[1])
    
    response: GetNotificationsResponse = await notification_service.get_notifications(page=count_page)
    builder = InlineKeyboardBuilder()
    for notification in response.notifications:
        builder.button(text=f"Name: {notification.name}", callback_data=f"notification_{notification.id}")
    
    another_builder = InlineKeyboardBuilder()
    
    if count_page > 1:
        another_builder.button(text="<<", callback_data=f"previous_notifications_page:{count_page - 1}")
    
    count_pages = 6 if response.count_pages > 6 else response.count_pages
    for page in range(1 if response.count_pages < 6 else count_page + 1, count_pages + 1):
        another_builder.button(text=f"{page}", callback_data=f"notifications_page:{page}")
    
    builder.adjust(1)
    
    if count_page <  response.count_pages:
        another_builder.button(text=">>", callback_data=f"next_notifications_page:{count_page + 1}")
    
    builder.attach(another_builder)
    
    await callback.message.edit_reply_markup(reply_markup=builder.as_markup())

@router.callback_query(F.data.startswith("previous_notifications_page:"))
async def get_notification_next_page_command(callback: CallbackQuery) -> None:
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await callback.message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    count_page = int(callback.data.split(':')[1])
    
    response: GetNotificationsResponse = await notification_service.get_notifications(page=count_page)
    builder = InlineKeyboardBuilder()
    for notification in response.notifications:
        builder.button(text=f"Name: {notification.name}", callback_data=f"notification_{notification.id}")
    
    another_builder = InlineKeyboardBuilder()
    
    if count_page > 1:
        another_builder.button(text="<<", callback_data=f"previous_notifications_page:{count_page - 1}")
    
    count_pages = 6 if response.count_pages > 6 else response.count_pages
    for page in range(1 if response.count_pages < 6 else count_page + 1, count_pages + 1):
        another_builder.button(text=f"{page}", callback_data=f"notifications_page:{page}")
    
    builder.adjust(1)
    
    if count_page <  response.count_pages:
        another_builder.button(text=">>", callback_data=f"next_notifications_page:{count_page + 1}")
    
    builder.attach(another_builder)
    
    await callback.message.edit_reply_markup(reply_markup=builder.as_markup())