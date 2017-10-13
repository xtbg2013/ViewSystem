using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalModel
{
    public interface ILogicalManager
    {
        void InitManager();
        bool Connect(bool isConnected);
        bool Calibration(out string msg); //标定
        bool ReadData(out Dictionary<int, double> data, out string msg);
        bool Save(string path);
    }
}
