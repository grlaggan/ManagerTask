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
        KeyboardButton(text="Показать задачи"),
        KeyboardButton(text='Показать задачу')]
    ],
    resize_keyboard=True
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

operations_with_task = ReplyKeyboardMarkup(
    keyboard=[
        [KeyboardButton(text="Изменить состояние")],
        [KeyboardButton(text="Отменить")]
    ],
    resize_keyboard=True
)