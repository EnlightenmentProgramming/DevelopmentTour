USE [Higo.Game.GD]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [IX_T_Agent]    Script Date: 2018-08-11 17:25:28 ******/
CREATE NONCLUSTERED INDEX [IX_T_Agent] ON [dbo].[T_Agent]
(
	[ParentID] ASC,
	[WashType] ASC,
	[State] ASC,
	[CreateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


USE [Higo.Game.GD]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [UX_T_Agent]    Script Date: 2018-08-11 17:25:39 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_T_Agent] ON [dbo].[T_Agent]
(
	[LogName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


