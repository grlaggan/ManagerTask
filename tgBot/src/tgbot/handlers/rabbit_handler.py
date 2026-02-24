import aio_pika
import json

from aiogram import Bot
from aiogram.types import ChatIdUnion

from models.telegram_message import TelegramMessage
from models.notification_message import NotificationMessage
from config.config import Config
from services.tasks_service import TasksService
from services.notifications_service import NotificationsService
from models.tasks.get_task_repsponse import GetTaskResponse


def create_handler(bot: Bot):
    async def handler(message: aio_pika.IncomingMessage):
        async with message.process():
            raw_data = json.loads(message.body.decode())
            
            try:
                TelegramMessage(**raw_data)
                await handler_task_message(message)
            except:
                await handler_notification_message(message)
    
    async def handler_notification_message(message: aio_pika.IncomingMessage):
        config = Config(".env.tgBot")
        
        async with message.process():
            raw_data = json.loads(message.body.decode())
            notification_message = NotificationMessage(**raw_data)
            
            ms = f'Название задачи: {notification_message.name}\nОписание уведомления: {notification_message.message}'
                
            await bot.send_message(
                chat_id=notification_message.chatId,
                text=ms
            )
            
            return
        
    async def handler_task_message(message: aio_pika.IncomingMessage):
        config = Config(".env.tgBot")
        task_service = TasksService(config)
        
        async with message.process():
            raw_data = json.loads(message.body.decode())
            tg_message = TelegramMessage(**raw_data)
            
            if tg_message.minutes != 0:
                ms = f'Название задачи: {tg_message.name}\nОписание задачи: {tg_message.description}\nНазвание таблицы: {tg_message.tableName}\n\nДедлайн через {tg_message.minutes} минут!'
                
                await bot.send_message(
                    chat_id=tg_message.chatId,
                    text=ms
                )
                return
            if tg_message.hours != 0:
                ms = f'Название задачи: {tg_message.name}\nОписание задачи: {tg_message.description}\nНазвание таблицы: {tg_message.tableName}\n\nДедлайн через {tg_message.hours} часов!'
                
                await bot.send_message(
                    chat_id=tg_message.chatId,
                    text=ms
                )
                return

            if tg_message.days != 0:
                ms = f'Название задачи: {tg_message.name}\nОписание задачи: {tg_message.description}\nНазвание таблицы: {tg_message.tableName}\n\nДедлайн через {tg_message.days} дней!'
                
                await bot.send_message(
                    chat_id=tg_message.chatId,
                    text=ms
                )
                return
            
            ms = f'Название задачи: {tg_message.name}\nОписание задачи: {tg_message.description}\nНазвание таблицы: {tg_message.tableName}\n\nВы не успели выполнить задание!'
            
            response: GetTaskResponse = await task_service.get_task_by_name(tg_message.name)
            await task_service.change_status_on_failed(response.id)
            
            await bot.send_message(
                chat_id=tg_message.chatId,
                text=ms
            )
            return
    
    return handler