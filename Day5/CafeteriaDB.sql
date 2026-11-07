create database  CafeteriaDB;

CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255)
);
CREATE TABLE Suppliers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20)
);

CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Quantity INT NOT NULL,
    CategoryId INT NOT NULL,
    SupplierId INT NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id)
);
CREATE TABLE PurchaseOrders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderDate DATETIME2 NOT NULL,
    Status NVARCHAR(30) NOT NULL,
    SupplierId INT NOT NULL,

    FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id)
);

CREATE TABLE PurchaseOrderItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PurchaseOrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,

    FOREIGN KEY (PurchaseOrderId) REFERENCES PurchaseOrders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);








