# 代理后台交互说明

> 1. 本服务在.Net FrameWork4.5下使用C#语言开发,使用了简单三层架构。 
> 2. 开发工具使用vs2015。
> 3. 使用了Dos.ORM框架与SQL Server交互 
> 4. 使用了SQLite存储了服务通信IP与端口等基本配置
> 5. 本服务使用websocket进行通信，报文数据格式为JSON
> 6. 生产环境中使用了RSA对配置文件"ConnetionString节点"进行加密 
> 7. 使用Windows窗体应用程序简单的进行服务程序监管与设置 

## 具体接口及必要逻辑说明如下

### 一、报文简介

> 1 此服务默认是以JSON字符串进行通信，同时也预留配置，在特殊情况下可指定以其他报文格式进行通信
>
> 2 此服务默认会对所有传输的JSON字符串进行"GZip压缩"，同时也支持指定某个交易不压缩报文进行传输
>
> 3 所有的交易都是请求应答一一对应，均由以下元素组成：
>
> * 包头数据(Head): 包含登录认证、登录账号信息、接口名称、客户端IP
> * 请求数据(Request):根据具体交易进行定义
> * 应答数据(Response):根据具体交易进行定义
> * 结果信息(Error):包含结果代码、结果描述
>
> ```json
> {
>     Head:
>     {
>         Account:"登录账号",
>         LoginID:"登录ID",
>         Token:"登录认证",
>         Ip:"客户端IP",
>         Method:"交易名称"
>     },
>     Request:
>     {
>         IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
>         RType:"应答报文数据格式 默认是Json格式",
>         CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
>         CVer:"客户端版本号",
>         RequestParams:"请求参数Json字符串"//根据具体交易进行指定参数
>     },
>     Response:{},//根据具体交易进行指定数据内容
>     Error://结果信息
>     {
>         ErrNo:"结果代码",//应答任何交易都会包含此参数，客户端可根据此参数做相应的逻辑处理
>         ErrMsg:"结果描述"
>     }
> }
> ```
>
> > 1. Account、LoginID、Token三个参数，除了"登录(Login)"和"心跳(HeartBeat)"交易以外的所有交易必填
> >
> > 2. Token参数是登录成功之后由服务端返回
> >
> > 3. 如果客户端与服务端之间有中转则必须提供IP参数
> >
> > 4. 任何交易必须指定交易名称(Method)参数
> >
> > 5.  结果代码说明
> >
> >    * "0000": 操作成功
> >    * "0001": 系统内部错误
> >    * "0002": 系统繁忙稍后再试
> >    * "0003": 报文格式错误
> >    * "0004": 指定错误
> >    * "0005": 未知错误
> >
> > 6. 在获取分页数据时
> >
> >    * PageSize: 如果未对此参数赋值或者赋值为负数，则服务端会重新将此参数赋值为20
> >    * CurePage: 如果未对此参数赋值或者赋值为负数，则服务端会重新将此参数赋值为1
> >
> > 7. 在获取指定时段内数据时，如果未对时间StartDate、EndDate参数赋值，则服务端会将时间赋值为当天时间
> >
> > 8. 在获取代理或会员相关的任何数据时，服务端首先会判断要获取的代理或会员是否在当前登录代理分支下
> >

### 二、具体接口及逻辑说明

#### 1. 心跳

* 接口名称: HeartBeat

* 请求报文

  ```json
  {
   	Head:
      {
          Method:"HeartBeat"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号"
      }   
  }
  ```

* 应答报文

  ```json
  {
      Head:
      {
          Method:"HeartBeat"
      },
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"连接正常"
      }
  }
  ```

* 接口说明: 当客户端连接成功之后，按照特定时间频率进行心跳请求，以检验连接有效性

#### 2. 登录

* 接口名称: Login 

* 接口说明: 除了心跳，其他所有交易必须在登录成功的基础上进行；如果此账户已登录则向之前登录发送踢人动作，并断开之前连接，所以客户端做断线重连需要排除此种断线情况

  ```json
  {
      Head:
      {
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GoOut"
      },
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"此账号已在别处登录"
      }
  }
  ```

  

* 请求报文

  ```json
  {
   	Head:
      {
          Method:"Login"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_UserID:"登录账号",
          	A_Pwd:"登录密码"//客户端需要进行MD5加密
      	}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"Login",
          Token:"登录成功后，服务端生成唯一标识"
      },
      Response:
      {
          JsonData:
          [{
              A_ID:"登陆代理ID",
              A_UserID:"代理登陆ID",
              A_Name:"代理名称",
              A_PID:"父级代理ID",
              A_State:"状态",
              A_Sub:"是否子账号",
              A_MX_Z:"庄最大限红",
              A_MN_Z:"庄最小限红",
              A_MX_X:"闲最大限红",
              A_MN_X:"闲最小限红",
              A_MX_H:"和最大限红",
              A_MN_H:"和最小限红",
              A_MX_ZD:"庄对最大限红",
              A_MN_ZD:"庄对最小限红",
              A_MX_XD:"闲对最大限红",
              A_MN_XD:"闲对最小限红",
              A_Prncpl:"可用余额",
              A_IntoR:"占成",
              A_DrawR:"和局率",
              A_WashT:"洗码类型",
              A_WashR:"洗码率",							
              A_Perm:"抽水和配分权限",
              A_H5CashMger:"H5会员管理代理"
          }]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"登录成功"
      }  
  }
  ```

* 特殊数据说明：

> 1. 当此代理处于锁定状态时不允许登录
> 2. A_State: NO = 锁定,PAUSE = 暂停,YES = 启用
> 3. A_Sub: "1"= 子账号,"0" = 代理
> 4. A_Perm: {"MatchP":true,"SetPV":true} 的Base64编码

#### 3. 根据代理ID获取指定代理数据 

* 接口名称: GetAListByID

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetAListByID"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"代理ID",
      	}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetAListByID",
      },
      Response:
      {
          JsonData:
          [{
              A_ID:"登陆代理ID",
              A_UserID:"代理登陆ID",
              A_Name:"代理名称",
              A_PID:"父级代理ID",
              A_State:"状态",
              A_Sub:"是否子账号",
              A_MX_Z:"庄最大限红",
              A_MN_Z:"庄最小限红",
              A_MX_X:"闲最大限红",
              A_MN_X:"闲最小限红",
              A_MX_H:"和最大限红",
              A_MN_H:"和最小限红",
              A_MX_ZD:"庄对最大限红",
              A_MN_ZD:"庄对最小限红",
              A_MX_XD:"闲对最大限红",
              A_MN_XD:"闲对最小限红",
              A_Prncpl:"可用余额",
              A_IntoR:"占成",
              A_DrawR:"和局率",
              A_WashT:"洗码类型",
              A_WashR:"洗码率",							
              A_Perm:"抽水和配分权限",
              A_H5CashMger:"H5会员管理代理"
          }]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"获取数据成功"
      }  
  }
  ```

* 接口说明: 当客户端是通过子账号登录时，可以通过子账号所属代理ID请求到代理数据

#### 4 .获取指定代理今日下单统计

* 接口名称: GetSelfCenterInfo

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetSelfCenterInfo"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"代理ID",
      	}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetSelfCenterInfo",
      },
      Response:
      {
          JsonData:
          [{
              A_ID:"代理ID",
              A_UserID:"代理登陆ID",
              A_Name:"代理名称",
              A_CreDate:"时间",	
              A_TotalWin:"总赢",
              A_WashF:"洗码费",
              A_WashF_No:"未结洗码费",//指此代理所有未结洗码费，而非今日
              A_CmpyMony:"公司金额",
              A_Mony:"代理金额",
              A_BetS:"总下注",
              A_BetSAct:"实际下注",
              A_WashS:"洗码量",
              A_WashS_No:"未结洗码量",//指此代理所有未结洗码，而非今日
              A_Charge:"今日上分",
              A_Cash:"今日下分",
              A_Prncpl:"可用余额",
              A_WashR:"洗码率",
              A_IntoR:"占成",
              A_DrawR:"和局率",
              A_TipSum:"小费",
              A_DrawF:"和局费",
              A_DrawS:"和局量",
              A_WashS_Z:"庄洗码量",
              A_AblePoint:"群组余额",
              A_ClntSum:"会员总数",
              A_TotalOdds:"今日抽水",
              A_TotalOdds_No:"未结抽水"//指此代理所有未结抽水，而非今日
          }]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"获取数据成功"
      }  
  }
  ```

* 接口说明: 获取到指定代理今日输赢、上下分等合计数据

* 特殊数据说明：

> 1. 以上返回的代理金额 = (总赢+洗码费)x(1-占成)  [A_Mony = (A_TotalWin+A_WashF )x(1-A_IntoR)]，这是在没有抽水的情况下的计算方式，如果有抽水则需要用以下公式进行计算：代理金额 = (总赢+洗码费+抽水)*(1-占成) [A_Mony = (A_TotalWin+A_WashF + A_TotalOdds)x(1-A_IntoR)]，也就是说客户端显示代理金额时需要以第二种计算公式进行转换
> 2. 以上返回的公司金额 =  (总赢+洗码费)x占成  [A_Mony = (A_TotalWin+A_WashF )xA_IntoR]，这是在没有抽水的情况下的计算方式，如果有抽水则需要用以下公式进行计算：公司金额 = (总赢+洗码费+抽水)*占成 [A_Mony = (A_TotalWin+A_WashF + A_TotalOdds)xA_IntoR]，也就是说客户端显示代公司金额时需要以第二种计算公式进行转换

#### 5 获取指定代理会员在线情况

* 接口名称: CenterPlayList

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"CenterPlayList"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"代理ID",
      	}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"CenterPlayList",
      },
      Response:
      {
          JsonData:
          [{
              C_ID:"会员ID",
              C_UserID:"会员登陆ID",
              C_Name:"会员名",
              C_Balnc:"会员余额",
              C_LoginTime:"登陆时间",
              C_IP:"登陆IP",
              C_Addr:"登陆地址",
              C_TableN:"所在桌台名称",
          },......]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"获取数据成功"
      }  
  }
  ```

* 接口说明: 此接口用来展示登录代理下的在线会员情况

#### 6.  获取指定时段内指定代理统计数据

* 接口名称: GetLoginACount

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetLoginACount"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"代理ID",
          	StartDate:"开始时间",//yyyy-MM-dd格式
          	EndDate:"结束时间"//yyyy-MM-dd格式
      	}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetLoginACount",
      },
      Response:
      {
          JsonData:
          [{
              A_ID:"代理ID",
              A_UserID:"代理登陆ID",
              A_Name:"代理名称",
              A_TotalWin:"总赢",
              A_WashF:"洗码费",
              A_WashS:"洗码量",
              A_DrawS:"和局量",
              A_DrawF:"和局费",
              A_CmpSum:"公司金额",
              A_Money:"代理金额",
              A_Bets:"总下注",
              A_BetSAct:"实际下注",
              A_Date:"时间",
              A_Amount3B:"三宝费",
          },......]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"获取数据成功"
      }  
  }
  ```

