from aiogram.fsm.state import StatesGroup, State

class GetTasksByTableNameState(StatesGroup):
    table_name = State()