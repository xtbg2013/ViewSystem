using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgressODoom;
namespace ViewSystem
{
    class ViewCell:ProgressBarEx
    {
        private int rowId;
        private int colId;
        public ViewCell(int rowId, int colId)
        {
            this.rowId = rowId;
            this.colId = colId;
        }
        public int RowId { get { return rowId; } }
        public int ColId { get { return colId; } }
    }
}
