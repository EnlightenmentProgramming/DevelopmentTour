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
            // 
            // btn_End
            // 
            this.btn_End.Location = new System.Drawing.Point(401, 9);
            this.btn_End.Name = "btn_End";
            this.btn_End.Size = new System.Drawing.Size(110, 23);
            this.btn_End.TabIndex = 3;
            this.btn_End.Text = "停止";
            this.btn_End.UseVisualStyleBackColor = true;
            // 
            // btn_Setting
            // 
            this.btn_Setting.Location = new System.Drawing.Point(1140, 8);
            this.btn_Setting.Name = "btn_Setting";
            this.btn_Setting.Size = new System.Drawing.Size(110, 23);
            this.btn_Setting.TabIndex = 4;
            this.btn_Setting.Text = "设置参数";
            this.btn_Setting.UseVisualStyleBackColor = true;
            // 
            // WatchWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1260, 551);
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
    }
}

