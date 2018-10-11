-- All instances of '#schema#' will be replaced by the variable injected by the initializer (or provisioner)
-- Can be used to spereate schemas by a tenantId, or similar scenarios

CREATE TABLE [#schema#].[ApiKeys](
		[ApiKey]				UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		[Name]					NVARCHAR (120) NOT NULL,
		[Description]			NVARCHAR (400) NOT NULL DEFAULT '',
		[CreatedDate]			DATETIME NOT NULL
	)
Go

--ID & Name are constraied to be unique
CREATE UNIQUE INDEX [#schema#_Index_ApiNameIndex] ON [#schema#].[ApiKeys] ([Name])
GO