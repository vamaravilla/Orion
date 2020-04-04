USE SevenDays;

/************************
         MOVIES
*************************/
SELECT * FROM [Movie];

--DELETE [Movie]


SELECT * FROM [Inventory]

--DELETE [Inventory] -- CLEAN TABLE

SELECT * FROM [AuditMovieLog] order by IdAuditMovieLog asc;

--DELETE [AuditMovieLog] -- CLEAN TABLE


/************************
           USER
*************************/

SELECT * FROM [User];

--DELETE [User] WHERE IdUser IN (1,2);

SELECT * FROM [Sale];

-- DELETE [Sale]; -- CLEAN TABLE

SELECT * FROM [Rental];

-- DELETE [Rental] -- CLEAN TABLE

SELECT * FROM [Liked];

-- DELETE [Liked] -- CLEAN TABLE