* 接口说明: 此接口提供的数据用以展示登录代理一周、一月等时段的统计数据

#### 7. 获取指定时段内指定代理下会员统计数据

* 接口名称: AgentClientCount

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"AgentClientCount"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"代理ID",
          	StartDate:"开始时间",//yyyy-MM-dd格式
          	EndDate:"结束时间"//yyyy-MM-dd格式
      	}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"AgentClientCount",
      },
      Response:
      {
          JsonData:
          [{
              C_ID:"会员ID",
              A_UserID:"会员登陆ID",
              A_CName:"会员名称",
              A_TotalWin:"总赢",
              A_BetS:"总下注",
              A_WashS:"洗码量",
              A_WashF:"洗码费",
              A_DrawS:"和局量",
              A_DrawF:"和局费"
          },......]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"获取数据成功"
      }  
  }
  ```

* 接口说明: 此接口返回数据呈现了代理下会员的一周、一月等时段的数据统计情况

#### 8. 获取指定代理或指定会员所属代理统计数据（这里查询的是今日统计数据）

* 接口名称: GetAorCAgentData

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetAorCAgentData"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"代理ID",
          	A_UserID:"代理/会员登录账号"        
      	}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetAorCAgentData",
      },
      Response:
      {
          JsonData:
          [{
              A_ID:"代理ID",
              A_UserID:"代理登陆ID",
              A_Name:"代理名称",
              A_Prncpl:"可用余额",
              A_DrawR:"和局率",
              A_WashR:"洗码率",
              A_IntoR:"占成",
              A_MX_Z:"庄最大限红",
              A_MX_ZD:"庄对最大限红",	
              A_MX_X:"闲最大限红",
              A_MX_XD:"闲对最大限红",
              A_MX_H:"和最大限红",
              A_MN_Z:"庄最小限红",
              A_MN_ZD:"庄对最大限红",
              A_MN_X:"闲最小限红",
              A_MN_XD:"闲对最小限红",
              A_MN_H:"和最小限红",
              A_Perm:"代理抽水配分权限",
              A_TotalWin:"总赢",
              A_WashF:"洗码费",
              A_WashF_No:"未结洗码费",
              A_CmpyMony:"公司金额",
              A_Mony:"代理金额",
              A_BetS:"总下注",
              A_BetSAct:"实际下注",
              A_WashS:"洗码量",
              A_WashS_No:"未结洗码量",
              A_Charge:"今日上分",
              A_Cash:"今日下分",
              A_DrawF:"和局费",
              A_DrawS:"和局量",
              A_AblePoint:"群组余额",
              A_TotalOdds:"抽水",
              A_TotalOdds_No:"未结抽水"
          }]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"获取数据成功"
      }  
  }
  ```

  * 接口说明: 可以根据代理ID，代理登录账号，会员登录账号进行查询，会优先判断客户端是否传递了登录账号，并且会更具登录账号的长度判断传递过来的是代理登录账号还是会员登录账号，如果是会员登录账号则会差此会员所属代理的统计数据，如果明确代理ID的情况尽量直接传递代理ID，这样会少走两步判断

  * 特殊说明:此接口返回的数据是指今日合计数据，但是未结相关的费用是指所有，如未结洗码费是指当前代理所有未结算的洗码费

#### 9. 获取公告

* 接口名称: GetPubInfo

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetPubInfo"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:{}//此接口不需要具体参数，但是需要发送登录验证信息
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetPubInfo",
      },
      Response:
      {
          JsonData:"公告内容"
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"获取数据成功"
      }  
  }
  ```

* 接口说明: 获取代理需要的公告信息，公告会以"(1)"开始对公告信息进行标序

#### 10. 获取指定代理及它的直属代理列表数据

* 接口名称: GetAllAgents

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetAllAgents"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"代理ID",
          	A_UserID:"代理登录账号",
          	PageSize:"每页显示数据条数",
          	CurePage:"当前显示",
     		}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetAllAgents",
      },
      Response:
      {
          JsonData:
          [{
              A_ID:"代理ID",
              A_UserID:"代理登陆名称",
              A_PID:"父级代理ID",
              A_Name:"代理名称",
              A_CreTime:"创建时间",
              A_MX_Z:"庄最大限红",
              A_MN_Z:"庄最小限红",
              A_MX_X:"闲最大限红",
              A_MN_X:"闲最小限红",
              A_MX_H:"和最大限红",
              A_MN_H:"和最小限红",
              A_MX_ZD:"庄对最大限红",
              A_MN_ZD:"庄对最小限红",
              A_MX_XD:"闲对最大限红",
              A_MN_XD:"闲对最小限红",
              A_Hide:"是否作废",
              A_Web:"是否允许登陆",
              A_State:"状态",
              A_Prncpl:"可用余额",
              A_WashT:"洗码类型",//"S"=双边  "D"=单边 ""/null = 未指定
              A_WashR:"洗码率",
              A_IntoR:"占成",
              A_SPrncpl:"信用额度",
              A_TotalWin:"总赢",
              A_WashS:"洗码量",
              A_BetS:"总下注",
              A_WashF:"洗码费",
              A_TotalFee:"A_TotalFee",
              A_CompanyFee:"公司金额",
              A_DrawR:"和局率",
              A_DrawF:"和局费",
              A_DrawS:"和局量",
              A_AblePoint:"群组余额",//此代理分支所有代理和会员余额总和
              A_F2:"备注",
              A_Perm:"抽水配分权限",
              A_TotalOdds:"今日抽水",
              A_Odds_No:"未结抽水",
              A_WashF_No:"未结洗码费",
              A_WashS_No:"未结洗码量",
              A_ClntCounts:"总会员数",
              A_SubCounts:"直属代理数量",
              A_ProA:"推广量",//表示此代理直接邀请的H5会员数量
              A_ProA_T:"总推广量",//此代理分支发展的所有H5会员总数
              A_H5AbPoint:"H5会员群组余额"//此代理分支所有H5会员余额总和
          },......],
          Count:"此条件查询总数据条数",
          AgentNav://查询代理到登录代理关系链
          [{
              A_ID:"代理ID",
              A_UserID:"代理登陆名称",
              A_PID:"父级代理ID",
              A_Name:"代理名称",
              A_MX_Z:"最大限红",
              A_MN_Z:"最小限红",
              A_Prncpl:"可用余额",
              A_WashR:"洗码率",
              A_IntoR:"占成",
              A_Perm:"抽水配分权限",
          },.....]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"获取数据成功"
      }  
  }
  ```

* 接口说明:

> 1. 此接口返回的数据如总赢、洗码量等都是指今日数据
> 2. 此接口返回的未结算费用是指所有费用，如未结算洗码费是指所有未结算的，而非今日未结算
> 3. IsHide:
>
> > * TRUE: 表示作废，也就是逻辑删除的标记，同时代理也是处于锁定状态，此时代理不允许登录
> > * FALSE: 表示正常状态
>
> 4. State: 
>
> > * NO:表示锁定，此时代理不允许登录，也就没有任何权限
> > * PAUSE:表示暂停，此时可登录及查看数据，但是不具有增删改及上下分功能
> > * YES: 表示启用，此时具有所有权限
>
> 5. 调用此接口时，首先会判断当前搜索条件查询代理是否在登录代理分支下，如果不在则直接返回
> 6. 调用此接口时，会优先以登录账号(A_UserID)过滤，如果没有此参数再以代理ID(A_ID)为条件进行过滤
> 7. 最后返回的数据集是优先按照代理群组余额进行倒序排序

#### 11. 获取指定代理下逻辑删除的代理列表数据

