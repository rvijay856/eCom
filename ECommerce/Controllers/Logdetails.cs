using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using System.Globalization;
using System.Diagnostics;
using System.Text;
namespace EcomErrorLog
{
    public class Logdetails
    {


        public static void LogError(string sPageURL, string sMethodName, string sLogMessage)
        {
            try
            {
                string sPath = ConfigurationManager.AppSettings["LOGPATH"] + "LogDetails.txt";

                if (!File.Exists(sPath))
                {
                    File.Create(sPath).Close();
                }
                using (StreamWriter w = File.AppendText(sPath))
                {
                    w.WriteLine("___________________________________________________START_______________________________________________");
                    w.WriteLine("\r\nLog Entry : ");
                    w.WriteLine("{0}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    string err = "Log Details  " + sPageURL + " Method Name: " + sMethodName +
                                  ". log Message:" + sLogMessage;
                    w.WriteLine(err);
                    w.WriteLine("____________________________________________________END________________________________________________");
                    w.Flush();
                    w.Close();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }
}