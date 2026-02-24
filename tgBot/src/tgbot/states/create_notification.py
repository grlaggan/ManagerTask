from aiogram.fsm.state import StatesGroup, State

class CreateNotificationState(StatesGroup):
    name = State()
    message = State()
    notification_time = State()
    minutes = State()
    hours = State()
    days = State()