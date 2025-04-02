--Creates Reminders Table
--TODO add FK constraint to StreetId
CREATE TABLE IF NOT EXISTS main.Reminders (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    Message TEXT NOT NULL,
    ScheduledDateTime TEXT NOT NULL,
    Status TEXT NOT NULL,
    PhoneNumber TEXT NOT NULL,
    StreetId INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%fZ', 'now')),
    ModifiedAt TEXT,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id)
)