CREATE TABLE [Users] (
    [UserID]        UNIQUEIDENTIFIER        NOT NULL,
    [UserName]      NVARCHAR (80)			PRIMARY KEY,
	[Active]		BIT				        NOT NULL DEFAULT 1,
)
GO