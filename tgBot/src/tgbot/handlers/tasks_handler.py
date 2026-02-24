from aiogram import Router, F
from aiogram.filters import Command
from aiogram.types import Message, CallbackQuery
from aiogram.fsm.context import FSMContext
from aiogram.utils.keyboard import InlineKeyboardBuilder
from datetime import datetime, timedelta, timezone
from uuid import UUID

from keyboards.keyboards import tasks_keyboard, times_keyboard, operations_with_task
from config.config import Config
from services.tasks_service import TasksService
from services.health_check_service import HealthCheckService
from models.tasks.task_dto import TaskDto
from models.tasks.create_task_request import CreateTaskRequest
from models.tasks.get_task_repsponse import GetTaskResponse
from models.tasks.get_tasks_response import GetTasksResponse
from states.form import Form
from states.get_task import GetTaskState
from states.get_tasks_by_table_name import GetTasksByTableNameState
from exceptions.task_already_exists_exception import TaskAlreadyExistsException
from exceptions.table_not_found import TableNotFound
from exceptions.task_invalid_send_time_past import TaskInvalidSendTimePastException
from exceptions.task_not_found import TaskNotFoundException

router = Router()
config = Config("./.env.tgBot")
tasks_service = TasksService(config)
health_check_service = HealthCheckService(config)

@router.message(F.text == "Задачи" or Command('tasks'))
async def process_tasks_command(message: Message) -> None:
    await message.answer("Вы выбрали раздел 'Задачи'. Что бы вы хотели сделать?", reply_markup=tasks_keyboard)

@router.callback_query(F.data.startswith("task_"))
async def get_task_command(callback: CallbackQuery) -> None:
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await callback.message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    task_id = callback.data.split('_')[1]
    try:
        response: TaskDto = await tasks_service.get_task_by_id(UUID(task_id))
        
        match response.status:
            case 0:
                status = 'Ожидание'
            case 1:
                status = 'Выполнено'
            case 2:
                status = 'Провалено'
        
        answer = f"Название: {response.name}\nОписание: {response.description}\nНазвание таблицы: {response.table.name}\nСтатус: {status}"
        
        return callback.message.answer(answer)
    except TaskNotFoundException:
        await callback.message.answer("Задача не найдена!")
        return
    except:
        await callback.message.answer("Произошла ошибка! Попробуйте еще раз.")
        return

@router.message(F.text == "Показать задачи")
async def get_tasks_command(message: Message) -> None:
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    builder = InlineKeyboardBuilder()
    response: GetTasksResponse = await tasks_service.get_tasks(chat_id=str(message.chat.id))
    
    if len(response.tasks) == 0:
        await message.answer("Список пуст")
        return
    
    for task in response.tasks:
        builder.button(text=f"Name: {task.name}", callback_data=f"task_{task.id}")
    
    builder.adjust(1)
    
    if response.count_pages > 0:
        another_builder = InlineKeyboardBuilder()
        
        another_builder.button(text="1", callback_data="empty")
        
        if response.count_pages > 1:
            another_builder.button(text=">>", callback_data=f"next_tasks_page:{2}")
            
        builder.attach(another_builder)
    
    await message.answer("Here's list of notifications", reply_markup=builder.as_markup())

@router.callback_query(F.data.startswith("next_tasks_page:"))
async def get_tasks_next_page_command(callback: CallbackQuery) -> None:
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await callback.message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    count_page = int(callback.data.split(':')[1])
    table_name = callback.data.split(':')[2]
    
    response: GetTasksResponse = await tasks_service.get_tasks(chat_id=str(callback.chat.id) ,page=count_page, table_name=table_name or None)
    builder = InlineKeyboardBuilder()
    for task in response.tasks:
        builder.button(text=f"Name: {task.name}", callback_data=f"task_{task.id}")
        
    another_builder = InlineKeyboardBuilder()
    
    if count_page > 1:
        another_builder.button(text="<<", callback_data=f"previous_tasks_page:{count_page - 1}")
    
    another_builder.button(text=f"{count_page}", callback_data="empty")
    
    builder.adjust(1)
    
    if count_page <  response.count_pages:
        another_builder.button(text=">>", callback_data=f"next_tasks_page:{count_page + 1}")
    
    builder.attach(another_builder)
    
    await callback.message.edit_reply_markup(reply_markup=builder.as_markup())

