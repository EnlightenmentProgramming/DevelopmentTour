--总存量查询
			declare @AgentID nvarchar(40)
			declare @StartDate nvarchar(20)
			declare @EndDate nvarchar(20)
			declare @pageSize int
			declare @curePage int
			declare @TotalRecords int
			declare @SubAgent int --指定代理下直属代理数量
			declare @SubClent int --指定代理直接邀请会员数量
			declare @LeftOver int --根据查询条件中的CurPage和PageSize计算出除了直属代理还需要查询的数据条数

			--set @AgentID = '${AgentID}'
			--set @StartDate ='${StartDate}'
			--set @EndDate ='${EndDate}'
			--set @pageSize ='${pageSize}'
			--set @curePage ='${curePage}'

			set @AgentID ='c4f71884dd954ec69cd7b0f8451db7e0'
			set @StartDate ='2018-01-01'
			set @EndDate ='2018-12-12'
			set @pageSize ='15'
			set @curePage ='1'

			declare @StartD nvarchar(40) --将查询时间转化成包含时分秒格式的
			declare @EndD nvarchar(40) --将查询时间转化成包含时分秒格式的
			select @StartD = convert(varchar(11),@StartDate,120)+ ' ' + value from t_cfg where name = 'Use_WORKTIME_START' --归档时间
			select @EndD = convert(varchar(11),DATEADD(DAY,1,@EndDate),120)+ ' ' + value from t_cfg where name = 'Use_WORKTIME_START' --归档时间
			
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
			
			--递归查找会员邀请关系
			;WITH AgentCTE (ClientID,InviterID,InviterLogName,TopClientID,IsTop)AS(
				SELECT ClientID,InviterID,InviterLogName, ClientID, 1 
					FROM T_WeiTou_Detail 
					where InviterID= @AgentID or ClientID = @AgentID 
				UNION ALL
				SELECT a.ClientID,a.InviterID,a.InviterLogName,cte.TopClientID, 0 
					FROM T_WeiTou_Detail a JOIN AgentCTE cte ON a .InviterID = cte.ClientID 
			)
			select a.* into #IDTemp1  from AgentCTE a--,T_Client c where a.ClientID = c.ClientID  and c.CreateTime between @StartD and @EndD 
				 
			select TopClientID,COUNT(TopClientID) couts into #proTemp1 from #IDTemp1 where IsTop=0 group by TopClientID

			--查询直接邀请会员列表
			select 
				a.ClientID C_ID
				,c.ClientName C_Name
				,a.logName C_UserID
				,'OwnC' C_Type
				,InviterID C_InID
				,InviterLogName C_InUserID
				,InviterType C_InT
				,AgentID3Bao C_AID
				,(select COUNT(*) from T_WeiTou_Detail b,T_Client d where b.InviterID = a.ClientID and b.ClientID = d.ClientID and d.CreateTime between @StartD and @EndD) C_ProA --推广量
				,isnull((select couts from #proTemp1 where TopClientID = a.ClientID),0) C_ProA_T --总推广量
				,CAST(0 as numeric(20,4)) C_TotalWin --推广会员总赢
				,CAST(0 as numeric(20,4)) C_WashS --推广会员洗码量
				,CAST(0 as numeric(20,4)) C_WashF --推广会员洗码费
				,CAST(0 as numeric(20,4)) C_BetS
				,CAST(0 as numeric(20,4)) C_BetActS 
				,CAST(0 as numeric(20,4)) C_3BaoA --推广会三宝费
				,CAST(0 as numeric(20,4)) C_Charge--推广会上分
				,CAST(0 as numeric(20,4)) C_Cash--推广会下分
				,CAST(0 as numeric(20,4)) C_TotalWin_Self --推广会员总赢
				,CAST(0 as numeric(20,4)) C_WashS_Self --推广会员洗码量
				,CAST(0 as numeric(20,4)) C_WashF_Self --推广会员洗码费
				,CAST(0 as numeric(20,4)) C_BetS_Self
				,CAST(0 as numeric(20,4)) C_BetActS_Self 
				,(select SUM(Amount3Bao) from T_WeiTou_BillWash t ,T_Client tt where a.ClientID = t.ClientID and t.ClientID= tt.ClientID and tt.CreateTime between @StartD and @EndD) C_3BaoA_Self --推广会三宝费
				,CAST(0 as numeric(20,4)) C_Charge_Self--推广会上分
				,CAST(0 as numeric(20,4)) C_Cash_Self--推广会下分
				,c.CreateTime C_CreTime
			into #ClntTemp
			from T_WeiTou_Detail a  left join T_Client c on a.ClientID= c.ClientID
			where InviterID = @AgentID

			--查询直属代理列表
			select 
				AgentID C_ID
				,AgentName C_Name
				,logName C_UserID
				,'OwnA' C_Type
				,'' C_InID
				,'' C_InUserID
				,'' C_InT
				,ParentID C_AID
				,CAST(0 as int) C_ProA --推广量(select A_Prom from #aTreeTemp b where a.AgentID = b.AgentID)
				,CAST(0 as int) C_ProA_T --总推广量 (select A_PromT from #aTreeTemp b where a.AgentID = b.AgentID)
				,CAST(0 as numeric(20,4)) C_TotalWin --推广会员总赢
				,CAST(0 as numeric(20,4)) C_WashS --推广会员洗码量
				,CAST(0 as numeric(20,4)) C_WashF --推广会员洗码费
				,CAST(0 as numeric(20,4)) C_BetS
				,CAST(0 as numeric(20,4)) C_BetActS 
				,CAST(0 as numeric(20,4)) C_3BaoA --推广会三宝费
				,CAST(0 as numeric(20,4)) C_Charge--推广会上分
				,CAST(0 as numeric(20,4)) C_Cash--推广会下分
				,CAST(0 as numeric(20,4)) C_TotalWin_Self --推广会员总赢
				,CAST(0 as numeric(20,4)) C_WashS_Self --推广会员洗码量
				,CAST(0 as numeric(20,4)) C_WashF_Self --推广会员洗码费
				,CAST(0 as numeric(20,4)) C_BetS_Self
				,CAST(0 as numeric(20,4)) C_BetActS_Self 
				,CAST(0 as numeric(20,4)) C_3BaoA_Self --推广会三宝费
				,CAST(0 as numeric(20,4)) C_Charge_Self--推广会上分
				,CAST(0 as numeric(20,4)) C_Cash_Self--推广会下分
				,CreateTime C_CreTime
			into #AgentTemp
			from T_Agent a
			where ParentID = @AgentID and IsHide = 'FALSE'

			set @SubAgent = (select COUNT(*) from #AgentTemp)

			select @SubClent = (select COUNT(*) from #ClntTemp)

			if ((@pageSize*(@curePage-1)>@SubAgent+@SubClent and (@SubAgent+@SubClent)>0 and @curePage>1) or (@SubAgent+@SubClent)<=0)
			begin
				raiserror('没有找到数据！', 16, 1)
				return
			end

			if @SubAgent>= @pageSize*@curePage
			begin
				set @LeftOver = 0
			end else 
			begin
				set @LeftOver = @pageSize*@curePage - @SubAgent
			end			
				
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
				a.C_ProA = b.A_Prom
				,a.C_ProA_T = b.A_PromT
			from #AgentTemp a, #aTreeTemp b where a.C_ID = b.AgentID

			SELECT TOP (@pageSize) * into #targetTemp 	
			From (
				SELECT TOP (@pageSize * @curePage) ROW_NUMBER() OVER (order by  C_ProA desc ,C_ProA_T desc) AS rowNo
					,CAST(0 as int) TotalRecords
					,*
				From #AgentTemp
			)as a
			WHERE rowNo BETWEEN (((@curePage - 1) * @pageSize) + 1)
			AND (@pageSize * @curePage)
			order by  C_ProA desc ,	C_ProA_T desc

			update a set  
				a.C_TotalWin = b.C_TotalWin
				,a.C_WashS = b.C_WashS
				,a.C_WashF = b.C_WashF
				,a.C_BetS = b.C_BetS
				,a.C_BetActS = b.C_BetActS
				,a.C_3BaoA = b.C_3BaoA
				,a.C_Charge = b.C_Charge
				,a.C_Cash = b.C_Cash
			from #targetTemp a,#aTreeTemp b where a.C_ID = b.AgentID

			update #targetTemp set TotalRecords = @SubAgent

			if @LeftOver < = 0 --指定代理的直属会员数量已经满足当前页查询所需数据条数
			begin
				select * from #targetTemp
			end else begin  --指定代理的直属会员数量不满足当前页查询所需数据条数,还需继续加上指定代理直接邀请会员数据
				declare @tempPageSize int  --计算出直接邀请会员每页条数
				declare @tempCurePage int  --计算出直接邀请会员当前页
				declare @beginNum int --
				declare @endNum int
								
				if @subAgent <= 0 --根据查询条件的PageSize 和CurePage 以及直属代理数量计算所需要查询直接邀请会员情况
				begin
					set @tempCurePage = @CurePage  
					set @tempPageSize =@pageSize
					set @beginNum = (((@tempCurePage-1)*@tempPageSize)+1)
					set @endNum = (@tempCurePage*@tempPageSize)
				end else
				begin
					set @tempCurePage = (@CurePage -(@SubAgent/@pageSize))
					if @SubAgent % @pageSize !=0
					begin
						if @tempCurePage >1
						begin
							set @tempPageSize =@pageSize
							set @beginNum = ((@pageSize - (@SubAgent % @pageSize)) + ((@tempCurePage-2)*@tempPageSize)+1)
							set @endNum = (@beginNum + @tempPageSize)
						end else
						begin
							set @tempPageSize = @pageSize - (@SubAgent % @pageSize)	
							set @beginNum = 1
							set @endNum = @tempPageSize
						end						
					end else
					begin						
						set @tempPageSize =@pageSize
						set @beginNum = (((@tempCurePage-1)*@tempPageSize)+1)
						set @endNum = (@tempCurePage*@tempPageSize)
					end					
				end				
				 
				if @tempPageSize <= 0
				begin
					set @tempPageSize = 1
				end
				
				if @tempCurePage <= 0
				begin
					set @tempCurePage = 1
				end							

				SELECT TOP (@tempPageSize) * into #targetTemp1 	 	
					From (
						SELECT TOP (@tempPageSize*@tempCurePage) ROW_NUMBER() OVER (order by  C_ProA desc,C_ProA_T desc) AS rowNo
				,CAST(0 as int) TotalRecords
					,*
					From #ClntTemp
				)as a
			WHERE rowNo BETWEEN @beginNum--(((@tempCurePage - 1) * @tempPageSize) + 1)
			AND @endNum--(@tempPageSize * @tempCurePage)--@LeftOver--
			order by  C_ProA desc,C_ProA_T desc		

			select 
				b.TopClientID ClientID
				,SUM(Amount3Bao) Amount3Bao	
				into #feeTemp1
				from T_WeiTou_BillWash a ,#IDTemp1 b
				where a.ClientID = b.ClientID and a.StatisticsDay between @StartDate and @EndDate
				group by b.TopClientID

			update a set 
				a.C_3BaoA = isnull(b.Amount3Bao,0)
				from #targetTemp1 a,#feeTemp1 b  
				where a.C_ID = b.ClientID 

			select 
				b.TopClientID ClientID				
				,SUM(WashFee) WashFee
				,SUM(WashSum_Client) WashSum
				,SUM(a.WinSum) WinSum
				,SUM(a.BetSum) BetSum
				,SUM(a.BetSumAct) BetSumAct
			into #winTemp1
			from T_BetBill  a,#IDTemp1 b
			where a.ClientID = b.ClientID and a.State='#PAY' and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			insert #winTemp1(ClientID,WashFee,WashSum,WinSum,BetSum,BetSumAct)

			select 
				b.TopClientID ClientID
				,SUM(WashFee) WashFee
				,SUM(WashSum_Client) WashSum
				,SUM(a.WinSum) WinSum
				,SUM(a.BetSum) BetSum
				,SUM(a.BetSumAct) BetSumAct
			from T_BetBill_Archive a(nolock),#IDTemp1 b 
			where a.ClientID = b.ClientID and a.State='#PAY' and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			select 
				ClientID
				,SUM(WashFee) WashFee 
				,SUM(WashSum) WashSum
				,SUM(WinSum) WinSum 
				,SUM(BetSum) BetSum
				,SUM(BetSumAct) BetSumAct
				into #winSTemp1
				from #winTemp1 group by ClientID

			update a set
				a.C_TotalWin = isnull(b.WinSum,0)
				,a.C_WashF = isnull(b.WashFee,0)
				,a.C_WashS = isnull(b.WashSum,0)
				,a.C_BetS = isnull(b.BetSum,0)
				,a.C_BetActS = isnull(b.BetSumAct,0)
			from #targetTemp1 a,#winSTemp1 b where a.C_ID = b.ClientID

			select 
				a.ClientID
				,SUM(WinSum) WinSum
				,SUM(WashFee) WashFee
				,SUM(WashSum_Client) WashSum
				,SUM(BetSum) BetSum
				,SUM(BetSumAct) BetSumAct
			into #selfWTemp
			from T_BetBill a,#IDTemp1 b
			where a.ClientID = b.ClientID and b.IsTop=1 and a.State='#PAY' and a.StatisticsDay between @StartDate and @EndDate
			group by a.ClientID

			insert #selfWTemp(ClientID,WinSum,WashFee,WashSum,BetSum,BetSumAct)

			select 
				a.ClientID
				,SUM(WinSum) WinSum
				,SUM(WashFee) WashFee
				,SUM(WashSum_Client) WashSum
				,SUM(BetSum) BetSum
				,SUM(BetSumAct) BetSumAct
			from T_BetBill_Archive a,#IDTemp1 b
			where a.ClientID = b.ClientID and b.IsTop=1 and a.State='#PAY' and a.StatisticsDay between @StartDate and @EndDate 
			group by a.ClientID

			select 
				ClientID
				,SUM(WinSum) WinSum
				,SUM(WashFee) WashFee
				,SUM(WashSum) WashSum
				,SUM(BetSum) BetSum
				,SUM(BetSumAct) BetSumAct
			into #sumSelfWTemp
			from #selfWTemp group by ClientID

			update a set  
				a.C_TotalWin_Self = isnull(b.WinSum,0)
				,a.C_WashF_Self = isnull(b.WashFee,0)
				,a.C_WashS_Self = isnull(b.WashSum,0)
				,a.C_BetActS_Self = isnull(b.BetSumAct,0)
				,a.C_BetS_Self = isnull(b.BetSum,0)
			from #targetTemp1 a,#sumSelfWTemp b where a.C_ID = b.ClientID

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			into #chTemp1
			from T_Point a ,#IDTemp1 b
			where a.TargetID = b.ClientID and a.OperationType='BD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			insert #chTemp1(TargetID,Delta)

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			from T_Point_Archive a (nolock),#IDTemp1 b 
			where a.TargetID = b.ClientID and a.OperationType='BD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			insert #chTemp1(TargetID,Delta)

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			from T_Point a,#IDTemp1 b 
			where a.SourceID = b.ClientID and a.OperationType='XD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			insert #chTemp1(TargetID,Delta)

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			from T_Point_Archive a (nolock),#IDTemp1 b 
			where a.SourceID = b.ClientID and a.OperationType='XD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			select TargetID,SUM(Delta) Delta into #chsTemp1 from #chTemp1 group by TargetID

			update a set
				a.C_Charge = isnull(b.Delta,0)
			from #targetTemp1 a,#chsTemp1 b where a.C_ID= b.TargetID

			select 
				a.TargetID
				,SUM(Delta) Delta
			into #selfChTemp
			from T_Point a,#IDTemp1 b
			where a.TargetID = b.ClientID and b.IsTop=1 and a.OperationType='BD' and a.StatisticsDay between @StartDate and @EndDate
			group by a.TargetID

			insert #selfChTemp(TargetID,Delta)
			
			select 
				a.TargetID
				,SUM(Delta) Delta
			from T_Point_Archive a(nolock),#IDTemp1 b
			where a.TargetID = b.ClientID and b.IsTop=1 and a.OperationType='BD' and a.StatisticsDay between @StartDate and @EndDate
			group by a.TargetID

			insert #selfChTemp(TargetID,Delta)
			
			select 
				a.SourceID
				,SUM(Delta) Delta
			from T_Point a,#IDTemp1 b
			where a.SourceID = b.ClientID and b.IsTop=1 and a.OperationType='XD' and a.StatisticsDay between @StartDate and @EndDate
			group by a.SourceID

			insert #selfChTemp(TargetID,Delta)
			
			select 
				a.SourceID
				,SUM(Delta) Delta
			from T_Point_Archive a(nolock),#IDTemp1 b
			where a.SourceID = b.ClientID and b.IsTop=1 and a.OperationType='XD' and a.StatisticsDay between @StartDate and @EndDate
			group by a.SourceID

			select TargetID,SUM(Delta) Delta into #sumSelfChTemp from #selfChTemp group by TargetID

			update a set a.C_Charge_Self = ISNULL(b.Delta,0) from #targetTemp1 a,#sumSelfChTemp b where a.C_ID = b.TargetID

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			into #cashTemp1
			from T_Point a ,#IDTemp1 b
			where a.TargetID = b.ClientID and (a.OperationType='XD' or a.OperationType='QL' or a.OperationType='QK') and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			insert #cashTemp1(TargetID,Delta)

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			from T_Point_Archive a(nolock) ,#IDTemp1 b
			where a.TargetID = b.ClientID and (a.OperationType='XD' or a.OperationType='QL' or a.OperationType='QK') and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			insert #cashTemp1(TargetID,Delta)

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			from T_Point a ,#IDTemp1 b
			where a.SourceID = b.ClientID and a.OperationType='BD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			insert #cashTemp1(TargetID,Delta)

			select
				b.TopClientID TargetID
				,SUM(Delta) Delta
			from T_Point_Archive a(nolock) ,#IDTemp1 b
			where a.SourceID = b.ClientID and a.OperationType='BD' and a.StatisticsDay between @StartDate and @EndDate
			group by b.TopClientID

			select TargetID,SUM(Delta) Delta into #cashsTemp1 from #cashTemp1 group by TargetID

			update a set 
				a.C_Cash = isnull(b.Delta,0)
				from #targetTemp1 a ,#cashsTemp1 b where a.C_ID = b.TargetID
			
			select 
				a.TargetID
				,SUM(Delta) Delta
			into #selfCashTemp
			from T_Point a,#IDTemp1 b 
			where a.TargetID = b.ClientID and b.IsTop=1 and (a.OperationType='XD' or a.OperationType='QL' or a.OperationType='QK')
			and a.StatisticsDay between @StartDate and @EndDate
			group by a.TargetID

			insert #selfCashTemp(TargetID,Delta)

			select 
				a.TargetID
				,SUM(Delta) Delta
			from T_Point_Archive a(nolock),#IDTemp1 b 
			where a.TargetID = b.ClientID and b.IsTop=1 and (a.OperationType='XD' or a.OperationType='QL' or a.OperationType='QK')
			and a.StatisticsDay between @StartDate and @EndDate
			group by a.TargetID

			insert #selfCashTemp(TargetID,Delta)

			select 
				a.SourceID
				,SUM(Delta) Delta
			from T_Point a,#IDTemp1 b 
			where a.SourceID = b.ClientID and b.IsTop=1 and a.OperationType='BD'
			and a.StatisticsDay between @StartDate and @EndDate
			group by a.SourceID

			insert #selfCashTemp(TargetID,Delta)

			select 
				a.SourceID
				,SUM(Delta) Delta
			from T_Point_Archive a(nolock),#IDTemp1 b 
			where a.SourceID = b.ClientID and b.IsTop=1 and a.OperationType='BD'
			and a.StatisticsDay between @StartDate and @EndDate
			group by a.SourceID

			select TargetID,SUM(Delta) Delta into #sumSelfCashTemp from #selfCashTemp group by TargetID

			update a set a.C_Cash_Self = ISNULL(Delta,0) from #targetTemp1 a,#sumSelfCashTemp b where a.C_ID = b.TargetID			
			
			update #targetTemp1 set TotalRecords = @SubAgent + @SubClent
			
			update #targetTemp set TotalRecords = @SubAgent + @SubClent			
			
			select * from #targetTemp
			union all
			select * from #targetTemp1			
		
			drop table #selfChTemp
			drop table #sumSelfChTemp
			drop table #cashTemp1
			drop table #cashsTemp1
			drop table #selfCashTemp
			drop table #sumSelfCashTemp
			drop table #chsTemp1
			drop table #chTemp1
			drop table #feeTemp1
			drop table #targetTemp1
			drop table #winSTemp1
			drop table #winTemp1
			drop table #selfWTemp
			drop table #sumSelfWTemp
		end
		drop table #AgentTemp
		drop table #aTreeTemp
		drop table #ClntTemp
		drop table #IDTemp1
		drop table #proTemp1
		drop table #targetTemp 
		drop table #cFeeTemp
		drop table #feeTemp
		drop table #bFeeTemp
		drop table #cashsTemp
		drop table #cashTemp
		drop table #chsTemp
		drop table #chTemp