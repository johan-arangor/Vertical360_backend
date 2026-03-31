-- ============================================================
-- Migración: AddCompanyBusinessFields
-- Contexto: MasterDbContext (SharedDb)
-- Descripción: Expande la tabla Companies con todos los campos
--              del negocio de la unidad residencial.
-- ============================================================
-- APLICAR con:
--   dotnet ef migrations add AddCompanyBusinessFields --context MasterDbContext
--   dotnet ef database update --context MasterDbContext
-- O ejecutar este script directamente contra vertical360_shared.
-- ============================================================

USE vertical360_shared;

ALTER TABLE `Companies`
    -- Identificación
    ADD COLUMN `Nit`          VARCHAR(20)  NOT NULL DEFAULT '' AFTER `Id`,
    ADD COLUMN `BusinessName` VARCHAR(300) NOT NULL DEFAULT '' AFTER `Name`,

    -- Contacto
    ADD COLUMN `Phone`        VARCHAR(20)  NULL AFTER `BusinessName`,
    ADD COLUMN `MobilePhone`  VARCHAR(20)  NULL AFTER `Phone`,
    ADD COLUMN `Address`      VARCHAR(300) NULL AFTER `MobilePhone`,

    -- Ubicación
    ADD COLUMN `CountryId`    CHAR(36)     NULL AFTER `Address`,
    ADD COLUMN `DepartmentId` CHAR(36)     NULL AFTER `CountryId`,
    ADD COLUMN `CityId`       CHAR(36)     NULL AFTER `DepartmentId`,
    ADD COLUMN `PostalCode`   VARCHAR(10)  NULL AFTER `CityId`,

    -- Representante legal
    ADD COLUMN `LegalRepresentativeName`   VARCHAR(200) NULL AFTER `PostalCode`,
    ADD COLUMN `LegalRepresentativeEmail`  VARCHAR(200) NULL AFTER `LegalRepresentativeName`,
    ADD COLUMN `LegalRepresentativePhone`  VARCHAR(20)  NULL AFTER `LegalRepresentativeEmail`,
    ADD COLUMN `LegalRepresentativeMobile` VARCHAR(20)  NULL AFTER `LegalRepresentativePhone`,

    -- Administrador
    ADD COLUMN `AdminName`  VARCHAR(200) NOT NULL DEFAULT '' AFTER `LegalRepresentativeMobile`,
    ADD COLUMN `AdminEmail` VARCHAR(200) NOT NULL DEFAULT '' AFTER `AdminName`,
    ADD COLUMN `AdminPhone` VARCHAR(20)  NULL     AFTER `AdminEmail`,

    -- Metadata
    ADD COLUMN `IsActive`   TINYINT(1) NOT NULL DEFAULT 1 AFTER `AdminPhone`,
    ADD COLUMN `CreatedAt`  DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) AFTER `IsActive`,
    ADD COLUMN `UpdatedAt`  DATETIME(6) NULL AFTER `CreatedAt`;

-- Índice único en NIT
ALTER TABLE `Companies`
    ADD UNIQUE INDEX `IX_Companies_Nit` (`Nit`);

-- Claves foráneas a geografía (nullable — no obligatorias)
ALTER TABLE `Companies`
    ADD CONSTRAINT `FK_Companies_Countries_CountryId`
        FOREIGN KEY (`CountryId`) REFERENCES `Countries` (`Id`)
        ON DELETE RESTRICT ON UPDATE RESTRICT,

    ADD CONSTRAINT `FK_Companies_Departments_DepartmentId`
        FOREIGN KEY (`DepartmentId`) REFERENCES `Departments` (`Id`)
        ON DELETE RESTRICT ON UPDATE RESTRICT,

    ADD CONSTRAINT `FK_Companies_Cities_CityId`
        FOREIGN KEY (`CityId`) REFERENCES `Cities` (`Id`)
        ON DELETE RESTRICT ON UPDATE RESTRICT;
