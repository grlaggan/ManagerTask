import asyncio
import logging
import aio_pika

from aiogram import Bot, Dispatcher
from aiogram.client.default import DefaultBotProperties
from aiogram.enums import ParseMode

from config.config import FORMAT_LOGGER, Config

from handlers import commands_handlers, tables_handler, tasks_handler, rabbit_handler, notifications_handler

logger = logging.getLogger(__name__)

async def main():
    logging.basicConfig(
        level=logging.INFO,
        format=FORMAT_LOGGER
    )
    
    config = Config(path_env="./.env.tgBot")
    
    bot = Bot(
        token=config.tg_bot.token,
        default=DefaultBotProperties(parse_mode=ParseMode.HTML)
    )
    dp = Dispatcher()
    handler = rabbit_handler.create_handler(bot)
    connection = await aio_pika.connect(config.rabbitmq_url)
    channel = await connection.channel()
    queue = await channel.get_queue(config.queue_name)
    
    await queue.consume(handler)
    
    logger.info("Starting bot")
    
    dp.include_routers(commands_handlers.router, tasks_handler.router, tables_handler.router, notifications_handler.router)
    await bot.delete_webhook(drop_pending_updates=True)
    await dp.start_polling(bot)

if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        logger.info("Bot stopped")    