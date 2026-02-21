import aiohttp

from config.config import Config

class HealthCheckService:
    def __init__(self, config: Config):
        self.config = config
    
    async def check_health_api(self) -> bool:
        async with aiohttp.ClientSession() as session:
            async with session.get(self.config.base_endpoint + "/health") as response:
                return response.status == 200