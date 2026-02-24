from dataclasses import dataclass

from models.tasks.task_dto import TaskDto

@dataclass
class GetTasksResponse:
    tasks: list[TaskDto]
    page: int
    offset: int
    count_pages: int