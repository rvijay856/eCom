using AutobuyDirectApi.Models;
using EcomErrorLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Web.Http;
using System.Web.Security;
using System.Web.UI;

namespace AutobuyDirectApi.Controllers
{
    public class OrderController : ApiController
    {
        EcommEntities context = new EcommEntities();

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Order/GetOrder")]
        public JObject GetOrder()
        {
            int cust_id = 0;
            int Login_status = 0;
            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];

           
            JObject final = new JObject(
               new JProperty("Order", ""));

            return final;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Order/GetOrderList")]
        public JObject GetOrderList()
        {
            int cust_id = 0;
            int Login_status = 0;
            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];

           
            JObject final = new JObject(
               new JProperty("OrderList", ""));

            return final;
        }
    }
}
