-- ✅ Tùy chọn, set một lần ở đầu
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

--------------------------------------------------
-- Category
--------------------------------------------------
CREATE TABLE [dbo].[Category](
    [CategoryID]   INT IDENTITY(1,1) NOT NULL,
    [CategoryName] NVARCHAR(100)     NOT NULL,
    [Decription]   NVARCHAR(100)     NULL,
    [isActive]     BIT               NOT NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([CategoryID] ASC)
);

--------------------------------------------------
-- Customer
--------------------------------------------------
CREATE TABLE [dbo].[Customer](
    [CustomerID]   INT IDENTITY(1,1) NOT NULL,
    [CustomerName] NVARCHAR(100)     NOT NULL,
    [Phone]        VARCHAR(50)       NOT NULL,
    [Email]        VARCHAR(100)      NULL,
    [Address]      NVARCHAR(200)     NULL,
    [isActive]     BIT               NOT NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([CustomerID] ASC)
);

--------------------------------------------------
-- Staff
--------------------------------------------------
CREATE TABLE [dbo].[Staff](
    [StaffID]    INT IDENTITY(1,1) NOT NULL,
    [StaffName]  NVARCHAR(50)      NOT NULL,
    [Role]       TINYINT           NOT NULL,
    [Phone]      VARCHAR(50)       NOT NULL,
    [Email]      VARCHAR(100)      NULL,
    [Address]    NVARCHAR(200)     NULL,
    [BaseSalary] DECIMAL(10,2)     NULL,
    [isActive]   BIT               NOT NULL,
    CONSTRAINT [PK_Staff] PRIMARY KEY CLUSTERED ([StaffID] ASC)
);

--------------------------------------------------
-- Supplier
--------------------------------------------------
CREATE TABLE [dbo].[Supplier](
    [SupplierID]   INT IDENTITY(1,1) NOT NULL,
    [SupplierName] NVARCHAR(100)     NOT NULL,
    [Phone]        VARCHAR(50)       NOT NULL,
    [Email]        VARCHAR(100)      NOT NULL,
    [Address]      NVARCHAR(200)     NOT NULL,
    [isActive]     BIT               NOT NULL,
    CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED ([SupplierID] ASC)
);

--------------------------------------------------
-- Product
--------------------------------------------------
CREATE TABLE [dbo].[Product](
    [ProductID]   INT IDENTITY(1,1) NOT NULL,
    [ProductName] NVARCHAR(100)     NULL,
    [CategoryID]  INT               NOT NULL,
    [SupplierID]  INT               NOT NULL,
    [Price]       DECIMAL(10,2)     NULL,
    [isActive]    BIT               NOT NULL,
    [Quantity]    INT               NOT NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ProductID] ASC)
);

--------------------------------------------------
-- Log
--------------------------------------------------
CREATE TABLE [dbo].[Log](
    [LogID]      INT IDENTITY(1,1) NOT NULL,
    [Decription] NVARCHAR(200)     NOT NULL,
    [LogLevel]   TINYINT           NOT NULL,
    [CreateDate] DATETIME          NULL,
    [StaffID]    INT               NOT NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([LogID] ASC)
);

--------------------------------------------------
-- Order
--------------------------------------------------
CREATE TABLE [dbo].[Order](
    [OrderID]    INT IDENTITY(1,1) NOT NULL,
    [CustomerID] INT               NOT NULL,
    [StaffID]    INT               NOT NULL,
    [CreateDate] DATETIME          NOT NULL,
    [Status]     TINYINT           NULL,
    CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([OrderID] ASC)
);

--------------------------------------------------
-- OrderDetail
-- (Giữ nguyên theo script của mi: OrderDetailID PK, OrderID IDENTITY)
--------------------------------------------------
CREATE TABLE [dbo].[OrderDetail](
    [OrderDetailID] INT IDENTITY(1,1) NOT NULL,
    [OrderID]       INT NOT NULL,
    [ProductID]     INT NOT NULL,
    [Quantity]      INT NOT NULL,
    CONSTRAINT [PK_OrderDetail] PRIMARY KEY CLUSTERED ([OrderDetailID] ASC)
);

--------------------------------------------------
-- PurchaseOrder
--------------------------------------------------
CREATE TABLE [dbo].[PurchaseOrder](
    [PurchaseOrderID] INT IDENTITY(1,1) NOT NULL,
    [SupplierID]      INT               NOT NULL,
    [CreateDate]      DATETIME          NOT NULL,
    [Status]          TINYINT           NOT NULL,
    [isActive]        BIT               NOT NULL,
    CONSTRAINT [PK_PurchaseOrder] PRIMARY KEY CLUSTERED ([PurchaseOrderID] ASC)
);