* 接口名称: GetDeletedA

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetDeletedA"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"代理ID"
     		}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetDeletedA"
      },
      Response:
      {
          JsonData:
          [{
              A_ID:"代理ID",
              A_UserID:"代理登陆ID",
              A_Name:"代理名称",
              A_Prncpl:"可用余额",
              A_CreTime:"创建时间",
              A_Charge:"最近充值金额",
              A_State:"状态",
              A_WashR:"洗码率",	
              A_DrawR:"和局率",
              A_IntoR:"占成",	
          },......]      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"获取数据成功"
      }  
  }
  ```

#### 12. 新增代理

* 接口名称: InsertAgent

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"InsertAgent"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_WashT:"洗码类型",
              A_Password:"密码",//必传，并需要MD5加密	
              A_Perm:"可否抽水",
              A_MacthP:"可否配分",
              A_Name:"代理名称",//必传
              A_DrawR:"和局率",
              A_F2:"备注",
              A_IntoR:"占成",
              A_Hide:"是否作废",//默认false已屏蔽 
              A_Web:"是否允许登陆",//默认true 已屏蔽
              A_UserID:"代理登陆名称",//必传
              A_MX_Z:"最大限红",//对子和赋值为此值除以10
              A_MN_Z:"最小限红",
              A_PID:"父级代理ID",//必传
              A_State:"状态",//默认为YES
              A_WashR:"洗码率",//<=上级代理洗码率 >=0
     		}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"InsertAgent"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断新增代理操作
> 2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断新增代理操作
> 3. 服务端会验证必填参数: A_UserID,A_PID,A_Pwd,A_Name，如果参数不完整则中断新增代理操作
> 4. 在请求报文中提到的屏蔽两个选项目前不需要考虑，如果在之后有必要的时候会重新启用
> 5. 服务端会以请求参数中的父级代理ID(A_PID)查询指定代理，如果没有找到则中断新增代理操作
> 6. 当为新增代理设置了抽水权限，而步骤5中查询出来的父级代理没有抽水权限，则中断新增代理操作，因为父级代理没有抽水权限，则他的子代理不允许有抽水权限
> 7. 当为新增代理设置了配分权限，而步骤5中查询出来的父级代理没有配分权限，则中断新增代理操作，因为父级代理没有配分权限，则他的子代理不允许有配分权限
> 8. 当新增代理的洗码率(A_WashR)大于步骤5中查询出来的父级代理的洗码率，则服务端会将新增代理的洗码率重新赋值为父级代理的洗码率，并继续新增代理操作
> 9. 当前新增代理的和局率(A_DrawR)大于步骤5中查询出来的父级代理的和局率，则服务端会将新增代理的和局率重新赋值为父级代理的洗码率，并继续新增代理操作
> 10. 当新增代理的占成(A_InttoR)大于步骤5中查询出来的父级代理的占成，则服务端会将新增代理的占成重新赋值为父级代理的占成，并继续新增代理操作
> 11. 当新增代理的最大限红(A_MX_Z)大于步骤5中查询出来的父级代理的最大限红，则服务端会将新增代理的最大限红赋值为父级代理的最大限红，并继续新增代理操作
> 12. 当新增代理的最小限红(A_MN_Z)小于步骤5中查询出来的父级代理的最小限红，则服务端会将新增代理的最小限红赋值为父级代理的最小限红，并继续新增代理操作

#### 13. 修改代理

* 接口名称: UpdateAgent

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"UpdateAgent"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"代理ID",//此参数必传
              A_UserID:"代理登陆ID",
              A_SetPv:"抽水权限",//true false 
              A_MacthP:"配分权限",//true false
              A_IntoR:"占成",
              A_MX_Z:"最大限红",
              A_MN_Z:"最小限红",
              A_F2:"备注",
              A_WashT:"洗码类型",//true双边 false 单边
              A_WashR:"洗码率",//<=上级代理洗码率 >=0
          	A_DrawR:"和局率",
              A_Web:"是否允许登陆",//true false屏蔽
              A_Hide:"是否隐藏",//屏蔽	
     		}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"UpdateAgent"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1. 在调用修改代理接口时，可以根据实际修改的字段进行传参，比如说只修改了代理名称，则可以只传代理名称，而不用传没有修改的洗码率等其他字段
> 2. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断修改代理操作
> 3. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断修改代理操作
> 4. 检查必填参数代理ID(A_ID)，如果没有值，则中断修改代理操作
> 5. 如果父级代理没有抽水权限，而给自己设置抽水权限，则中断修改代理操作
> 6. 如果下级代理有抽水权限，而取消自己的抽水权限，则中断修改代理操作
> 7. 如果代理的占成为0则不能给此代理设置配分权限
> 8. 修改代理配分权限之前需要检查此代理的群组余额(这里不包括代理分支下的H5会员余额的和)是否大于0，如果是则中断修改代理操作，因为在修改代理配分权限则必须先清理代理
> 9. 如果父级代理没有配分权限，而给自己设置配分权限，则中断修改代理操作
> 10. 如果下级代理有配分权限，而取消自己的配分权限，则中断修改代理操作
> 11. 判断洗码率是否大于父级代理洗码率，如果是则中断修改代理操作
> 12. 判断和局率是否大于父级代理和局率，如果是则中断修改代理操作
> 13. 判断占成是否大于父级代理占成，如果是则中断修改代理操作
> 14. 判断占成是否小于下级代理最大占成，如果是则中断修改代理操作
> 15. 判断最大限红是否大于父级代理最大限红，如果是则中断操作
> 16. 判断最小限红是否小于父级最小限红，如果是则中断操作
> 17. 当把代理的抽水权限从有到无设置之后，服务端会将此代理分支下所有会员的赔率修改为标准赔率

#### 14. 修改下级代理密码

* 接口名称: AgentModifyPwd

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"AgentModifyPwd"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"被操作代理ID",
          	A_PWd:"修改后的密码",//需要MD5加密过的
     		}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"AgentModifyPwd"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断修改子代理密码操作
> 2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断修改子代理密码操作
> 3. 检查必填参数代理ID(A_ID)及MD5加密的代理密码(A_Pwd)，如果没有值，则中断修改子代理密码操作

#### 15. 修改登录代理密码

* 接口名称: AgentSelfModifyPwd

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"AgentSelfModifyPwd"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"被操作代理ID",
          	A_OldPwd:"初始密码",//需要MD5加密过的
          	A_PWd:"修改后的密码",//需要MD5加密过的
     		}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"AgentSelfModifyPwd"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断修改登录代理密码
> 2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断修改登录代理密码
> 3. 检查必填参数代理ID(A_ID)、MD5加密的初始密码(A_OldPwd)及MD5加密的代理密码(A_Pwd)，如果没有值，则中断修改登录代理密码
> 4. 检验初始密码是否正确，如果不正确则中断修改登录代理密码

#### 16. 代理上下分

* 接口名称: AgentPoint

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"AgentPoint"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"被操作代理ID",//必传参数
              A_LevelPoint:"上级/登录代理上下分",//"1"=上级代理 否则登录代理
              A_Point:"上下分点数",//必传参数
              A_IsAdd:"上分还是下分",//必传参数
              A_PID:"被操作代理父级代理ID",//必传参数
     		}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"AgentPoint"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明: （上下分对象 = 被操作代理）

> 1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断代理上下分
>
> 2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断代理上下分
>
> 3. 检查必传参数是否都有，如果参数不完整则中断代理上下分
>
> 4. 检查上下分点数是否大于0，如果不成立则中断代理上下分
>
> 5. 检验如果从被操作代理到登录代理之间任何一个有配分权限，此时如果不是上级代理上下分则中断操作
>
> 6. 参数中的上下分点数是经过配分计算转换后的点数
>
> 7. 上下分模式说明：
>
>    > * 登录代理上下分：实额上下分，其中分的来源和去处是当前登录代理
>    > * 上级代理上下分：实额度上下分，其中分的来源和去处是被操作代理的父级代理
>    > * 上级代理配分上下分：上下分的点数是经过配分计算的，其中分的来源和去处是被操作代理的父级代理
>
> 8. 分源(即上下分点数的来源与去处)说明:
>
>    > * 登录代理上下分时：分源 = 登录代理
>    > * 上级代理上下分和上级代理配分上下分时：分源 = 被操作代理的父级代理
>
> 9. 上下分模式判断：
>
>    > * 被操作代理有配分权限，此时只能以"上级代理配分上下分"模式进行操作
>    >
>    > * 被操作代理没有配分权限
>    >
>    > > * 被操作代理到登录代理之间其他任何代理有配分权限，不包括登录代理，此时只能以"上级代理上下分"模式进行操作(也就是不能跨级上下分)
>    > > * 被操作代理到登录代理之间任何代理都没有配分权限，不包括登录代理，此时能以除了"上级代理配分上下分"的其他两种模式进行操作
>
> 10. 下分时，如果下分点数超过自己的剩余点数时中断下分操作
>
> 11. 上分时，如果上分点数超过分源剩余点数时中断上分操作

#### 17. 代理清零

* 接口名称: AgentClear

* 请求报文:

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"AgentClear"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"被操作代理ID",//必传参数
     		}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"AgentClear"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断代理清零
> 2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断代理清零
> 3. 如果必填参数不完整，则中断代理清零
> 4. 代理清零会将此代理分支下除了H5会员的所有代理及会员的剩余点数致为0

#### 18. 结算代理抽水

* 接口名称: SettleOdds4Agent

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"SettleOdds4Agents"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"被操作代理ID",//必传参数
          	StartDate:"开始结算时间",//必传参数 yyyy-MM-dd :HH:mm:ss格式
          	EndDate:"结束结算时间"//必传参数 yyyy-MM-dd :HH:mm:ss格式
     		}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"SettleOdds4Agent"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断结算代理抽水
> 2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断结算代理抽水
> 3. 如果必填参数不完整，则中断结算代理抽水
> 4. 当前结算金额大于父级代理剩余点数时中断结算代理抽水

#### 19. 结算代理洗码费

* 接口名称: SettleWashF4Agent

* 请求报文

  ~~~json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"SettleWashF4Agent"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	A_ID:"被操作代理ID",//必传参数
          	StartDate:"开始结算时间",//必传参数 yyyy-MM-dd :HH:mm:ss格式
          	EndDate:"结束结算时间"//必传参数 yyyy-MM-dd :HH:mm:ss格式
     		}
      }   
  }
  ~~~

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"SettleWashF4Agent"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

>1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断结算代理抽水
>2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断结算代理抽水
>3. 如果必填参数不完整，则中断结算代理抽水
>4. 当前结算金额大于父级代理剩余点数时中断结算代理洗码费

#### 20. 获取自动生成不重复的6位代理登录账号

* 接口名称: GetAgentLogName

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetAgentLogName"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:{}//此接口无需其他参数，但是报文头中的参数必须传，以作登录验证
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetAgentLogName"
      },
      Response:
      {
          JsonData:
          {
             UserID:"代理登录账号"
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

#### 21. 获取自动生成不重复的8位会员登录账号 

* 接口名称: GetClientLogName

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetClientLogName"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:{}//此接口无需其他参数，但是报文头中的参数必须传，以作登录验证
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetClientLogName"
      },
      Response:
      {
          JsonData:
          {
             UserID:"会员登录账号"
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```



#### 22. 获取自动生成不重复的5位子账号登录账号

- 接口名称: GetSubLogName

