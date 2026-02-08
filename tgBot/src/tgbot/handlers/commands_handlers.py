from aiogram import Router
from aiogram.filters import CommandStart
from aiogram.types import Message

from keyboards.keyboards import start_keyboard

router = Router()

@router.message(CommandStart())
async def process_start_command(message: Message) -> None:
    await message.answer(
        "Привет! Я бот для управления задачами. Используй команды для взаимодействия со мной.",
        reply_markup=start_keyboard)