--Description: Script to Create Database for "Seven days Challenge project" and load initial data
--Author: Victor Maravilla
--Date: 2020-03-27

/************************
     METADATA - DDL
*************************/

CREATE DATABASE [SevenDays];

USE [SevenDays];

CREATE TABLE [Movie](
	[IdMovie] int IDENTITY(1,1) NOT NULL,
	[Title] varchar(100) NOT NULL,
	[Description] varchar(500) NULL,
	[Image] varchar(50) NULL,
	[Stock] int NULL,
	[RentalPrice] decimal(9, 3) NULL,
	[SalePrice] decimal(9, 3) NULL,
	[IsAvailable] bit NULL,
	[LikesCounter] int NULL DEFAULT 0,
	CONSTRAINT [PK_Movie] PRIMARY KEY (IdMovie)
);

CREATE TABLE [Inventory](
	[IdInventory] int IDENTITY(1,1) NOT NULL,
	[IdMovie] int NOT NULL,
	[IsNew] bit NULL,
	[IsAvailable] bit NULL,
	CONSTRAINT [PK_Inventory] PRIMARY KEY ([IdInventory]),
	CONSTRAINT [FK_MovieInventory] FOREIGN KEY ([IdMovie]) REFERENCES [Movie]([IdMovie])
);

CREATE TABLE [User](
	[IdUser] int IDENTITY(1,1) NOT NULL,
	[Name] varchar(100) NOT NULL,
	[Email] varchar(100) NULL,
	[Password] varchar(100) NULL, --Password must be encrypted!
	[CreatedDate] datetime DEFAULT GETDATE(),
	[Profile] int NULL, --1 = Admin, 2 = Customer
	[IsActive] bit NULL,
	CONSTRAINT [PK_User] PRIMARY KEY (IdUser)
);

CREATE TABLE [Rental](
	[IdRental] int IDENTITY(1,1) NOT NULL,
	[IdInventory] int NOT NULL,
	[IdUser] int NOT NULL,
	[RentalDate] datetime NULL,
	[ReturnDate] datetime NULL,
	[RentalPrice] decimal(9, 3) NULL,
	[Penalty] decimal(9, 3) NULL,
	CONSTRAINT [PK_Rental] PRIMARY KEY ([IdRental]),
	CONSTRAINT [FK_InventoryRental] FOREIGN KEY ([IdInventory]) REFERENCES [Inventory]([IdInventory]),
	CONSTRAINT [FK_UserRental] FOREIGN KEY ([IdUser]) REFERENCES [User]([IdUser]),
);

CREATE TABLE [Sale](
	[IdInventory] int NOT NULL,
	[IdUser] int NOT NULL,
	[SaleDate] datetime NULL,
	[SalePrice] decimal(9, 3) NULL,
	CONSTRAINT [PK_Sale] PRIMARY KEY ([IdInventory],[IdUser]),
	CONSTRAINT [FK_InventorySale] FOREIGN KEY ([IdInventory]) REFERENCES [Inventory]([IdInventory]),
	CONSTRAINT [FK_UserSale] FOREIGN KEY ([IdUser]) REFERENCES [User]([IdUser]),
);


CREATE TABLE [Liked](
	[IdMovie] int NOT NULL,
	[IdUser] int NOT NULL,
	CONSTRAINT [PK_Liked] PRIMARY KEY ([IdMovie],[IdUser]),
	CONSTRAINT [FK_MovieLiked] FOREIGN KEY ([IdMovie]) REFERENCES [Movie]([IdMovie]),
	CONSTRAINT [FK_UserLiked] FOREIGN KEY ([IdUser]) REFERENCES [User]([IdUser]),
);

CREATE TABLE [AuditMovieLog](
	[IdAuditMovieLog] int IDENTITY(1,1) NOT NULL,
	[IdMovie] int NOT NULL,
	[IdUser] int NOT NULL,
	[Title] varchar(100) NULL,
	[RentalPrice] decimal(9, 3) NULL,
	[SalePrice] decimal(9, 3) NULL,
	[Action] varchar(20) NULL,
	[ActionDate] datetime DEFAULT GETDATE(),
	CONSTRAINT [PK_AuditMovieLog] PRIMARY KEY ([IdAuditMovieLog])
);


