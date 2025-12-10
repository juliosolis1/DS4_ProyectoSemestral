-- Crear base de datos
CREATE DATABASE BD_FeriaUniversitaria;
GO

USE BD_FeriaUniversitaria;
GO

-- ========== TABLA DE PRODUCTOS ==========
CREATE TABLE Productos (
    ProductoId      INT IDENTITY(1,1) PRIMARY KEY,
    Nombre          NVARCHAR(100) NOT NULL,
    Descripcion     NVARCHAR(250) NULL,
    PrecioVenta     DECIMAL(18,2) NOT NULL,
    Stock           INT NOT NULL,
    Activo          BIT NOT NULL DEFAULT 1,
    FechaCreacion   DATETIME NOT NULL DEFAULT GETDATE(),
    FechaEliminacion DATETIME NULL
);
GO

ALTER TABLE Productos
ADD FechaEliminacion DATETIME NULL;
GO

ALTER TABLE Productos
ADD ImagenPortada NVARCHAR(260) NULL;
GO

SELECT * FROM Productos

-- ========== TABLAS DE VENTAS ==========
CREATE TABLE Ventas (
    VentaId         INT IDENTITY(1,1) PRIMARY KEY,
    Fecha           DATETIME NOT NULL DEFAULT GETDATE(),
    Total           DECIMAL(18,2) NOT NULL,
    Observaciones   NVARCHAR(250) NULL
);
GO

SELECT * FROM Ventas

-----------------------------------------
CREATE TABLE DetalleVentas (
    DetalleId       INT IDENTITY(1,1) PRIMARY KEY,
    VentaId         INT NOT NULL,
    ProductoId      INT NOT NULL,
    Cantidad        INT NOT NULL,
    PrecioUnitario  DECIMAL(18,2) NOT NULL,
    Subtotal        AS (Cantidad * PrecioUnitario) PERSISTED
);

ALTER TABLE DetalleVentas
ADD CONSTRAINT FK_DetalleVentas_Ventas
    FOREIGN KEY (VentaId) REFERENCES Ventas(VentaId);

ALTER TABLE DetalleVentas
ADD CONSTRAINT FK_DetalleVentas_Productos
    FOREIGN KEY (ProductoId) REFERENCES Productos(ProductoId);
GO

SELECT * FROM DetalleVentas

-- ========== TABLA PARA “CIERRE DE CAJA” DIARIO ==========
CREATE TABLE CierresCaja (
    CierreId                INT IDENTITY(1,1) PRIMARY KEY,
    FechaCierre             DATE NOT NULL,
    TotalVentas             DECIMAL(18,2) NOT NULL,
    CantidadTransacciones   INT NOT NULL
);
GO

SELECT * FROM CierresCaja
