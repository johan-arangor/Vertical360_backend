-- ============================================================
-- SCRIPT DE RESCATE — Vertical360 MasterDbContext
-- ============================================================
-- PROBLEMA: EnsureCreatedAsync() creó las tablas sin registrar
-- nada en __EFMigrationsHistory. EF no sabe que ya están aplicadas
-- y falla al intentar recrearlas.
--
-- SOLUCIÓN en 3 pasos:
--   PASO 1: Crear __EFMigrationsHistory si no existe.
--   PASO 2: Registrar las migraciones ya aplicadas.
--   PASO 3: Aplicar los cambios nuevos (Companies expandida).
--
-- EJECUTAR contra: vertical360_shared
-- LUEGO ejecutar: dotnet ef database update --context MasterDbContext
-- ============================================================

USE `vertical360_shared`;

-- ── PASO 1: Crear la tabla de historial de migraciones ──────────────────────
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId`    VARCHAR(150) NOT NULL,
    `ProductVersion` VARCHAR(32)  NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

-- ── PASO 2: Registrar migraciones ya aplicadas (EnsureCreatedAsync) ──────────
-- Solo inserta si aún no están registradas.
INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES
    ('20251107050227_InitialCreate',               '8.0.22'),
    ('20251114071431_AddAcronymAndCodeToLocations', '8.0.22');

-- ── PASO 3: Agregar columnas nuevas a Companies ──────────────────────────────
-- Usamos IF NOT EXISTS a través de procedimiento para ser idempotente.
-- Si ya corriste el script anterior (AddCompanyBusinessFields.sql), omite esta sección.

-- Verificar y agregar cada columna de forma segura:
SET @db = DATABASE();

-- Nit
SET @col = 'Nit';
SET @tbl = 'Companies';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `Nit` VARCHAR(20) NOT NULL DEFAULT \'\' AFTER `Id`',
    'SELECT \'Column Nit already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- BusinessName
SET @col = 'BusinessName';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `BusinessName` VARCHAR(300) NOT NULL DEFAULT \'\' AFTER `Name`',
    'SELECT \'Column BusinessName already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Phone
SET @col = 'Phone';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `Phone` VARCHAR(20) NULL AFTER `BusinessName`',
    'SELECT \'Column Phone already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- MobilePhone
SET @col = 'MobilePhone';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `MobilePhone` VARCHAR(20) NULL AFTER `Phone`',
    'SELECT \'Column MobilePhone already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Address
SET @col = 'Address';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `Address` VARCHAR(300) NULL AFTER `MobilePhone`',
    'SELECT \'Column Address already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- CountryId
SET @col = 'CountryId';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `CountryId` CHAR(36) NULL AFTER `Address`',
    'SELECT \'Column CountryId already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- DepartmentId
SET @col = 'DepartmentId';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `DepartmentId` CHAR(36) NULL AFTER `CountryId`',
    'SELECT \'Column DepartmentId already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- CityId
SET @col = 'CityId';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `CityId` CHAR(36) NULL AFTER `DepartmentId`',
    'SELECT \'Column CityId already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- PostalCode
SET @col = 'PostalCode';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `PostalCode` VARCHAR(10) NULL AFTER `CityId`',
    'SELECT \'Column PostalCode already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- LegalRepresentativeName
SET @col = 'LegalRepresentativeName';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `LegalRepresentativeName` VARCHAR(200) NULL AFTER `PostalCode`',
    'SELECT \'Column LegalRepresentativeName already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- LegalRepresentativeEmail
SET @col = 'LegalRepresentativeEmail';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `LegalRepresentativeEmail` VARCHAR(200) NULL AFTER `LegalRepresentativeName`',
    'SELECT \'Column LegalRepresentativeEmail already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- LegalRepresentativePhone
SET @col = 'LegalRepresentativePhone';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `LegalRepresentativePhone` VARCHAR(20) NULL AFTER `LegalRepresentativeEmail`',
    'SELECT \'Column LegalRepresentativePhone already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- LegalRepresentativeMobile
SET @col = 'LegalRepresentativeMobile';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `LegalRepresentativeMobile` VARCHAR(20) NULL AFTER `LegalRepresentativePhone`',
    'SELECT \'Column LegalRepresentativeMobile already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- AdminName
SET @col = 'AdminName';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `AdminName` VARCHAR(200) NOT NULL DEFAULT \'\' AFTER `LegalRepresentativeMobile`',
    'SELECT \'Column AdminName already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- AdminEmail
SET @col = 'AdminEmail';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `AdminEmail` VARCHAR(200) NOT NULL DEFAULT \'\' AFTER `AdminName`',
    'SELECT \'Column AdminEmail already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- AdminPhone
SET @col = 'AdminPhone';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `AdminPhone` VARCHAR(20) NULL AFTER `AdminEmail`',
    'SELECT \'Column AdminPhone already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- IsActive
SET @col = 'IsActive';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `IsActive` TINYINT(1) NOT NULL DEFAULT 1 AFTER `AdminPhone`',
    'SELECT \'Column IsActive already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- CreatedAt
SET @col = 'CreatedAt';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `CreatedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) AFTER `IsActive`',
    'SELECT \'Column CreatedAt already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- UpdatedAt
SET @col = 'UpdatedAt';
SELECT COUNT(*) INTO @exists
  FROM information_schema.COLUMNS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND COLUMN_NAME = @col;
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD COLUMN `UpdatedAt` DATETIME(6) NULL AFTER `CreatedAt`',
    'SELECT \'Column UpdatedAt already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Índice único en Nit (idempotente)
SELECT COUNT(*) INTO @exists
  FROM information_schema.STATISTICS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND INDEX_NAME = 'IX_Companies_Nit';
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD UNIQUE INDEX `IX_Companies_Nit` (`Nit`)',
    'SELECT \'Index IX_Companies_Nit already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- FK CountryId
SELECT COUNT(*) INTO @exists
  FROM information_schema.TABLE_CONSTRAINTS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND CONSTRAINT_NAME = 'FK_Companies_Countries_CountryId';
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD CONSTRAINT `FK_Companies_Countries_CountryId` FOREIGN KEY (`CountryId`) REFERENCES `Countries`(`Id`) ON DELETE RESTRICT',
    'SELECT \'FK_Companies_Countries_CountryId already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- FK DepartmentId
SELECT COUNT(*) INTO @exists
  FROM information_schema.TABLE_CONSTRAINTS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND CONSTRAINT_NAME = 'FK_Companies_Departments_DepartmentId';
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD CONSTRAINT `FK_Companies_Departments_DepartmentId` FOREIGN KEY (`DepartmentId`) REFERENCES `Departments`(`Id`) ON DELETE RESTRICT',
    'SELECT \'FK_Companies_Departments_DepartmentId already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- FK CityId
SELECT COUNT(*) INTO @exists
  FROM information_schema.TABLE_CONSTRAINTS
 WHERE TABLE_SCHEMA = @db AND TABLE_NAME = @tbl AND CONSTRAINT_NAME = 'FK_Companies_Cities_CityId';
SET @sql = IF(@exists = 0,
    'ALTER TABLE `Companies` ADD CONSTRAINT `FK_Companies_Cities_CityId` FOREIGN KEY (`CityId`) REFERENCES `Cities`(`Id`) ON DELETE RESTRICT',
    'SELECT \'FK_Companies_Cities_CityId already exists\' AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ── VERIFICACIÓN FINAL ───────────────────────────────────────────────────────
SELECT `MigrationId`, `ProductVersion` FROM `__EFMigrationsHistory` ORDER BY `MigrationId`;
SELECT 'Script ejecutado correctamente.' AS Resultado;