- 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetSubLogName"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:{}//此接口无需其他参数，但是报文头中的参数必须传，以作登录验证
      }   
  }
  ```

- 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetSubLogName"
      },
      Response:
      {
          JsonData:
          {
             UserID:"子账号登录账号"
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

#### 23. 获取会员列表数据

* 接口名称: GetClntList_Invite

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetClntList_Invite"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              "C_AID":"被操作代理ID",
              "C_UserID":"代理登陆名称",
      	    "C_Type":"会员类型",//"C_H5"(H5会员) "C_Common"(常规会员)
      	    "C_InType":"邀请类型",//"A"(代理直接邀请) "C"(下级会员邀请) "O" 表示常规会员
              "PageSize":"每页显示几条数据",
              "CurePage "："当前页"
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetSubLogName"
      },
      Response:
      {
          JsonData:
          [{
              C_ID:"会员ID",
              C_UserID:"会员登录ID",
              C_InID:"邀请人ID",
              C_InUserID:"邀请人登录ID",
              C_InType:"邀请人类型",//"A"代理直接邀请 "C"代理下会员邀请 "O"常规会员
              C_Type:"会员类型",//"C_H5"H5会员 "C_Common"常规会员
              C_ProA:"推广量",//只有推广量大于0 此行的登录ID才需要加超链接
              C_ProA_T:"总推广量",
              C_WashS_No:"未结洗码量",
              C_WashF_No:"未结洗码费",
              C_Name:"会员名称",
              C_CreTime:"创建时间",
              C_Balnc:"可用余额",
              C_AID:"此会员所属代理ID",
              C_AUserID:"此会员所属代理登录ID",
              C_State:"会员状态",
              C_Prnpl:"剩余额度",
              C_WashT:"洗码类型",
              C_WashR:"洗码率",
              C_DrawR:"和局率",
              C_F1:"是否在线",//>0 在线,否则不在线
              C_Hide:"是否作废",
              C_HdShow:"显示洗码否",//2=显示 1=隐藏
              C_MX_Z:"庄最大限红",
              C_MN_Z:"庄最小限红",
              C_MX_X:"闲最大限红",
              C_MN_X:"闲最小限红",
              C_MX_H:"和最大限红",
              C_MN_H:"和最小限红",
              C_MX_ZD:"庄对最大限红",
              C_MN_ZD:"庄对最小限红",
              C_MX_XD:"闲对最大限红",
              C_MN_XD:"闲对最小限红",
              C_F2:"备注",
              C_ODF:"虎赔率",
              C_ODH:"和赔率",
              C_ODHe:"龙虎和赔率",
              C_ODL:"龙赔率",
              C_ODX:"闲赔率",
              C_ODXD:"闲对赔率",
              C_ODZ:"庄赔率",
              C_ODZD:"庄对赔率",
              TotalRecords:"总记录数"
          },......],
          AgentNav:
           [{
           	A_ID:"代理ID",
              A_UserID:"代理登陆名称",
              A_PID:"父级代理ID",
              A_Name:"代理名称",
              A_MX_Z:"最大限红",
              A_MN_Z:"最小限红",
              A_Prncpl:"可用余额",
              A_WashR:"洗码率",
              A_IntoR:"占成",
              A_Perm:"抽水配分权限",
           },......],
          CurrentClnt:
          [{
              C_ID:"会员ID",
              C_UserID:"会员登录ID",
              C_InID:"邀请人ID",
              C_InUserID:"邀请人登录ID",
              C_InType:"邀请人类型",//"A"代理直接邀请 "C"代理下会员邀请 "O"常规会员
              C_Type:"会员类型",//"C_H5"H5会员 "C_Common"常规会员
              C_ProA:"推广量",//只有推广量大于0 此行的登录ID才需要加超链接
              C_ProA_T:"总推广量",
              C_WashS_No:"未结洗码量",
              C_WashF_No:"未结洗码费",
              C_Name:"会员名称",
              C_CreTime:"创建时间",
              C_Balnc:"可用余额",
              C_AID:"此会员所属代理ID",
              C_AUserID:"此会员所属代理登录ID",
              C_State:"会员状态",
              C_Prnpl:"剩余额度",
              C_WashT:"洗码类型",
              C_WashR:"洗码率",
              C_DrawR:"和局率",
              C_F1:"是否在线",//>0 在线,否则不在线
              C_Hide:"是否作废",
              C_HdShow:"显示洗码否",//2=显示 1=隐藏
              C_MX_Z:"庄最大限红",
              C_MN_Z:"庄最小限红",
              C_MX_X:"闲最大限红",
              C_MN_X:"闲最小限红",
              C_MX_H:"和最大限红",
              C_MN_H:"和最小限红",
              C_MX_ZD:"庄对最大限红",
              C_MN_ZD:"庄对最小限红",
              C_MX_XD:"闲对最大限红",
              C_MN_XD:"闲对最小限红",
              C_F2:"备注",
              C_ODF:"虎赔率",
              C_ODH:"和赔率",
              C_ODHe:"龙虎和赔率",
              C_ODL:"龙赔率",
              C_ODX:"闲赔率",
              C_ODXD:"闲对赔率",
              C_ODZ:"庄赔率",
              C_ODZD:"庄对赔率",
          }]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明:

> 1. 在没有接收到会员登录账号(C_UserID)参数时，是根据代理ID(C_AID)获取此代理下普通直属会员、此代理直接邀请的H5会员
> 2. 如果接收到会员登录账号(C_UserID)参数时，是获取此会员本身及此会员邀请的H5会员
> 3. C_Type:
>
> > * "C_Common": 过滤普通会员
> > * "C_H5": 过滤H5会员
>
> 4. C_InType
>
> > * "A": 邀请人是代理的H5会员
> > * "C": 邀请人是会员的H5会员
> > * "O": 普通会员
> > * "AC": 过滤所有H5会员
> > * "AO": 过滤邀请人是代理的H5会员和普通会员
> > * "OC": 过滤邀请人是会员的H5会员和普通会员
> > * "AOC": 所有会员
>
> 5. 此接口最终返回的会员列表数据集以会员余额倒序排序

#### 24. 获取指定代理下逻辑删除的会员

* 接口名称: GetDeletedC

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetDeletedC"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
          	C_AID:"代理ID"
              PageSize:"每页数据条数",
              CurePage:"当前页"
     		}
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetDeletedC"
      },
      Response:
      {
          JsonData:
          [{
              C_ID:"会员ID",
              C_UserID:"会员登录ID",
              C_Name:"会员名称",
              C_Balnc:"可用余额",
              C_CreTime:"创建时间", 
              C_Charge:"最近充值",
              C_State:"状态",
              C_WashR:"洗码率"
          },......],        
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

#### 25. 获取会员标准赔率

* 接口名称: GetClntOdds

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetClntOdds"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:{}//此接口不需要具体参数      
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetClntOdds"
      },
      Response:
      {
          JsonData:
          [{
              C_ODH:"和赔率",
              C_ODX:"闲赔率",
              C_ODXD:"闲对赔率",
              C_ODZ:"庄赔率",
              C_ODZD:"庄对赔率",
              C_ODHe:"龙虎和赔率",
              C_ODL:"龙赔率",
              C_ODF:"虎赔率"
          },......],        
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

#### 26. 新增会员

* 接口名称: InsertClient

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"InsertClient"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              C_AID:"所属代理ID",//必传
              C_Name:"会员名称",//必传
              C_F2:"备注",
              C_Hide:"是否作废",//false只能传false,
              C_UserID:"会员登录ID",//8位随机生成	 必传
              C_MX_Z:"最大限红",
              C_MN_Z:"最小限红",
              C_Password:"密码",//MD5加密后的  必传
              C_State:"状态",//默认为YES=启用 ,NO=锁定  PAUSE= 暂停
              C_WashR:"洗码率",
              C_WashT:"洗码类型",//true = 双边 false = 单边
              C_HdShow:"显示洗码",//true = 显示 false= 隐藏 其他=未设置
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"InsertClient"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断新增会员操作
> 2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断新增会员操作
> 3. 服务端会验证必填参数: C_UserID,C_AID,C_Pwd,C_Name，如果参数不完整则中断新增会员操作
> 4. 服务端会以请求参数中的所属代理ID(C_AID)查询指定代理，如果没有找到则中断新增会员操作
> 5. 当新增会员的洗码率(C_WashR)大于步骤4中查询出来的父级代理的洗码率，则服务端会将新增会员的洗码率重新赋值为父级代理的洗码率，并继续新增会员操作
> 6. 当前新增会员的和局率(C_DrawR)大于步骤4中查询出来的父级代理的和局率，则服务端会将新增会员的和局率重新赋值为父级代理的洗码率，并继续新增会员操作
> 7. 当新增会员的最大限红(C_MX_Z)大于步骤4中查询出来的父级代理的最大限红，则服务端会将新增会员的最大限红赋值为父级代理的最大限红，并继续新增会员操作
> 8. 当新增会员的最小限红(C_MN_Z)小于步骤4中查询出来的父级代理的最小限红，则服务端会将新增会员的最小限红赋值为父级代理的最小限红，并继续新增会员操作

#### 27. 直属会员清零

* 接口名称: AllClientZero

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"AllClientZero"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              C_AID:"所属代理ID",//必传
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"AllClientZero"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断操作
> 2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断操作
> 3. 不会清零H5会员

#### 28. 清零指定会员

* 接口名称: ClientZero

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"ClientZero"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              C_ID:"会员ID",//必传
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"ClientZero"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明:

> 1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断操作
> 2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断操作
> 3. 不会清零H5会员

#### 29. 会员清卡

* 接口名称:ClearCard4Clnt

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"ClearCard4Clnt"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              C_ID:"会员ID",//必传
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"ClearCard4Clnt"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断操作
> 2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断操作
> 3. 不会对H5会员清卡

#### 30. 修改会员

* 接口名称: UpdateClient

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"UpdateClient"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              C_ID:"会员ID",
              C_UserID:"会员登录ID",
              C_Name:"会员名称",
              C_MX_Z:"最大限红",//对子和等于它除以10
              C_MN_Z:"最小限红",
              C_F2:"备注",
              C_WashT:"洗码类型",//true= 双边 false=单边
              C_WashR:"洗码率",
              C_State:"状态",
              C_DrawR:"和局率",
              C_HdShow:"显示洗码"//true=显示 false=隐藏
              C_ODF:"虎抽水",
              C_ODH:"和抽水",
              C_ODHe:"龙虎和抽水",
              C_ODL:"龙抽水",
              C_ODX:"闲抽水",
              C_ODXD:"闲对抽水",
              C_ODZ:"庄抽水",
              C_ODZD:"庄对抽水",
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"UpdateClient"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断操作
> 2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断操作 
> 3. 可以只传修改字段
> 4. 判断洗码率是否大于父级代理洗码率，如果是则中断操作
> 5. 判断和局率是否大于父级代理和局率，如果是则中断操作
> 6. 判断最大限红是否大于父级代理占成，如果是则中断操作
> 7. 判断最小限红是否大于父级代理占成，如果是则中断操作
> 8. 在父级代理有抽水权限的情况下，才会修改会员的赔率

#### 31. 修改会员密码

* 接口名称:ClientModifyPwd

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"ClientModifyPwd"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              C_ID:"会员ID",//必传
              C_Pwd:"修改后的密码"//需要MD5加密的 必传
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"ClientModifyPwd"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

>1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断操作
>2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断操作 
>3. 检查必填参数，如果参数不完整则中断操作

#### 32. 会员上下分

* 接口名称: ClientPoint

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"ClientPoint"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              C_ID:"会员ID",//必传
              C_IsAdd:"上下分标记",//true = 下分 false = 上分 必传
              C_Point:"上下分点数",// 必传
          	C_AID:"所属代理ID",//必传
              C_UserID:"会员登录账号"//必传
              C_LevelPoint:"上级代理标记"//可选参数 "1" = 上级代理上下分  其他 = 登录代理上下分
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"ClientPoint"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

>1. 服务端会依据Head中的LoginID判断登录代理是否是子账号
>
>> * 如果此子账号是H5管理代理子账号，并且是H5会员下分操作时，此操作继续
>> * 如果不是第一种情况，则中断操作
>
>1. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断操作 
>2. 检查必传参数，如果参数不完整，则中断操作
>3. 判断所属代理到登录代理之间是否有配分权限，如果有并且参数C_LevelPoint != "1"则中断操作，有配分权限不允许跨级操作
>4. 上下分模式
>
>> * H5会员下分: H5会员下分只能是系统指定的H5会员管理代理及它的子账号，并且分源也是系统指定的H5会员收分代理
>> * 上级代理上下分: C_LevelPoint = "1"标记此模式，此时分源是被操作会员的父级代理
>> * 登录代理上下分: C_LevelPoint  !="1" 标记此模式，此时分源是登录代理
>
>6. 上分
>
>> * 上级代理上分: 如果父级代理余额小于上分点数时中断操作
>> * 登录代理上分: 如果登录代理余额小于上分点数时中断操作
>
>7. 下分: 如果会员余额小于下分点数时中断操作

#### 33. 结算指定会员洗码费

* 接口名称: SettleWashF4Clnt

* 请求参数

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"SettleWashF4Clnt"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              C_ID:"会员ID",//必传
              StartDate:"开始结算时间",//必传 yyyy-MM-dd :HH:mm:ss格式
              EndDate:"结束结算时间",// 必传 yyyy-MM-dd :HH:mm:ss格式
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"SettleWashF4Clnt"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

>1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断操作
>2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断操作 
>3. 如果必填参数不完整，则中断操作
>4. 当前结算金额大于父级代理剩余点数时中断操作

#### 34. 获取指定代理下子账号列表数据

* 接口名称: GetAgentSubs

* 应答报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetAgentSubs"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              A_ID:"代理ID",//必传
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetAgentSubs"
      },
      Response:
      {
          JsonData:
          {
              AS_ID:"子账号ID",
              AS_AID:"子账号所属代理ID",
              AS_UserID:"子账号登录ID",
              AS_Password:"密码",
              AS_State:"状态",
              AS_Name:"子账号名称",
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

#### 35. 新增子账号

* 接口名称: InsertAgentSub

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"InsertAgentSub"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              AS_AID:"所属代理ID",//必传
              AS_UserID:"子账号登录名称",//必传
              AS_Name:"子账号名称",//必传
              AS_Pwd:"子账号密码",//必传 MD5加密过的
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"InsertAgentSub"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断操作
> 2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断操作 
> 3. 如果必填参数不完整，则中断操作

#### 36. 修改子账号

* 接口名称: UpdateAgentSub

* 请求参数

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"UpdateAgentSub"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              AS_ID:"所属代理ID",//必传
              AS_UserID:"子账号登录名称",//可选
              AS_Name:"子账号名称",//可选
              AS_Pwd:"子账号密码",//可选 MD5加密过的
              AS_State:"子账号状态"            
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"UpdateAgentSub"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

>1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断操作
>2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断操作 
>3. 如果必填参数不完整，则中断操作

#### 37. 删除子账号

* 接口名称: DeleteAgentSub

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"DeleteAgentSub"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              AS_ID:"所属代理ID",//必传
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"DeleteAgentSub"
      },
      Response:
      {
          JsonData:
          {
             Result:"true/false"//客户端可以直接根据结果代码判断是否成功
          }      
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1. 服务端会依据Head中的LoginID判断登录代理是否是子账号，如果是子账号则中断操作
> 2. 服务端会依据Head中的LoginID判断登录代理是否是启用状态，如果不是则中断操作 
> 3. 如果必填参数不完整，则中断操作

#### 38. 获取指定代理指定时段内的统计

* 接口名称: GetAStatistics

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetAStatistics"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              A_ID:"代理ID",
              A_UserID:"代理登录名称",
              GameT:"游戏类型",//"百家乐" "龙虎" 默认是两种类型一起查
              EndDate:"结束时间",//"yyyy-MM-dd"
              StartDate:"开始时间",//"yyyy-MM-dd"
              PageSize:"每页数据条数",
              CurePage:"当前页"
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetAStatistics"
      },
      Response:
      {
          JsonData:
          [{
              A_ID:"代理ID",
              A_UserID:"代理登录ID",
              A_Name:"代理名称",
              A_TotalWin:"总赢",
              A_WashF:"洗码费",
              A_CmpyMony:"公司金额",
              A_Mony:"代理金额",
              A_BetS:"总下注",
              A_BetSAct:"实际下注",
              A_WashS:"洗码量",
              A_TipSum:"A_TipSum",
              A_DrawF:"和局费",
              A_DrawS:"和局量",
              A_WashS_Z:"庄洗码",
              A_TotalOdds:"抽水",
              A_Perm:"抽水配分权限",
              A_Prncpl:"余额",
              A_DrawR:"和局率",
              A_WashR:"洗码率",
              A_IntoR:"占成"
          },......],
          AgentNav:
           [{
           	A_ID:"代理ID",
              A_UserID:"代理登陆名称",
              A_PID:"父级代理ID",
              A_Name:"代理名称",
              A_MX_Z:"最大限红",
              A_MN_Z:"最小限红",
              A_Prncpl:"可用余额",
              A_WashR:"洗码率",
              A_IntoR:"占成",
              A_Perm:"抽水配分权限",
           },......],
          AgentSum://合计数据
          [{
              TotalRecords:"总记录数",
              A_Bets_T:"总下注",
              A_BetSAct_T:"实际下注",
              A_DrawF_T:"和局费",
              A_DrawS_T:"和局量",
              A_TipSum_T:"A_TipSum_T",
              A_TotalOdds_T:"抽水",
              A_TotalWin_T:"总赢",
              A_WashF_T:"洗码费",
              A_WashS_Z_T:"A_WashS_Z_T",
              A_WashS_T:"洗码量",
              A_CmpyMony_T:"公司金额",
              A_Mony_T:"代理金额"
          }],
          Count:"总记录数"
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1. 会检查当前查询代理是否在登录代理分支下，如果不在则中断查询
> 2. 如果登录账号参数不为空时，会以登录账号进行过滤

#### 39. 获取指定代理下指定时段内的代理每日数据统计

* 接口名称:GetADayStatistics

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetADayStatistics"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              A_ID:"代理ID",
              A_UserID:"代理登录名称",
              GameT:"游戏类型",//"百家乐" "龙虎" 默认是两种类型一起查
              EndDate:"结束时间",//"yyyy-MM-dd"
              StartDate:"开始时间",//"yyyy-MM-dd"
              PageSize:"每页数据条数",
              CurePage:"当前页"
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetADayStatistics"
      },
      Response:
      {
          JsonData:
          [{
              A_ID:"代理ID",
              A_UserID:"代理登录ID",
              A_Name:"代理名称",
              A_CreDate:"时间",
              A_TotalWin:"总赢",
              A_WashF:"洗码费",
              A_CmpyMony:"公司金额",
              A_Mony:"代理金额",
              A_BetS:"总下注",
              A_BetSAct:"实际下注",
              A_WashS:"洗码量",
              A_Charge:"上分",
              A_Cash:"下分",
              A_Perm:"抽水配分权限",
              A_Prncpl:"可用余额",
              A_WashR:"洗码率",
              A_IntoR:"占成",
              A_DrawR:"和局率",
              A_WashT:"洗码类型",
              A_TipSum:"A_TipSum",
              A_DrawF:"和局费",
              A_DrawS:"和局量",
              A_WashS_Z:"A_WashS_Z",
              A_ToatalOdds:"抽水",
          },......],
          AgentNav:
           [{
           	A_ID:"代理ID",
              A_UserID:"代理登陆名称",
              A_PID:"父级代理ID",
              A_Name:"代理名称",
              A_MX_Z:"最大限红",
              A_MN_Z:"最小限红",
              A_Prncpl:"可用余额",
              A_WashR:"洗码率",
              A_IntoR:"占成",
              A_Perm:"抽水配分权限",
           },......],
          Count:"总记录数"
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

>1. 会检查当前查询代理是否在登录代理分支下，如果不在则中断查询

#### 40.获取指定代理抽水及洗码费统计

* 接口名称: GetOddsWashF4Agent

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetOddsWashF4Agent"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              A_ID:"代理ID",
              A_UserID:"代理登录名称",
              GameT:"游戏类型",//"百家乐" "龙虎" 默认是两种类型一起查
              EndDate:"结束时间",
              StartDate:"开始时间",
              PageSize:"每页数据条数",
              CurePage:"当前页"
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetOddsWashF4Agent"
      },
      Response:
      {
          JsonData:
          [{
              A_ID:"代理ID",
              A_Name:"代理名称",
              A_UserID:"代理登录ID",      
              A_LastTime:"结算开始时间",
              A_EndTime:"结算结束时间",
              A_Date:"操作时间",
              A_WashR:"洗码率",
              A_WashS:"洗码量",
              A_Amount:"结算金额",
              A_Count:"结算注单数量",
              A_Type:"结算类型"//结算洗码费-结算抽水
          },......],
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1.会检查当前查询代理是否在登录代理分支下，如果不在则中断查询
>
> 2.此接口返回的数据集由以下部分组成
>
> * 2条之前的时间最近结算记录,如果此代理之前有结算的情况
> * 统计出此代理到当前为止可以结算的洗码费为一条记录
> * 统计处代理到当前为止可以计算的抽水为一条记录

#### 41. 获取指定代理下指定时段内会员统计

* 接口名称:GetClntStatistics

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetClntStatistics"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              A_ID:"代理ID",
              C_UserID:"会员登录名称",
              GameT:"游戏类型",//"百家乐" "龙虎" 默认是两种类型一起查
              EndDate:"结束时间",//"yyyy-MM-dd"
              StartDate:"开始时间",//"yyyy-MM-dd"
              PageSize:"每页数据条数",
              CurePage:"当前页"
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetAStatistics"
      },
      Response:
      {
          JsonData:
          [{
              C_ID:"会员ID",
              C_UserID:"会员登录名称",
              C_Name:"会员名称",
              C_TotalWin:"总赢",
              C_WashF:"洗码费",
              C_LoseWin:"会员输赢",
              C_BetS:"总下注",
              C_BetSAct:"实际下注",
              C_WashS:"洗码量",
              C_Charge:"上分",
              C_Cash:"下分",
              C_Balnc:"可用余额",
              C_WashR:"洗码率",
              C_DrawR:"和局率",
              C_TipSum:"C_TipSum",
              C_DrawS:"和局量",
              C_DrawF:"和具费",
              C_WashS_Z:"庄洗码",
              C_WashR:"洗码率",
              C_DrawR:"和局率"
          },......],
          AgentNav:
           [{
           	A_ID:"代理ID",
              A_UserID:"代理登陆名称",
              A_PID:"父级代理ID",
              A_Name:"代理名称",
              A_MX_Z:"最大限红",
              A_MN_Z:"最小限红",
              A_Prncpl:"可用余额",
              A_WashR:"洗码率",
              A_IntoR:"占成",
              A_Perm:"抽水配分权限",
           },......],
          AgentSum://合计数据
          [{
              TotalRecords:"总记录数",
              C_BetS_T:"总下注",
              C_BetActS_T:"实际下注",
              C_DrawF_T:"和局费",
              C_DrawS_T:"和局量",
              C_TotalWin_T:"总赢",
              C_LoseWin_T:"C_LoseWin_T",
              C_WashF_T:"洗码费",
              C_Wash_Z_T:"庄洗码",
              C_WashS_T:"洗码量",
              C_Charge_T:"上分",
              C_Cash_T:"下分"
          }],
          Count:"总记录数"
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

> 1.会检查当前查询代理/会员是否在登录代理分支下，如果不在则中断查询

#### 42. 获取指定会员指定时段内每日统计

* 接口名称: GetClntDayStatistics

* 请求报文

  ````json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetClntStatistics"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              A_ID:"代理ID",
              GameT:"游戏类型",//"百家乐" "龙虎" 默认是两种类型一起查
              EndDate:"结束时间",
              StartDate:"开始时间",
              PageSize:"每页数据条数",
              CurePage:"当前页"
          }
      }   
  }
  ````

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetAStatistics"
      },
      Response:
      {
          JsonData:
          [{
              C_ID:"会员ID",
              C_UserID:"会员登录名称",
              C_Name:"会员名称",
              C_TotalWin:"总赢",
              C_WashF:"洗码费",
              C_CreDate:"时间",
              C_LoseWin:"会员输赢",
              C_BetS:"总下注",
              C_BetSAct:"实际下注",
              C_WashS:"洗码量",
              C_Charge:"上分",
              C_Cash:"下分",
              C_Balnc:"可用余额",
              C_WashR:"洗码率",
              C_DrawR:"和局率",
              C_TipSum:"小费",
              C_DrawS:"和局量",
              C_DrawF:"和具费",
              C_WashS_Z:"庄洗码",
              C_WashR:"洗码率",
              C_DrawR:"和局率",
          },......],       
          Count:"总记录数"
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

#### 43. 下注明细查询

* 接口名称:GetClntBetBills

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetClntBetBills"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              C_ID:"会员ID",
              C_UserID:"会员登录账号",
              GameT:"游戏类型",//"百家乐" "龙虎" 默认是两种类型一起查
              EndDate:"结束时间",//"yyyy-MM-dd"
              StartDate:"开始时间",//"yyyy-MM-dd"
              PageSize:"每页数据条数",
              CurePage:"当前页"
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetClntBetBills"
      },
      Response:
      {
          JsonData:
          [{
              B_ID:"注单号",
              G_ID:"游戏ID",
              T_Name:"桌台名称",
              T_Ju:"局",
              C_Kou:"口",
              C_CreTime:"押码时间",
              C_ResTime:"开牌时间",
              C_ID:"会员ID",
              C_UserID:"会员登录ID",
              C_Name:"会员名称",
              C_RMBalnc:"可用余额",
              C_TotalWin:"总赢",
              C_WashF:"洗码费",
              C_GameR:"游戏结果",
              C_BZ:"庄下注/虎",
              C_BX:"闲下注/龙",
              C_BH:"和下注",
              C_BZD:"庄对下注",
              C_BXD:"闲对下注",
              C_GameT:"游戏类型",
              C_BetS:"总下注",
              C_BetSAct:"实际下注",         
              C_WashR:"洗码率",
              C_WashS:"洗码量",
              C_WashT:"洗码类型",
              C_TipSum:"C_TipSum",
              C_DrawS:"和局量",
              C_WashS_Z:"C_WashS_Z",         
              U_IP:"IP",
              U_Addr:"地址",
              C_Balnc_B:"下注前余额",
              C_DrawR:"和局率",         
              C_DrawF:"和局费",
              U_Memo:"备注",
          },......],
          AgentNav:
           [{
           	A_ID:"代理ID",
              A_UserID:"代理登陆名称",
              A_PID:"父级代理ID",
              A_Name:"代理名称",
              A_MX_Z:"最大限红",
              A_MN_Z:"最小限红",
              A_Prncpl:"可用余额",
              A_WashR:"洗码率",
              A_IntoR:"占成",
              A_Perm:"抽水配分权限",
           },......],
          Count:"总记录数"
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明:

  >1. 首先会判断是否有会员登录账号(C_UserID)参数，如果有则优先以此参数进行过滤，如果没有则以会员ID(C_ID)进行过滤

#### 44. 获取指定代理或会员上下分明细

* 接口名称:GetPointDetail

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetPointDetail"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              C_ID:"会员ID",
              A_ID:"代理ID",
              C_UserID:"会员登录账号",
              A_UserID:"代理登录账号",
              EndDate:"结束时间",//"yyyy-MM-dd"
              StartDate:"开始时间",//"yyyy-MM-dd"
              PageSize:"每页数据条数",
              CurePage:"当前页",
              PointWay:"上下分方式",
              PointType:"分类过滤上分/下分/清卡/清零",
              PointRange:"按范围过滤上下分数据",
              IsAll:"false",//过滤掉上下分金额为0的记录
              Minum:"按最小上下分金额过滤",
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetPointDetail"
      },
      Response:
      {
          JsonData:
          [{
              P_ID:"上下分ID",
              C_CreID:"操作员ID",
              C_CreTime:"时间",
              C_Delta:"分数",
              C_SID:"源ID",
              C_SPrncpl_B:"源操作前余额",
              C_SPrncpl_E:"源操作后余额",
              C_TID:"目标ID",
              C_Tbalnc_B:"目标操作前余额",//目标是会员
              C_Tbalnc_E:"目标操作后余额",
              C_OpTYpe:"操作类型",//上分-下分-清零
              C_TPrncpl_B:"目标操作前余额",//目标是代理
              C_TPrncpl_E:"目标操作后余额",
              U_IP:"IP",
              U_Addr:"地址",
              U_IPLocal:"U_IPLocal",
              C_SUserID:"源登录ID",
              C_SName:"源名称",
              C_TUserID:"目标登录ID",
              C_TName:"目标名称",
              C_Type:"目标类型",//会员--代理
              U_Memo:"备注",
          },......],
          AgentNav:
           [{
           	A_ID:"代理ID",
              A_UserID:"代理登陆名称",
              A_PID:"父级代理ID",
              A_Name:"代理名称",
              A_MX_Z:"最大限红",
              A_MN_Z:"最小限红",
              A_Prncpl:"可用余额",
              A_WashR:"洗码率",
              A_IntoR:"占成",
              A_Perm:"抽水配分权限",
           },......],
          PointSum:
          [{
           	TotalRecords:"总记录数",
              C_Charge_T:"上分合计",
              C_Cash_T:"下分合计",
              C_Zero_T:"清零合计",
              C_ClearCa_T:"清卡合计",
              C_SumDelta:"上下分总店数"
          }],
          Count:"总记录数"
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

  > 1. 如果参数会员ID(C_ID)不为空，此时判断此会员是否在登录代理分支下，如果在则过滤此会员上下分数据，如果不在则中断查询
  >
  > 2. 如果参数会员登录账号(C_UserID)不为空，此时判断此会员是否在登录代理分支下，如果在则过滤此会员上下分数据，如果不在则中断查询
  >
  > 3. 如果参数代理ID(A_ID)不为空，此时判断此代理是否在登录代理分支下，如果不在则中断查询，如果在则判断上下分范围参数(PointRange)是否为空，如果为空则过滤代理本身上下分数据，如果不为空则按以下情况分类处理
  >
  >    > * OwnA: 过滤此代理的直属代理上下分数据
  >    > * OwnC: 过滤此代理的直属会员上下分数据
  >    > * All: 过滤此代理分支所有的上下分数据
  >    > * Self和其他标记: 过滤此代理本身上下分数据
  >
  > 4. 如果参数代理登录账号(A_UserID)不为空，此时判断此代理是否在登录代理分支下，如果不在则中断查询，如果在则判断上下分范围参数(PointRange)是否为空，如果为空则过滤代理本身上下分数据，如果不为空则按以下情况分类处理
  >
  >    > * OwnA: 过滤此代理的直属代理上下分数据
  >    > * OwnC: 过滤此代理的直属会员上下分数据
  >    > * All: 过滤此代理分支所有的上下分数据
  >    > * Self和其他标记: 过滤此代理本身上下分数据
  >
  > 5. 对参数(C_ID/C_UserID/A_ID/A_UserID)的判断是相互排斥的，并且优先级从高到底按以上步骤1~4，比如参数C_ID不为空则不会对后面的3个参数进行判断，如果以上4个参数都为空，则会按Head报文中的LoginID过滤出登录代理本身的上下分数据
  >
  > 6. 参数IsAll=false 时，过滤掉上下分金额为0的上下分记录，如果IsAll等于其他值则不会不过掉上下分点数为0的记录
  >
  > 7. 如果参数Minum不为空，则过滤出上下分点数大于Minum的记录
  >
  > 8. 参数PointType如果不为空则按以下方式进行判断
  >
  >    > * "BD": 查询上分记录
  >    > * "XD": 查询下分记录
  >    > * "QL": 查询清零记录
  >    > * "QK": 查询清卡记录
  >
  > 9. 参数PointWay如果不为空则按以下方式处理
  >
  >    > * "Third": 查询第三方上下分记录
  >    > * "Other": 查询非第三方上下分记录
  >    > * "All"和其他: 查询所有上下分记录
  >

#### 45. 获取游戏结果

* 接口名称:GetTableResult

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetTableResult"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              T_ID:"桌台ID",
              T_Ju:"局数",
              StartDate:"时间",//"yyyy-MM-dd"
              PageSize:"每页数据条数",
              CurePage:"当前页",
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetTableResult"
      },
      Response:
      {
          JsonData:
          [{
              T_ID:"桌台ID",
              T_Name:"桌台名称",
              T_GameT:"游戏类型",      
              T_JU:"局",
              T_Kou:"口",
              T_State:"状态",
              T_CDTime_B:"倒计时开始时间",
              T_CDTime_E:"倒计时结束时间",
              T_GameR:"游戏结果",
              T_ResTime:"结果录入时间",
              T_RID:"T_RID",
              U_Memo:"备注",
              T_SDay:"T_SDay",        
          },......],
          TableJson:
           [{
           	T_ID:"桌台ID",
              T_Name:"桌台名称",
              T_MX_Z:"庄最大限红",      
              T_MN_Z:"庄最小限红",
              T_MX_X:"闲最大限红",
              T_MN_X:"闲最小限红",
              T_MX_H:"和最大限红",
              T_MN_H:"和最小限红",
              T_MX_ZD:"庄对最大限红",
              T_MN_ZD:"庄对最小限红",
              T_MX_XD:"闲对最大限红",
              T_MN_XD:"闲对最小限红",
           },......],
          JuJson:
          [{
           	T_Ju:"局"
          }],
          Count:"总记录数"
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

  > 1. 如果没有传T_Ju时，会默认去Table中的第一局

#### 46. 获取指定代理下的会员推广统计数据 增量

* 接口名称: PromotionA_Clnt4A

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"PromotionA_Clnt4A"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              A_ID:"代理ID",
              C_UserID:"代理/会员登录账号",
              StartDate:"开始时间",//"yyyy-MM-dd"
              EndDate:"结束时间",//"yyyy-MM-dd"
              PageSize:"每页数据条数",
              CurePage:"当前页",
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"PromotionA_Clnt4A"
      },
      Response:
      {
          JsonData:
          [{
              C_ID:"会员ID",
              C_Name:"会员名称",
              C_UserID:"会员登陆名称",
              C_Type:"用户类型",//SelfA（代理本身）OwnA(直属代理) OwnC(直属会员)
              C_InID:"邀请人ID",
              C_InUserID:"邀请人登陆名称",
              C_InT:"邀请人类型",
              C_AID:"所属代理ID",
              C_ProA:"推广量",
              C_ProA_T:"总推广量",
              C_TotalWin:"推广会员总赢",
              C_WashS:"推广会员洗码量",
              C_WashF:"推广会员洗码费",
              C_BetS:"推广会员总下注量",
              C_BetActS:"推广会员实际下注量",
              C_3BaoA:"推广会员三宝费",
              C_Charge:"推广会员总上分",
              C_Cash:"推广会员总下分",
              C_TotalWin_Self:"会员本身总赢",
              C_WashS_Self:"会员本身洗码量",
              C_WashF_Self:"会员本身洗码费",
              C_BetS_Self:"会员本身总下注量",
              C_BetActS_Self:"会员本身实际下注量",
              C_3BaoA_Self:"会员本身三宝费",
              C_Charge_Self:"会员本身总上分",
              C_Cash_Self:"会员本身总下分",
              C_CreTime:"会员创建时间",
              TotalRecords:"总记录数"
          },......],
          Promotion_A:
           [{
           	A_ID:"代理ID",
              A_Name:"代理名称",
              A_UserID:"代理登陆名称",
              A_Type:"当前用户类型",//SelfA（代理本身）OwnA(直属代理) OwnC(直属会员)
              P_ID:"所属代理ID",
              A_ProA_T:"推广量",
              A_ProA:"总推广量",
              A_TotalWin:"代理总赢",
              A_WashS:"代理洗码量",
              A_WashF:"代理洗码费",
              A_BetS:"总下注量",
              A_BetActS:"实际下注量",
              A_3BaoA:"三宝费",
              A_Charge:"总上分",
              A_Cash:"总下分",
              A_TotalWin_Self:"代理本身总赢",
              A_WashS_Self:"代理本身洗码量",
              A_WashF_Self:"代理本身洗码费",
              A_BetS_Self:"本身总下注量",
              A_BetActS_Self:"本身实际下注量",
              A_3BaoA_Self:"本身三宝费",
              A_Charge_Self:"本身总上分",
              A_Cash_Self:"本身总下分",
              A_CreTime:"创建时间"  
           }],
          AgentNav:
          [{
           	A_ID:"代理ID",
              A_UserID:"代理登陆名称",
              A_PID:"父级代理ID",
              A_Name:"代理名称",
              A_MX_Z:"最大限红",
              A_MN_Z:"最小限红",
              A_Prncpl:"可用余额",
              A_WashR:"洗码率",
              A_IntoR:"占成",
              A_Perm:"抽水配分权限",
          },......]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明:

  > 1. 如果参数代理/会员登录名称不为空，则判断当前查询的代理或会员是否在登录代理分支下，如果不在则中断查询，如果在则过滤出代理或会员的增量推广数据
  > 2. 如果登录名称为空，则判断代理ID是否在登录代理分支下，如果不在则中断查询，如果在则过滤出代理的增量推广数据

#### 47. 获取指定代理下的会员推广统计数据 存量

- 接口名称: PromotionA_AllClnt4A

- 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"PromotionA_AllClnt4A"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              A_ID:"代理ID",
              C_UserID:"代理/会员登录账号",
              StartDate:"开始时间",//"yyyy-MM-dd"
              EndDate:"结束时间",//"yyyy-MM-dd"
              PageSize:"每页数据条数",
              CurePage:"当前页",
          }
      }   
  }
  ```

