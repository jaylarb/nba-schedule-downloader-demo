-- Index on Date
CREATE INDEX IX_Game_Date ON [dbo].[Game] ([Date]);
GO;

-- Index on HomeTeamId
CREATE INDEX IX_Game_HomeTeamId ON [dbo].[Game] ([HomeTeamId]);
GO;

-- Index on AwayTeamId
CREATE INDEX IX_Game_AwayTeamId ON [dbo].[Game] ([AwayTeamId]);
GO;