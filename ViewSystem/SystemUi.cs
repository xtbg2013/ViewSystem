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
using System.Reflection;
using LogicalModel;
using LogicalModel.Data;
using System.IO;
namespace ViewSystem
{
    public partial class SystemUi : Form
    {
        private ConfigReader _configReader;
        private Dictionary<string, string> _param;
        private ILog _logger;
        private ILogicalManager _logicalManager;
       
        public SystemUi()
        {
            InitializeComponent();
            
        }

        private void LoadSetting()
        {
            this._param = new Dictionary<string, string>();
            try
            {
                this._configReader = new ConfigReader("Settings.xml");
                this._param["MaxValue"] = this._configReader.GetItem("MaxValue");
                this._param["Row"] = this._configReader.GetItem("Row");
                this._param["Col"] = this._configReader.GetItem("Col");
                this._param["Interval"] = this._configReader.GetItem("Interval");
                this._param["Port"] = this._configReader.GetItem("Port");
            }
            catch (Exception e)
            {
                this._logger.Error(e.Message);
                this._logger.Error(e.StackTrace);
            }
        }
        private void InitSystem()
        {
            this._logger = new LogView(this.textBoxInfo, new LogHelper());
            ConfigParam param = new ConfigParam();
            if (this._param.ContainsKey("Port"))
                param.Port = this._param["Port"];
            if (this._param.ContainsKey("Row"))
                param.Row = int.Parse(this._param["Row"]);
            if (this._param.ContainsKey("Col"))
                param.Col = int.Parse(this._param["Col"]);
            try
            {
                this._logicalManager = new LogicalManager(this._logger, param);
                this._logicalManager.InitManager();
            }
            catch (Exception e)
            {
                this._logger.Error(e.Message);
                this._logger.Error(e.StackTrace);
            }
            
            this.btnTest.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnStop.Enabled = false;
            this.btnBd.Enabled = false;

            
        }

        private void SystemUi_Load(object sender, EventArgs e)
        {
            LoadSetting();
            InitSystem();
            AddViewUC();
        }
        
        private void AddViewUC()
        {

            this.ViewUc = new ViewUc(this._logger, this._logicalManager, this._param);
            this.ViewUc.Location = new System.Drawing.Point(0, 0);
            this.ViewUc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ViewUc.Name = "boarduc";
            this.panelView.Controls.Add(this.ViewUc);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (this._logicalManager != null)
            {
                bool res = false;
                try
                {
                    res = this._logicalManager.Connect(true);
                }
                catch (Exception ex)
                {
                    this._logger.Error(ex.Message);
                    this._logger.Error(ex.StackTrace);
                    res = false;
                }
                
                if (res)
                {
                    this._logger.Info("connect success");
                    this.btnConnect.Enabled = false;
                    this.btnBd.Enabled = true;
                    this.btnTest.Enabled = false; ;
                    this.btnStop.Enabled = false;
                    this.btnSave.Enabled = false;
                }
                else
                {
                    this.btnBd.Enabled = false;
                    this.btnStop.Enabled = false;
                    this.btnSave.Enabled = false;
                    this.btnTest.Enabled = false;
                    this._logger.Error("connect exception");
                }

            }
            else
            {
                this.btnStop.Enabled = false;
                this.btnSave.Enabled = false;
                this.btnTest.Enabled = false;
                this._logger.Error("the object IData is not exist");
            }

        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            this.btnBd.Enabled = false;
            this.btnTest.Enabled = false;
            this.btnConnect.Enabled = false;
            this.btnStop.Enabled = true;
            this.ViewUc.Run(true);
            this._logger.Info("test is running");    
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.btnTest.Enabled = true;
            this.btnConnect.Enabled = false;
            this.btnStop.Enabled = false;
            this.btnBd.Enabled = true;
            this.ViewUc.Run(false);
            this._logger.Info("test is stopped");
        }

        private void btnBd_Click(object sender, EventArgs e)
        {


            //Task t = Task.Run(() => {
            //    string msg;
            //    if (this._logicalManager.Calibration(out msg))
            //    {
            //        this.btnTest.Enabled = true;
            //        this.btnConnect.Enabled = false;
            //        this.btnStop.Enabled = false;
            //        this._logger.Info("calibration success");
            //    }
            //    else
            //    {
            //        this.btnTest.Enabled = false;
            //        this.btnConnect.Enabled = false;
            //        this.btnStop.Enabled = false;
            //        this._logger.Info("calibration unsuccess," + msg);
            //    }
            //    this._logger.Info("calibrating over");
            //});


            this._logger.Info("begin calibrating..............");

            try
            {

                string msg;
                if (this._logicalManager.Calibration(out msg))
                {
                    this.btnTest.Enabled = true;
                    this.btnConnect.Enabled = false;
                    this.btnStop.Enabled = false;
                    this._logger.Info("calibration success");
                }
                else
                {
                    this.btnTest.Enabled = false;
                    this.btnConnect.Enabled = false;
                    this.btnStop.Enabled = false;
                    this._logger.Info("calibration unsuccess," + msg);
                }
                this._logger.Info("calibrating over");
            }
            catch (Exception ex)
            {
                this._logger.Error(ex.Message);
                this._logger.Error(ex.StackTrace); 
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog save  = new SaveFileDialog();
            save.Filter = "xlsx files   (*.xlsx)|*.xlsx";
            save.FilterIndex = 2;
            save.RestoreDirectory = true;

            if (save.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (this._logicalManager.Save(save.FileName))
                        this._logger.Info("save success");
                    else
                        this._logger.Error("save exception");
                }
                catch (Exception ex)
                {
                    this._logger.Error(ex.Message);
                    this._logger.Error(ex.StackTrace);
                }
                
            }
        }
    }
}
