from aiogram.types import ReplyKeyboardMarkup, KeyboardButton

start_keyboard = ReplyKeyboardMarkup(
    keyboard=[
        [KeyboardButton(text="Задачи"),
        KeyboardButton(text="Таблицы")]
    ]
)

tasks_keyboard = ReplyKeyboardMarkup(
    keyboard=[
        [KeyboardButton(text="Добавить задачу"),
        KeyboardButton(text="Показать задачи")]
    ]
)

tables_keyboard = ReplyKeyboardMarkup(
    keyboard=[
        [KeyboardButton(text="Показать таблицы"),
        KeyboardButton(text="Добавить таблицу")]
    ]
)

times_keyboard = ReplyKeyboardMarkup(
    keyboard=[
        [KeyboardButton(text="Минуты"),
        KeyboardButton(text="Часы"),
        KeyboardButton(text="Дни")]
    ],
    resize_keyboard=True
)