[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/mr-hqvA6)


Race Day System

A simple race management system for managing racing events, participants, horses, jockeys, and enrolments
Key Functionality
* Event creation and management
  Overview
The RaceDay System is a comprehensive race management database designed for SQL Server Management Studio (SSMS). This database schema supports the complete management of racing events, including:

* User management with role-based access (Organisers & Participants)

* Race event creation and management

* Horse and jockey registration

* Participant enrolment in race categories

* Race results tracking

* Payment and enrolment status management

* Participant enrolment in race categories

* Horse and jockey registration

* Race results capture and tracking

* Role-based access control


  *Technology Stack*

Database	Microsoft SQL Server for now
Database Schema
Complete Table List (8 Tables)

Entities (8 entities):
User - System users (Organisers and Participants)

1.Role - User roles (Organiser, Participant)

2.UserRole - Junction table for User-Role many-to-many

3.Race - Main racing events

4.Horse - Race horses

5.Jockey - Horse riders

6.RaceHorse - Junction table for Race-Horse with race-specific details

7.Enrolment - Participant enrolments in race categories
 Installation Guide

Prerequisites
* SQL Server Management Studio (SSMS)

* SQL Server 2019 or later

* Appropriate database permissions (CREATE TABLE, INSERT)

Step 1: Create Database
sql
-- Create the database
CREATE DATABASE RaceDayDB;
GO

-- Switch to the new database
USE RaceDayDB;
GO
Step 2: Run the Schema Script
Open SSMS and connect to your SQL Server instance

Copy the complete SQL script provided in the file

Paste it into a new query window in SSMS

Execute the script (press F5 or click Execute)

Step 3: Verify Installation
After execution, you should see:

* All 11 tables created successfully

* All foreign key constraints added

* Sample data inserted (4 users, 2 roles, 3 races, etc.)

* Verification queries showing results

Step 4: Check Your Tables
sql
-- List all tables in the database
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
