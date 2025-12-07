-- Create database
CREATE DATABASE IF NOT EXISTS RobotoCalendarScheduler;
USE RobotoCalendarScheduler;

-- Create role
CREATE ROLE IF NOT EXISTS 'roboto_role';

-- Grant privileges to the role
GRANT ALL PRIVILEGES ON RobotoCalendarScheduler.* TO 'roboto_role';

-- Create user
CREATE USER IF NOT EXISTS 'roboto'@'%' IDENTIFIED BY 'change_this_password';

-- Grant role to user
GRANT 'roboto_role' TO 'roboto'@'%';

-- NOW set default role (requires previous GRANT)
ALTER USER 'roboto'@'%' DEFAULT ROLE 'roboto_role';

FLUSH PRIVILEGES;

-- Create tables
CREATE TABLE IF NOT EXISTS Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL UNIQUE,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Salt VARCHAR(255) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE UNIQUE INDEX idx_username ON Users (Username);
CREATE UNIQUE INDEX idx_email ON Users (Email);

CREATE TABLE IF NOT EXISTS CalendarEvents (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT,
    Title VARCHAR(100) NOT NULL,
    StartDateTime DATETIME NOT NULL,
    Duration DOUBLE NOT NULL,
    IsAllDay BOOLEAN NOT NULL,
    BlockCalendar BOOLEAN NOT NULL,
    Details TEXT,
    EndDateTime DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