- 应答报文

  ```json
  {
    	Head:
      {
          Method:"PromotionA_AllClnt4A"
      },
      Response:
      {
          JsonData:
          [{
              C_ID:"会员ID",
              C_Name:"会员名称",
              C_UserID:"会员登陆名称",
              C_Type:"用户类型",//SelfA（代理本身）OwnA(直属代理) OwnC(直属会员)
              C_InID:"邀请人ID",
              C_InUserID:"邀请人登陆名称",
              C_InT:"邀请人类型",
              C_AID:"所属代理ID",
              C_ProA:"推广量",
              C_ProA_T:"总推广量",
              C_TotalWin:"推广会员总赢",
              C_WashS:"推广会员洗码量",
              C_WashF:"推广会员洗码费",
              C_BetS:"推广会员总下注量",
              C_BetActS:"推广会员实际下注量",
              C_3BaoA:"推广会员三宝费",
              C_Charge:"推广会员总上分",
              C_Cash:"推广会员总下分",
              C_TotalWin_Self:"会员本身总赢",
              C_WashS_Self:"会员本身洗码量",
              C_WashF_Self:"会员本身洗码费",
              C_BetS_Self:"会员本身总下注量",
              C_BetActS_Self:"会员本身实际下注量",
              C_3BaoA_Self:"会员本身三宝费",
              C_Charge_Self:"会员本身总上分",
              C_Cash_Self:"会员本身总下分",
              C_CreTime:"会员创建时间",
              TotalRecords:"总记录数"
          },......],
          Promotion_A:
           [{
           	A_ID:"代理ID",
              A_Name:"代理名称",
              A_UserID:"代理登陆名称",
              A_Type:"当前用户类型",//SelfA（代理本身）OwnA(直属代理) OwnC(直属会员)
              P_ID:"所属代理ID",
              A_ProA_T:"推广量",
              A_ProA:"总推广量",
              A_TotalWin:"代理总赢",
              A_WashS:"代理洗码量",
              A_WashF:"代理洗码费",
              A_BetS:"总下注量",
              A_BetActS:"实际下注量",
              A_3BaoA:"三宝费",
              A_Charge:"总上分",
              A_Cash:"总下分",
              A_TotalWin_Self:"代理本身总赢",
              A_WashS_Self:"代理本身洗码量",
              A_WashF_Self:"代理本身洗码费",
              A_BetS_Self:"本身总下注量",
              A_BetActS_Self:"本身实际下注量",
              A_3BaoA_Self:"本身三宝费",
              A_Charge_Self:"本身总上分",
              A_Cash_Self:"本身总下分",
              A_CreTime:"创建时间"  
           }],
          AgentNav:
          [{
           	A_ID:"代理ID",
              A_UserID:"代理登陆名称",
              A_PID:"父级代理ID",
              A_Name:"代理名称",
              A_MX_Z:"最大限红",
              A_MN_Z:"最小限红",
              A_Prncpl:"可用余额",
              A_WashR:"洗码率",
              A_IntoR:"占成",
              A_Perm:"抽水配分权限",
          },......]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

- 接口说明:

  > 1. 如果参数代理/会员登录名称不为空，则判断当前查询的代理或会员是否在登录代理分支下，如果不在则中断查询，如果在则过滤出代理或会员的存量推广数据
  > 2. 如果登录名称为空，则判断代理ID是否在登录代理分支下，如果不在则中断查询，如果在则过滤出代理的存量推广数据

#### 48. H5会员第三方上分明细

* 接口名称: H5ClntPointDetail

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"H5ClntPointDetail"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              C_ID:"会员ID",
              C_UserID:"会员登录ID",
              StartDate:"查询开始时间", //时间格式 'yyyy-MM-dd HH:mm:ss'
              EndDate:"查询结束时间",//时间格式 'yyyy-MM-dd HH:mm:ss'
              PageSize:"每页显示几条数据",
              CurePage:"当前页"
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"H5ClntPointDetail"
      },
      Response:
      {
          JsonData:
          [{
              C_ID:"会员ID",
              C_UserID:"会员登录ID",
              C_Name:"会员名称",
              Delta:"上分金额",
              C_Balanc_B:"上分前余额",
              C_Balanc_E:"上分后余额",
              C_SrID:"分源ID",
              C_SrUserID:"分源登录ID",
              C_SrName:"分源名称",
              C_SrBalanc_B:"分源操作前余额",
              C_SrBalanc_E:"分源操作后余额",
              C_CreTime:"上分时间",
              TotalRecords:"总记录条数",
          },......],
          AgentPointSum:
           [{
           	Counts:"上分总次数",
              SumMoney:"上分总金额",
              StartDate:"第一笔上分时间",
              EndDate:"最后一笔上分时间",
              A_ID:"代理ID",
              A_BorrowUserID:"代理登录ID",
              A_BorrowName:"代理名称", 
           }]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

  > 1. 获取指定H5会员的第三方上分明细

#### 49. 获取指定代理下所有H5会员第三方上分明细

* 接口名称: H5ClntPointSum_A

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"H5ClntPointSum_A"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              A_ID:"被操作代理ID",
              A_UserID:"代理登录ID",
              StartDate:"查询开始时间", //时间格式 'yyyy-MM-dd HH:mm:ss'
              EndDate:"查询结束时间",//时间格式 'yyyy-MM-dd HH:mm:ss'
              PageSize:"每页显示几条数据",
              CurePage:"当前页"
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"H5ClntPointSum_A"
      },
      Response:
      {
          JsonData:
          [{
              C_ID:"会员ID",
              C_UserID:"会员登录ID",
              C_Name:"会员名称",
              Delta:"上分金额",
              C_Balanc_B:"上分前余额",
              C_Balanc_E:"上分后余额",
              C_SrID:"分源ID",
              C_SrUserID:"分源登录ID",
              C_SrName:"分源名称",
              C_SrBalanc_B:"分源操作前余额",
              C_SrBalanc_E:"分源操作后余额",
              C_CreTime:"上分时间",
              TotalRecords:"总记录条数",
          },......],
          AgentPointSum:
           [{
           	Counts:"上分总次数",
              SumMoney:"上分总金额",
              StartDate:"第一笔上分时间",
              EndDate:"最后一笔上分时间",
              A_ID:"代理ID",
              A_BorrowUserID:"代理登录ID",
              A_BorrowName:"代理名称", 
           }]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

  > 1.如果查询代理不是系统配置的H5会员的管理代理或者系统配置的放分代理，则过滤出当前代理的H5会员第三方上分数据
  >
  > 2.如果登录账号是会员账号则会返回此会员第三方上分明细

#### 50.指定代理下H5会员第三方上分合计接口

* 接口名称: H5ClntPointSum

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"H5ClntPointSum"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              A_ID:"被操作代理ID",
              A_UserID:"代理登录ID",
              StartDate:"查询开始时间", //时间格式 'yyyy-MM-dd HH:mm:ss'
              EndDate:"查询结束时间",//时间格式 'yyyy-MM-dd HH:mm:ss'
              PageSize:"每页显示几条数据",
              CurePage:"当前页"
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"H5ClntPointSum"
      },
      Response:
      {
          JsonData:
          [{
              Counts:"上分次数",
              Delta:"上分金额",
              C_ID:"会员ID",
              C_UserID:"会员登录ID",
              C_Name:"会员名称",
              C_AID:"所属代理ID",
              C_OwnerUserID:"所属代理登录ID",
              C_OwnerName:"所属代理名称",
              StartDate:"第一笔上分时间",
              EndDate:"最后一笔上分时间",
              TotalRecords:"总记录条数",
          },......],
          AgentPointSum:
           [{
           	Counts:"上分总次数",
              SumMoney:"上分总金额",
              StartDate:"第一笔上分时间",
              EndDate:"最后一笔上分时间",
              A_ID:"代理ID",
              A_BorrowUserID:"代理登录ID",
              A_BorrowName:"代理名称", 
           }]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

  > 如果查询代理不是系统配置的H5会员的管理代理或者系统配置的放分代理，则过滤出当前代理的H5会员第三方上分数据

#### 51. 查询指定代理自己及直属代理及直属会员下红包发放合计

* 接口名称: RedEnvelopeSum

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"RedEnvelopeSum"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              A_ID:"被操作代理ID",
              A_UserID:"代理登录ID",
              StartDate:"查询开始时间", //时间格式 'yyyy-MM-dd HH:mm:ss'
              EndDate:"查询结束时间",//时间格式 'yyyy-MM-dd HH:mm:ss'
              PageSize:"每页显示几条数据",
              CurePage:"当前页"
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"RedEnvelopeSum"
      },
      Response:
      {
          JsonData:
          [{
              C_ID:"主键ID",
              C_Name:"名称",
              C_UserID:"账号",
              FirstTime:"首次红包时间",
              LastTime:"尾次红包时间",
              OpID:"OpID",
              RedAmount:"红包总金额",
              RedCounts:"发放红包总数",
              InviterID:"InviterID",
              RegCount:"成功激活红包数",
              RebackAmount:"可回收红包金额",
              TotalRecords:"总记录条数",
          },......]        
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明

  > 优先判断参数A_UserID是否为空

#### 52. 查询指定会员下红包发送明细

- 接口名称: RedEnvelopeDetail

- 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"RedEnvelopeDetail"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              A_ID:"被操作代理ID",
              A_UserID:"代理登录ID",
              GameT:"红包状态",
              StartDate:"查询开始时间", //时间格式 'yyyy-MM-dd HH:mm:ss'
              EndDate:"查询结束时间",//时间格式 'yyyy-MM-dd HH:mm:ss'
              PageSize:"每页显示几条数据",
              CurePage:"当前页"
          }
      }   
  }
  ```

