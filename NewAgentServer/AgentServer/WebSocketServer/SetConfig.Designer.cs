namespace WebSocketServer
{
    partial class SetConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ck_Lanuge = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tB_RecIP = new System.Windows.Forms.TextBox();
            this.bt_exit = new System.Windows.Forms.Button();
            this.bt_set = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tB_PassWord = new System.Windows.Forms.TextBox();
            this.cB_Auto = new System.Windows.Forms.CheckBox();
            this.tB_Times = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tB_Port = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tB_IP = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ck_Lanuge
            // 
            this.ck_Lanuge.AutoSize = true;
            this.ck_Lanuge.Checked = true;
            this.ck_Lanuge.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ck_Lanuge.Location = new System.Drawing.Point(79, 175);
            this.ck_Lanuge.Name = "ck_Lanuge";
            this.ck_Lanuge.Size = new System.Drawing.Size(120, 16);
            this.ck_Lanuge.TabIndex = 30;
            this.ck_Lanuge.Text = "开机自动启动程序";
            this.ck_Lanuge.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 146);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 29;
            this.label6.Text = "闲置：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 28;
            this.label3.Text = "接收中心IP：";
            // 
            // tB_RecIP
            // 
            this.tB_RecIP.Location = new System.Drawing.Point(80, 106);
            this.tB_RecIP.Name = "tB_RecIP";
            this.tB_RecIP.Size = new System.Drawing.Size(139, 21);
            this.tB_RecIP.TabIndex = 27;
            // 
            // bt_exit
            // 
            this.bt_exit.Location = new System.Drawing.Point(123, 208);
            this.bt_exit.Name = "bt_exit";
            this.bt_exit.Size = new System.Drawing.Size(75, 29);
            this.bt_exit.TabIndex = 26;
            this.bt_exit.Text = "退  出";
            this.bt_exit.UseVisualStyleBackColor = true;
            this.bt_exit.Click += new System.EventHandler(this.bt_exit_Click);
            // 
            // bt_set
            // 
            this.bt_set.Location = new System.Drawing.Point(31, 208);
            this.bt_set.Name = "bt_set";
            this.bt_set.Size = new System.Drawing.Size(75, 29);
            this.bt_set.TabIndex = 25;
            this.bt_set.Text = "设  置";
            this.bt_set.UseVisualStyleBackColor = true;
            this.bt_set.Click += new System.EventHandler(this.bt_set_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(117, 147);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 24;
            this.label5.Text = "分钟";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 23;
            this.label4.Text = "锁屏密码：";
            // 
            // tB_PassWord
            // 
            this.tB_PassWord.Location = new System.Drawing.Point(80, 75);
            this.tB_PassWord.Name = "tB_PassWord";
            this.tB_PassWord.Size = new System.Drawing.Size(139, 21);
            this.tB_PassWord.TabIndex = 22;
            // 
            // cB_Auto
            // 
            this.cB_Auto.AutoSize = true;
            this.cB_Auto.Location = new System.Drawing.Point(150, 146);
            this.cB_Auto.Name = "cB_Auto";
            this.cB_Auto.Size = new System.Drawing.Size(72, 16);
            this.cB_Auto.TabIndex = 21;
            this.cB_Auto.Text = "自动断开";
            this.cB_Auto.UseVisualStyleBackColor = true;
            // 
            // tB_Times
            // 
            this.tB_Times.Location = new System.Drawing.Point(80, 142);
            this.tB_Times.Name = "tB_Times";
            this.tB_Times.Size = new System.Drawing.Size(34, 21);
            this.tB_Times.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 19;
            this.label2.Text = "端口：";
            // 
            // tB_Port
            // 
            this.tB_Port.Location = new System.Drawing.Point(80, 44);
            this.tB_Port.Name = "tB_Port";
            this.tB_Port.Size = new System.Drawing.Size(139, 21);
            this.tB_Port.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 17;
            this.label1.Text = "IP地址：";
            // 
            // tB_IP
            // 
            this.tB_IP.Location = new System.Drawing.Point(80, 14);
            this.tB_IP.Name = "tB_IP";
            this.tB_IP.Size = new System.Drawing.Size(139, 21);
            this.tB_IP.TabIndex = 16;
            // 
            // SetConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(241, 273);
            this.Controls.Add(this.ck_Lanuge);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tB_RecIP);
            this.Controls.Add(this.bt_exit);
            this.Controls.Add(this.bt_set);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tB_PassWord);
            this.Controls.Add(this.cB_Auto);
            this.Controls.Add(this.tB_Times);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tB_Port);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tB_IP);
            this.Name = "SetConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SetConfig";
            this.Load += new System.EventHandler(this.SetConfig_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ck_Lanuge;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tB_RecIP;
        private System.Windows.Forms.Button bt_exit;
        private System.Windows.Forms.Button bt_set;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tB_PassWord;
        private System.Windows.Forms.CheckBox cB_Auto;
        private System.Windows.Forms.TextBox tB_Times;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tB_Port;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tB_IP;
    }
}