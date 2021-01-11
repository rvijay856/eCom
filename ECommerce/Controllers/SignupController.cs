using AutobuyDirectApi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using EcomErrorLog;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using EcomSMS;

namespace AutobuyDirectApi.Controllers
{
    public class SignupController : ApiController
    {
        EcommEntities1 context = new EcommEntities1();

        [System.Web.Http.HttpPost]
        public JObject signupsite(JObject param)
        {

            int status = 0;
            string name = "";
            string email = "";
            string pword = "";
            string mobile = "";
            int email_Count = 0;
            int mobile_count = 0;
            String result = "";
            Random Ran = new Random();
            JArray Cust = new JArray();
            try
            {
                name = (string)param.GetValue("name");
                email = (string)param.GetValue("email");
                pword = (string)param.GetValue("password");
                mobile = (string)param.GetValue("mobile");

                name = name.Trim();
                email = email.Trim();
                pword = pword.Trim();
                mobile = mobile.Trim();

                string OTP = "";
                OTP = Ran.Next(0, 1000000).ToString("D6");

                email_Count = context.Customers.AsNoTracking().Where(a => a.cust_email == email).Count();
                mobile_count = context.Customers.AsNoTracking().Where(a => a.cust_mobile == mobile).Count();
                if (email_Count!=0)
                {
                    JObject sg = new JObject(
                             new JProperty("cust_id", ""),
                             new JProperty("cust_name", ""),
                             new JProperty("cust_mobile", ""),
                             new JProperty("cust_email", ""),
                             new JProperty("cust_status", ""),
                             new JProperty("Created_date", ""),
                             new JProperty("Updated_date", ""),
                             new JProperty("status", "2")
                         );
                    Cust.Add(sg);
                }
                if(mobile_count!=0)
                {
                    JObject sg = new JObject(
                            new JProperty("cust_id", ""),
                            new JProperty("cust_name", ""),
                            new JProperty("cust_mobile", ""),
                            new JProperty("cust_email", ""),
                            new JProperty("cust_status", ""),
                            new JProperty("Created_date", ""),
                            new JProperty("Updated_date", ""),
                            new JProperty("status", "3")
                        );
                    Cust.Add(sg);
                }
                if (email_Count==0 && mobile_count==0)
                {
                    Customer cus = new Customer();
                    cus.cust_name = name;
                    cus.cust_email = email;
                    cus.cust_mobile = mobile;
                    cus.cust_status = 0;
                    cus.cust_otp = int.Parse(OTP);
                    cus.cat_password = pword;
                    cus.Created_date = DateTime.Now;
                    cus.Updated_date = DateTime.Now;

                    context.Customers.Add(cus);
                    context.SaveChanges();

                    SMSSend smss = new SMSSend();
                    result = smss.sendSMS(mobile,OTP);

                    var custo = context.Customers.AsNoTracking().Where(a=>a.cust_mobile==mobile && a.cust_email==email);

                    foreach (Customer cu in custo)
                    {

                        JObject sg = new JObject(
                            new JProperty("cust_id", cu.cust_id),
                            new JProperty("cust_name", cu.cust_name),
                            new JProperty("cust_mobile", cu.cust_mobile),
                            new JProperty("cust_email", cu.cust_email),
                            new JProperty("cust_status", cu.cust_status),
                            new JProperty("Created_date", cu.Created_date),
                            new JProperty("Updated_date", cu.Updated_date),
                            new JProperty("status", "1")
                        );
                        Cust.Add(sg);
                    }
                }
                
            }
            catch(Exception e)
            {
                Logdetails.LogError("Post Error", "signuperror signupcontroller (22)", e.Message);
            }

            JObject final = new JObject(
               new JProperty("Customer_Details", Cust));

            return final;
        }

