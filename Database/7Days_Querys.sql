USE SevenDays;

SELECT * FROM [Movie];

SELECT * FROM [Inventory]

SELECT * FROM [User];

SELECT * FROM [Sale];

SELECT * FROM [AuditMovieLog] order by IdAuditMovieLog asc;

UPDATE [Movie] SET LikesCounter = 20 WHERE IdMovie = 2
UPDATE [Movie] SET LikesCounter = 16 WHERE IdMovie = 3
UPDATE [Movie] SET LikesCounter = 11 WHERE IdMovie = 1

UPDATE [Inventory] SET IsAvailable = 1 WHERE IdInventory = 1;


DELETE [Sale] WHERE IdInventory = 1;