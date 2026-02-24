import uuid

from aiogram import Router, F
from aiogram.types import Message, CallbackQuery
from aiogram.fsm.context import FSMContext
from aiogram.utils.keyboard import InlineKeyboardBuilder
from datetime import datetime, timezone, timedelta

from config.config import Config
from services.notifications_service import NotificationsService
from services.health_check_service import HealthCheckService
from keyboards.keyboards import notifications_keyboard, times_keyboard
from models.notifications.get_notifications_response import GetNotificationsResponse
from models.notifications.get_notification_by_id_response import GetNotificatinByIdResponse
from models.notifications.create_notification_request import CreateNotificationRequest
from states.create_notification import CreateNotificationState

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
    
    response: GetNotificationsResponse = await notification_service.get_notifications(str(message.chat.id), )
    
    if len(response.notifications) == 0:
        await message.answer("Список пуст")
        return
    
    for notification in response.notifications:
        builder.button(text=f"Name: {notification.name}", callback_data=f"notification_{notification.id}")
    
    builder.adjust(1)
    
    if response.count_pages > 0:
        another_builder = InlineKeyboardBuilder()
        
        another_builder.button(text="1", callback_data="empty")
        if response.count_pages > 1:
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
    
    response: GetNotificationsResponse = await notification_service.get_notifications(str(callback.chat.id), page=count_page)
    builder = InlineKeyboardBuilder()
    for notification in response.notifications:
        builder.button(text=f"Name: {notification.name}", callback_data=f"notification_{notification.id}")
    
    another_builder = InlineKeyboardBuilder()
    
    if count_page > 1:
        another_builder.button(text="<<", callback_data=f"previous_notifications_page:{count_page - 1}")
    
    another_builder.button(text=f"{count_page}", callback_data="empty")
    
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
    
    response: GetNotificationsResponse = await notification_service.get_notifications(str(callback.chat.id), page=count_page)
    builder = InlineKeyboardBuilder()
    for notification in response.notifications:
        builder.button(text=f"Name: {notification.name}", callback_data=f"notification_{notification.id}")
    
    another_builder = InlineKeyboardBuilder()
    
    if count_page > 1:
        another_builder.button(text="<<", callback_data=f"previous_notifications_page:{count_page - 1}")
    
    another_builder.button(text=f"{count_page}", callback_data="empty")
    
    builder.adjust(1)
    
    if count_page <  response.count_pages:
        another_builder.button(text=">>", callback_data=f"next_notifications_page:{count_page + 1}")
    
    builder.attach(another_builder)
    
    await callback.message.edit_reply_markup(reply_markup=builder.as_markup())

@router.message(F.text == "Создать уведомление")
async def create_notification_process(message: Message, state: FSMContext) -> None:
    await state.set_state(CreateNotificationState.name)
    await message.answer(text="Отправьте название уведомления")

@router.message(CreateNotificationState.name)
async def create_notification_name_process(message: Message, state: FSMContext) -> None:
    await state.update_data(name=message.text)
    await state.set_state(CreateNotificationState.message)
    await message.answer(text="Отправьте описание для уведомления")

@router.message(CreateNotificationState.message)
async def create_notification_message_process(message: Message, state: FSMContext) -> None:
    await state.update_data(message=message.text)
    await state.set_state(CreateNotificationState.notification_time)
    await message.answer("Через сколько должно придти упоминание?(Выберите, что именно вы желаете добавить ко времени упоминания)", reply_markup=times_keyboard)

@router.message(CreateNotificationState.notification_time, F.text == "Минуты")
async def create_notification_choose_minutes_process(message: Message, state: FSMContext) -> None:
    await state.set_state(CreateNotificationState.minutes)
    await message.answer("Через сколько минут?")

@router.message(CreateNotificationState.notification_time, F.text == "Часы")
async def create_notification_choose_hours_process(message: Message, state: FSMContext) -> None:
    await state.set_state(CreateNotificationState.hours)
    await message.answer("Через сколько часов?")

@router.message(CreateNotificationState.notification_time, F.text == "Дни")
async def create_notification_choose_days_process(message: Message, state: FSMContext) -> None:
    await state.set_state(CreateNotificationState.days)
    await message.answer("Через сколько дней?")

@router.message(CreateNotificationState.minutes)
async def create_notification_minutes_process(message: Message, state: FSMContext) -> None:
    try:
        int(message.text)
    except:
        await message.answer("Значение должно быть натуральным числом! Отправьте снова.")
        return
    
    if int(message.text) < 0:
        await message.answer("Значение должно быть положительным числом! Отправьте снова.")
        return
    
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    state_data = await state.get_data()
    
    utc_now = datetime.now(timezone.utc)
    send_time = utc_now + timedelta(minutes=int(message.text))
    create_notification_request = CreateNotificationRequest(state_data['name'], state_data['message'], send_time.strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z')
    
    id = await notification_service.create_notification(str(message.chat.id), create_notification_request)
    
    if id:
        await message.answer("Задача успешно создана!")
        return
    else:
        await message.answer("Произошла ошибка! Попробуйте снова!")

@router.message(CreateNotificationState.hours)
async def create_notification_hours_process(message: Message, state: FSMContext) -> None:
    try:
        int(message.text)
    except:
        await message.answer("Значение должно быть натуральным числом! Отправьте снова.")
        return
    
    if int(message.text) < 0:
        await message.answer("Значение должно быть положительным числом! Отправьте снова.")
        return
    
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    state_data = await state.get_data()
    
    utc_now = datetime.now(timezone.utc)
    send_time = utc_now + timedelta(hours=int(message.text))
    create_notification_request = CreateNotificationRequest(state_data['name'], state_data['message'], send_time.strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z')
    
    id = await notification_service.create_notification(str(message.chat.id), create_notification_request)
    
    if id:
        await message.answer("Задача успешно создана!")
        return
    else:
        await message.answer("Произошла ошибка! Попробуйте снова!")

@router.message(CreateNotificationState.days)
async def create_notification_days_process(message: Message, state: FSMContext) -> None:
    try:
        int(message.text)
    except:
        await message.answer("Значение должно быть натуральным числом! Отправьте снова.")
        return
    
    if int(message.text) < 0:
        await message.answer("Значение должно быть положительным числом! Отправьте снова.")
        return
    
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    state_data = await state.get_data()
    
    utc_now = datetime.now(timezone.utc)
    send_time = utc_now + timedelta(days=int(message.text))
    create_notification_request = CreateNotificationRequest(state_data['name'], state_data['message'], send_time.strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z')
    
    id = await notification_service.create_notification(str(message.chat.id), create_notification_request)
    
    if id:
        await message.answer("Задача успешно создана!")
        return
    else:
        await message.answer("Произошла ошибка! Попробуйте снова!")
