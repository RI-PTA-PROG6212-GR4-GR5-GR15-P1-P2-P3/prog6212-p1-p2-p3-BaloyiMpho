-- RaceDay SQL Server Schema + Seed Data
IF DB_ID('RaceDay') IS NULL CREATE DATABASE RaceDay;
GO
USE RaceDay;
GO

IF OBJECT_ID('Results') IS NOT NULL DROP TABLE Results;
IF OBJECT_ID('Enrolments') IS NOT NULL DROP TABLE Enrolments;
IF OBJECT_ID('Categories') IS NOT NULL DROP TABLE Categories;
IF OBJECT_ID('Events') IS NOT NULL DROP TABLE Events;
IF OBJECT_ID('Users') IS NOT NULL DROP TABLE Users;
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    ProfilePictureUrl NVARCHAR(500) NULL
);

CREATE TABLE Events (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Date DATETIME2 NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    Distance FLOAT NOT NULL,
    EventType NVARCHAR(20) NOT NULL CHECK (EventType IN ('Run', 'Walk', 'Cycle')),
    BannerImageUrl NVARCHAR(500) NULL,
    OrganiserId INT NOT NULL,
    CONSTRAINT FK_Events_Users FOREIGN KEY (OrganiserId) REFERENCES Users(Id)
);

CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    EventId INT NOT NULL,
    CONSTRAINT FK_Categories_Events FOREIGN KEY (EventId) REFERENCES Events(Id) ON DELETE CASCADE
);

CREATE TABLE Enrolments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantId INT NOT NULL,
    EventId INT NOT NULL,
    CategoryId INT NOT NULL,
    EnrolmentDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Confirmed')),
    CONSTRAINT FK_Enrolments_Users FOREIGN KEY (ParticipantId) REFERENCES Users(Id),
    CONSTRAINT FK_Enrolments_Events FOREIGN KEY (EventId) REFERENCES Events(Id),
    CONSTRAINT FK_Enrolments_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    CONSTRAINT UQ_Enrolment UNIQUE (ParticipantId, EventId, CategoryId)
);

CREATE TABLE Results (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    ParticipantId INT NOT NULL,
    FinishTime TIME NOT NULL,
    FinishingPosition INT NOT NULL,
    CONSTRAINT FK_Results_Events FOREIGN KEY (EventId) REFERENCES Events(Id),
    CONSTRAINT FK_Results_Users FOREIGN KEY (ParticipantId) REFERENCES Users(Id),
    CONSTRAINT UQ_Result UNIQUE (EventId, ParticipantId)
);
GO

-- Seed Users (BCrypt hash for "Password123!")
DECLARE @hash NVARCHAR(255) = '$2a$11$8Oq0QZj2d7L7r1J2YvQ0w.dHt5yv9zEq3HhG6eGh5xT5T5T5T5T5T';

INSERT INTO Users (Name, Email, PasswordHash, Role) VALUES
('Alice Organiser', 'alice@raceday.com', @hash, 'Organiser'),
('Bob Organiser',   'bob@raceday.com',   @hash, 'Organiser'),
('Charlie Participant','charlie@raceday.com', @hash, 'Participant'),
('Diana Participant','diana@raceday.com', @hash, 'Participant');

INSERT INTO Events (Title, Description, Date, Location, Distance, EventType, OrganiserId) VALUES
('Cape Town City Marathon', 'Annual city marathon through the streets of Cape Town.', '2025-09-15', 'Cape Town', 42.2, 'Run', 1),
('Durban Trail Run', 'Scenic trail run along the Durban coastline.', '2025-10-05', 'Durban', 21.0, 'Run', 2),
('Joburg Cycle Challenge', 'Exciting cycle race through Johannesburg.', '2025-11-20', 'Johannesburg', 50.0, 'Cycle', 1);

INSERT INTO Categories (Name, EventId) VALUES
('Under 20', 1), ('Senior', 1), ('10km', 1), ('21km', 1),
('Under 20', 2), ('Senior', 2), ('10km', 2), ('21km', 2),
('Under 20', 3), ('Senior', 3), ('10km', 3), ('50km', 3);

INSERT INTO Enrolments (ParticipantId, EventId, CategoryId, Status) VALUES
(3, 1, 2, 'Confirmed'),
(4, 2, 5, 'Pending'),
(3, 3, 11, 'Confirmed');

INSERT INTO Results (EventId, ParticipantId, FinishTime, FinishingPosition) VALUES
(1, 3, '03:45:00', 1),
(3, 3, '02:15:00', 2);
GO

PRINT 'RaceDay database created and seeded.';
