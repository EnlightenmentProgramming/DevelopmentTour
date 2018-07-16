using DAL;
using DAL.LogicDAL;
using Dos.Common;
using Model.Comunication;
using Model.LogicModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.LogicBLL
{
    public class StatisticsBLL
    {
        ComunicationMsg sendMsg = new ComunicationMsg();
        ErrorMessage error = new ErrorMessage();
        HeadMessage sendHead = new HeadMessage();
        AStatisticsDAL sDal = new AStatisticsDAL();
        ClntStatisticsDAL cDal = new ClntStatisticsDAL();
        public string GetStatisticsData(string requestMsg,HeadMessage head)
        {
            sendHead.Method = head.Method ?? "";
            error.ErrNo = "0004";
            string responseMsg = "";
            string pId;
            try
            {
                if (CommonDAL.IsSubAgent(head.LoginID, out pId))
                {
                    head.LoginID = pId;//如果当前登录代理是子账号则，将此子账号的所属代理ID赋值为当前登录代理ID
                }
                StatisticsModel sModel = JSON.ToObject<StatisticsModel>(requestMsg);
                if (sModel == null)
                {
                    error.ErrMsg = "没有接收到正确的请求参数";
                }
                else
                {
                    switch(sendHead.Method)
                    {
                        case "GetAStatistics"://获取代理统计数据
                            responseMsg = sDal.GetAStatistics(sModel, head, out error);
                            break;
                        case "GetADayStatistics"://获取代理每日统计数据
                            responseMsg = sDal.GetADayStatistics(sModel, head, out error);
                            break;
                        case "GetClntStatistics"://获取会员统计数据
                            responseMsg = cDal.GetClntStatistics(sModel, head, out error);
                            break;
                        case "GetClntDayStatistics"://获取会员每日统计数据
                            responseMsg = cDal.GetClntDayStatistics(sModel, head, out error);
                            break;
                        case "GetClntBetBills"://获取会员下注明细
                            responseMsg = cDal.GetClntBetBills(sModel, head, out error);
                            break;
                        case "GetPointDetail"://获取上下分明细
                            responseMsg = cDal.GetPointDetail(sModel, head, out error);
                            break;
                        case "GetTableResult"://获取游戏结果数据
                            responseMsg = cDal.GetTableResult(sModel, head, out error);
                            break;
                        case "PromotionA_Clnt4A"://获取指定代理下的会员推广统计数据 增量
                            responseMsg = cDal.PromotionA_Clnt4A(sModel, head, out error);
                            break;
                        case "PromotionA_AllClnt4A"://获取指定代理下的会员推广统计数据 存量
                            responseMsg = cDal.PromotionA_AllClnt4A(sModel, head, out error);
                            break;
                        case "H5ClntPointSum"://指定代理下H5会员第三方上分合计
                            responseMsg = cDal.H5ClntPointSum(sModel, head, out error);
                            break;
                        case "H5ClntPointDetail"://H5会员第三方上分明细
                            responseMsg = cDal.H5ClntPointDetail(sModel, head, out error);
                            break;
                        case "H5ClntPointSum_A"://获取指定代理下所有H5会员第三方上分明细
                            responseMsg =cDal.H5ClntPointDetail_A(sModel, head, out error);
                            break;
                        case "RedEnvelopeSum"://查询指定代理自己及直属代理及直属会员下红包发放合计
                            responseMsg = cDal.RedEnvelopeSum(sModel, head, out error);
                            break;
                        case "RedEnvelopeDetail"://查询指定会员下红包发送明细
                            responseMsg = cDal.RedEnvelopeDetail(sModel, head, out error);
                            break;
                    }
                }
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Error = JSON.ToJSON(error);
                sendMsg.Reponse = string.IsNullOrEmpty(responseMsg) ? "{}" : responseMsg;
                return JSON.ToJSON(sendMsg);
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(LoginBLL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Reponse = "{}";
                sendMsg.Error = JSON.ToJSON(error);
                return JSON.ToJSON(sendMsg);
            }
        }
    }
}
