using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicalModel;
using LogicalModel.Data;
using System.IO.Ports;
namespace DataService
{ 
    public class ComData: IData
    {
        private ConfigParam _param;
        private SerialPort _com;
        private static object lockObj = new object();
        private static byte[] buffer = new byte[0x2000];
        private static int position = 0;
        private static int bufferBytes = 0;

        public ComData(ConfigParam param)
        {
            this._param = param;
            this._com = new SerialPort();
            this._com.BaudRate = 115200;
            this._com.StopBits = StopBits.One;
            this._com.DataBits = 8;
            this._com.Parity = Parity.None;
            this._com.PortName = param.Port;
            this._com.ReadTimeout = 5000;
        }

        private int ConvertToInt(byte highByte, byte lowByte)
        {
            int x = highByte << 8;
            int y = lowByte;
            return (x+y);
        }
        public void ReadData(ref Dictionary<int, int> data)
        {
            int readLen = 220;
            int packLen = 22;

            lock (lockObj)
            {
                int len = this._com.Read(buffer, position, readLen);
                if (len > 0)
                {
                    position += len;
                }
                
                while (position >= packLen)
                {
                    //判断包头
                    int high = buffer[1] & 0xF0;
                    int low = buffer[1] & 0x0F;

                    if ((buffer[0] == 0xA5) && high == 0xb0) //数据完整
                    {
                        if (low >= 1 && low <= 10)   //数据正常
                        {
                            for (int i = 2; i < packLen;)
                            {
                                data[(low - 1) * 10 + (i - 2) / 2] = ConvertToInt(buffer[i], buffer[i + 1]);
                                i += 2;
                            }
                        }
                        else
                        {
                            return;
                        }

                        Array.Copy(buffer, packLen, buffer, 0, position - packLen);//前移动22字节数据,丢弃整包数据
                        position -= packLen;
                    }
                    else
                    {
                        Array.Copy(buffer, 2, buffer, 0, position - 2);//前移动2字节数据
                        position -= 2;
                    }
                }
            }
            
        }

        public void ReadData1(ref Dictionary<int, int> data, byte[] recvData, int recvLen)
        {
            int packLen = 22;
            lock (lockObj)
            {
                Array.Copy(recvData, 0, buffer, position, recvLen);
                int len = recvLen;
                if (len > 0)
                {
                    position += len;
                }
                while (position >= packLen)
                {
                    //判断包头
                    int high = buffer[1] & 0xF0;
                    int low = buffer[1] & 0x0F;

                    if ((buffer[0] == 0xA5) && high == 0xb0) //数据完整
                    {
                        if (low >= 1)   //数据正常
                        {
                            for (int i = 2; i < packLen;)
                            {
                                data[(low - 1) * 10 + (i - 2) / 2] = ConvertToInt(buffer[i], buffer[i + 1]);
                                i += 2;
                            }
                        }
                        Array.Copy(buffer, packLen, buffer, 0, position - packLen);//前移动22字节数据,丢弃整包数据
                        position -= packLen;
                    }
                    else
                    {
                        Array.Copy(buffer, 2, buffer, 0, position - 2);//前移动2字节数据
                        position -= 2;
                    }
                }
            }

        }

        private void TestRecv()
        {
            byte[] recvData = new byte[220];
            recvData[0] = 0xa5;
            recvData[1] = 0xb1;
            for (int i = 2; i < 22; i++)
            {
                recvData[i] = (byte)i;
            }

            Dictionary<int, int> data = new Dictionary<int, int>();
            ReadData1(ref data, recvData, 22);

            recvData[0] = 0xa5;
            recvData[1] = 0xb1;
            for (int i = 2; i < 22; i++)
            {
                recvData[i] = (byte)i;
            }
            recvData[22] = 0xa5;
            recvData[23] = 0xb2;
            for (int i = 24; i < 43; i++)
            {
                recvData[i] = (byte)i;
            }

            ReadData1(ref data, recvData, 43);

            recvData[0] = 0x10;
            recvData[1] = 0xa5;
            recvData[2] = 0xb3;

            for (int i = 3; i < 22; i++)
            {
                recvData[i] = (byte)i;
            }
            ReadData1(ref data, recvData, 23);

        }
        public bool OpenPort(bool enable)
        {
            bool res = false;


            if (enable)
            {
                if (this._com.IsOpen == false)
                {
                    this._com.Open();
                    res = true;
                }
                else
                {
                    res = false;
                }
            }
            else
            {
                if (this._com.IsOpen == true)
                {
                    this._com.Close();
                    res = true;
                }
                else
                {
                    res = false;
                }
            }

            return res;
            
        }
            
        
    }
}
