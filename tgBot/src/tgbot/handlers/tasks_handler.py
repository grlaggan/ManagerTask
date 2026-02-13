from aiogram import Router, F
from aiogram.filters import Command
from aiogram.types import Message
from aiogram.fsm.context import FSMContext
from datetime import datetime, timedelta, timezone

from keyboards.keyboards import tasks_keyboard, times_keyboard, operations_with_task
from config.config import Config
from services.tasks_service import TasksService
from models.tasks.task_dto import TaskDto
from models.tasks.create_task_request import CreateTaskRequest
from models.tasks.get_task_repsponse import GetTaskResponse
from states.form import Form
from states.get_task import GetTaskState

router = Router()
config = Config("./.env")
tasks_service = TasksService(config)

@router.message(F.text == "Задачи" or Command('tasks'))
async def process_tasks_command(message: Message) -> None:
    await message.answer("Вы выбрали раздел 'Задачи'. Что бы вы хотели сделать?", reply_markup=tasks_keyboard)

@router.message(F.text == "Показать задачи")
async def get_tasks_command(message: Message) -> None:
    data = await tasks_service.get_tasks()
    answer = ''
    
    task: TaskDto
    for task in data:
        answer += f'Название: {task.name}\
                    \nОписание: {task.description}\
                    \nСоздана: {str(task.created_at).split('.')[0]}\
                    \nТаблица: {task.table.name}\
                    \n Время выполнения: {str(task.send_time).split('.')[0]}\n\n'
    
    await message.answer(answer)

@router.message(F.text == "Добавить задачу")
async def create_task_command(message: Message, state: FSMContext) -> None:
    await state.set_state(Form.name)
    await message.answer("Отправьте название задачи.")

@router.message(Form.name)
async def create_task_command_name(message: Message, state: FSMContext) -> None:
    await state.update_data(name=message.text)
    await state.set_state(Form.description)
    await message.answer("Отправьте описание задачи.")

@router.message(Form.description)
async def create_task_command_description(message: Message, state: FSMContext) -> None:
    await state.update_data(description=message.text)
    await state.set_state(Form.table_name)
    await message.answer("Отправьте название таблицы.")

@router.message(Form.table_name)
async def create_task_command_table_name(message: Message, state: FSMContext) -> None:
    await state.update_data(table_name=message.text)
    await state.set_state(Form.send_time)
    # await message.answer("Через сколько часов должно придти упоминание?")
    await message.answer("Через сколько должно придти упоминание?(Выберите, что именно вы желаете добавить ко времени упоминания)", reply_markup=times_keyboard)

@router.message(Form.send_time, F.text == "Минуты")
async def create_task_command_time_minutes(message: Message, state: FSMContext) -> None:
    await state.set_state(Form.send_time_minutes)
    await message.answer("Через сколько минут?")

@router.message(Form.send_time, F.text == "Часы")
async def create_task_command_time_hours(message: Message, state: FSMContext) -> None:
    await state.set_state(Form.send_time_minutes)
    await message.answer("Через сколько часов?")

@router.message(Form.send_time, F.text == "Дни")
async def create_task_command_time_days(message: Message, state: FSMContext) -> None:
    await state.set_state(Form.send_time_minutes)
    await message.answer("Через сколько дней?")

@router.message(Form.send_time_minutes)
async def create_task_command_send_time_minutes_complete(message: Message, state: FSMContext) -> None:
    state_data = await state.get_data()
    
    utc_now = datetime.now(timezone.utc)
    
    send_time = utc_now + timedelta(minutes=int(message.text))
    
    create_task_request = CreateTaskRequest(state_data['name'], state_data['description'], state_data['table_name'], send_time.strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z')
    
    id = await tasks_service.create_task(create_task_request)
    
    if id:
        await message.answer("Задача успешно создана!")
        return
    else:
        await message.answer("Произошла ошибка! Попробуйте снова!")

@router.message(Form.send_time_hours)
async def create_task_command_send_time_hours_complete(message: Message, state: FSMContext) -> None:
    state_data = await state.get_data()
    
    utc_now = datetime.now(timezone.utc)
    
    send_time = utc_now + timedelta(hours=int(message.text))
    
    create_task_request = CreateTaskRequest(state_data['name'], state_data['description'], state_data['table_name'], send_time.strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z')
    
    id = await tasks_service.create_task(create_task_request)
    
    if id:
        await message.answer("Задача успешно создана!")
        return
    else:
        await message.answer("Произошла ошибка! Попробуйте снова!")

@router.message(Form.send_time_days)
async def create_task_command_send_time_days_compelte(message: Message, state: FSMContext) -> None:
    state_data = await state.get_data()
    
    utc_now = datetime.now(timezone.utc)
    
    send_time = utc_now + timedelta(days=int(message.text))
    
    create_task_request = CreateTaskRequest(state_data['name'], state_data['description'], state_data['table_name'], send_time.strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z')
    
    id = await tasks_service.create_task(create_task_request)
    
    await state.clear()
        
    if id:
        await message.answer("Задача успешно создана!")
        return
    else:
        await message.answer("Произошла ошибка! Попробуйте снова!")

@router.message(F.text == 'Показать задачу')
async def get_task_by_name_command(message: Message, state: FSMContext) -> None:
    await state.set_state(GetTaskState.task_name)
    await message.answer('Отправьте название задачи.')
    
@router.message(GetTaskState.task_name, F.text == "Изменить состояние")
async def change_status_task_command(message: Message, state: FSMContext) -> None:
    try:
        state_data = await state.get_data()
        await tasks_service.change_status(state_data['id'])
        await message.answer("Состояние изменено!")
        await state.clear()
    except:
        await message.answer('Произошла ошибка! Попробуйте еще раз!')
        return

@router.message(GetTaskState.task_name, F.text == "Отменить")
async def cancel_change_task_command(message: Message, state: FSMContext) -> None:
    await state.clear()
    await message.answer('Отмена изменения задачи')
    
@router.message(GetTaskState.task_name)
async def get_task_by_name_command_process(message: Message, state: FSMContext) -> None:
    try:
        response: GetTaskResponse = await tasks_service.get_task_by_name(message.text)
        
        task_state = ''
        match response.status:
            case 0:
                task_state = 'Ожидание'
            case 1:
                task_state = 'Выполнено'
            case 2:
                task_state = 'Провалено'
        
        answer = f'Название: {response.name}\nОписание: {response.description}\nНазвание таблицы: {response.table_name}\nСостояние задачи: {task_state}'
        await state.update_data(id=response.id)
        await message.answer(answer)
        await message.answer("Выберите дальнейшую операцию", reply_markup=operations_with_task)
    except:
        await message.answer('Произошла ошибка! Попробуйте еще раз!')
        return


