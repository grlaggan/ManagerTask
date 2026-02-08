from aiogram import Router, F
from aiogram.types import Message

from keyboards.keyboards import tables_keyboard

router = Router()

@router.message(F.text == "Таблицы")
async def process_tables_command(message: Message) -> None:
    await message.answer("Вы выбрали раздел 'Таблицы'. Что бы вы хотели сделать?", reply_markup=tables_keyboard)