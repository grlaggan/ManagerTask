import aiohttp

from uuid import UUID
from dataclasses import asdict

from config.config import Config
from exceptions.user_not_found import UserNotFoundException
from exceptions.user_already_exists import UserAlreadyExistsException
from models.users.user_dto import UserDto
from models.users.create_user_request import CreateUserRequest

class UsersService:
    def __init__(self, config: Config):
        self.config = config
    
    async def get_user_by_chat_id(self, chat_id: int):
        async with aiohttp.ClientSession() as session:
            async with session.get(self.config.users_endpoint + f"/{chat_id}") as response:
                if response.status == 404:
                    data = await response.json()
                    error_code = data['errorCode']
                    if error_code == 'user.not.found':
                        raise UserNotFoundException("User was not found")

                if response.status != 200:
                    raise ValueError(f"HTTP Error: {response.status} {response.reason}")
                
                data = await response.json()
                
                user_dto = UserDto(
                    id=data['id'],
                    chat_id=int(data['chatId'])
                )
                
                return user_dto

    async def create_user(self, request: CreateUserRequest) -> UUID:
        async with aiohttp.ClientSession() as session:
            async with session.post(self.config.users_endpoint, json=asdict(request)) as response:
                if response.status == 409:
                    data = await response.json()
                    error_code = data['errorCode']
                    
                    if error_code == "user.already.exists":
                        raise UserAlreadyExistsException("User already exists")
                
                if response.status != 200:
                    raise ValueError(f"HTTP Error: {response.status} {response.reason}")
                
                data = await response.json()
                user_id = data['id']
                
                return UUID(user_id)