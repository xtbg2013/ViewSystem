using System.Collections.Generic;
using System.Drawing;
using ProgressODoom;
using System;
using System.Xml;
namespace ViewSystem
{
    class PainterFactory
    {
        
        public  static PlainBorderPainter GetPlainBoarderPainter()
        {
            PlainBorderPainter ret = new PlainBorderPainter(Color.Transparent);
            ret.RoundedCorners = false;
            ret.Style = PlainBorderPainter.PlainBorderStyle.Flat;
            return ret;
        }
        public static  PlainProgressPainter GetProgressPainter(int maxVal,double value)
        {
            int val = CalulateColorValue(maxVal,value);
            val = 255 - val;
            PlainProgressPainter ret = new PlainProgressPainter();
            ret.Color = Color.FromArgb(val, val, val);
            ret.GlossPainter = null;
            ret.LeadingEdge = System.Drawing.Color.Transparent;
            ret.ProgressBorderPainter = null;
            return ret;
        }
        private static int CalulateColorValue(int maxVax,double value)
        {
            double colorValue = value * (Convert.ToDouble(255) / Convert.ToDouble(maxVax));
            int val = Convert.ToInt32(colorValue);
            return val;
        }

        
    }
}
