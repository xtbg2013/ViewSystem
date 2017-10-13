using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Log
{
    class LogView : ILog
    {
        private TextBox _richTextBox;
        delegate void AddLogDelegate(string line);
        private LogHelper _logHelper;
        public LogView(TextBox textBox, LogHelper logHelper)
        {
            this._richTextBox = textBox;
            this._logHelper = logHelper;
        }
        private void Write(string line)
        {
            if (this._richTextBox.InvokeRequired)
            {
                _richTextBox.BeginInvoke(new AddLogDelegate(Write), line);
            }
            else
            {
                this._richTextBox.AppendText(line);
                this._richTextBox.Focus();
                this._richTextBox.Select(this._richTextBox.TextLength, 0);
                this._richTextBox.ScrollToCaret();
            }
        }
        public void Info(string info)
        {
            this.Write("Info: "+info+ "\r\n");
            this._logHelper.Info(info);
            
        }

        public void Debug(string info)
        {

            this.Write("Debug: " + info + "\r\n");
            this._logHelper.Debug(info);

        }

        public void Warn(string info)
        {
            this.Write("Warn: " + info + "\r\n");
            this._logHelper.Warn(info);
        }
        public void Error(string info)
        {
            this.Write("Error: " + info + "\r\n");
            this._logHelper.Error(info);
        }

        public void Fetal(string info)
        {

            this.Write("Fetal: " + info + "\r\n");
            this._logHelper.Fetal(info);
        }
    }
}