- 应答报文

  ```json
  {
    	Head:
      {
          Method:"RedEnvelopeDetail"
      },
      Response:
      {
          JsonData:
          [{
              C_ReID:"红包接收ID",
              C_ReUserID:"接收账号",
              C_ReName:"接收名称",
              cFlag:"状态",
              cCeateTime:"红包发放时间",
              cPickupTime:"cPickupTime",
              cRegTime:"激活时间",
              cAboTime:"作废时间",
              C_SendID:"红包发放ID",
              C_SendUserID:"发放人登录ID",
              C_SendName:"发放人名称",
              C_TotalWin:"总赢",
              TotalRecords:"总记录条数",
          },......]        
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

- 接口说明

  > 1.优先判断参数A_UserID是否为空
  >
  > 2.红包状态参数值可以以以下字符串以英文半角逗号分隔
  >
  > * "REG": 成功激活
  > * "PICKUP"
  > * "ABO"
  > * "YES"
  > * "UPD"

#### 53.获取会员的洗码费统计

* 接口名称: GetWashF4Clnt

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetWashF4Clnt"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              C_ID:"会员ID",
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetWashF4Clnt"
      },
      Response:
      {
          JsonData:
          [{
              C_ID:"会员ID",
              C_Name:"会员名称",
              C_UserID:"会员登录ID",      
              C_LastWTime:"洗码费结算开始时间",
              C_EndWTime:"洗码费结算结束时间",
              C_Date:"操作时间",
              C_WashR:"洗码率",
              C_WashS:"洗码量",
              C_WashF:"洗码费",
              C_Count:"结算注单数量",
          },......]        
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明:

  > 1.会检查当前查询会员是否在登录代理分支下，如果不在则中断查询
  >
  > 2.此接口返回的数据集由以下部分组成
  >
  > - 2条之前的时间最近结算记录,如果此会员之前有结算的情况
  > - 统计出此会员到当前为止可以结算的洗码费为一条记录

#### 54. 获取结算日志

* 接口名称: GetSettleAccounts

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetSettleAccounts"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              C_UserID:"结算源登录账号",
              A_UserID:"操作员登录账号",
              GameT:"结算类型",//结算洗码  结算抽水
              StartDate:"查询开始时间", //时间格式 'yyyy-MM-dd HH:mm:ss'
              EndDate:"查询结束时间",//时间格式 'yyyy-MM-dd HH:mm:ss'
              PageSize:"每页显示几条数据",
              CurePage:"当前页"
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetSettleAccounts"
      },
      Response:
      {
          JsonData:
          [{
              A_ID:"主键ID",
              A_Op:"操作员",
              A_Source:"结算源",      
              A_AObj:"结算目标",
              A_Amount:"结算金额",
              A_Count:"结算注单数量",
              A_WashR:"洗码率",
              A_Type:"结算类型",
              A_WashS:"洗码量",
              A_Start:"结算开始时间",
              A_End:"结算结束时间",
              A_Date:"操作时间",
              A_State:"结算状态", 
          },......],
          SumJson:
           [{
              TotalRecords:"总记录数",
              A_SumAmount:"结算金额",
              A_WashS:"洗码量",
           }],
          Count:"总记录数"
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

#### 55.获取交易日志

* 接口名称: GetTransactions

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetTransactions"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              L_Operator:"操作员登录账号",
              L_SourceUser:"交易源登录账号/上级代理登录账号",
              StartDate:"查询开始时间", //时间格式 'yyyy-MM-dd HH:mm:ss'
              EndDate:"查询结束时间",//时间格式 'yyyy-MM-dd HH:mm:ss'
              PageSize:"每页显示几条数据",
              CurePage:"当前页"
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetTransactions"
      },
      Response:
      {
          JsonData:
          [{
              L_ID:"日志ID",
              L_OpType:"交易类型",
              L_OpReason:"交易原因",      
              L_UserID:"交易目标登录ID",
              L_UserName:"交易目标昵称",
              L_TPoint:"交易金额",
              L_SourceUser:"交易源登录ID",
              L_SoureInto:"来源占成",
              L_RealInto:"实际金额",
              L_RealPoint:"实际分数",
              L_TPoint_B:"交易前余额",
              L_TPoint_E:"交易后余额",      
              L_OpTime:"交易时间",
              L_Operator:"操作员登录ID",
              L_OpInfo:"交易详情",
              L_IP:"IP",
              L_Address:"地址",
              L_Remark:"备注",
              L_OperatorID:"操作员ID",
          },......],
          SumJson:
           [{
              L_TPoint:"交易金额合计",
              L_RealInto:"实际金额合计",
              L_RealPoint:"实际分数合计",
           }]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

* 接口说明: 会检查当前操作员是否在登录代理分支下，如果不在则中断查询

#### 56. 获取登录日志

* 接口名称:GetLoginLog

* 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetLoginLog"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              L_User:"登录账号",
              L_PAgent:"上级代理登录账号",
              StartDate:"查询开始时间", //时间格式 'yyyy-MM-dd HH:mm:ss'
              EndDate:"查询结束时间",//时间格式 'yyyy-MM-dd HH:mm:ss'
              PageSize:"每页显示几条数据",
              CurePage:"当前页"
          }
      }   
  }
  ```

