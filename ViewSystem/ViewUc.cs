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
using DataService;
namespace ViewSystem
{
    public partial class ViewUc : UserControl
    {
        private Timer _redrawTimer;
        private int _row;
        private int _col;
        private ViewCell[][] _viewList;
        private int _MaxVal;

        private ILog _logger;
        private IData _data;
        public ViewUc(ILog logger, IData data, Dictionary<string,string> param)
        {
            InitializeComponent();
            this._logger = logger;
            this._data = data;
            LoadParam(param);
            InitTimer();
            InitViewCell();
        }


        private void LoadParam(Dictionary<string, string> param)
        {
            try
            {
                this._MaxVal = int.Parse(param["MaxValue"]);
                this._row = int.Parse(param["Row"]);
                this._col = int.Parse(param["Col"]);
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
                Interval = 100
            };
            this._redrawTimer.Tick += (s, e) =>
            {
                Dictionary <int ,double> value = this._data.ReadData();
                for (int i = 0; i < this._row; i++)
                    for (int j = 0; j < this._col; j++)
                    {
                        int key = i * this._row + j;
                        if(value.ContainsKey(key))
                            this._viewList[j][j].ProgressPainter = PainterFactory.GetProgressPainter(this._MaxVal, value[key]);
                    }
                
            };
            this._redrawTimer.Start();
        }
        private void InitViewCell()
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
                    Cell(out this._viewList[i][j], i, j, viewPanel);
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
            this.SuspendLayout();
            this.Controls.Add(viewPanel);
            this.ResumeLayout(false);
            this.PerformLayout();
            return viewPanel;
        }

        private void Cell(out ViewCell bc, int row, int col, TableLayoutPanel tbLayoutPanel)
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
