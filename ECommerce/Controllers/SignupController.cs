using AutobuyDirectApi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using TokenAuthentication;

namespace AutobuyDirectApi.Controllers
{
    public class SignupController : ApiController
    {
        EcommEntities context = new EcommEntities();

        [System.Web.Http.HttpPost]
        public int signupsite(JObject param)
        {

            int status = 0;
            string fname = "";
            string lname = "";
            string email = "";
            string pword = "";
            try
            {
                fname = (string)param.GetValue("firstname");
                lname = (string)param.GetValue("lastname");
                email = (string)param.GetValue("email");
                pword = (string)param.GetValue("password");

                user_details us = new user_details();
                us.fname = fname;
                us.lname = lname;
                us.email = email;
                us.password = pword;
                us.user_type = 1;
                us.user_status = 1;
                us.created_date = DateTime.Now;
                context.user_details.Add(us);
                context.SaveChanges();
                status = 1;
            }
            catch(Exception e)
            {
                Logdetails.LogError("Post Error", "signuperror signupcontroller (65)", e.Message);
            }

            return status;
        }

        [System.Web.Http.Authorize]
        public JObject GetUserClaims()
        {

            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;

            var claims1 = claimsIdentity.Claims.Select(x => x.Value);

            JObject d = new JObject(
                new JProperty("hai", claims1
                ));

            JArray s = (JArray)d.GetValue("hai");
            string ab = (string)s[0];
            

            var user_deta = context.user_details.AsNoTracking().Where(a => a.email == ab);

            decimal user_id = 0;
            var fname = "";
            var lname = "";
            var email = "";
            var address = "";
            int user_type=0;

            foreach (user_details urd in user_deta)
            {
                user_id = urd.user_id;
                fname = urd.fname;
                lname = urd.lname;
                email = urd.email;
                address = urd.address;
                user_type = urd.user_type;
            }
            JObject ud = new JObject(
                new JProperty("user_id", user_id),
                 new JProperty("fname", fname),          
                 new JProperty("lname", lname),
                 new JProperty("email", email),
                 new JProperty("address", address),
                 new JProperty("user_type", user_type)
                ); ;
            DateTime lastlogin = DateTime.Now;

           
            var users = context.user_details.Where(x => x.user_id== user_id);

            foreach (user_details user in users)
            {
                user.last_login = lastlogin;

            }
            context.SaveChanges();

            return ud;
        }
    }
}
