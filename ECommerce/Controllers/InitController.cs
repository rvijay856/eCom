using System;
using System.Collections.Generic;
using AutobuyDirectApi.Models;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace AutobuyDirectApi.Controllers
{
    public class InitController : ApiController
    {
        EcommEntities1 context = new EcommEntities1();
        public JObject Login()
        {

            decimal user_id = 0;
            int user_status = 0;

            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;

            //var claims = claimsIdentity.Claims.Select(x => new { type = x.Type, value = x.Value });

            var claims1 = claimsIdentity.Claims.Select(x => x.Value);

            JObject d = new JObject(

                new JProperty("hai", claims1
                ));

            JArray s = (JArray)d.GetValue("hai");
            string ab = (string)s[0];

            var login = context.user_details.Where(a => a.email.Trim().ToLower() == ab.Trim().ToLower());

            foreach (user_details uf in login)
            {
                user_id = uf.user_id;
                user_status = (uf.user_status == null) ? 0 : (int)uf.user_status;

            }
            JObject bd = new JObject(

                    new JProperty("User_id", user_id),
                    new JProperty("Login_status", user_status));


            return bd;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Blog/GetBlogs")]
        public JObject GetBlogs()
        {
            var blog = context.user_details.AsNoTracking().OrderByDescending(a => a.user_id);
            JArray array = new JArray();
            
                JObject bo = new JObject(
                    new JProperty("Blog_Id", "ID"),
                    new JProperty("Blog_Title", "blg.Title"),
                    new JProperty("CreateBy", "blg.CreateBy"),
                    new JProperty("Created_Date", "blg.Created_Date")
                    );
                array.Add(bo);
            JObject final = new JObject(
              new JProperty("Blog_Details", array));

            return final;
        }
    }
}
