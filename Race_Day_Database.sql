
-- ============================================================
-- RACEDAY SYSTEM
-- For Running, Walking & Cycling Events
-- Note When Testing for creating Tables create 
--them one by one as shown on the ReadME.md document
-- ============================================================

-- 1. USER Table
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


-- 2. ROLE Table
CREATE TABLE ROLE (
    role_id INT IDENTITY(1,1) PRIMARY KEY,
    role_name VARCHAR(30) UNIQUE NOT NULL,  -- 'Organiser', 'Participant'
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

-- Adding Foreign Keys for USERROLE
ALTER TABLE USERROLE ADD CONSTRAINT FK_UserRole_User 
    FOREIGN KEY (user_id) REFERENCES [USER](user_id);
GO

ALTER TABLE USERROLE ADD CONSTRAINT FK_UserRole_Role 
    FOREIGN KEY (role_id) REFERENCES ROLE(role_id);
GO

ALTER TABLE USERROLE ADD CONSTRAINT FK_UserRole_AssignedBy 
    FOREIGN KEY (assigned_by) REFERENCES [USER](user_id);
GO


-- 4. EVENT Table

CREATE TABLE EVENT (
    event_id INT IDENTITY(1,1) PRIMARY KEY,
    event_name VARCHAR(100) NOT NULL,
    event_date DATE NOT NULL,
    event_time TIME NOT NULL,
    venue VARCHAR(100) NOT NULL,
    location VARCHAR(200) NOT NULL,
    distance_km DECIMAL(5,2) NOT NULL,
    event_type VARCHAR(50) NOT NULL,  -- 'Running', 'Walking', 'Cycling'
    entry_fee DECIMAL(10,2),
    status VARCHAR(20) DEFAULT 'Scheduled',  -- 'Scheduled', 'Open', 'Closed', 'In Progress', 'Completed', 'Cancelled'
    created_by INT NOT NULL,  
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);
GO

-- Adding Foreign Key for EVENT
ALTER TABLE EVENT ADD CONSTRAINT FK_Event_User 
    FOREIGN KEY (created_by) REFERENCES [USER](user_id);
GO


-- 5. CATEGORY Table
CREATE TABLE CATEGORY (
    category_id INT IDENTITY(1,1) PRIMARY KEY,
    event_id INT NOT NULL,
    category_name VARCHAR(50) NOT NULL,  -- Example 'Men 18-34', 'Women 35-49', 'Open'
    min_age INT,
    max_age INT,
    gender_restriction VARCHAR(10),  -- 'Male' or 'Female', 
    max_participants INT,
    entry_fee DECIMAL(8,2),
    created_by INT NOT NULL,  -- FK to User (Organiser)
    created_at DATETIME DEFAULT GETDATE()
);
GO

-- Adding Foreign Keys for CATEGORY
ALTER TABLE CATEGORY ADD CONSTRAINT FK_Category_Event 
    FOREIGN KEY (event_id) REFERENCES EVENT(event_id);
GO

ALTER TABLE CATEGORY ADD CONSTRAINT FK_Category_User 
    FOREIGN KEY (created_by) REFERENCES [USER](user_id);
GO

-- 6. ENROLMENT Table
CREATE TABLE ENROLMENT (
    enrolment_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,  -- FK to User (Participant)
    event_id INT NOT NULL,
    category_id INT NOT NULL,
    enrolment_date DATETIME DEFAULT GETDATE(),
    enrolment_status VARCHAR(20) DEFAULT 'Pending',  -- 'Pending', 'Confirmed', 'Withdrawn'
    payment_status VARCHAR(20) DEFAULT 'Unpaid',  -- 'Unpaid', 'Paid', 'Refunded'
    amount_paid DECIMAL(8,2),
    payment_date DATETIME,
    created_by INT NOT NULL  
);
GO

-- Adding Foreign Keys for ENROLMENT
ALTER TABLE ENROLMENT ADD CONSTRAINT FK_Enrolment_User 
    FOREIGN KEY (user_id) REFERENCES [USER](user_id);
GO

ALTER TABLE ENROLMENT ADD CONSTRAINT FK_Enrolment_Event 
    FOREIGN KEY (event_id) REFERENCES EVENT(event_id);
GO

ALTER TABLE ENROLMENT ADD CONSTRAINT FK_Enrolment_Category 
    FOREIGN KEY (category_id) REFERENCES CATEGORY(category_id);
GO

ALTER TABLE ENROLMENT ADD CONSTRAINT FK_Enrolment_CreatedBy 
    FOREIGN KEY (created_by) REFERENCES [USER](user_id);
GO

-- ============================================================
-- 7. PARTICIPANT_EVENT Table (Replaces RaceHorse)
-- ============================================================
CREATE TABLE PARTICIPANT_EVENT (
    participant_event_id INT IDENTITY(1,1) PRIMARY KEY,
    event_id INT NOT NULL,
    user_id INT NOT NULL,  -- FK to User (Participant)
    category_id INT NOT NULL,
    bib_number INT UNIQUE,  -- Unique race number
    gate_position INT,  -- Starting position
    finishing_position INT,
    finish_time_seconds DECIMAL(8,3),
    pace_per_km DECIMAL(5,2),
    entry_status VARCHAR(20) DEFAULT 'Registered',  -- 'Registered', 'Confirmed', 'Scratched', 'Completed'
    entered_at DATETIME DEFAULT GETDATE(),
    updated_by INT  -- FK to User (Organiser)
);
GO

-- Adding Foreign Keys for PARTICIPANT_EVENT
ALTER TABLE PARTICIPANT_EVENT ADD CONSTRAINT FK_ParticipantEvent_Event 
    FOREIGN KEY (event_id) REFERENCES EVENT(event_id);
GO

ALTER TABLE PARTICIPANT_EVENT ADD CONSTRAINT FK_ParticipantEvent_User 
    FOREIGN KEY (user_id) REFERENCES [USER](user_id);
GO

ALTER TABLE PARTICIPANT_EVENT ADD CONSTRAINT FK_ParticipantEvent_Category 
    FOREIGN KEY (category_id) REFERENCES CATEGORY(category_id);
GO

ALTER TABLE PARTICIPANT_EVENT ADD CONSTRAINT FK_ParticipantEvent_UpdatedBy 
    FOREIGN KEY (updated_by) REFERENCES [USER](user_id);
GO

-- ============================================================
-- 8. WEATHER Table
-- ============================================================
CREATE TABLE WEATHER (
    weather_id INT IDENTITY(1,1) PRIMARY KEY,
    event_id INT NOT NULL,
    forecast_date DATE NOT NULL,
    temperature DECIMAL(4,1),  -- Degrees Celsius
    humidity DECIMAL(4,1),  -- Percentage
    wind_speed DECIMAL(4,1),  -- km/h
    wind_direction VARCHAR(20),  -- 'N', 'NE', 'E', etc.
    precipitation_chance DECIMAL(4,1),  -- Percentage
    conditions VARCHAR(100),  -- 'Sunny', 'Cloudy', 'Rainy', etc.
    forecast_time DATETIME DEFAULT GETDATE()
);
GO

-- Add Foreign Key for WEATHER
ALTER TABLE WEATHER ADD CONSTRAINT FK_Weather_Event 
    FOREIGN KEY (event_id) REFERENCES EVENT(event_id);
GO


-- 9. ROUTE Table

CREATE TABLE ROUTE (
    route_id INT IDENTITY(1,1) PRIMARY KEY,
    event_id INT NOT NULL,
    route_name VARCHAR(100) NOT NULL,
    distance_km DECIMAL(5,2) NOT NULL,
    terrain_type VARCHAR(50),  -- 'Road', 'Trail', 'Mixed', 'Track'
    elevation_gain INT,  -- meters
    elevation_loss INT,  -- meters
    route_description TEXT,
    gpx_data TEXT,  -- GPX file data as XML/JSON
    start_latitude DECIMAL(10,8),
    start_longitude DECIMAL(11,8),
    end_latitude DECIMAL(10,8),
    end_longitude DECIMAL(11,8),
    created_at DATETIME DEFAULT GETDATE()
);
GO

-- Add Foreign Key for ROUTE
ALTER TABLE ROUTE ADD CONSTRAINT FK_Route_Event 
    FOREIGN KEY (event_id) REFERENCES EVENT(event_id);
GO


-- 10. ROUTE_WAYPOINT Table

CREATE TABLE ROUTE_WAYPOINT (
    waypoint_id INT IDENTITY(1,1) PRIMARY KEY,
    route_id INT NOT NULL,
    latitude DECIMAL(10,8) NOT NULL,
    longitude DECIMAL(11,8) NOT NULL,
    sequence_order INT NOT NULL,
    waypoint_type VARCHAR(30),  -- 'Start', 'Finish', 'Water Point', 'Checkpoint', 'Turn'
    description VARCHAR(200)
);
GO

-- Add Foreign Key for ROUTE_WAYPOINT
ALTER TABLE ROUTE_WAYPOINT ADD CONSTRAINT FK_RouteWaypoint_Route 
    FOREIGN KEY (route_id) REFERENCES ROUTE(route_id);
GO

-- ============================================================
-- 11. RESULT Table
-- ============================================================
CREATE TABLE RESULT (
    result_id INT IDENTITY(1,1) PRIMARY KEY,
    participant_event_id INT NOT NULL,
    user_id INT NOT NULL,
    event_id INT NOT NULL,
    category_id INT NOT NULL,
    finishing_position INT,
    finish_time_seconds DECIMAL(8,3),
    pace_per_km DECIMAL(5,2),
    overall_rank INT,
    category_rank INT,
    prize_money DECIMAL(10,2),
    result_date DATETIME DEFAULT GETDATE(),
    updated_by INT  -- FK to User (Organiser)
);
GO

-- Add Foreign Keys for RESULT
ALTER TABLE RESULT ADD CONSTRAINT FK_Result_ParticipantEvent 
    FOREIGN KEY (participant_event_id) REFERENCES PARTICIPANT_EVENT(participant_event_id);
GO

ALTER TABLE RESULT ADD CONSTRAINT FK_Result_User 
    FOREIGN KEY (user_id) REFERENCES [USER](user_id);
GO

ALTER TABLE RESULT ADD CONSTRAINT FK_Result_Event 
    FOREIGN KEY (event_id) REFERENCES EVENT(event_id);
GO

ALTER TABLE RESULT ADD CONSTRAINT FK_Result_Category 
    FOREIGN KEY (category_id) REFERENCES CATEGORY(category_id);
GO

ALTER TABLE RESULT ADD CONSTRAINT FK_Result_UpdatedBy 
    FOREIGN KEY (updated_by) REFERENCES [USER](user_id);
GO

-- ============================================================
-- INSERT SAMPLE DATA
-- ============================================================

-- 1. Insert Roles
INSERT INTO ROLE (role_name, description) VALUES 
('Organiser', 'Can create, edit, and delete events, manage categories, capture results'),
('Participant', 'Can browse events, enrol, view own enrolments and results');
GO

-- 2. Insert Users
INSERT INTO [USER] (username, email, password_hash, first_name, last_name, date_of_birth, phone, is_active) VALUES 
('alice_organiser', 'alice@raceday.com', 'hashed_password_123', 'Alice', 'Johnson', '1985-05-15', '555-0101', 1),
('bob_participant', 'bob@raceday.com', 'hashed_password_456', 'Bob', 'Smith', '1990-08-20', '555-0102', 1),
('carol_participant', 'carol@raceday.com', 'hashed_password_789', 'Carol', 'Williams', '1992-11-10', '555-0103', 1),
('dave_organiser', 'dave@raceday.com', 'hashed_password_101', 'Dave', 'Brown', '1988-03-25', '555-0104', 1);
GO

-- 3. Assign Roles
INSERT INTO USERROLE (user_id, role_id, assigned_by) VALUES 
(1, 1, 1),  
(2, 2, 1),  
(3, 2, 1),  
(4, 1, 4);  
GO

-- 4. Insert Events
INSERT INTO EVENT (event_name, event_date, event_time, venue, location, distance_km, event_type, entry_fee, status, created_by) VALUES 
('Cape Town Marathon', '2026-10-15', '06:00:00', 'Cape Town Stadium', 'Cape Town, Western Cape', 42.2, 'Running', 450.00, 'Scheduled', 1),
('Comrades Ultra Marathon', '2026-06-11', '05:30:00', 'Pietermaritzburg City Hall', 'Pietermaritzburg, KwaZulu-Natal', 89.0, 'Running', 750.00, 'Scheduled', 4),
('Cape Town Cycle Tour', '2026-03-08', '06:30:00', 'Grand Parade', 'Cape Town, Western Cape', 109.0, 'Cycling', 350.00, 'Scheduled', 1),
('Park Run - Greenpoint', '2026-01-20', '08:00:00', 'Greenpoint Park', 'Cape Town, Western Cape', 5.0, 'Running', 0.00, 'Completed', 1);
GO

-- 5. Insert Categories
INSERT INTO CATEGORY (event_id, category_name, min_age, max_age, gender_restriction, max_participants, entry_fee, created_by) VALUES 
(1, 'Men 18-34', 18, 34, 'Male', 1000, 450.00, 1),
(1, 'Women 18-34', 18, 34, 'Female', 800, 450.00, 1),
(1, 'Men 35-49', 35, 49, 'Male', 1200, 450.00, 1),
(1, 'Women 35-49', 35, 49, 'Female', 1000, 450.00, 1),
(1, 'Men 50+', 50, 99, 'Male', 500, 450.00, 1),
(1, 'Women 50+', 50, 99, 'Female', 400, 450.00, 1),
(2, 'Open', 18, 99, 'Any', 5000, 750.00, 4),
(3, 'Elite Men', 18, 40, 'Male', 100, 350.00, 1),
(3, 'Elite Women', 18, 40, 'Female', 80, 350.00, 1),
(3, 'Open Men', 18, 99, 'Male', 10000, 350.00, 1),
(3, 'Open Women', 18, 99, 'Female', 8000, 350.00, 1),
(4, 'Open', 8, 99, 'Any', 1000, 0.00, 1);
GO

-- 6. Insert Enrolments
INSERT INTO ENROLMENT (user_id, event_id, category_id, enrolment_status, payment_status, amount_paid, created_by) VALUES 
(2, 1, 1, 'Confirmed', 'Paid', 450.00, 2),
(3, 1, 2, 'Pending', 'Unpaid', NULL, 3),
(2, 2, 7, 'Confirmed', 'Paid', 750.00, 2),
(3, 4, 12, 'Confirmed', 'Paid', 0.00, 3);
GO

-- 7. Insert Participant Events
INSERT INTO PARTICIPANT_EVENT (event_id, user_id, category_id, bib_number, entry_status, entered_at) VALUES 
(1, 2, 1, 1024, 'Registered', '2026-01-15 10:00:00'),
(1, 3, 2, 2048, 'Registered', '2026-01-16 11:30:00'),
(2, 2, 7, 3072, 'Registered', '2026-01-20 09:00:00'),
(4, 3, 12, 4096, 'Completed', '2026-01-10 08:00:00');
GO

-- 8. Insert Weather Forecasts
INSERT INTO WEATHER (event_id, forecast_date, temperature, humidity, wind_speed, wind_direction, precipitation_chance, conditions) VALUES 
(1, '2026-10-15', 22.5, 65.0, 15.0, 'SE', 10.0, 'Partly Cloudy'),
(2, '2026-06-11', 18.0, 70.0, 10.0, 'SW', 20.0, 'Cloudy'),
(3, '2026-03-08', 24.0, 60.0, 20.0, 'S', 5.0, 'Sunny'),
(4, '2026-01-20', 26.0, 55.0, 12.0, 'NW', 0.0, 'Clear');
GO

-- 9. Insert Routes
INSERT INTO ROUTE (event_id, route_name, distance_km, terrain_type, elevation_gain, elevation_loss, route_description, start_latitude, start_longitude, end_latitude, end_longitude) VALUES 
(1, 'Cape Town Marathon Route', 42.2, 'Road', 350, 350, 'Scenic route along the Cape Town coastline', -33.9027, 18.4171, -33.9027, 18.4171),
(2, 'Comrades Marathon Route', 89.0, 'Road', 800, 800, 'The ultimate human race - Pietermaritzburg to Durban', -29.6006, 30.3794, -29.8587, 31.0218),
(3, 'Cape Town Cycle Tour Route', 109.0, 'Road', 550, 550, 'Iconic Cape Town cycle tour around the peninsula', -33.9249, 18.4241, -33.9249, 18.4241);
GO

-- 10. Insert Route Waypoints
INSERT INTO ROUTE_WAYPOINT (route_id, latitude, longitude, sequence_order, waypoint_type, description) VALUES 
(1, -33.9027, 18.4171, 1, 'Start', 'Cape Town Stadium Start'),
(1, -33.9100, 18.4200, 5, 'Water Point', 'Water Point 1 - 5km'),
(1, -33.9200, 18.4100, 10, 'Checkpoint', 'Checkpoint - 10km'),
(1, -33.9027, 18.4171, 20, 'Finish', 'Cape Town Stadium Finish');
GO

-- ============================================================
--  To VERIFICATION QUERIES
-- ============================================================

-- Check all tables
SELECT * FROM [USER];
SELECT * FROM ROLE;
SELECT * FROM USERROLE;
SELECT * FROM EVENT;
SELECT * FROM CATEGORY;
SELECT * FROM ENROLMENT;
SELECT * FROM PARTICIPANT_EVENT;
SELECT * FROM WEATHER;
SELECT * FROM ROUTE;
SELECT * FROM ROUTE_WAYPOINT;
SELECT * FROM RESULT;
GO


SELECT 
    u.user_id,
    u.username,
    u.first_name,
    u.last_name,
    r.role_name
FROM [USER] u
INNER JOIN USERROLE ur ON u.user_id = ur.user_id
INNER JOIN ROLE r ON ur.role_id = r.role_id
ORDER BY u.user_id;
GO


SELECT 
    e.event_id,
    e.event_name,
    e.event_date,
    e.venue,
    e.location,
    e.event_type,
    e.status,
    u.first_name + ' ' + u.last_name AS organiser
FROM EVENT e
INNER JOIN [USER] u ON e.created_by = u.user_id;
GO


SELECT 
    en.enrolment_id,
    u.first_name + ' ' + u.last_name AS participant,
    e.event_name,
    c.category_name,
    en.enrolment_status,
    en.payment_status,
    en.amount_paid
FROM ENROLMENT en
INNER JOIN [USER] u ON en.user_id = u.user_id
INNER JOIN EVENT e ON en.event_id = e.event_id
INNER JOIN CATEGORY c ON en.category_id = c.category_id;
GO


SELECT 
    e.event_name,
    e.event_date,
    w.temperature,
    w.humidity,
    w.wind_speed,
    w.conditions,
    w.precipitation_chance
FROM EVENT e
LEFT JOIN WEATHER w ON e.event_id = w.event_id
WHERE e.event_id = 1;
GO


SELECT 
    e.event_name,
    r.route_name,
    r.distance_km,
    r.terrain_type,
    r.elevation_gain,
    r.elevation_loss,
    COUNT(rw.waypoint_id) AS waypoint_count
FROM EVENT e
INNER JOIN ROUTE r ON e.event_id = r.event_id
LEFT JOIN ROUTE_WAYPOINT rw ON r.route_id = rw.route_id
GROUP BY e.event_name, r.route_name, r.distance_km, r.terrain_type, r.elevation_gain, r.elevation_loss;
GO
