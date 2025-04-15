--Creates the street sweeping dates schedule by street

CREATE TABLE IF NOT EXISTS main.StreetSweepingDates (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    StreetId INTEGER NOT NULL,
    StreetSweepingDate TEXT NOT NULL,
    CreatedAt TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%fZ', 'now')),
    ModifiedAt TEXT,
    FOREIGN KEY (StreetId) REFERENCES Streets(ID)
);