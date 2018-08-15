--当前代理存量查询
			declare @AgentID nvarchar(40)
			declare @StartDate nvarchar(20)
			declare @EndDate nvarchar(20)

			set @AgentID = '${AgentID}'
			set @StartDate ='${StartDate}'
			set @EndDate ='${EndDate}'

			set @AgentID ='c4f71884dd954ec69cd7b0f8451db7e0'
			set @StartDate ='2018-01-01'
			set @EndDate ='2018-12-12'

			declare @StartD nvarchar(40)
			declare @EndD nvarchar(40)
			select @StartD = convert(varchar(11),@StartDate,120)+ ' ' + value from t_cfg where name = 'Use_WORKTIME_START' --归档时间
			select @EndD = convert(varchar(11),DATEADD(DAY,1,@EndDate),120)+ ' ' + value from t_cfg where name = 'Use_WORKTIME_START' --归档时间
				
			select
				AgentID A_ID
				,AgentName A_Name
				,LogName A_UserID
				,'SelfA' A_Type
				,ParentID P_ID
				,CAST(0 as int) A_ProA_T
				,CAST(0 as int) A_ProA
				,CAST(0 as numeric(20,4)) A_TotalWin
				,CAST(0 as numeric(20,4)) A_WashS
				,CAST(0 as numeric(20,4)) A_WashF
				,CAST(0 as numeric(20,4)) A_BetS
				,CAST(0 as numeric(20,4)) A_BetActS
				,CAST(0 as numeric(20,4)) A_3BaoA
				,CAST(0 as numeric(20,4)) A_Charge
				,CAST(0 as numeric(20,4)) A_Cash
				,CAST(0 as numeric(20,4)) A_TotalWin_Self --推广会员总赢
				,CAST(0 as numeric(20,4)) A_WashS_Self --推广会员洗码量
				,CAST(0 as numeric(20,4)) A_WashF_Self --推广会员洗码费
				,CAST(0 as numeric(20,4)) A_BetS_Self
				,CAST(0 as numeric(20,4)) A_BetActS_Self 
				,CAST(0 as numeric(20,4)) A_3BaoA_Self --推广会三宝费
				,CAST(0 as numeric(20,4)) A_Charge_Self--推广会上分
				,CAST(0 as numeric(20,4)) A_Cash_Self--推广会下分
				,CreateTime A_CreTime
			into #agentTemp
			from T_Agent a where AgentID = @AgentID and IsHide = 'FALSE'

			;WITH AtreeCTE(AgentID,ParentID,TopAID,Level) AS(
						select AgentID,ParentID,AgentID,0 from T_Agent where  AgentID = @AgentID and IsHide = 'FALSE'
						union all 
						select a.AgentID,a.ParentID,cte.TopAID,cte.Level+1 from T_Agent a  join AtreeCTE cte on a.ParentID = cte.AgentID where a.IsHide ='FALSE'
					)

			select AgentID,ParentID,TopAID,Level 
				,(select COUNT(ClientID) from T_WeiTou_Detail where InviterID = a.AgentID) A_Prom
				,(select COUNT(ClientID) from T_WeiTou_Detail where AgentID3Bao = a.AgentID) A_PromT
				,CAST(0 as numeric(20,4)) C_TotalWin --推广会员总赢
				,CAST(0 as numeric(20,4)) C_WashS --推广会员洗码量
				,CAST(0 as numeric(20,4)) C_WashF --推广会员洗码费
				,CAST(0 as numeric(20,4)) C_BetS
				,CAST(0 as numeric(20,4)) C_BetActS 
				,CAST(0 as numeric(20,4)) C_3BaoA --推广会三宝费
				,CAST(0 as numeric(20,4)) C_Charge--推广会上分
				,CAST(0 as numeric(20,4)) C_Cash--推广会下分
			into #aTreeTemp from AtreeCTE a	
				
			--计算所查代理/直属代理下推广会员的费用合计
			select 
				c.AgentID
				,SUM(a.WinSum) WinSum
				,SUM(a.BetSum) BetSum
				,SUM(a.BetSumAct) BetSumAct
				,SUM(a.WashFee) WashFee
				,SUM(a.WashSum_Client) WashSum
			into #cFeeTemp
			from T_BetBill a,T_WeiTou_Detail b,#aTreeTemp c 
			where c.AgentID = b.AgentID3Bao and a.ClientID = b.ClientID and a.State='#PAY' and a.StatisticsDay between @StartDate and @EndDate
			group by c.AgentID				

			insert #cFeeTemp(AgentID,WinSum,BetSum,BetSumAct,WashFee,WashSum)
					
			select 
				c.AgentID
				,SUM(a.WinSum) WinSum
				,SUM(a.BetSum) BetSum
				,SUM(a.BetSumAct) BetSumAct
				,SUM(a.WashFee) WashFee
				,SUM(a.WashSum_Client) WashSum
			from T_BetBill_Archive a (nolock),T_WeiTou_Detail b,#aTreeTemp c
			where c.AgentID = b.AgentID3Bao and a.ClientID = b.ClientID and a.State='#PAY' and a.StatisticsDay between @StartDate and @EndDate
			group by c.AgentID

			select 
				AgentID 
				,SUM(WashFee) WashFee
				,SUM(WashSum) WashSum
				,SUM(BetSum) BetSum
				,SUM(BetSumAct) BetSumAct
				,SUM(WinSum) WinSum
			into #feeTemp
			from #cFeeTemp  group by AgentID

			update a set 
				a.C_BetActS = isnull(b.BetSumAct,0)
				,a.C_BetS = isnull(b.BetSum,0)
				,a.C_TotalWin = isnull(b.WinSum,0)
				,a.C_WashF = isnull(b.WashFee,0)
				,a.C_WashS = isnull(b.WashSum,0)
			from #aTreeTemp a,#feeTemp b where a.AgentID = b.AgentID

			--计算所查代理/直属代理的三宝费
			select 
				b.AgentID AgentID3Bao
				,SUM(Amount3Bao) Amount3Bao 
			into #bFeeTemp 
			from T_WeiTou_BillWash a,#aTreeTemp b 
			where a.AgentID3Bao = b.AgentID and a.StatisticsDay between @StartDate and @EndDate
			group by b.AgentID					

			update #aTreeTemp set C_3BaoA = isnull((select Amount3Bao from #bFeeTemp where AgentID3Bao = #aTreeTemp.AgentID),0)

			--计算所查代理/直属代理下推广会员的上分合计
			select
				b.AgentID TargetID
				,SUM(Delta) Delta
			into #chTemp
			from T_Point a ,#aTreeTemp b,T_WeiTou_Detail c
			where b.AgentID = c.AgentID3Bao and a.TargetID = c.ClientID and a.OperationType='BD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.AgentID

			insert #chTemp(TargetID,Delta)

			select
				b.AgentID TargetID
				,SUM(Delta) Delta
			from T_Point_Archive a (nolock),#aTreeTemp b,T_WeiTou_Detail c 
			where b.AgentID = c.AgentID3Bao and a.TargetID = c.ClientID and a.OperationType='BD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.AgentID

			insert #chTemp(TargetID,Delta)

			select
				b.AgentID TargetID
				,SUM(Delta) Delta
			from T_Point a ,#aTreeTemp b,T_WeiTou_Detail c 
			where b.AgentID = c.AgentID3Bao and a.SourceID = c.ClientID and a.OperationType='XD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.AgentID

			insert #chTemp(TargetID,Delta)

			select
				b.AgentID TargetID
				,SUM(Delta) Delta
			from T_Point_Archive a (nolock),#aTreeTemp b,T_WeiTou_Detail c 
			where b.AgentID = c.AgentID3Bao and a.SourceID = c.ClientID and a.OperationType='XD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.AgentID

			select TargetID,SUM(Delta) Delta into #chsTemp from #chTemp group by TargetID

			update a set
				a.C_Charge =isnull( b.Delta,0)
				from #aTreeTemp a,#chsTemp b where a.AgentID= b.TargetID
				
			--计算所查代理/直属代理下推广会员的下分合计
			select
				b.AgentID TargetID
				,SUM(Delta) Delta
			into #cashTemp
			from T_Point a ,#aTreeTemp b,T_WeiTou_Detail c 
			where b.AgentID = c.AgentID3Bao and a.TargetID = c.ClientID and (a.OperationType='XD' or a.OperationType='QL' or a.OperationType='QK') 
			and a.StatisticsDay between @StartDate and @EndDate
			group by b.AgentID

			insert #cashTemp(TargetID,Delta)

			select
				b.AgentID TargetID
				,SUM(Delta) Delta
			from T_Point_Archive a (nolock),#aTreeTemp b,T_WeiTou_Detail c
			where b.AgentID = c.AgentID3Bao and a.TargetID = c.ClientID and (a.OperationType='XD' or a.OperationType='QL' or a.OperationType='QK') 
			and a.StatisticsDay between @StartDate and @EndDate
			group by b.AgentID

			insert #cashTemp(TargetID,Delta)

			select
				b.AgentID TargetID
				,SUM(Delta) Delta
			from T_Point a ,#aTreeTemp b,T_WeiTou_Detail c
			where b.AgentID = c.AgentID3Bao and a.SourceID = c.ClientID and a.OperationType='BD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.AgentID

			insert #cashTemp(TargetID,Delta)

			select
				b.AgentID TargetID
				,SUM(Delta) Delta
			from T_Point_Archive a (nolock) ,#aTreeTemp b,T_WeiTou_Detail c
			where b.AgentID = c.AgentID3Bao and a.SourceID = c.ClientID and a.OperationType='BD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.AgentID

			select TargetID,SUM(Delta) Delta into #cashsTemp from #cashTemp group by TargetID

			update a set 
				a.C_Cash = isnull(b.Delta,0)
				from #aTreeTemp a ,#cashsTemp b where a.AgentID = b.TargetID
				
			--计算总推广量
			DECLARE @Level int;
			select @Level = max(Level) from #aTreeTemp;

			while (@Level>=0) begin	
				update #aTreeTemp set 
					#aTreeTemp.A_PromT = isnull(#aTreeTemp.A_PromT,0)  +
					isnull((select sum(isnull(b.A_PromT,0)) from #aTreeTemp b where b.ParentID = #aTreeTemp.AgentID ),0)
					,#aTreeTemp.C_TotalWin = isnull(#aTreeTemp.C_TotalWin,0)  +
					isnull((select sum(isnull(b.C_TotalWin,0)) from #aTreeTemp b where b.ParentID = #aTreeTemp.AgentID ),0) 
					,#aTreeTemp.C_WashS = isnull(#aTreeTemp.C_WashS,0)  +
					isnull((select sum(isnull(b.C_WashS,0)) from #aTreeTemp b where b.ParentID = #aTreeTemp.AgentID ),0)
					,#aTreeTemp.C_WashF = isnull(#aTreeTemp.C_WashF,0)  +
					isnull((select sum(isnull(b.C_WashF,0)) from #aTreeTemp b where b.ParentID = #aTreeTemp.AgentID ),0) 
					,#aTreeTemp.C_BetS = isnull(#aTreeTemp.C_BetS,0)  +
					isnull((select sum(isnull(b.C_BetS,0)) from #aTreeTemp b where b.ParentID = #aTreeTemp.AgentID ),0) 
					,#aTreeTemp.C_BetActS = isnull(#aTreeTemp.C_BetActS,0)  +
					isnull((select sum(isnull(b.C_BetActS,0)) from #aTreeTemp b where b.ParentID = #aTreeTemp.AgentID ),0)
					,#aTreeTemp.C_3BaoA = isnull(#aTreeTemp.C_3BaoA,0)  +
					isnull((select sum(isnull(b.C_3BaoA,0)) from #aTreeTemp b where b.ParentID = #aTreeTemp.AgentID ),0)  
					,#aTreeTemp.C_Charge = isnull(#aTreeTemp.C_Charge,0)  +
					isnull((select sum(isnull(b.C_Charge,0)) from #aTreeTemp b where b.ParentID = #aTreeTemp.AgentID ),0)  
					,#aTreeTemp.C_Cash = isnull(#aTreeTemp.C_Cash,0)  +
					isnull((select sum(isnull(b.C_Cash,0)) from #aTreeTemp b where b.ParentID = #aTreeTemp.AgentID ),0)  
				where #aTreeTemp.level = @Level
				set @Level = @Level -1
			end;				

			update a set
				a.A_ProA = b.A_Prom
				,a.A_ProA_T = b.A_PromT
				,a.A_TotalWin = b.C_TotalWin
				,a.A_WashS = b.C_WashS
				,a.A_WashF = b.C_WashF
				,a.A_BetS = b.C_BetS
				,a.A_BetActS = b.C_BetActS
				,a.A_3BaoA = b.C_3BaoA
				,a.A_Charge = b.C_Charge
				,a.A_Cash = b.C_Cash
			from #agentTemp a,#aTreeTemp b where a.A_ID = b.AgentID

			select * from #agentTemp

			drop table #agentTemp
			drop table #aTreeTemp
			drop table #bFeeTemp
			drop table #cashsTemp
			drop table #cashTemp
			drop table #cFeeTemp
			drop table #chsTemp
			drop table #chTemp
			drop table #feeTemp