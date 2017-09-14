using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataService
{
    public class ComData: IData
    {
        public Dictionary<int, double> ReadData()
        {
            return new Dictionary<int, double>();
        }
        public bool Connect()
        {
            return true;
        }
        public bool EnableRun(bool isRun)
        {
            return true;
        }
    }
}
