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

class TasksService:
    def __init__(self, config: Config):
        self.config = config
    
    async def get_tasks(self, table_name: str = None) -> GetTasksResponse:
        async with aiohttp.ClientSession() as session:
            async with session.get(self.config.tasks_endpoint + f"?tableName={table_name}" if table_name else self.config.tasks_endpoint) as response:
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
                    
                    return tasks
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
    
    async def create_task(self, create_task_request: CreateTaskRequest) -> UUID:
        async with aiohttp.ClientSession() as session:
            async with session.post(self.config.tasks_endpoint + "/byTableName", json=asdict(create_task_request)) as response:
                
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