--------------------------------------------------
-- PurchaseOrderDetail
--------------------------------------------------
CREATE TABLE [dbo].[PurchaseOrderDetail](
    [PurchaseOrderDetailID] INT IDENTITY(1,1) NOT NULL,
    [PurchaseOrderID]       INT               NOT NULL,
    [ProductID]             INT               NOT NULL,
    [Quantity]              INT               NULL,
    [Price]                 DECIMAL(10,2)     NULL,
    CONSTRAINT [PK_PurchaseOrderDetail] PRIMARY KEY CLUSTERED ([PurchaseOrderDetailID] ASC)
);

--------------------------------------------------
-- ReportRevenue
--------------------------------------------------
CREATE TABLE [dbo].[ReportRevenue](
    [ReportRevenueID] INT IDENTITY(1,1) NOT NULL,
    [DayStart]        NCHAR(10)         NULL,
    [DayEnd]          NCHAR(10)         NULL,
    [Month]           INT               NULL,
    [Year]            INT               NULL,
    [CreateDate]      DATETIME          NULL,
    [StaffID]         INT               NOT NULL,
    [AmountRevenue]   DECIMAL(20,0)     NULL,
    [AmountCost]      DECIMAL(20,0)     NULL,
    [AmountSalary]    DECIMAL(20,0)     NULL,
    [Profit]          DECIMAL(20,0)     NULL,
    CONSTRAINT [PK_ReportRevenue] PRIMARY KEY CLUSTERED ([ReportRevenueID] ASC)
);

--------------------------------------------------
-- Salary
--------------------------------------------------
CREATE TABLE [dbo].[Salary](
    [SalaryID]   INT IDENTITY(1,1)   NOT NULL,
    [StaffID]    INT           NOT NULL,
    [Bonus]      DECIMAL(10,2) NULL,
    [Deduction]  DECIMAL(10,2) NULL,
    [Amount]     DECIMAL(10,2) NULL,
    [Month]      INT           NOT NULL,
    [Year]       INT           NOT NULL,
    [Status]     TINYINT       NOT NULL,
    [DayPayment] INT           NULL,
    CONSTRAINT [PK_Salary] PRIMARY KEY CLUSTERED ([SalaryID] ASC)
);

--------------------------------------------------
-- FOREIGN KEYS
--------------------------------------------------

ALTER TABLE [dbo].[Log] 
    ADD CONSTRAINT [FK_Log_Staff]
    FOREIGN KEY ([StaffID]) REFERENCES [dbo].[Staff]([StaffID]);

ALTER TABLE [dbo].[Order] 
    ADD CONSTRAINT [FK_Order_Customer]
    FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer]([CustomerID]);

ALTER TABLE [dbo].[Order] 
    ADD CONSTRAINT [FK_Order_Staff]
    FOREIGN KEY ([StaffID]) REFERENCES [dbo].[Staff]([StaffID]);

ALTER TABLE [dbo].[OrderDetail] 
    ADD CONSTRAINT [FK_OrderDetail_Order]
    FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order]([OrderID]);

ALTER TABLE [dbo].[OrderDetail] 
    ADD CONSTRAINT [FK_OrderDetail_Product]
    FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product]([ProductID]);

ALTER TABLE [dbo].[Product] 
    ADD CONSTRAINT [FK_Product_Category]
    FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Category]([CategoryID]);

ALTER TABLE [dbo].[Product] 
    ADD CONSTRAINT [FK_Product_Supplier]
    FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier]([SupplierID]);

ALTER TABLE [dbo].[PurchaseOrder] 
    ADD CONSTRAINT [FK_PurchaseOrder_Supplier]
    FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier]([SupplierID]);

ALTER TABLE [dbo].[PurchaseOrderDetail] 
    ADD CONSTRAINT [FK_PurchaseOrderDetail_Product]
    FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product]([ProductID]);

ALTER TABLE [dbo].[PurchaseOrderDetail] 
    ADD CONSTRAINT [FK_PurchaseOrderDetail_PurchaseOrder]
    FOREIGN KEY ([PurchaseOrderID]) REFERENCES [dbo].[PurchaseOrder]([PurchaseOrderID]);

ALTER TABLE [dbo].[ReportRevenue] 
    ADD CONSTRAINT [FK_ReportRevenue_Staff]
    FOREIGN KEY ([StaffID]) REFERENCES [dbo].[Staff]([StaffID]);

ALTER TABLE [dbo].[Salary] 
    ADD CONSTRAINT [FK_Salary_Staff1]
    FOREIGN KEY ([StaffID]) REFERENCES [dbo].[Staff]([StaffID]);
