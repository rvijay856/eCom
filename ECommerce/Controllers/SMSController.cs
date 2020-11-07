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

namespace AutobuyDirectApi.Controllers
{
    public class SMSController : ApiController
    {
        public string sendSMS()
        {
            String message = HttpUtility.UrlEncode("This is your message Test");
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues("https://api.textlocal.in/send/", new NameValueCollection()
                {
                {"apikey" , "vdEIHmmAO1I-mOEQQxuw1txTsilR8dd1KOcNDHsJ0U"},
                {"numbers" , "8248399788"},
                {"message" , message},
                {"sender" , "Zuzu Bazaar"}
                });
                string result = System.Text.Encoding.UTF8.GetString(response);
                return result;
            }
        }

		[System.Web.Http.HttpGet]
		[System.Web.Http.Route("api/SMS/GetsendSMS")]
		public string GetsendSMS()
		{
			String result;
			string apiKey = "vdEIHmmAO1I-mOEQQxuw1txTsilR8dd1KOcNDHsJ0U";
			string numbers = "918248399788"; // in a comma seperated list
			string message = "This is your message";
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
			return result;
		}
	}
}
