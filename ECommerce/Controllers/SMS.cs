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
			string result;
			string apiKey = "vdEIHmmAO1I-5QhLD6sQ9qU26aNqCexE50qjF59zLy";
			string numbers = "91"+phone; // in a comma seperated list
			string message = "We are from ZUZU. The OTP is "+ otp + " to login for the website or app.";
			string sender = "RHIGLO";

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
			return result;
		}
    }
}