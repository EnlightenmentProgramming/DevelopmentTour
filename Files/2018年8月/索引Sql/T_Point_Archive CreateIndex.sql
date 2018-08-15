USE [Higo.Game.GD]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [IX_T_Point_Archive_SourceID]    Script Date: 2018-08-11 17:31:38 ******/
CREATE NONCLUSTERED INDEX [IX_T_Point_Archive_SourceID] ON [dbo].[T_Point_Archive]
(
	[SourceID] ASC,
	[OperationType] ASC,
	[StatisticsDay] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


USE [Higo.Game.GD]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [IX_T_Point_Archive_TargetID]    Script Date: 2018-08-11 17:31:42 ******/
CREATE NONCLUSTERED INDEX [IX_T_Point_Archive_TargetID] ON [dbo].[T_Point_Archive]
(
	[TargetID] ASC,
	[OperationType] ASC,
	[StatisticsDay] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


