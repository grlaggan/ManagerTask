import aiohttp

from dataclasses import asdict
from datetime import datetime
from uuid import UUID

from config.config import Config
from models.tasks.get_tasks_response import GetTasksResponse
from models.tasks.get_task_repsponse import GetTaskResponse
from models.tasks.task_dto import TaskDto
from models.tables.table_dto import TableDto
from models.tasks.create_task_request import CreateTaskRequest
from exceptions.task_already_exists_exception import TaskAlreadyExistsException
from exceptions.table_not_found import TableNotFound
from exceptions.task_invalid_send_time_past import TaskInvalidSendTimePastException
from exceptions.task_not_found import TaskNotFoundException

class TasksService:
    def __init__(self, config: Config):
        self.config = config
    
    async def get_task_by_id(self, id: UUID) -> TaskDto:
        async with aiohttp.ClientSession() as session:
            async with session.get(self.config.tasks_endpoint + f"/{id}") as response:
                if response.status == 404:
                    data = await response.json()
                    status_code = data['errorCode']
                    
                    if status_code == "task.not.found":
                        raise TaskNotFoundException("Task not found")
                
                if response.status != 200:
                    raise ValueError(f"HTTP Error: {response.status} {response.reason}")
                
                data = await response.json()
                table_data = data['table']
                
                table_dto = TableDto(
                    id=table_data['id'],
                    name=table_data['name'],
                    description=table_data['description']
                )
                
                task_dto = TaskDto(
                    id=data['id'],
                    name=data['name'],
                    description=data['description'],
                    created_at=data['createdAt'],
                    table=table_dto,
                    send_time=data['sendTime'],
                    status=data['status']
                )
                
                return task_dto
    
    async def get_tasks(self, chat_id: str, table_name: str = None, page: int = 1, offset: int = 5) -> GetTasksResponse:
        async with aiohttp.ClientSession(headers={"chatId": chat_id}) as session:
            async with session.get(
                self.config.tasks_endpoint + f"?tableName={table_name}&page={page}&offset={offset}" if table_name else self.config.tasks_endpoint + f"?page={page}&offset={offset}") as response:
                if response.status != 200:
                    raise ValueError(f"HTTP Error: {response.status} {response.reason}")
            
                try:
                    data = await response.json()
                    
                    tasks = []
                    for task in data['tasks']:
                        newTableDto = TableDto(
                            id=task['table']['id'],
                            name=task['table']['name'],
                            description=task['table']['description'],
                        )
                        newTaskDto = TaskDto(
                            id=task['id'],
                            name=task['name'],
                            description=task['description'],
                            created_at=datetime.strptime(task['createdAt'], "%Y-%m-%dT%H:%M:%S.%fZ"),
                            table=newTableDto,
                            send_time=datetime.strptime(task['sendTime'], "%Y-%m-%dT%H:%M:%S.%fZ"),
                            status=task['status']
                        )
                        tasks.append(newTaskDto)
                    
                    response = GetTasksResponse(tasks, data['page'], data['offset'], data['countPages'])
                    
                    return response
                except aiohttp.ContentTypeError:
                    raise ValueError("Response is not valid JSON or has wrong Content-Type")
        
    async def get_task_by_name(self, task_name: str) -> GetTaskResponse:
        async with aiohttp.ClientSession() as session:
            async with session.get(self.config.tasks_endpoint + "/" + task_name) as response:
                if response.status != 200:
                    raise ValueError(f"HTTP Error: {response.status} {response.reason}")
                try:
                    data = await response.json()
                    result = GetTaskResponse(
                        data['id'],
                        data['name'],
                        data['description'],
                        data['tableName'],
                        data['status']
                    )
                    return result
                except aiohttp.ContentTypeError:
                    raise ValueError("Response is not valid JSON or has wrong Content-Type")
    
    async def create_task(self, chat_id: str, create_task_request: CreateTaskRequest) -> UUID:
        async with aiohttp.ClientSession(headers={"chatId": chat_id}) as session:
            async with session.post(self.config.tasks_endpoint + "/byTableName", json=asdict(create_task_request)) as response:
                if response.status == 409:
                    data = await response.json()
                    status_code = data['errorCode']
                    
                    if status_code == 'task.already.exists':
                        raise TaskAlreadyExistsException("Table already exists")
                
                if response.status == 404:
                    data = await response.json()
                    status_code = data['errorCode']
                    
                    if status_code == 'table.not.found':
                        raise TableNotFound("Table not found")
                
                if response.status == 400:
                    data = await response.json()
                    status_code = data['errorCode']
                    
                    if status_code == 'task.invalid.sendtime.past':
                        raise TaskInvalidSendTimePastException("Task send time is invalid")
                
                if response.status != 200:
                    raise ValueError(f"HTTP Error: {response.status} {response.reason}")
                
                try:
                    data = await response.json()
                    result = data['id']
                
                    return result
                except aiohttp.ContentTypeError:
                    raise ValueError("Response is not valid JSON or has wrong Content-Type")
    
    async def change_status(self, task_id: UUID):
        async with aiohttp.ClientSession() as session:
            async with session.patch(self.config.tasks_endpoint + f"/{task_id}/status") as response:
                if response.status != 200:
                    raise ValueError(f"HTTP Error: {response.status} {response.reason}")
    
    async def change_status_on_failed(self, task_id: UUID):
        async with aiohttp.ClientSession() as session:
            async with session.patch(self.config.tasks_endpoint + f"/{task_id}/status/failed") as response:
                if response.status != 200:
                    raise ValueError(f"HTTP Error: {response.status} {response.reason}")