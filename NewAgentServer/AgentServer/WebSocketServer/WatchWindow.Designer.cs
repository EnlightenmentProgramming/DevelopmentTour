namespace WebSocketServer
{
    partial class WatchWindow
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.cmb_IP = new System.Windows.Forms.ComboBox();
            this.txtB_Port = new System.Windows.Forms.TextBox();
            this.btn_Start = new System.Windows.Forms.Button();
            this.btn_End = new System.Windows.Forms.Button();
            this.btn_Setting = new System.Windows.Forms.Button();
            this.list_Clients = new System.Windows.Forms.ListView();
            this.Address = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Ip = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ComTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.State = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ConID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LoginID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.KeepTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // cmb_IP
            // 
            this.cmb_IP.FormattingEnabled = true;
            this.cmb_IP.Location = new System.Drawing.Point(12, 12);
            this.cmb_IP.Name = "cmb_IP";
            this.cmb_IP.Size = new System.Drawing.Size(134, 20);
            this.cmb_IP.TabIndex = 0;
            // 
            // txtB_Port
            // 
            this.txtB_Port.Location = new System.Drawing.Point(168, 10);
            this.txtB_Port.Name = "txtB_Port";
            this.txtB_Port.Size = new System.Drawing.Size(64, 21);
            this.txtB_Port.TabIndex = 1;
            // 
            // btn_Start
            // 
            this.btn_Start.Location = new System.Drawing.Point(266, 8);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(110, 23);
            this.btn_Start.TabIndex = 2;
            this.btn_Start.Text = "监听";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // btn_End
            // 
            this.btn_End.Location = new System.Drawing.Point(401, 9);
            this.btn_End.Name = "btn_End";
            this.btn_End.Size = new System.Drawing.Size(110, 23);
            this.btn_End.TabIndex = 3;
            this.btn_End.Text = "停止";
            this.btn_End.UseVisualStyleBackColor = true;
            this.btn_End.Click += new System.EventHandler(this.btn_End_Click);
            // 
            // btn_Setting
            // 
            this.btn_Setting.Location = new System.Drawing.Point(1140, 8);
            this.btn_Setting.Name = "btn_Setting";
            this.btn_Setting.Size = new System.Drawing.Size(110, 23);
            this.btn_Setting.TabIndex = 4;
            this.btn_Setting.Text = "设置参数";
            this.btn_Setting.UseVisualStyleBackColor = true;
            this.btn_Setting.Click += new System.EventHandler(this.btn_Setting_Click);
            // 
            // list_Clients
            // 
            this.list_Clients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Address,
            this.Ip,
            this.ComTime,
            this.State,
            this.ConID,
            this.LoginID,
            this.KeepTime});
            this.list_Clients.FullRowSelect = true;
            this.list_Clients.GridLines = true;
            this.list_Clients.Location = new System.Drawing.Point(12, 47);
            this.list_Clients.Name = "list_Clients";
            this.list_Clients.Size = new System.Drawing.Size(1238, 479);
            this.list_Clients.TabIndex = 5;
            this.list_Clients.UseCompatibleStateImageBehavior = false;
            this.list_Clients.View = System.Windows.Forms.View.Details;
            // 
            // Address
            // 
            this.Address.Text = "地址";
            this.Address.Width = 200;
            // 
            // Ip
            // 
            this.Ip.Text = "IP";
            this.Ip.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Ip.Width = 120;
            // 
            // ComTime
            // 
            this.ComTime.Text = "通讯时间";
            this.ComTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ComTime.Width = 127;
            // 
            // State
            // 
            this.State.Text = "状态";
            this.State.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ConID
            // 
            this.ConID.Text = "唯一键";
            this.ConID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ConID.Width = 160;
            // 
            // LoginID
            // 
            this.LoginID.Text = "登录ID";
            this.LoginID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.LoginID.Width = 130;
            // 
            // KeepTime
            // 
            this.KeepTime.Text = "发呆/分钟";
            this.KeepTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.KeepTime.Width = 70;
            // 
            // WatchWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1260, 551);
            this.Controls.Add(this.list_Clients);
            this.Controls.Add(this.btn_Setting);
            this.Controls.Add(this.btn_End);
            this.Controls.Add(this.btn_Start);
            this.Controls.Add(this.txtB_Port);
            this.Controls.Add(this.cmb_IP);
            this.Name = "WatchWindow";
            this.Text = "AgentService";
            this.Load += new System.EventHandler(this.WatchWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmb_IP;
        private System.Windows.Forms.TextBox txtB_Port;
        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.Button btn_End;
        private System.Windows.Forms.Button btn_Setting;
        private System.Windows.Forms.ListView list_Clients;
        private System.Windows.Forms.ColumnHeader Address;
        private System.Windows.Forms.ColumnHeader Ip;
        private System.Windows.Forms.ColumnHeader ComTime;
        private System.Windows.Forms.ColumnHeader State;
        private System.Windows.Forms.ColumnHeader ConID;
        private System.Windows.Forms.ColumnHeader LoginID;
        private System.Windows.Forms.ColumnHeader KeepTime;
    }
}

