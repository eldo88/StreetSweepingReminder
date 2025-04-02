--Creates the Streets Table 

CREATE TABLE IF NOT EXISTS main.Streets (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    StreetName TEXT NOT NULL,
    CreatedAt TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%fZ', 'now')),
    ModifiedAt TEXT,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id)
)