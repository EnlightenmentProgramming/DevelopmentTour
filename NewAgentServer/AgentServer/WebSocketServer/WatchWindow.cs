using BLL.Comunication;
using BLL.ConfigBLL;
using Common;
using Model.SqliteModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebSocketServer
{
    public partial class WatchWindow : Form
    {
        #region 成员变量
        //配置对象
        T_ConfigBLL configBll = new T_ConfigBLL();
        //锁屏密码
        public string _Lockpass = string.Empty;
        //自定义websocket对象
        WsSocket socket = new WsSocket();

        #endregion
        public WatchWindow()
        {
            InitializeComponent();
            //LogHelper.WriteWarnLog(typeof(WatchWindow),"测试日志");
        }
        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WatchWindow_Load(object sender, EventArgs e)
        {
            try
            {
                T_Config config = configBll.GetConfig();
                cmb_IP.Items.AddRange(CommonHelper.GetLocIps());
                cmb_IP.Text = config.Ip;
                _Lockpass = config.LockPass;
                txtB_Port.Text = config.Port.ToString();
                btn_Start_Click(sender, e);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(WatchWindow), ex);
            }
           
        }
        /// <summary>
        /// 点击监听按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Start_Click(object sender, EventArgs e)
        {
            try
            {
                if (socket.StartListen(cmb_IP.Text, txtB_Port.Text))//(socket.StartListen("192.168.1.106","27000"))//
                {
                    btn_Start.Enabled = false;
                    btn_End.Enabled = true;
                }
                else
                {
                    MessageBox.Show("端口被占用或未能监听");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(WatchWindow), ex);
            }
        }
        /// <summary>
        /// 点击停止监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_End_Click(object sender, EventArgs e)
        {
            socket.StopListen();
            btn_Start.Enabled = true;
            btn_End.Enabled = false;
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Setting_Click(object sender, EventArgs e)
        {
            SetConfig setConfig = new SetConfig();
            setConfig.ShowDialog();
            if(setConfig._returnVal)
            {
                cmb_IP.Text = setConfig._ip;
                txtB_Port.Text = setConfig._port;
                _Lockpass = setConfig._lockPass;               
            }
        }
    }
}
