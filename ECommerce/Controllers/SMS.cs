using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using System.Globalization;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.Collections.Specialized;

namespace EcomSMS
{
    public class SMSSend
    {
        public string sendSMS(string phone,string otp)
        {
            String message = HttpUtility.UrlEncode("This is your otp message "+otp);
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues("https://api.textlocal.in/send/", new NameValueCollection()
                {
                {"apikey" ,  ConfigurationManager.AppSettings["apiKey"].ToString()},
                {"numbers" , phone},
                {"message" , message},
                {"sender" , "TXTLCL"}
                });
                string result = System.Text.Encoding.UTF8.GetString(response);
                return result;
            }
        }
    }
}