@router.callback_query(F.data.startswith("previous_tasks_page:"))
async def get_tasks_previous_page_command(callback: CallbackQuery) -> None:
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await callback.message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    count_page = int(callback.data.split(':')[1])
    table_name = callback.data.split(':')[2]
    
    response: GetTasksResponse = await tasks_service.get_tasks(chat_id=str(callback.chat.id) ,page=count_page, table_name=table_name or None)
    builder = InlineKeyboardBuilder()
    for task in response.tasks:
        builder.button(text=f"Name: {task.name}", callback_data=f"task_{task.id}")
        
    another_builder = InlineKeyboardBuilder()
    
    if count_page > 1:
        another_builder.button(text="<<", callback_data=f"previous_tasks_page:{count_page - 1}")
    
    another_builder.button(text=f"{count_page}", callback_data="empty")
    
    builder.adjust(1)
    
    if count_page <  response.count_pages:
        another_builder.button(text=">>", callback_data=f"next_tasks_page:{count_page + 1}")
    
    builder.attach(another_builder)
    
    await callback.message.edit_reply_markup(reply_markup=builder.as_markup())

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
    try:
        int(message.text)
    except:
        await message.answer("Значение должно быть натуральным числом! Отправьте снова.")
        return
    
    if int(message.text) < 0:
        await message.answer("Значение должно быть положительным числом! Отправьте снова.")
        return
    
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    state_data = await state.get_data()
    
    utc_now = datetime.now(timezone.utc)
    
    send_time = utc_now + timedelta(minutes=int(message.text))
    
    create_task_request = CreateTaskRequest(state_data['name'], state_data['description'], state_data['table_name'], send_time.strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z')
    
    (id, error) = await create_task(chat_id=str(message.chat.id), request=create_task_request)
    
    if error:
        await message.answer(error)
        return
    
    await state.clear()
    
    if id:
        await message.answer("Задача успешно создана!")
        return
    else:
        await message.answer("Произошла ошибка! Попробуйте снова!")

@router.message(Form.send_time_hours)
async def create_task_command_send_time_hours_complete(message: Message, state: FSMContext) -> None:
    try:
        int(message.text)
    except:
        await message.answer("Значение должно быть натуральным числом! Отправьте снова.")
        return
    
    if int(message.text) < 0:
        await message.answer("Значение должно быть положительным числом! Отправьте снова.")
        return
    
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    state_data = await state.get_data()
    
    utc_now = datetime.now(timezone.utc)
    
    send_time = utc_now + timedelta(hours=int(message.text))
    
    create_task_request = CreateTaskRequest(state_data['name'], state_data['description'], state_data['table_name'], send_time.strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z')
    
    (id, error) = await create_task(chat_id=str(message.chat.id), request=create_task_request)
    
    if error:
        await message.answer(error)
        return
    
    await state.clear()
    
    if id:
        await message.answer("Задача успешно создана!")
        return
    else:
        await message.answer("Произошла ошибка! Попробуйте снова!")

@router.message(Form.send_time_days)
async def create_task_command_send_time_days_compelte(message: Message, state: FSMContext) -> None:
    try:
        int(message.text)
    except:
        await message.answer("Значение должно быть натуральным числом! Отправьте снова.")
        return
    
    if int(message.text) < 0:
        await message.answer("Значение должно быть положительным числом! Отправьте снова.")
        return
    
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    state_data = await state.get_data()
    
    utc_now = datetime.now(timezone.utc)
    
    send_time = utc_now + timedelta(days=int(message.text))
    
    create_task_request = CreateTaskRequest(state_data['name'], state_data['description'], state_data['table_name'], send_time.strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z')
    
    (id, error) = await create_task(chat_id=str(message.chat.id), request=create_task_request)
    
    if error:
        await message.answer(error)
        return
    
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
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
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
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
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

@router.message(F.text == "Показать задачи по названию таблицы")
async def get_tasks_by_table_name_command(message: Message, state: FSMContext) -> None:
    await state.set_state(GetTasksByTableNameState.table_name)
    await message.answer("Отправьте название таблицы")

@router.message(GetTasksByTableNameState.table_name)
async def get_tasks_by_table_name_command_process(message: Message, state: FSMContext) -> None:
    health_check_result = await health_check_service.check_health_api()
    
    if not health_check_result:
        await message.answer(text="Возникла ошибка на стороне сервера! Попробуйте позже.")
        return
    
    builder = InlineKeyboardBuilder()
    response: GetTasksResponse = await tasks_service.get_tasks(chat_id=str(message.chat.id) ,table_name=message.text)
    
    if len(response.tasks) == 0:
        await message.answer("Список пуст")
        await state.clear()
        return
    
    for task in response.tasks:
        builder.button(text=f"Name: {task.name}", callback_data=f"task_{task.id}")
    
    builder.adjust(1)
    
    if response.count_pages > 0:
        another_builder = InlineKeyboardBuilder()
        
        another_builder.button(text="1", callback_data="empty")
        
        if response.count_pages > 1:
            another_builder.button(text=">>", callback_data=f"next_tasks_page:{2}:{message.text}")
            
        builder.attach(another_builder)
    
    await state.clear()
    await message.answer("Here's list of notifications", reply_markup=builder.as_markup())

async def create_task(chat_id: str ,request: CreateTaskRequest) -> tuple[UUID, str]:
    try:
        id = await tasks_service.create_task(chat_id=chat_id, create_task_request=request)
        return (id, None)
    except TaskAlreadyExistsException:
        return (None, "Задача с таким именем уже существует!")
    except TableNotFound:
        return (None, "Таблицы с таким именем не существует!")
    except TaskInvalidSendTimePastException:
        return (None, "Время упоминания не валидное!")
