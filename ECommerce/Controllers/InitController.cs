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
        EcommEntities context = new EcommEntities();

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
