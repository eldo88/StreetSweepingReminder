--Creates the ReminderSchedule Table


CREATE TABLE IF NOT EXISTS main.RemindersSchedule (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Message TEXT NOT NULL,
    DayOfWeek INTEGER NOT NULL,           -- 0 = Sunday, 1 = Monday, ..., 6 = Saturday
    WeekOfMonth INTEGER NOT NULL,         -- 1 = First, 2 = Second, ..., -1 = Last
    StartMonth INTEGER NOT NULL,          -- 4 for April
    EndMonth INTEGER NOT NULL,            -- 11 for November
    TimeOfDay TEXT NOT NULL,              -- '07:00'
    TimeZone TEXT NOT NULL,               -- 'America/Denver'
    IsRecurring INTEGER,                  -- No boolean type in SQLite 
    CreatedAt TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%fZ', 'now')),
    ModifiedAt TEXT
);