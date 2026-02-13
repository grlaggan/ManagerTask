from aiogram.fsm.state import StatesGroup, State

class Form(StatesGroup):
    name = State()
    description = State()
    table_name = State()
    send_time = State()
    send_time_minutes = State()
    send_time_hours = State()
    send_time_days = State()
    