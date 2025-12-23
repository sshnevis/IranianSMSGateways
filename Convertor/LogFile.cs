using System;
using System.Collections.Generic;
using System.Text;

namespace IranianSMSGateways.Convertor
{
    static internal class Logging
    {
        public static void LogFile(string fileName, Exception ex)
        {
            System.IO.StreamWriter w = new System.IO.StreamWriter(fileName, true);
            w.WriteLine(ex.Data);
            w.WriteLine(ex.InnerException);
            w.WriteLine(ex.Message);
            w.WriteLine(ex.Source);
            w.WriteLine(ex.StackTrace);
            w.WriteLine(ex.TargetSite);
            w.WriteLine("---------------------- " + DateTime.Now.ToString() + " ----------------------");
            w.Close();
        }
        public static void LogFile(string fileName, string ex)
        {
            System.IO.StreamWriter w = new System.IO.StreamWriter(fileName, true);
            w.WriteLine(ex);
            w.WriteLine("---------------------- " + DateTime.Now.ToString() + " ----------------------");
            w.Close();
        }
    }
}
