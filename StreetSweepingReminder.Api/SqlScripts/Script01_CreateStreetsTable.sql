--Creates the Streets Table 

CREATE TABLE IF NOT EXISTS main.Streets (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    StreetName TEXT NOT NULL,
    ZipCode TEXT NOT NULL, 
    CreatedAt TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%fZ', 'now'))
)