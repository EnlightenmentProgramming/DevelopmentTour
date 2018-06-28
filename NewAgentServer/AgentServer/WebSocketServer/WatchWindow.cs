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
        T_ConfigBLL configBll = new T_ConfigBLL();
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
            T_Config config = configBll.GetConfig();
        }


    }
}
