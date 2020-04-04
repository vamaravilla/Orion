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
	[Image] varchar(100) NULL,
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
--1
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'Aladdin',
	'Aladdin is a poor street urchin who spends his time stealing food from the marketplace in the city of Agrabah. His adventures begin when he meets a young girl who happens to be Princess Jasmine, who is forced to be married by her wacky yet estranged father.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/Aladdin.jpg'
	,0,7.00,32.00,1,0
);
--2
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'Alita',
	'Alita is a creation from an age of despair. Found by the mysterious Dr. Ido while trolling for cyborg parts, Alita becomes a lethal, dangerous being. She cannot remember who she is, or where she came from.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/Alita.jpg'
	,0,5.00,30.00,1,0
);
--3
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'Avatar',
	'On the lush alien world of Pandora live the Navi, beings who appear primitive but are highly evolved. ... Jake Sully, a paralyzed former Marine, becomes mobile again through one such Avatar and falls in love with a Navi woman. As a bond with her grows, he is drawn into a battle for the survival of her world.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/Avatar.jpg'
	,0,8.00,27.00,1,0
);
--4
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'Beauty and the beast',
	'When a young village girl named Belle (Emma Watson) comes in search of her imprisoned father, Maurice (Kevin Kline), the Beast takes her in his place. Over time, Belle sees beyond the furry coat and she befriends him. The two spend long days together in the snow and the sun and gradually they fall in love.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/BeautyAndTheBeast.jpg'
	,0,5.00,30.00,1,0
);
--5
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'Black Panther',
	'Five African tribes war over a meteorite containing Vibranium. One warrior ingests a "heart-shaped herb" affected by the metal and gains superhuman abilities, becoming the first "Black Panther". He unites all but the Jabari Tribe to form the nation of Wakanda.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/BlackPanther.jpg'
	,0,5.00,30.00,1,0
);
--6
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'Frozen 2',
	'Frozen II (2019) Anna, Elsa, Kristoff, Olaf and Sven leave Arendelle to travel to an ancient, autumn-bound forest of an enchanted land. They set out to find the origin of Elsa''s powers in order to save their kingdom.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/Frozen2.jpg'
	,0,7.00,35.00,1,0
);
--7
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'Harry Potter',
	'The film is the first instalment of the Harry Potter film series and was written by Steve Kloves and produced by David Heyman. Its story follows Harry Potter''s first year at Hogwarts School of Witchcraft and Wizardry as he discovers that he is a famous wizard and begins his education.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/HarryPotter.jpg'
	,0,5.00,20.00,1,0
);
--8
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'John Wick 2',
	'Bound by an inescapable blood debt to the Italian crime lord, Santino D''Antonio, and with his precious 1969 Mustang still stolen, John Wick--the taciturn and pitiless assassin who thirsts for seclusion--is forced to visit Italy to honour his promise.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/JohnWick.jpg'
	,0,5.00,30.00,1,0
);
--9
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'Joker',
	'In Gotham City, mentally troubled comedian Arthur Fleck is disregarded and mistreated by society. He then embarks on a downward spiral of revolution and bloody crime. This path brings him face-to-face with his alter-ego: the Joker.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/Joker.jpg'
	,0,5.00,30.00,1,0
);
--10
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'Jupirer Ascending',
	'Jupiter Ascending (2015) A young woman discovers her destiny as an heiress of intergalactic nobility and must fight to protect the inhabitants of Earth from an ancient and destructive industry.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/Jupiter.jpg'
	,0,5.00,25.00,1,0
);
--11
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'Parasite',
	'Parasite is a scabrous black comedy-slash-farce that resonates beyond its generic limits – a movie about status envy, aspiration, materialism, the patriarchal family unit and the idea of having (or leasing) servants.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/Parasite.jpg'
	,0,10.00,40.00,1,0
);
--12
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'Terminator: Dark Fate',
	'From the start of the Terminator franchise to Dark Fate, Linda Hamilton has been on an epic journey with her character. ... This is the first Terminator.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/Terminator.jpg'
	,0,5.00,30.00,1,0
);
--13
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'The Lion King',
	'The plot follows Simba, a young lion who must embrace his role as the rightful king of his native land following the murder of his father, Mufasa, at the hands of his uncle, Scar.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/TheLionKing.jpg'
	,0,5.00,30.00,1,0
);
--14
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'Weathering with you',
	'The film is set in Japan during a period of exceptionally rainy weather and tells the story of a high-school boy who runs away to Tokyo and befriends an orphan girl who has the ability to manipulate the weather.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/WeatheringWithYou.jpg'
	,0,7.00,35.00,1,0
);
--15
INSERT [SevenDays].[dbo].[Movie]([Title],[Description],[Image],[Stock],[RentalPrice],[SalePrice],[IsAvailable],[LikesCounter])
VALUES(
	'Your name',
	'Your Name tells the story of Taki, a high school boy in Tokyo and Mitsuha, a high school girl in a rural town, who suddenly and inexplicably begin to swap bodies.',
	'https://sevendaysstorage.blob.core.windows.net/moviesimages/YourName.jpg'
	,0,7.00,35.00,1,0
);





--Inventory
/*INSERT [SevenDays].[dbo].[Inventory]([IdMovie],[IsNew],[IsAvailable]) VALUES((SELECT IdMovie FROM [SevenDays].[dbo].[Movie] WHERE Title = 'Moana'),1,1);
INSERT [SevenDays].[dbo].[Inventory]([IdMovie],[IsNew],[IsAvailable]) VALUES((SELECT IdMovie FROM [SevenDays].[dbo].[Movie] WHERE Title = 'Moana'),0,1);

INSERT [SevenDays].[dbo].[Inventory]([IdMovie],[IsNew],[IsAvailable]) VALUES((SELECT IdMovie FROM [SevenDays].[dbo].[Movie] WHERE Title = 'Alita: Battle Angel'),1,1);
INSERT [SevenDays].[dbo].[Inventory]([IdMovie],[IsNew],[IsAvailable]) VALUES((SELECT IdMovie FROM [SevenDays].[dbo].[Movie] WHERE Title = 'Alita: Battle Angel'),0,1);

INSERT [SevenDays].[dbo].[Inventory]([IdMovie],[IsNew],[IsAvailable]) VALUES((SELECT IdMovie FROM [SevenDays].[dbo].[Movie] WHERE Title = 'John Wick'),1,1);
INSERT [SevenDays].[dbo].[Inventory]([IdMovie],[IsNew],[IsAvailable]) VALUES((SELECT IdMovie FROM [SevenDays].[dbo].[Movie] WHERE Title = 'John Wick'),0,1);


*/