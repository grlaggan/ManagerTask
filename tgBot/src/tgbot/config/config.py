from dataclasses import dataclass
import environs

FORMAT_LOGGER = "%(asctime)s - %(name)s - %(levelname)s - %(message)s"

@dataclass
class TgBot:
    token: str

class Config:
    _env = environs.Env()
    _tg_bot = TgBot
    
    def __init__(self, path_env: str):
        self._env.read_env(path_env)
        self.tg_bot = self._tg_bot(token=self._env("BOT_TOKEN"))
        self.tasks_endpoint = self._env("TASKS_ENDPOINT")
        self.tables_endpoint = self._env("TABLES_ENDPOINT")