CREATE TABLE [dbo].[Game]
(
	[Id] INT NOT NULL IDENTITY(1, 1)
	, [Date] DATETIME NOT NULL
	, [SeasonId] INT NOT NULL
	, [HomeTeamId] INT NOT NULL
	, [AwayTeamId] INT NOT NULL
	, [VenueId] INT NULL
	, [Status] VARCHAR(50) NOT NULL
	, [HomeTeamScore] INT NULL
	, [AwayTeamScore] INT NULL
	, [SportRadarId] VARCHAR(50) NULL
	, [SportRadarGuid] UNIQUEIDENTIFIER NOT NULL
	, [DateCreated] DATETIME NOT NULL
	, [DateUpdated] DATETIME NOT NULL
	, CONSTRAINT PK_Game_Id PRIMARY KEY (Id)
	, CONSTRAINT FK_Game_Team_Home FOREIGN KEY (HomeTeamId) REFERENCES Team (Id)
	, CONSTRAINT FK_Game_Team_Away FOREIGN KEY (AwayTeamId) REFERENCES Team (Id)
	, CONSTRAINT FK_Game_Venue FOREIGN KEY (VenueId) REFERENCES Venue (Id)
);