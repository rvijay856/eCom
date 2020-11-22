using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using EcomErrorLog;
using System.Collections.Specialized;
using System.Web;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Xml;
using System.Configuration;

namespace AutobuyDirectApi.Controllers
{
    public class SMSController : ApiController
    {

		[System.Web.Http.HttpGet]
		[System.Web.Http.Route("api/SMS/GetsendSMS/{PhoneNo}")]
		public string GetsendSMS(string PhoneNo)
		{
			string Phone_No = PhoneNo;
			String result;
			Random Ran = new Random();
			string apiKey = ConfigurationManager.AppSettings["apiKey"].ToString();
			string numbers = Phone_No; // in a comma seperated list
			string OTP = "";
			OTP = Ran.Next(0, 1000000).ToString("D6");
			string message = "This is your OTP"+OTP;
			string sender = "TXTLCL";


            String url = "https://api.textlocal.in/send/?apikey=" + apiKey + "&numbers=" + numbers + "&message=" + message + "&sender=" + sender;
            //refer to parameters to complete correct url string

            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);

            objRequest.Method = "POST";
            objRequest.ContentLength = Encoding.UTF8.GetByteCount(url);
            objRequest.ContentType = "application/x-www-form-urlencoded";
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(url);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                myWriter.Close();
            }

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                // Close and clean up the StreamReader
                sr.Close();
            }
            return result+OTP;

		}
	}
}
