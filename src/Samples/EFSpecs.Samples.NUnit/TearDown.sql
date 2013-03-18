IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Test')
DROP DATABASE [Test]
GO