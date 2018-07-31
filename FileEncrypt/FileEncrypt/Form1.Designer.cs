namespace FileEncrypt
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.txt_FileName = new System.Windows.Forms.TextBox();
            this.btn_FileName = new System.Windows.Forms.Button();
            this.pwd = new System.Windows.Forms.TextBox();
            this.btn_encrypt = new System.Windows.Forms.Button();
            this.btn_Decrypt = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(81, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "请选择文件：";
            // 
            // txt_FileName
            // 
            this.txt_FileName.Location = new System.Drawing.Point(164, 33);
            this.txt_FileName.Name = "txt_FileName";
            this.txt_FileName.Size = new System.Drawing.Size(163, 21);
            this.txt_FileName.TabIndex = 1;
            // 
            // btn_FileName
            // 
            this.btn_FileName.Location = new System.Drawing.Point(333, 33);
            this.btn_FileName.Name = "btn_FileName";
            this.btn_FileName.Size = new System.Drawing.Size(33, 21);
            this.btn_FileName.TabIndex = 2;
            this.btn_FileName.Text = "...";
            this.btn_FileName.UseVisualStyleBackColor = true;
            this.btn_FileName.Click += new System.EventHandler(this.btn_FileName_Click);
            // 
            // pwd
            // 
            this.pwd.Enabled = false;
            this.pwd.Location = new System.Drawing.Point(164, 76);
            this.pwd.Name = "pwd";
            this.pwd.PasswordChar = '*';
            this.pwd.Size = new System.Drawing.Size(163, 21);
            this.pwd.TabIndex = 4;
            this.pwd.Visible = false;
            // 
            // btn_encrypt
            // 
            this.btn_encrypt.Location = new System.Drawing.Point(83, 136);
            this.btn_encrypt.Name = "btn_encrypt";
            this.btn_encrypt.Size = new System.Drawing.Size(108, 34);
            this.btn_encrypt.TabIndex = 5;
            this.btn_encrypt.Text = "加密文件";
            this.btn_encrypt.UseVisualStyleBackColor = true;
            this.btn_encrypt.Click += new System.EventHandler(this.btn_encrypt_Click);
            // 
            // btn_Decrypt
            // 
            this.btn_Decrypt.Location = new System.Drawing.Point(258, 136);
            this.btn_Decrypt.Name = "btn_Decrypt";
            this.btn_Decrypt.Size = new System.Drawing.Size(108, 34);
            this.btn_Decrypt.TabIndex = 6;
            this.btn_Decrypt.Text = "解密文件";
            this.btn_Decrypt.UseVisualStyleBackColor = true;
            this.btn_Decrypt.Click += new System.EventHandler(this.btn_Decrypt_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(81, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "请输入密码：";
            this.label2.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(471, 212);
            this.Controls.Add(this.btn_Decrypt);
            this.Controls.Add(this.btn_encrypt);
            this.Controls.Add(this.pwd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_FileName);
            this.Controls.Add(this.txt_FileName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "文件加密工具";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_FileName;
        private System.Windows.Forms.Button btn_FileName;
        private System.Windows.Forms.TextBox pwd;
        private System.Windows.Forms.Button btn_encrypt;
        private System.Windows.Forms.Button btn_Decrypt;
        private System.Windows.Forms.Label label2;
    }
}

