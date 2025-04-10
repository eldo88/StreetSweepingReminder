--Creates Reminders Table

CREATE TABLE IF NOT EXISTS main.Reminders (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    Message TEXT NOT NULL,
    ScheduledDateTimeUtc TEXT NOT NULL,
    StreetSweepingDate TEXT NOT NULL, 
    Status TEXT NOT NULL,
    PhoneNumber TEXT NOT NULL,
    StreetId INTEGER NOT NULL, 
    CreatedAt TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%fZ', 'now')),
    ModifiedAt TEXT,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id),
    FOREIGN KEY (StreetId) REFERENCES Streets (Id)
)