        //[System.Web.Http.Authorize]
        //[System.Web.Http.HttpGet]
        //[System.Web.Http.Route("api/Sigup/GetUserClaims")]
        public JObject GetUserClaims()
        {

            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;

            var claims1 = claimsIdentity.Claims.Select(x => x.Value);

            JObject d = new JObject(
                new JProperty("hai", claims1
                ));

            JArray s = (JArray)d.GetValue("hai");
            string ab = (string)s[0];
            

            var user_deta = context.Customers.AsNoTracking().Where(a => a.cust_email == ab);

            decimal user_id = 0;
            var name = "";
            var email = "";
            var address = "";
            var city = "";
            var state = "";
            var country = "";
            var pincode = "";
            var mobile = "";
            var last_Login = "";
            int user_type = 0;


            foreach (Customer urd in user_deta)
            {
                user_id = urd.cust_id;
                name = urd.cust_name;
                email = urd.cust_email;
                user_type = (int)urd.cust_type;
                if (urd.cust_address_id != null && urd.cust_address_id != 0)
                {
                    address = context.customer_address.AsNoTracking().Where(a => a.id == urd.cust_address_id).Select(a => a.customer_address1).Single();
                    city = context.customer_address.AsNoTracking().Where(a => a.id == urd.cust_address_id).Select(a => a.customer_city).Single();
                    state = context.customer_address.AsNoTracking().Where(a => a.id == urd.cust_address_id).Select(a => a.customer_area).Single();
                    country = context.customer_address.AsNoTracking().Where(a => a.id == urd.cust_address_id).Select(a => a.customer_country).Single();
                    pincode = context.customer_address.AsNoTracking().Where(a => a.id == urd.cust_address_id).Select(a => a.customer_pincode).Single().ToString();
                    mobile = context.customer_address.AsNoTracking().Where(a => a.id == urd.cust_address_id).Select(a => a.customer_mobile).Single().ToString();
                }
                last_Login = urd.Last_login.ToString();
            }
            JObject ud = new JObject(
                new JProperty("user_id", user_id),
                 new JProperty("name", name),          
                 new JProperty("email", email),
                 new JProperty("user_type", user_type),
                 new JProperty("address", address),
                 new JProperty("city", city),
                 new JProperty("state", state),
                 new JProperty("country", country),
                 new JProperty("pincode", pincode),
                 new JProperty("mobile", mobile),
                 new JProperty("last_Login", last_Login)
                ); ;
            DateTime lastlogin = DateTime.Now;

           
            var users = context.Customers.Where(x => x.cust_id== user_id);

            foreach (Customer user in users)
            {
                user.Last_login = lastlogin;

            }
            context.SaveChanges();

            return ud;
        }

        //////Encrypt Card details
        public string Encryptdata(string password)
        {
            if (password != "")
            {
                string EncrptKey = "Administrator";
                byte[] byKey = { };
                byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
                byKey = System.Text.Encoding.UTF8.GetBytes(EncrptKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(password);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                password = Convert.ToBase64String(ms.ToArray());
            }
            return password;
        }

        //////Decrypt Card details
        public string Decryptdata(string encryptpwd)
        {
            if (encryptpwd != "")
            {
                encryptpwd = encryptpwd.Replace(" ", "+");
                string DecryptKey = "Administrator";
                byte[] byKey = { };
                byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
                byte[] inputByteArray = new byte[encryptpwd.Length];

                byKey = System.Text.Encoding.UTF8.GetBytes(DecryptKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(encryptpwd.Replace(" ", "+"));
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                encryptpwd = encoding.GetString(ms.ToArray());
            }
            return encryptpwd;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Sigup/GetOtp/{Cust_ID}/{Otp}")]
        public JObject GetOtp(int Cust_ID,int Otp)
        {
            JObject final = new JObject();
            int check_otp = (int)context.Customers.AsNoTracking().Where(a => a.cust_id == Cust_ID).Select(a => a.cust_otp).Single();
            if (check_otp==Otp)
            {
                final = new JObject(
               new JProperty("Otp_Vlidation", "Success"));
            }
            else
            {
                final = new JObject(
                        new JProperty("Otp_Vlidation", "Failed"));
            }
              
            return final;
        }
    }
}
