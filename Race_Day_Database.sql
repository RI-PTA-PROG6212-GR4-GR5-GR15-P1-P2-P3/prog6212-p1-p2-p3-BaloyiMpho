--  Creating the database

CREATE DATABASE Race_Day;

Use Race_Day;

--1. User Table
CREATE TABLE [USER] (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    date_of_birth DATE,
    phone VARCHAR(20),
    created_at DATETIME DEFAULT GETDATE(),
    last_login DATETIME,
    is_active BIT DEFAULT 1
);
GO
    --  Creating the database

CREATE DATABASE Race_Day;

Use Race_Day;

--1. User Table
CREATE TABLE [USER] (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    date_of_birth DATE,
    phone VARCHAR(20),
    created_at DATETIME DEFAULT GETDATE(),
    last_login DATETIME,
    is_active BIT DEFAULT 1
);
GO
--2     Role Table
CREATE TABLE ROLE (
    role_id INT IDENTITY(1,1) PRIMARY KEY,
    role_name VARCHAR(30) UNIQUE NOT NULL,  
    description VARCHAR(200),
    created_at DATETIME DEFAULT GETDATE()
);
GO
-- 3. USERROLE Table (Junction Table)

CREATE TABLE USERROLE (
    userrole_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    role_id INT NOT NULL,
    assigned_at DATETIME DEFAULT GETDATE(),
    assigned_by INT 
);
GO

-- Add Foreign Keys for USERROLE
ALTER TABLE USERROLE ADD CONSTRAINT FK_UserRole_User 
    FOREIGN KEY (user_id) REFERENCES [USER](user_id);
GO

ALTER TABLE USERROLE ADD CONSTRAINT FK_UserRole_Role 
    FOREIGN KEY (role_id) REFERENCES ROLE(role_id);
GO

ALTER TABLE USERROLE ADD CONSTRAINT FK_UserRole_AssignedBy 
    FOREIGN KEY (assigned_by) REFERENCES [USER](user_id);
GO
-- 4. RACE Table
CREATE TABLE RACE (
    race_id INT IDENTITY(1,1) PRIMARY KEY,
    race_name VARCHAR(100) NOT NULL,
    race_date DATE NOT NULL,
    race_time TIME NOT NULL,
    venue VARCHAR(100) NOT NULL,
    distance_meters INT NOT NULL,
    race_type VARCHAR(50) NOT NULL,
    prize_pool DECIMAL(12,2),
    status VARCHAR(20) DEFAULT 'Scheduled',  
    created_by INT NOT NULL,  
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);
GO
--Altering the Race table
ALTER TABLE RACE ADD CONSTRAINT FK_Race_User 
    FOREIGN KEY (created_by) REFERENCES [USER](user_id);
GO
-- 5 Horse Table
CREATE TABLE HORSE (
    horse_id INT IDENTITY(1,1) PRIMARY KEY,
    horse_name VARCHAR(100) NOT NULL,
    breed VARCHAR(50),
    age INT,
    color VARCHAR(30),
    gender VARCHAR(10),
    trainer_id INT,
    owner_id INT,
    registration_date DATE,
    created_by INT NOT NULL, 
    created_at DATETIME DEFAULT GETDATE()
);
GO

ALTER TABLE HORSE ADD CONSTRAINT FK_Horse_User 
    FOREIGN KEY (created_by) REFERENCES [USER](user_id);
GO
-- 6. JOCKEY Table

CREATE TABLE JOCKEY (
    jockey_id INT IDENTITY(1,1) PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    date_of_birth DATE,
    license_number VARCHAR(50) UNIQUE,
    experience_years INT,
    weight_kg DECIMAL(5,2),
    created_by INT NOT NULL, 
    created_at DATETIME DEFAULT GETDATE()
);
GO
ALTER TABLE JOCKEY ADD CONSTRAINT FK_Jockey_User 
    FOREIGN KEY (created_by) REFERENCES [USER](user_id);
GO


--2     Role Table
CREATE TABLE ROLE (
    role_id INT IDENTITY(1,1) PRIMARY KEY,
    role_name VARCHAR(30) UNIQUE NOT NULL,  
    description VARCHAR(200),
    created_at DATETIME DEFAULT GETDATE()
);
GO
-- 3. USERROLE Table (Junction Table)

