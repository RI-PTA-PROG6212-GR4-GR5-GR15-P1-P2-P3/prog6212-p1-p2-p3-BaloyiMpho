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
