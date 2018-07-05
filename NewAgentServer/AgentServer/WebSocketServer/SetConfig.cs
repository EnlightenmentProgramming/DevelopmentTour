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
    public partial class SetConfig : Form
    {
        #region 成员变量
        T_Config _config;
        public string _ip;
        public string _port;
        public int _isCheck = 0;
        public int _iTimes = 0;
        public string _lockPass = string.Empty;
        public string _recIp;
        public bool _returnVal;
        #endregion
        public SetConfig()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 窗体加载完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetConfig_Load(object sender, EventArgs e)
        {
            _returnVal = false;
            _config = new T_Config();
            if(_config != null)
            {
                string s = CommonHelper.GetLocIps()[0];
                tB_IP.Text = string.IsNullOrEmpty(_config.Ip) ? s : _config.Ip;
                tB_Port.Text = _config.Port.ToString();
                tB_Times.Text = _config.AutoCutTime.ToString();
                tB_RecIP.Text = _config.RecIp;
                if(_config.IsAutoCut ==1)
                {
                    cB_Auto.Checked = true;
                }
                else
                {
                    cB_Auto.Checked = false;
                }
            }
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_set_Click(object sender, EventArgs e)
        {
            //判断不能为空
            _ip = tB_IP.Text;
            _port = tB_Port.Text;
            string _times = tB_Times.Text;
            string _password = tB_PassWord.Text;
            _recIp = tB_RecIP.Text;
            if (string.IsNullOrEmpty(_ip))
            {
                MessageBox.Show("ip地址不能为空。请填写正确的IP地址。");
                return;
            }
            if (string.IsNullOrEmpty(_port))
            {
                MessageBox.Show("端口不能为空,而且必须是数字。");
                return;
            }
            if (cB_Auto.Checked && string.IsNullOrEmpty(_times))
            {
                MessageBox.Show("自动踢人，必须填写踢人时间。");
                return;
            }
            _config.Ip = _ip;
            _config.Port = int.Parse(_port);
            _config.AutoCutTime = int.Parse(_times);
            _config.RecIp = _recIp;
            _iTimes = int.Parse(_times);
            if (cB_Auto.Checked)
                _config.IsAutoCut = 1;
            else
                _config.IsAutoCut = 0;
            _isCheck = _config.IsAutoCut;
            _config.id = 999;
            if (!string.IsNullOrEmpty(_password))
            {
                _config.LockPass = _password.MD5();
                _lockPass = _password;
            }

            if(new T_ConfigBLL().UpdateConfig(_config))
            {
                this.bt_set.Enabled = false;
                _returnVal = true;
            }
            else
            {
                _returnVal = false;
            }   

            
        }

    }
}
