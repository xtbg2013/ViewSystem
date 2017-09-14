using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GlobalFiles;
using System.Diagnostics;
using Log;
using DataService;
namespace ViewSystem
{
    public partial class SystemUi : Form
    {
        private ConfigReader _configReader;
        private Dictionary<string, string> _param;
        private ILog _logger;
        private IData _dataComm;
        public SystemUi()
        {
            InitializeComponent();
            LoadParam();
        }
        private void LoadParam()
        {
            this._logger = new LogHelper();
            this._param = new Dictionary<string, string>();
            this._dataComm = new ComData();
            try
            {
                this._configReader = new ConfigReader("Settings.xml");
                this._param["MaxValue"] = _configReader.GetItem("MaxValue");
                this._param["Row"] = _configReader.GetItem("Row");
                this._param["Col"] = _configReader.GetItem("Col");
            }
            catch (Exception e)
            {
                this._logger.Error(e.Message);
            }
        }

        private void SystemUi_Load(object sender, EventArgs e)
        {
            AddViewUC();
        }
        
        private void AddViewUC()
        {
           
            this.ViewUc = new ViewUc(this._logger, this._dataComm, this._param);
            this.ViewUc.Location = new System.Drawing.Point(0, 0);
            this.ViewUc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ViewUc.Name = "boarduc";
            this.panelView.Controls.Add(this.ViewUc);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

        }

        private void btnTest_Click(object sender, EventArgs e)
        {

        }

        private void btnStop_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}
