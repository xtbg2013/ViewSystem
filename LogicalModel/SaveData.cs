using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Spire.Xls;
namespace LogicalModel
{
    
    public class SaveData
    {
        public bool SaveXls(string path, Dictionary<int, int> data)
        {

            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];
            sheet.Name = "记录数据";

            sheet.Range["A1"].Text = "1";
            sheet.Range["A1"].Text = "1";
            sheet.Range["A1"].Text = "1";


            foreach (var item in data)
            {

                 
            }
             
            sheet.Range["A2"].Text = "1";

            workbook.SaveToFile(path, ExcelVersion.Version2013);

            return true;
        }
    }
}
