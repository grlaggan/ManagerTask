from dataclasses import dataclass

from models.tasks.task_dto import TaskDto

@dataclass
class GetTasksResponse:
    data: list[TaskDto]