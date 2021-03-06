USE [Higo.Game.GD]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [IX_T_BetBill]    Script Date: 2018-08-11 17:27:57 ******/
CREATE NONCLUSTERED INDEX [IX_T_BetBill] ON [dbo].[T_BetBill]
(
	[ClientID] ASC,
	[State] ASC,
	[StatisticsDay] ASC,
	[CreateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

