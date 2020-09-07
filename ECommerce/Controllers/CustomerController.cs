using AutobuyDirectApi.Models;
using EcomErrorLog;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AutobuyDirectApi.Controllers
{
    public class CustomerController : ApiController
    {
        EcommEntities context = new EcommEntities();

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/customer/GetCart/{user_id}")]
        public JObject GetCart(int user_id)
        {
            int user_id_ = user_id;
            var cartlist = context.Carts.AsNoTracking().Where(a => a.cust_id==user_id_ && a.cart_status==1);
            JArray array = new JArray();
            foreach (Cart car in cartlist)
            {
                JObject cart_list = new JObject(
                    new JProperty("cart_id", car.id),
                    new JProperty("product_id", car.prod_id),
                    new JProperty("item_id", car.item_id),
                    new JProperty("quantity", car.quantity),
                    new JProperty("basket_session", car.basket_session),
                    new JProperty("Created_Date", car.Created_date),
                    new JProperty("updated_Date", car.Updated_date),
                    new JProperty("Status", car.cart_status)
                    );
                array.Add(cart_list);
            }
            JObject final = new JObject(
               new JProperty("Cartlist", array));

            return final;
        }

        [System.Web.Http.HttpPost]
        public int addtocart(JObject param)
        {
            int status = 0;
            int user_id =0;
            int prod_id = 0;
            try
            {
                user_id = (int)param.GetValue("userid");
                prod_id = (int)param.GetValue("prodid");

                Cart car = new Cart();
                car.cust_id = user_id;
                car.prod_id = prod_id;
                car.basket_session = ""; // where it will be get
                car.item_id = 0; // what is this
                car.quantity = 0; // now its temp data once get in angular will update
                car.Created_date = DateTime.Now;
                car.Updated_date = DateTime.Now;
                car.cart_status = 1;
                context.Carts.Add(car);
                context.SaveChanges();
                status = 1;
               
            }
            catch (Exception e)
            {
                Logdetails.LogError("Post Error", "addtocarterror customercontroller (46)", e.Message);
            }

            return status;
        }
        public int removefromcart(JObject param)
        {
            int status = 0;
            int user_id = 0;
            int prod_id = 0;
            try
            {
                user_id = (int)param.GetValue("userid");
                prod_id = (int)param.GetValue("prodid");

                var cart_table = context.Carts.Where(a => a.cust_id == user_id && a.prod_id == prod_id);
                foreach (Cart car in cart_table)
                {
                    car.Updated_date = DateTime.Now;
                    car.cart_status = 0;
                }
                context.SaveChanges();
                status = 1;
            }
            catch (Exception e)
            {
                Logdetails.LogError("Post Error", "addtocarterror customercontroller (46)", e.Message);
            }

            return status;
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/customer/GetMyprofile/{user_id}")]
        public JObject GetMyprofile(int user_id)
        {
            int user_id_ = user_id;
            var userlist = context.user_details.AsNoTracking().Where(a => a.user_id == user_id_);
            JArray array = new JArray();
            foreach (user_details user in userlist)
            {
                JObject user_list = new JObject(
                    new JProperty("fname", user.fname),
                    new JProperty("product_id", user.lname),
                    new JProperty("item_id", user.email),
                    new JProperty("quantity", user.address),
                    new JProperty("basket_session", user.last_login)
                    );
                array.Add(user_list);
            }
            JObject final = new JObject(
               new JProperty("Wishlist", array));

            return final;
        }

    }
}
