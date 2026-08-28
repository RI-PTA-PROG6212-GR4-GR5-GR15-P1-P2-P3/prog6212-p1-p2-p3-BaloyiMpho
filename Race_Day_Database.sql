--  Creating the database

CREATE DATABASE Race_Day;

Use Race_Day;

--1. User Tabkle
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
