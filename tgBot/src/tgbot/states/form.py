from aiogram.fsm.state import StatesGroup, State

class Form(StatesGroup):
    name = State()
    description = State()
    table_name = State()
    send_time = State()