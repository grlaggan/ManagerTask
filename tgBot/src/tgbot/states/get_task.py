from aiogram.fsm.state import StatesGroup, State

class GetTaskState(StatesGroup):
    task_name = State()