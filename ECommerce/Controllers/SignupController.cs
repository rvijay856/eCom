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
using Newtonsoft.Json;

namespace AutobuyDirectApi.Controllers
{
    public class SignupController : ApiController
    {
        EcommEntities context = new EcommEntities();

        [System.Web.Http.HttpPost]
        public JObject signupsite(JObject param)
        {

            //int status = 0;
            string name = "";
            string email = "";
            string pword = "";
            string mobile = "";
            int mobile_count = 0;
            string Custmobile_OTP = "";
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
                OTP = Ran.Next(1, 1000000).ToString("D6");

                mobile_count = context.Customers.AsNoTracking().Where(a => a.cust_mobile == mobile).Count();
                if(mobile_count!=0)
                {
                    Custmobile_OTP = context.Customers.AsNoTracking().Where(a => a.cust_mobile == mobile).Select(a => a.cust_otp).Single();
                }
                if (mobile_count != 0 && string.Equals(Custmobile_OTP, "0", StringComparison.OrdinalIgnoreCase))
                {
                    JObject sg = new JObject(
                            new JProperty("cust_id", ""),
                            new JProperty("cust_name", ""),
                            new JProperty("cust_mobile", ""),
                            new JProperty("cust_email", ""),
                            new JProperty("cust_status", ""),
                            new JProperty("Created_date", ""),
                            new JProperty("Updated_date", ""),
                            new JProperty("status", "You are already registered. Please log in."),
                            new JProperty("status_id", 1)
                        );
                    Cust.Add(sg);
                }
                else if (mobile_count != 0 && !string.Equals(Custmobile_OTP, "0", StringComparison.OrdinalIgnoreCase))
                {
                    SMSSend smss = new SMSSend();
                    result = smss.sendSMS(mobile, OTP);
                    var custos = context.Customers.Where(a => a.cust_mobile == mobile);

                    foreach (Customer cu in custos)
                    {
                        //cu.cust_status = 1;
                        cu.cust_otp = OTP;
                        cu.Updated_date = DateTime.Now;
                    }
                    context.SaveChanges();

                    var custo = context.Customers.AsNoTracking().Where(a => a.cust_mobile == mobile);

                    foreach (Customer cu in custo)
                    {
                        JObject sg = new JObject(
                            new JProperty("cust_id", cu.cust_id),
                            new JProperty("cust_name", cu.cust_name),
                            new JProperty("cust_mobile", cu.cust_mobile),
                            new JProperty("cust_email", cu.cust_email),
                            new JProperty("cust_status", cu.cust_status),
                            new JProperty("cust_type", cu.cust_type),
                            new JProperty("Created_date", cu.Created_date),
                            new JProperty("Updated_date", cu.Updated_date),
                            new JProperty("status", "You are already registered. Please verify mobile number."),
                            new JProperty("status_id", 2)
                        );
                        Cust.Add(sg);
                    }
                }
                else if (mobile_count==0)
                {
                    Customer cus = new Customer();
                    cus.cust_name = name;
                    cus.cust_email = email;
                    cus.cust_mobile = mobile;
                    cus.cust_status = 1;
                    cus.cust_otp = OTP;
                    cus.cat_password = pword;
                    cus.cust_type = 1;
                    cus.Created_date = DateTime.Now;
                    cus.Updated_date = DateTime.Now;

                    context.Customers.Add(cus);
                    context.SaveChanges();

                    SMSSend smss = new SMSSend();
                    result = smss.sendSMS(mobile,OTP);

                    var custo = context.Customers.AsNoTracking().Where(a => a.cust_mobile == mobile);

                    foreach (Customer cu in custo)
                    {

                        JObject sg = new JObject(
                            new JProperty("cust_id", cu.cust_id),
                            new JProperty("cust_name", cu.cust_name),
                            new JProperty("cust_mobile", cu.cust_mobile),
                            new JProperty("cust_email", cu.cust_email),
                            new JProperty("cust_status", cu.cust_status),
                            new JProperty("cust_type", cu.cust_type),
                            new JProperty("Created_date", cu.Created_date),
                            new JProperty("Updated_date", cu.Updated_date),
                            new JProperty("status", "Sign in Successfully"),
                            new JProperty("status_id", 3)
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

        [System.Web.Http.Authorize]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Signup/GetUserClaims")]
        public JObject GetUserClaims()
        {
            int cust_id = 0;
            int Login_status = 0;
            InitController Login = new InitController();
            JObject param = Login.Login();
            string json = JsonConvert.SerializeObject(param);
            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];
            String result = "";

            string otp = "";
            decimal user_id = 0;
            var name = "";
            var email = "";
            var address = "";
            var area = "";
            var landmark = "";
            var city = "";
            var state = "";
            var country = "";
            var pincode = "";
            var mobile = "";
            var last_Login = "";
            int user_type = 0;
            int cart_count = 0;
            int wishlist_count = 0;
            JObject ud = new JObject();

            otp = context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id).Select(a => a.cust_otp).Single();

            if (string.Equals(otp, "0",StringComparison.OrdinalIgnoreCase))
            {
                var user_deta = context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id);
                foreach (Customer urd in user_deta)
                {
                    user_id = urd.cust_id;
                    name = urd.cust_name;
                    email = urd.cust_email;
                    user_type = (int)urd.cust_type;
                    if (urd.cust_address_id != null && urd.cust_address_id != 0)
                    {
                        address = context.customer_address.AsNoTracking().Where(a => a.id == urd.cust_address_id).Select(a => a.customer_address1).Single();
                        area = context.customer_address.AsNoTracking().Where(a => a.id == urd.cust_address_id).Select(a => a.customer_area).Single();
                        landmark = context.customer_address.AsNoTracking().Where(a => a.id == urd.cust_address_id).Select(a => a.Landmark).Single();
                        city = context.customer_address.AsNoTracking().Where(a => a.id == urd.cust_address_id).Select(a => a.customer_city).Single();
                        state = context.customer_address.AsNoTracking().Where(a => a.id == urd.cust_address_id).Select(a => a.customer_state).Single();
                        country = context.customer_address.AsNoTracking().Where(a => a.id == urd.cust_address_id).Select(a => a.customer_country).Single();
                        pincode = context.customer_address.AsNoTracking().Where(a => a.id == urd.cust_address_id).Select(a => a.customer_pincode).Single().ToString();
                        mobile = context.customer_address.AsNoTracking().Where(a => a.id == urd.cust_address_id).Select(a => a.customer_mobile).Single().ToString();
                    }
                    last_Login = urd.Last_login.ToString();
                }
                cart_count = context.Carts.AsNoTracking().Where(a => a.cust_id == cust_id && a.cart_status == 1).Count();
                wishlist_count = context.Wishlists.AsNoTracking().Where(a => a.cust_id == cust_id && a.wish_status == 1).Count();

                    ud = new JObject(
                     new JProperty("user_id", user_id),
                     new JProperty("name", name),
                     new JProperty("email", email),
                     new JProperty("user_type", user_type),
                     new JProperty("address", address),
                     new JProperty("area", area),
                     new JProperty("city", city),
                     new JProperty("landmark", landmark),
                     new JProperty("state", state),
                     new JProperty("country", country),
                     new JProperty("pincode", pincode),
                     new JProperty("mobile", mobile),
                     new JProperty("last_Login", last_Login),
                     new JProperty("cart_count", cart_count),
                     new JProperty("wishlist_count", wishlist_count),
                     new JProperty("Status", "Login Successfully."),
                     new JProperty("Status_id", 1)
                    );
                DateTime lastlogin = DateTime.Now;
                var users = context.Customers.Where(x => x.cust_id == user_id);

                foreach (Customer user in users)
                {
                    user.Last_login = lastlogin;
                }
                context.SaveChanges();
            }
            else
            {
                mobile = context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id).Select(a => a.cust_mobile).Single();
                Random Ran = new Random();
                string OTP = "";
                OTP = Ran.Next(1, 1000000).ToString("D6");

                SMSSend smss = new SMSSend();
                result = smss.sendSMS(mobile, OTP);
                var custos = context.Customers.Where(a => a.cust_mobile == mobile);

                foreach (Customer cu in custos)
                {
                    //cu.cust_status = 1;
                    cu.cust_otp = OTP;
                    cu.Updated_date = DateTime.Now;
                }
                context.SaveChanges();

                ud = new JObject(
                     new JProperty("Status", "Please verify mobile number."),
                     new JProperty("Status_id", 2)
                     );
            }
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

        [System.Web.Http.Authorize]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Sigup/GetOtp/{Otp}")]
        public JObject GetOtp(string Otp)
        {
            int cust_id = 0;
            int Login_status = 0;
            InitController Login = new InitController();
            JObject param = Login.Login();
            string json = JsonConvert.SerializeObject(param);
            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];

            JObject final = new JObject();
            var Otp_Confirm = context.Customers.Where(a => a.cust_id == cust_id);
            string check_otp = context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id).Select(a => a.cust_otp).Single();
            if (string.Equals(check_otp,Otp,StringComparison.OrdinalIgnoreCase))
            {
                foreach(Customer cu in Otp_Confirm)
                {
                    cu.cust_otp = "0";
                }
                context.SaveChanges();
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
