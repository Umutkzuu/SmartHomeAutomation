-- 1. Veritabanı Oluşturma ve Seçme
CREATE DATABASE IF NOT EXISTS SmartHomeDB;
USE SmartHomeDB;

-- 2. USERS Tablosu
CREATE TABLE USERS (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(256) NOT NULL,
    Role VARCHAR(20) NOT NULL DEFAULT 'User',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_Role CHECK (Role IN ('Admin', 'User', 'Guest'))
);

-- 3. ROOMS Tablosu
CREATE TABLE ROOMS (
    RoomID INT AUTO_INCREMENT PRIMARY KEY,
    RoomName VARCHAR(50) NOT NULL UNIQUE,
    FloorLevel INT NOT NULL DEFAULT 1,
    CONSTRAINT chk_FloorLevel CHECK (FloorLevel >= -1)
);

-- 4. DEVICE_CATEGORIES Tablosu
CREATE TABLE DEVICE_CATEGORIES (
    CategoryID INT AUTO_INCREMENT PRIMARY KEY,
    CategoryName VARCHAR(50) NOT NULL UNIQUE,
    UnitSymbol VARCHAR(10) NULL
);

-- 5. DEVICES Tablosu
CREATE TABLE DEVICES (
    DeviceID INT AUTO_INCREMENT PRIMARY KEY,
    DeviceName VARCHAR(100) NOT NULL,
    RoomID INT NOT NULL,
    CategoryID INT NOT NULL,
    IsActive BOOLEAN NOT NULL DEFAULT FALSE,
    CurrentValue DECIMAL(5,2) NULL,
    IsOnline BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT fk_Device_Room FOREIGN KEY (RoomID) REFERENCES ROOMS(RoomID) ON DELETE CASCADE,
    CONSTRAINT fk_Device_Category FOREIGN KEY (CategoryID) REFERENCES DEVICE_CATEGORIES(CategoryID) ON DELETE RESTRICT,
    CONSTRAINT chk_CurrentValue CHECK (CurrentValue >= 0)
);

-- 6. DEVICE_LOGS Tablosu
CREATE TABLE DEVICE_LOGS (
    LogID BIGINT AUTO_INCREMENT PRIMARY KEY,
    DeviceID INT NOT NULL,
    UserID INT NULL,
    ActionType VARCHAR(50) NOT NULL,
    OldValue DECIMAL(5,2) NULL,
    NewValue DECIMAL(5,2) NULL,
    Timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_Log_Device FOREIGN KEY (DeviceID) REFERENCES DEVICES(DeviceID) ON DELETE CASCADE,
    CONSTRAINT fk_Log_User FOREIGN KEY (UserID) REFERENCES USERS(UserID) ON DELETE SET NULL
);

-- --------------------------------------------------------
-- ZORUNLU TEST VERİLERİNİN (DUMMY DATA) EKLENMESİ
-- --------------------------------------------------------

INSERT INTO USERS (FirstName, LastName, Email, PasswordHash, Role) VALUES
('Ahmet', 'Yılmaz', 'ahmet@ev.com', 'hash1', 'Admin'),
('Ayşe', 'Yılmaz', 'ayse@ev.com', 'hash2', 'Admin'),
('Can', 'Yılmaz', 'can@ev.com', 'hash3', 'User'),
('Zeynep', 'Yılmaz', 'zeynep@ev.com', 'hash4', 'User'),
('Misafir', 'Bir', 'm1@ev.com', 'hash5', 'Guest'),
('Misafir', 'İki', 'm2@ev.com', 'hash6', 'Guest'),
('Temizlik', 'Görevlisi', 'temizlik@ev.com', 'hash7', 'Guest'),
('Güvenlik', 'Görevlisi', 'guvenlik@ev.com', 'hash8', 'User'),
('Bakım', 'Uzmanı', 'bakim@ev.com', 'hash9', 'Guest'),
('Sistem', 'Yöneticisi', 'sys@ev.com', 'hash10', 'Admin');

INSERT INTO ROOMS (RoomName, FloorLevel) VALUES
('Salon', 1), ('Mutfak', 1), ('Yatak Odası 1', 2), ('Çocuk Odası', 2),
('Misafir Odası', 2), ('Banyo 1', 1), ('Banyo 2', 2), ('Garaj', -1),
('Kiler', -1), ('Teras', 3);

INSERT INTO DEVICE_CATEGORIES (CategoryName, UnitSymbol) VALUES
('Aydınlatma', '%'), ('Isıtma', '°C'), ('Soğutma', '°C'),
('Güvenlik Kamerası', NULL), ('Hareket Sensörü', NULL),
('Akıllı Priz', 'W'), ('Kilit Sistemi', NULL),
('Eğlence Sistemi', 'Vol'), ('Beyaz Eşya', NULL), ('Bahçe Sulama', 'L/dk');

INSERT INTO DEVICES (DeviceName, RoomID, CategoryID, IsActive, CurrentValue, IsOnline) VALUES
('Salon Ana Lamba', 1, 1, FALSE, 0, TRUE),
('Salon Termostat', 1, 2, TRUE, 22.5, TRUE),
('Mutfak Prizi', 2, 6, TRUE, 120, TRUE),
('Yatak Odası Klima', 3, 3, FALSE, 0, TRUE),
('Garaj Kamerası', 8, 4, TRUE, NULL, TRUE),
('Dış Kapı Kilidi', 1, 7, TRUE, NULL, TRUE),
('Bahçe Sulama Sistemi', 10, 10, FALSE, 0, FALSE),
('Çocuk Odası Lamba', 4, 1, TRUE, 80, TRUE),
('Banyo 1 Sensör', 6, 5, TRUE, NULL, TRUE),
('Teras Hoparlör', 10, 8, FALSE, 0, TRUE);

INSERT INTO DEVICE_LOGS (DeviceID, UserID, ActionType, OldValue, NewValue) VALUES
(1, 1, 'Açıldı', 0, 100),
(2, 2, 'Derece Değişti', 21.0, 22.5),
(3, NULL, 'Açıldı', 0, 120),
(4, 3, 'Kapandı', 24.0, 0),
(5, 1, 'Hareket Algılandı', NULL, NULL),
(6, 2, 'Kilitlendi', NULL, NULL),
(7, NULL, 'Bağlantı Koptu', NULL, NULL),
(8, 4, 'Parlaklık Azaldı', 100, 80),
(9, NULL, 'Hareket Algılandı', NULL, NULL),
(10, 1, 'Kapandı', 40, 0);