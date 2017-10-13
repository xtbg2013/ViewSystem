using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log
{
    public interface ILog
    {
        void Info(string msg);
        void Warn(string msg);
        void Error(string msg);
        void Debug(string msg);
        void Fetal(string msg);
    }
}
