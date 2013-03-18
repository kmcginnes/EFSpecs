IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Test')
BEGIN
	Declare @dbname sysname

	Set @dbname = 'Test'

	Declare @spid int
	Select @spid = min(spid) from master.dbo.sysprocesses
	where dbid = db_id(@dbname)
	While @spid Is Not Null
	Begin
			Execute ('Kill ' + @spid)
			Select @spid = min(spid) from master.dbo.sysprocesses
			where dbid = db_id(@dbname) and spid > @spid
	End
	DROP DATABASE [Test]
END
GO