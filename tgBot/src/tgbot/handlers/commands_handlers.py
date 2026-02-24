from aiogram import Router
from aiogram.filters import CommandStart, Command
from aiogram.types import Message

from keyboards.keyboards import start_keyboard
from config.config import Config
from services.users_service import UsersService
from services.health_check_service import HealthCheckService
from exceptions.user_not_found import UserNotFoundException
from models.users.create_user_request import CreateUserRequest

router = Router()

config = Config("./.env.tgBot")
users_service = UsersService(config)
health_check_service = HealthCheckService(config)

@router.message(CommandStart())
async def process_start_command(message: Message) -> None:
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    try:
        await users_service.get_user_by_chat_id(message.chat.id)
    except UserNotFoundException:
        try:
            id = await users_service.create_user(CreateUserRequest(str(message.chat.id)))
            await message.answer(
                "Привет! Я бот для управления задачами. Используй команды для взаимодействия со мной.",
                reply_markup=start_keyboard)
            return
        except:
            await message.answer("Что-то пошло не так, попробуйте позже!")
            return
    except:
        await message.answer("Что-то пошло не так, попробуйте позже!")
        return
    
    await message.answer(
        "Привет! Я бот для управления задачами. Используй команды для взаимодействия со мной.",
        reply_markup=start_keyboard)