* 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetLoginLog"
      },
      Response:
      {
          JsonData:
          [{
              LogID:"日志ID",
              L_ULevel:"用户级别",//代理-会员
              L_PAgent:"父级代理登录ID",      
              L_IP:"IP",
              L_Addre:"地址",
              L_Time:"登录时间",
              L_ReMark:"备注",
              L_User:"登录者登录ID",
              TotalRecords:"总记录数",
          },......]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

#### 57. 获取操作日志

- 接口名称:GetOperationLog

- 请求报文

  ```json
  {
      Head:
      {
          Account:"登录账号",
          LoginID:"登录ID",
          Token:"登录认证",
          Ip:"客户端IP",//如果有中转则需要提供此参数
          Method:"GetTransactions"
      },
      Request:
      {
          IsZ:"应答报文是否需要Gzip压缩 默认是压缩的",
          RType:"应答报文数据格式 默认是Json格式",
          CFlag:"标记请求的客户端类型，根据特定的类型做特定的处理",//预留
          CVer:"客户端版本号",
          RequestParams:
          {
              L_OperatorID:"操作员ID",
              StartDate:"查询开始时间", //时间格式 'yyyy-MM-dd HH:mm:ss'
              EndDate:"查询结束时间",//时间格式 'yyyy-MM-dd HH:mm:ss'
              PageSize:"每页显示几条数据",
              CurePage:"当前页"
          }
      }   
  }
  ```

- 应答报文

  ```json
  {
    	Head:
      {
          Method:"GetLoginLog"
      },
      Response:
      {
          JsonData:
          [{
              LogID:"日志ID",
              LogTime:"操作时间",
              LogType:"操作类型",      
              LogInfo:"操作描述",
              OpID:"操作员ID",
              TotalRecords:"总记录数",
          },......]
      }
      Error://结果信息
      {
          ErrNo:"0000",//参见结果代码说明
          ErrMsg:"成功"
      }  
  }
  ```

#### 