/************************
    LOGIN AND USER APP
*************************/

USE [master];
CREATE LOGIN [sevendaysapp] WITH PASSWORD = 'timeflies20.'--, CHECK_POLICY = OFF;
USE [SevenDays];;
CREATE USER [sevendaysapp] FOR LOGIN [sevendaysapp] WITH DEFAULT_SCHEMA=[dbo];

EXEC sp_addrolemember 'db_datareader', 'sevendaysapp';
EXEC sp_addrolemember 'db_datawriter', 'sevendaysapp';


/************************
         TEST DATA
*************************/


--Movies
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable])
VALUES('Moana','From Walt Disney Animation Studios comes Moana, a sweeping, CG-animated feature film about an adventurous teenager who sails out on a daring mission to save her people.','',2,5.00,20.00,1);

INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable] )
VALUES('Alita: Battle Angel','2019 American cyberpunk action film based on Japanese manga artist Yukito Kishiro''s 1990s series Gunnm and its 1993 original video animation.','',2,5.00,25.00,1);

INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable] )
VALUES('John Wick','The story focuses on John Wick (Reeves) searching for the men who broke into his home, stole his vintage car and killed his puppy, which was a last gift to him from his recently deceased wife.','',2,5.00,30.00,1);


INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable] )
VALUES('Movie One','Description One','',2,5.00,30.00,1);

INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable] )
VALUES('Movie Two','Description Two','',2,5.00,30.00,1);

INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable] )
VALUES('Movie Three','Description Three','',2,5.00,30.00,1);

INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable] )
VALUES('Movie Four','Description Four','',2,5.00,30.00,1);

INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable] )
VALUES('Movie Five','Description Five','',2,5.00,30.00,1);

INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable] )
VALUES('Movie Six','Description Six','',2,5.00,30.00,1);

INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable] )
VALUES('Movie Seven','Description Seven','',2,5.00,30.00,1);

INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable] )
VALUES('Movie Eight','Description Eight','',2,5.00,30.00,1);

INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable] )
VALUES('Movie Nine','Description Nine','',2,5.00,30.00,1);

INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable] )
VALUES('Movie Ten','Description Ten','',2,5.00,30.00,1);

--Inventory
INSERT [SevenDays].[dbo].[Inventory]([IdMovie],[IsNew],[IsAvailable]) VALUES((SELECT IdMovie FROM [SevenDays].[dbo].[Movie] WHERE Title = 'Moana'),1,1);
INSERT [SevenDays].[dbo].[Inventory]([IdMovie],[IsNew],[IsAvailable]) VALUES((SELECT IdMovie FROM [SevenDays].[dbo].[Movie] WHERE Title = 'Moana'),0,1);

INSERT [SevenDays].[dbo].[Inventory]([IdMovie],[IsNew],[IsAvailable]) VALUES((SELECT IdMovie FROM [SevenDays].[dbo].[Movie] WHERE Title = 'Alita: Battle Angel'),1,1);
INSERT [SevenDays].[dbo].[Inventory]([IdMovie],[IsNew],[IsAvailable]) VALUES((SELECT IdMovie FROM [SevenDays].[dbo].[Movie] WHERE Title = 'Alita: Battle Angel'),0,1);

INSERT [SevenDays].[dbo].[Inventory]([IdMovie],[IsNew],[IsAvailable]) VALUES((SELECT IdMovie FROM [SevenDays].[dbo].[Movie] WHERE Title = 'John Wick'),1,1);
INSERT [SevenDays].[dbo].[Inventory]([IdMovie],[IsNew],[IsAvailable]) VALUES((SELECT IdMovie FROM [SevenDays].[dbo].[Movie] WHERE Title = 'John Wick'),0,1);

--Users
INSERT [SevenDays].[dbo].[User]([Name],[Email],[Password],[Profile],[IsActive])
VALUES('Victor Maravilla','vawonder@gmail.com','timeflies20.',1,1);

INSERT [SevenDays].[dbo].[User]([Name],[Email],[Password],[Profile],[IsActive])
VALUES('Alexander Rodriguez','alex@gmail.com','timeflies20.',2,1);
