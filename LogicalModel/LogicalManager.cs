﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Log;
using System.Reflection;
using LogicalModel.Data;
using System.Threading;
using Spire;
namespace LogicalModel
{
    public class LogicalManager:ILogicalManager
    {
        private ILog _logger;
        private IData _iDataCom;
        private SaveData _save;
        private ConfigParam _param;
        private System.Timers.Timer _timer;
        private AutoResetEvent _autoEvent;
        private Dictionary<int, int> _calibrationValue;        //校准值
        private Dictionary<int, int> _calibrationCount;        //校准数据计数
        private Dictionary<int, double> _capacitance;
        private bool _calibrationSuccess;
        private int _readTimes;

        private void LoadCommLib()
        {
            string asmPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DataService.dll");
            if (!System.IO.File.Exists(asmPath))
            {
                this._logger.Error(" Can't find DataService.dll in the  path:" + AppDomain.CurrentDomain.BaseDirectory);
                return;
            }
            Assembly ass = System.Reflection.Assembly.LoadFile(asmPath);
            Type type = ass.GetType("DataService.ComData");
            this._iDataCom = (IData)Activator.CreateInstance(type, new object[] {this._param });
        }
        public LogicalManager(ILog logger, ConfigParam param)
        {
            this._logger = logger;
            this._param = param;
            this._readTimes = 5;
            this._calibrationSuccess = false;
            InitTimer();
        }
        private void SetCalibrationStatus(bool IsSuccess)
        {
            this._calibrationSuccess = IsSuccess;
        }
        private bool GetCalibrationStatus()
        {
            return this._calibrationSuccess;
        }
        private void InitTimer()
        {
            this._timer = new System.Timers.Timer();
            this._timer.Interval = 1;
            this._timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerRead);
        }

        private void TimerRead(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dictionary<int, int> data = new Dictionary<int, int>();
            this._iDataCom.ReadData(ref  data);
           
            foreach (var item in data)
            {
                if (this._calibrationCount[item.Key] < this._readTimes) 
                {
                    this._calibrationValue[item.Key] += item.Value;
                    this._calibrationCount[item.Key] += 1;
                }
            }

            bool stop = true;
            int count = GetCount();
            for (int i = 0; i < count; i++)
            {
                if (this._calibrationCount[i] < this._readTimes)
                {
                    stop = false;
                    break;
                }
            }
            if (stop)
            {
                for (int i = 0; i < this._calibrationValue.Count; i++)
                {
                    this._calibrationValue[i] /= this._readTimes;
                }
                this._timer.Stop();
                this._autoEvent.Set();
            }
            
        }
        private int GetCount()
        {
            return this._param.Row * this._param.Col;
        }

        private double CaculateCapacitance(int val)
        {
            double value = (4 * (double)val) / (double)65536;
            return value;
        }
        public double CalculateKpa(double CapacitanceValue)
        {
            CapacitanceValue *= 1000;
            double y = 2.65 * Math.Pow(Math.E, CapacitanceValue / 56.09) - 2.86;
            return y;
        }
        
        public void InitManager()
        {
            LoadCommLib();
            this._calibrationCount = new Dictionary<int, int>();
            this._calibrationValue = new Dictionary<int, int>();
            this._capacitance = new Dictionary<int, double>();
            this._autoEvent = new AutoResetEvent(false);
            this._save = new SaveData();
            int count = GetCount();
            InitCalibration();
        }

        private void InitCalibration()
        {
            int count = GetCount();
            for (int i = 0; i < count; i++)
            {
                this._calibrationCount[i] = 0;
                this._calibrationValue[i] = 0;
            }
        }
        public bool ReadData(ref Dictionary<int, double> data, out string msg)
        {
            msg = "";
            if (GetCalibrationStatus())
            {
                Dictionary<int, int> tempData = new Dictionary<int, int>();
                this._iDataCom.ReadData(ref tempData);
                foreach (var item in tempData)
                {
                    int val = tempData[item.Key] - this._calibrationValue[item.Key];
                    data[item.Key] = CaculateCapacitance(val);
                    //data[item.Key] = CalculateKpa(CaculateCapacitance(val)*1000);
                    this._capacitance[item.Key] = data[item.Key];
                }
                return true;
            }
            else
            {
                msg = "not calucation!";
                return false;
            }


            //Random ran = new Random();
            //Dictionary<int, double> x = new Dictionary<int, double>();
            //for (int i = 0; i < 100; i++)
            //{
            //    int RandKey = ran.Next(0, 255);
            //    data[i] = Convert.ToDouble(RandKey);
            //}
            //return true;
        }
        public bool Connect(bool isConnected)
        {
            if (this._iDataCom != null)
                return this._iDataCom.OpenPort(isConnected);
            else
                return false;
        }
        public bool Calibration(out string msg)
        {
            msg = "";
            bool res;
            InitCalibration();
            this._timer.Start();
            
            if (!this._autoEvent.WaitOne(10000))
            {
                res = false;
                int count = GetCount();
                for (int i = 0; i < count; i++)
                {
                    if (this._calibrationCount[i] < this._readTimes)
                    {
                        this._logger.Error("calibration error,number" + i+"----" + this._calibrationCount[i]);
                    }
                     
                }
                this._timer.Stop();
            }
            else
            {
                res = true;
                SetCalibrationStatus(res);
            }
            
            
            return res;
        }
        public bool Save(string path)
        {
            return this._save.SaveXls(path, this._calibrationCount);
        }
    }
}
