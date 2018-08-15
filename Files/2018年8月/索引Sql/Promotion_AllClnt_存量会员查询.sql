--当前会员存量查询
			declare @AgentID nvarchar(40)
			declare @StartDate nvarchar(20)
			declare @EndDate nvarchar(20)

			set @AgentID = '${AgentID}'
			set @StartDate ='${StartDate}'
			set @EndDate ='${EndDate}'

			set @AgentID = '52B245578A624AF4B571FF0AE6C52779'
			set @StartDate = '2018-04-01'
			set @EndDate = '2018-06-04'

			declare @StartD nvarchar(40)
			declare @EndD nvarchar(40)
			select @StartD = convert(varchar(11),@StartDate,120)+ ' ' + value from t_cfg where name = 'Use_WORKTIME_START' --归档时间
			select @EndD = convert(varchar(11),DATEADD(DAY,1,@EndDate),120)+ ' ' + value from t_cfg where name = 'Use_WORKTIME_START' --归档时间
			
			select
				ClientID A_ID
				,ClientName A_Name
				,LogName A_UserID
				,'SelfC' A_Type
				,AgentID P_ID
				,0 A_ProA_T
				,(select COUNT(*) from T_WeiTou_Detail b,T_Client c where b.InviterID = a.ClientID and b.ClientID = c.ClientID ) A_ProA --and c.CreateTime between @StartD and @EndD
				,(CAST(0 as numeric(20,2))) A_TotalWin
				,(CAST(0 as numeric(20,2))) A_TotalWin_Self
				,(CAST(0 as numeric(20,2))) A_WashS
				,(CAST(0 as numeric(20,2))) A_WashS_Self
				,(CAST(0 as numeric(20,2))) A_WashF
				,(CAST(0 as numeric(20,2))) A_WashF_Self
				,(CAST(0 as numeric(20,2))) A_BetS
				,(CAST(0 as numeric(20,2))) A_BetS_Self
				,(CAST(0 as numeric(20,2))) A_BetActS
				,(CAST(0 as numeric(20,2))) A_BetActS_Self
				,(CAST(0 as numeric(20,2))) A_3BaoA
				,(select SUM(Amount3Bao) from T_WeiTou_BillWash d where d.ClientID = @AgentID and a.CreateTime between @StartD and @EndD) A_3BaoA_Self
				,(CAST(0 as numeric(20,2))) A_Charge
				,(CAST(0 as numeric(20,2))) A_Charge_Self
				,(CAST(0 as numeric(20,2))) A_Cash
				,(CAST(0 as numeric(20,2))) A_Cash_Self
				,CreateTime A_CreTime
			into #agentTemp
			from T_Client a where ClientID=@AgentID

			;WITH AgentCTE (ClientID,InviterID,InviterLogName,TopClientID,IsTop)AS(
				SELECT ClientID,InviterID,InviterLogName, ClientID,1
					FROM T_WeiTou_Detail --a,#agentTemp b  
				where InviterID=@AgentID or ClientID=@AgentID--a.InviterID = b.A_ID or a.ClientID = b.A_ID
				UNION ALL
				SELECT a.ClientID,a.InviterID,a.InviterLogName,cte.TopClientID,0 
					FROM T_WeiTou_Detail a JOIN AgentCTE cte ON a .InviterID = cte.ClientID --join T_Client c on a.ClientID = c.ClientID and c.CreateTime between @StartD and @EndD
			)
			select * into #IDTemp from AgentCTE 
			
			--select * from #IDTemp
			select TopClientID,COUNT(TopClientID) couts into #proTemp2 from #IDTemp a,T_Client c where a.ClientID = c.ClientID and IsTop =0  group by TopClientID --and c.CreateTime between @StartD and @EndD
			
			update #agentTemp set A_ProA_T =isnull((select couts from #proTemp2 where  TopClientID = #agentTemp.A_ID),0)

			--update #agentTemp set A_3BaoA = isnull((select SUM(Amount3Bao) from T_WeiTou_BillWash where InviterID = @AgentID and StatisticsDay between @StartDate and @EndDate),0)

			--update #agentTemp set A_ProA = isnull((select COUNT(*) from T_WeiTou_Detail where InviterID = @AgentID),0)

			select 
				b.TopClientID
				,SUM(Amount3Bao) Amount3Bao
				into #feeTemp
				from T_WeiTou_BillWash a,#IDTemp b
				where a.ClientID = b.ClientID and a.StatisticsDay between @StartDate and @EndDate
				group by b.TopClientID

			update a set
				a.A_3BaoA = ISNULL(b.Amount3Bao,0)
				--,a.A_WashF = isnull(b.WashFee,0)
				--,a.A_WashS = isnull(b.WashSum,0)
			from #agentTemp a ,#feeTemp b where a.A_ID = b.TopClientID

			select 
				b.TopClientID
				,SUM(WashFee) WashFee
				,SUM(WashSum_Client) WashSum
				,SUM(WinSum) WinSum
				,SUM(BetSum) BetSum
				,SUM(BetSumAct) BetSumAct
			into #winTemp
			from T_BetBill a,#IDTemp b
			where a.ClientID= b.ClientID and State ='#PAY' and StatisticsDay between @StartDate and @EndDate 
			group by b.TopClientID

			insert #winTemp(TopClientID,WashFee,WashSum,WinSum,BetSum,BetSumAct)

			select 
				b.TopClientID
				,SUM(WashFee) WashFee
				,SUM(WashSum_Client) WashSum
				,SUM(WinSum) WinSum
				,SUM(BetSum) BetSum
				,SUM(BetSumAct) BetSumAct
			from T_BetBill_Archive a(nolock),#IDTemp b
			where a.ClientID= b.ClientID and State ='#PAY' and StatisticsDay between @StartDate and @EndDate 
			group by b.TopClientID

			select 
				TopClientID
				,SUM(WashFee) WashFee 
				,SUM(WashSum) WashSum
				,SUM(WinSum) WinSum
				,SUM(BetSum) BetSum
				,SUM(BetSumAct) BetSumAct
			into #winTemp_A from #winTemp group by TopClientID


			update a set a.A_TotalWin =ISNULL(c.WinSum,0)
						,a.A_BetActS = ISNULL(c.BetSumAct,0)
						,a.A_BetS = ISNULL(c.BetSum,0)
						,a.A_WashF =ISNULL(c.WashFee,0)
						,a.A_WashS = ISNULL(c.WashSum,0)
			  from #agentTemp a,#winTemp_A c where a.A_ID = c.TopClientID

			select  
				SUM(WinSum) WinSum
				,SUM(BetSum) BetSum
				,SUM(BetSumAct) BetSumAct
				,SUM(WashFee) WashFee
				,SUM(WashSum_Client) WashSum
			into #selfFeeTemp
 			from T_BetBill a--,T_Client b
			where a.ClientID= @AgentID --and a.ClientID = b.ClientID and b.CreateTime between @StartD and @EndD 
			and a.State='#PAY' and a.StatisticsDay between @StartDate and @EndDate

			insert #selfFeeTemp(WinSum,BetSum,BetSumAct,WashFee,WashSum)

			select  
				SUM(WinSum) WinSum
				,SUM(BetSum) BetSum
				,SUM(BetSumAct) BetSumAct
				,SUM(WashFee) WashFee
				,SUM(WashSum_Client) WashSum
 			from T_BetBill_Archive a(nolock)--,T_Client b
			where a.ClientID= @AgentID-- and a.ClientID= b.ClientID and b.CreateTime between @StartD and @EndD 
			and a.State='#PAY' and a.StatisticsDay between @StartDate and @EndDate

			select SUM(WashFee) WashFee,SUM(WashSum) WashSum,SUM(WinSum) WinSum,SUM(BetSum) BetSum,SUM(BetSumAct) BetSumAct into #sumSelfFeeTemp from #selfFeeTemp

			update a set
				a.A_TotalWin_Self = isnull(b.WinSum,0)
				,a.A_BetActS_Self = isnull(b.BetSumAct,0)
				,a.A_BetS_Self = isnull(b.BetSum,0)
				,a.A_WashF_Self = isnull(b.WashFee,0)
				,a.A_WashS_Self = isnull(b.WashSum,0)
			from #agentTemp a,#sumSelfFeeTemp b

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			into #chTemp
			from T_Point a ,#IDTemp b
			where a.TargetID = b.ClientID and a.OperationType='BD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			insert #chTemp(TargetID,Delta)

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			from T_Point_Archive a (nolock),#IDTemp b
			where a.TargetID =b.ClientID and a.OperationType='BD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			insert #chTemp(TargetID,Delta)

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			from T_Point a ,#IDTemp b
			where a.SourceID =b.ClientID and a.OperationType='XD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			insert #chTemp(TargetID,Delta)

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			from T_Point_Archive a (nolock),#IDTemp b
			where a.SourceID =b.ClientID and a.OperationType='XD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			select TargetID,SUM(Delta) Delta into #chsTemp from #chTemp group by TargetID

			update a set
				a.A_Charge = isnull(b.Delta,0)
			from #agentTemp a,#chsTemp b where a.A_ID= b.TargetID

			select 
				SUM(Delta) Delta
			into #selfChTemp
			from T_Point a--,T_Client b 
			where a.TargetID = @AgentID --and a.TargetID= b.ClientID and b.CreateTime between @StartD and @EndD
			and a.StatisticsDay between @StartDate and @EndDate and a.OperationType='BD'
			
			insert #selfChTemp(Delta)

			select 
				SUM(Delta) Delta
			from T_Point_Archive a(nolock)--,T_Client b 
			where a.TargetID = @AgentID --and a.TargetID= b.ClientID and b.CreateTime between @StartD and @EndD
			and a.StatisticsDay between @StartDate and @EndDate and a.OperationType='BD'
			
			insert #selfChTemp(Delta)

			select 
				SUM(Delta) Delta
			from T_Point a --,T_Client b 
			where a.SourceID = @AgentID --and a.TargetID= b.ClientID and b.CreateTime between @StartD and @EndD
			and a.StatisticsDay between @StartDate and @EndDate and a.OperationType='XD'
			
			insert #selfChTemp(Delta)

			select 
				SUM(Delta) Delta
			from T_Point_Archive a(nolock)--,T_Client b 
			where a.SourceID = @AgentID --and a.TargetID= b.ClientID and b.CreateTime between @StartD and @EndD
			and a.StatisticsDay between @StartDate and @EndDate and a.OperationType='XD'
			
			select SUM(Delta) Delta into #sumSelfChTemp from #selfChTemp

			update a set a.A_Charge_Self = ISNULL(b.Delta,0)  from #agentTemp a,#sumSelfChTemp b

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			into #cashTemp
			from T_Point a ,#IDTemp b
			where a.TargetID = b.ClientID and (a.OperationType='XD' or a.OperationType='QL' or a.OperationType='QK') and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			insert #cashTemp(TargetID,Delta)

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			from T_Point_Archive a (nolock),#IDTemp b
			where a.TargetID = b.ClientID and (a.OperationType='XD' or a.OperationType='QL' or a.OperationType='QK') and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			insert #cashTemp(TargetID,Delta)

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			from T_Point a,#IDTemp b
			where a.SourceID = b.ClientID and a.OperationType='BD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			insert #cashTemp(TargetID,Delta)

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			from T_Point_Archive a (nolock),#IDTemp b
			where a.SourceID = b.ClientID and a.OperationType='BD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID
			
			select TargetID,SUM(Delta) Delta into #cashsTemp from #cashTemp group by TargetID

			update a set 
				a.A_Cash =isnull( b.Delta,0)
			from #agentTemp a ,#cashsTemp b where a.A_ID = b.TargetID

			select 
				SUM(Delta) Delta
			into #selfCashTemp
			from T_Point a--,T_Client b
			where a.TargetID= @AgentID and (a.OperationType='XD' or a.OperationType='QL' or a.OperationType='QK') 
			and a.StatisticsDay between @StartDate and @EndDate --and a.TargetID= b.ClientID 
			--and b.CreateTime between @StartD and @EndD

			insert #selfCashTemp(Delta)

			select 
				SUM(Delta) Delta
			from T_Point_Archive a(nolock)--,T_Client b
			where a.TargetID= @AgentID and (a.OperationType='XD' or a.OperationType='QL' or a.OperationType='QK') 
			and a.StatisticsDay between @StartDate and @EndDate --and a.TargetID= b.ClientID 
			--and b.CreateTime between @StartD and @EndD

			insert #selfCashTemp(Delta)

			select 
				SUM(Delta) Delta
			from T_Point a --,T_Client b
			where a.SourceID= @AgentID and a.OperationType='BD'
			and a.StatisticsDay between @StartDate and @EndDate --and a.TargetID= b.ClientID 
			--and b.CreateTime between @StartD and @EndD

			insert #selfCashTemp(Delta)

			select 
				SUM(Delta) Delta
			from T_Point_Archive a(nolock)--,T_Client b
			where a.SourceID= @AgentID and a.OperationType='BD'
			and a.StatisticsDay between @StartDate and @EndDate --and a.TargetID= b.ClientID 
			--and b.CreateTime between @StartD and @EndD

			select SUM(Delta) Delta into #sumSelfCashTemp from #selfCashTemp

			update a set a.A_Cash_Self =ISNULL( b.Delta,0) from #agentTemp a,#sumSelfCashTemp b

			select * from #agentTemp

			drop table #cashsTemp
			drop table #cashTemp
			drop table #chsTemp
			drop table #chTemp
			drop table #feeTemp
			drop table #agentTemp
			drop table #proTemp2
			drop table #winTemp
			drop table #winTemp_A
			drop table #IDTemp
			drop table #selfFeeTemp
			drop table #sumSelfFeeTemp
			drop table #selfChTemp
			drop table #sumSelfChTemp
			drop table #sumSelfCashTemp
			drop table #selfCashTemp