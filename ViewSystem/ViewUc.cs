using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProgressODoom;
using GlobalFiles;
using System.Collections;
using Log;
using LogicalModel;
namespace ViewSystem
{
    public partial class ViewUc : UserControl
    {
        private Timer _redrawTimer;
        private int _row;
        private int _col;
        private int _interval;
        private ViewCell[][] _viewList;
        private int _MaxVal;

        private ILog _logger;
        private ILogicalManager _logicalManager;
        public ViewUc(ILog logger, ILogicalManager logicalManager, Dictionary<string,string> param)
        {
            InitializeComponent();
            this._logger = logger;
            this._logicalManager = logicalManager;
            LoadParam(param);
            InitTimer();
            CreateViewCell();
        }


        private void LoadParam(Dictionary<string, string> param)
        {
            try
            {
                this._MaxVal = int.Parse(param["MaxValue"]);
                this._row = int.Parse(param["Row"]);
                this._col = int.Parse(param["Col"]);
                this._interval = int.Parse(param["Interval"]);
            }
            catch (Exception e)
            {
                this._logger.Error(e.Message);
            }
        } 
        private void InitTimer()
        {
            this._redrawTimer = new Timer()
            {
                Interval = this._interval
            };
            this._redrawTimer.Tick += (s, e) =>
            {
                Dictionary<int, double> value = new Dictionary<int, double>();
                string msg;
                bool res = this._logicalManager.ReadData(ref value,out msg);
                if (!res)
                    this._logger.Error(msg);
                //for (int i = 0; i < this._row; i++)
                //    for (int j = 0; j < this._col; j++)
                //    {
                //        int key = i * this._row + j;
                //        if (value.ContainsKey(key))
                //        {
                //            this._viewList[i][j].ProgressPainter = PainterFactory.GetProgressPainter(this._MaxVal, value[key]);
                //            this._viewList[i][j].Text = value[key].ToString("F4");
                //        }

                //    }

                for (int j = 0; j < this._col - 5; j++)
                    for (int i = this._row - 1; i >= 0; i--)
                    {
                        int key = j * this._row + this._row - i - 1;
                        if (value.ContainsKey(key))
                        {
                            this._viewList[i][j].ProgressPainter = PainterFactory.GetProgressPainter(this._MaxVal, value[key]);
                            this._viewList[i][j].Text = value[key].ToString("F4");
                        }

                    }


                for (int j = 5; j <= this._col - 1; j++)
                    for (int i = 0; i <= this._col - 1; i++)
                    {
                        int key = j * this._row + i;
                        if (value.ContainsKey(key))
                        {
                            this._viewList[i][j].ProgressPainter = PainterFactory.GetProgressPainter(this._MaxVal, value[key]);
                            this._viewList[i][j].Text = value[key].ToString("F4");
                        }

                    }

            };
            
        }

        public void Run(bool isRun)
        {
            if (isRun)
                this._redrawTimer.Start();
            else
                this._redrawTimer.Stop();
        }
        private void CreateViewCell()
        {
            this._viewList = new ViewCell[this._row][];
            for (int i = 0; i < this._row; i++)
                this._viewList[i] = new ViewCell[this._col];
        }
  
        private void ViewUc_Load(object sender, EventArgs e)
        {
            TableLayoutPanel viewPanel = this.CreateViewPanel();
            viewPanel.SuspendLayout();
            for (int i = 0; i < this._row; i++)
            {
                for (int j = 0; j < this._row; j++)
                    InitiCell(out this._viewList[i][j], i, j, viewPanel);
            }
            viewPanel.ResumeLayout(false);
           
        }
        

        private TableLayoutPanel CreateViewPanel()
        {
            TableLayoutPanel viewPanel = new TableLayoutPanel();
            int panelrow = this._row;
            int panelcol = this._col;
            viewPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            viewPanel.ColumnCount = panelcol;
            viewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            viewPanel.Location = new System.Drawing.Point(0, 0);
            viewPanel.Name = "ViewLayoutPanel";
            viewPanel.RowCount = panelrow;
            viewPanel.Size = new System.Drawing.Size(500, 500);
            viewPanel.TabIndex = 0;
           
            
          //  float widthfirst = 30;
            float widthPercent = (float)((100.0) / panelcol) - 1;
            float heightPercent = (float)(100.0 / panelrow) - 1;
            //viewPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, widthfirst));
            for (int col = 0; col < panelcol; col++)
                viewPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, widthPercent));
            
                
            for (int row = 0; row < panelrow; row++)
                viewPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, heightPercent));

            viewPanel.RowStyles[0].Height = 0;
            this.SuspendLayout();
            this.Controls.Add(viewPanel);
            this.ResumeLayout(false);
            this.PerformLayout();
            return viewPanel;
        }

        private void InitiCell(out ViewCell bc, int row, int col, TableLayoutPanel tbLayoutPanel)
        {
            bc = new ViewCell(row, col);
            bc.BorderPainter  = PainterFactory.GetPlainBoarderPainter();
            bc.ProgressPainter = PainterFactory.GetProgressPainter(this._MaxVal,0);
            bc.ForeColor = Color.Red;
            bc.Dock = System.Windows.Forms.DockStyle.Fill;
            bc.ProgressType = ProgressODoom.ProgressType.Smooth;
            bc.ShowPercentage = false;
            int count = row * col + 1;
            bc.Text = "0.0";
            bc.Name = "Cell_" + row+"_"+col;
            bc.Value = 100;
            tbLayoutPanel.Controls.Add(bc);
        }
    }
}
