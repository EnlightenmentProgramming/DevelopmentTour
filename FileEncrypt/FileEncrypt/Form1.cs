using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileEncrypt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_FileName_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Filter = "xml文件(*.xml)|*.xml|文本文件(*.txt)|*.txt|所有文件|*.*";
            openfileDialog.Title = "请选择需要加|解密的文件";
            if(openfileDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                txt_FileName.Text = openfileDialog.FileName;
            }
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_encrypt_Click(object sender, EventArgs e)
        {
            string inFile = txt_FileName.Text.Trim();
            if(string.IsNullOrEmpty(inFile))
            {
                MessageBox.Show("请选择需要加密的文件");
                return;
            }
            string outFile = inFile + ".dat";
            //string password = pwd.Text.Trim();
            //if(string.IsNullOrEmpty(password))
            //{
            //    MessageBox.Show("请输入加密密码");
            //    return;
            //}
            string password = "skycity@+2017";
            FileEncrypt.DESFileClass.EncryptFile(inFile, outFile, password);//加密文件
            File.Delete(inFile);
            txt_FileName.Text = string.Empty;
            //pwd.Text = string.Empty;
            MessageBox.Show("文件加密成功");
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Decrypt_Click(object sender, EventArgs e)
        {
            string inFile = txt_FileName.Text;
            if (string.IsNullOrEmpty(inFile))
            {
                MessageBox.Show("请选择需要解密的文件");
                return;
            }
            string outFile = inFile.Substring(0, inFile.Length - 4);
            string password = pwd.Text.Trim();            
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("请输入解密密码");
                return;
            }
            FileEncrypt.DESFileClass.DecryptFile(inFile, outFile, password);
            File.Delete(inFile);
            txt_FileName.Text = string.Empty;
            pwd.Text = string.Empty;
            MessageBox.Show("文件解密成功");
        }
    }
}