CREATE TABLE USERROLE (
    userrole_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    role_id INT NOT NULL,
    assigned_at DATETIME DEFAULT GETDATE(),
    assigned_by INT 
);
GO

-- Add Foreign Keys for USERROLE
ALTER TABLE USERROLE ADD CONSTRAINT FK_UserRole_User 
    FOREIGN KEY (user_id) REFERENCES [USER](user_id);
GO

ALTER TABLE USERROLE ADD CONSTRAINT FK_UserRole_Role 
    FOREIGN KEY (role_id) REFERENCES ROLE(role_id);
GO

ALTER TABLE USERROLE ADD CONSTRAINT FK_UserRole_AssignedBy 
    FOREIGN KEY (assigned_by) REFERENCES [USER](user_id);
GO
-- 4. RACE Table
CREATE TABLE RACE (
    race_id INT IDENTITY(1,1) PRIMARY KEY,
    race_name VARCHAR(100) NOT NULL,
    race_date DATE NOT NULL,
    race_time TIME NOT NULL,
    venue VARCHAR(100) NOT NULL,
    distance_meters INT NOT NULL,
    race_type VARCHAR(50) NOT NULL,
    prize_pool DECIMAL(12,2),
    status VARCHAR(20) DEFAULT 'Scheduled',  
    created_by INT NOT NULL,  
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);
GO
--Altering the Race table
ALTER TABLE RACE ADD CONSTRAINT FK_Race_User 
    FOREIGN KEY (created_by) REFERENCES [USER](user_id);
GO
-- 5 Horse Table
CREATE TABLE HORSE (
    horse_id INT IDENTITY(1,1) PRIMARY KEY,
    horse_name VARCHAR(100) NOT NULL,
    breed VARCHAR(50),
    age INT,
    color VARCHAR(30),
    gender VARCHAR(10),
    trainer_id INT,
    owner_id INT,
    registration_date DATE,
    created_by INT NOT NULL, 
    created_at DATETIME DEFAULT GETDATE()
);
GO

ALTER TABLE HORSE ADD CONSTRAINT FK_Horse_User 
    FOREIGN KEY (created_by) REFERENCES [USER](user_id);
GO
-- 6. JOCKEY Table

CREATE TABLE JOCKEY (
    jockey_id INT IDENTITY(1,1) PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    date_of_birth DATE,
    license_number VARCHAR(50) UNIQUE,
    experience_years INT,
    weight_kg DECIMAL(5,2),
    created_by INT NOT NULL, 
    created_at DATETIME DEFAULT GETDATE()
);
GO
ALTER TABLE JOCKEY ADD CONSTRAINT FK_Jockey_User 
    FOREIGN KEY (created_by) REFERENCES [USER](user_id);
GO
-- 7. RACEHORSE Table (Junction Table)
CREATE TABLE RACEHORSE (
    racehorse_id INT IDENTITY(1,1) PRIMARY KEY,
    race_id INT NOT NULL,
    horse_id INT NOT NULL,
    category_id INT NOT NULL, 
    jockey_id INT,
    participant_id INT NOT NULL,  
    gate_position INT,
    finishing_position INT,
    race_time_seconds DECIMAL(8,3),
    odds DECIMAL(5,2),
    weight_carried DECIMAL(5,2),
    entry_status VARCHAR(20) DEFAULT 'Registered',  
    entered_at DATETIME DEFAULT GETDATE(),
    updated_by INT  
);
GO
-- Add Foreign Keys for RACEHORSE
ALTER TABLE RACEHORSE ADD CONSTRAINT FK_RaceHorse_Race 
    FOREIGN KEY (race_id) REFERENCES RACE(race_id);
GO

ALTER TABLE RACEHORSE ADD CONSTRAINT FK_RaceHorse_Horse 
    FOREIGN KEY (horse_id) REFERENCES HORSE(horse_id);
GO

ALTER TABLE RACEHORSE ADD CONSTRAINT FK_RaceHorse_Jockey 
    FOREIGN KEY (jockey_id) REFERENCES JOCKEY(jockey_id);
GO

ALTER TABLE RACEHORSE ADD CONSTRAINT FK_RaceHorse_Participant 
    FOREIGN KEY (participant_id) REFERENCES [USER](user_id);
GO

ALTER TABLE RACEHORSE ADD CONSTRAINT FK_RaceHorse_UpdatedBy 
    FOREIGN KEY (updated_by) REFERENCES [USER](user_id);
GO



