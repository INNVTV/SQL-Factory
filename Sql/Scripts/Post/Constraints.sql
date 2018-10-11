-- Categorization Contraints --

ALTER TABLE [#schema#].[Subcategory]
ADD CONSTRAINT [#schema#_FK_SubcategoryToCategoryID] FOREIGN KEY ([CategoryID])
REFERENCES [#schema#].[Category] ([CategoryID])
GO