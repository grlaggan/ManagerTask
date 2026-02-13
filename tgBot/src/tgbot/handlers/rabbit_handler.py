import aio_pika
import json

from aiogram import Bot
from aiogram.types import ChatIdUnion

from models.telegram_message import TelegramMessage

def create_handler(bot: Bot):
    async def handler(message: aio_pika.IncomingMessage):
        async with message.process():
            raw_data = json.loads(message.body.decode())
            tg_message = TelegramMessage(**raw_data)
            
            if tg_message.minutes != 0:
                ms = f'Название задачи: {tg_message.name}\nОписание задачи: {tg_message.description}\nНазвание таблицы: {tg_message.tableName}\n\nДедлайн через {tg_message.minutes} минут!'
                
                await bot.send_message(
                    chat_id=5218298123,
                    text=ms
                )
                return
            if tg_message.hours != 0:
                ms = f'Название задачи: {tg_message.name}\nОписание задачи: {tg_message.description}\nНазвание таблицы: {tg_message.tableName}\n\nДедлайн через {tg_message.hours} часов!'
                
                await bot.send_message(
                    chat_id=5218298123,
                    text=ms
                )
                return

            if tg_message.days != 0:
                ms = f'Название задачи: {tg_message.name}\nОписание задачи: {tg_message.description}\nНазвание таблицы: {tg_message.tableName}\n\nДедлайн через {tg_message.days} дней!'
                
                await bot.send_message(
                    chat_id=5218298123,
                    text=ms
                )
                return
    
    return handler