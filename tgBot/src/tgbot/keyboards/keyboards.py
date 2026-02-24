from aiogram.types import ReplyKeyboardMarkup, KeyboardButton

start_keyboard = ReplyKeyboardMarkup(
    keyboard=[
        [KeyboardButton(text="Задачи"),
        KeyboardButton(text="Таблицы"),
        KeyboardButton(text="Уведомления")]
    ],
    resize_keyboard=True
)

tasks_keyboard = ReplyKeyboardMarkup(
    keyboard=[
        [KeyboardButton(text="Добавить задачу"),
        KeyboardButton(text="Показать задачи"),
        KeyboardButton(text='Показать задачу')],
        [KeyboardButton(text="Показать задачи по названию таблицы")]
    ],
    resize_keyboard=True
)

tables_keyboard = ReplyKeyboardMarkup(
    keyboard=[
        [KeyboardButton(text="Показать таблицы"),
        KeyboardButton(text="Добавить таблицу")]
    ],
    resize_keyboard=True
)

times_keyboard = ReplyKeyboardMarkup(
    keyboard=[
        [KeyboardButton(text="Минуты"),
        KeyboardButton(text="Часы"),
        KeyboardButton(text="Дни")]
    ],
    resize_keyboard=True
)

operations_with_task = ReplyKeyboardMarkup(
    keyboard=[
        [KeyboardButton(text="Изменить состояние")],
        [KeyboardButton(text="Отменить")]
    ],
    resize_keyboard=True
)

notifications_keyboard = ReplyKeyboardMarkup(
    keyboard=[
        [KeyboardButton(text="Получить уведомления"),
        KeyboardButton(text="Создать уведомление")]
    ],
    resize_keyboard=True
)