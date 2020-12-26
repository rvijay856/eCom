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
    public class CustomerController : ApiController
    {
        EcommEntities1 context = new EcommEntities1();

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/customer/GetCart")]
        public JObject GetCart()
        {
            int cust_id = 0;
            int Login_status = 0;
            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["Login"]["User_id"];
            Login_status = (int)JObject.Parse(json)["Login"]["Login_status"];

            var cartlist = context.Carts.AsNoTracking().Where(a => a.cust_id== cust_id && a.cart_status==1);
            JArray array = new JArray();
            foreach (Cart car in cartlist)
            {
                JObject cart_list = new JObject(
                    new JProperty("cart_id", car.id),
                    new JProperty("product_id", car.prod_id),
                    new JProperty("item_id", car.item_id),
                    new JProperty("cust_id", car.cust_id),
                    new JProperty("product_name", context.Products.AsNoTracking().Where(a=>a.prod_id==car.prod_id).Select(a=>a.prod_name).Single()),
                    new JProperty("quantity", car.quantity),
                    new JProperty("basket_session", car.basket_session),
                    new JProperty("item_selling", car.item_selling),
                    new JProperty("item_MRP", context.Product_items.AsNoTracking().Where(a => a.id == car.item_id).Select(a => a.item_mrp).Single()),
                    new JProperty("item_spec", car.item_spec),
                    new JProperty("item_unit", car.item_unit),
                    new JProperty("item_img", car.item_img),
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
        public int addtocart(JObject parame)
        {
            int status = 0;
            //int user_id =0;
            int item_id = 0;
            int prod_id = 0;
            int quantity = 0;
            string item_spec = "";
            string item_unit = "";
            decimal item_selling = 0;
            string item_Img = "";
            int cartcount = 0;
            int cust_id = 0;
            int Login_status = 0;

            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["Login"]["User_id"];
            Login_status = (int)JObject.Parse(json)["Login"]["Login_status"];
            try
            {

                //user_id = cust_id;//(int)parame.GetValue("userid");
                item_id = (int)parame.GetValue("itemid");
                quantity = (int)parame.GetValue("quantity");

                var cart_item = context.Product_items.AsNoTracking().Where(a => a.id == item_id);
                cartcount = context.Carts.AsNoTracking().Where(a => a.cust_id == cust_id && a.item_id == item_id).Count();
                if (cartcount == 0)
                {
                    foreach (Product_items cr in cart_item)
                    {
                        prod_id = (int)cr.prod_id;
                        item_spec = cr.item_spec;
                        item_unit = cr.item_unit;
                        item_selling = (decimal)cr.item_selling;
                        item_Img = cr.item_image;
                    }
                    Cart car = new Cart();
                    car.cust_id = cust_id;
                    car.prod_id = prod_id;
                    car.basket_session = ""; // where it will be get
                    car.item_id = item_id;
                    car.quantity = quantity;
                    car.item_spec = item_spec;
                    car.item_unit = item_unit;
                    car.item_selling = item_selling;
                    car.item_img = item_Img;
                    car.Created_date = DateTime.Now;
                    car.Updated_date = DateTime.Now;
                    car.cart_status = 1;
                    context.Carts.Add(car);
                    context.SaveChanges();
                    status = 1;
                }
                else if(cartcount!=0)
                {
                    var cart_table = context.Carts.Where(a => a.cust_id == cust_id && a.item_id == item_id);
                    foreach (Cart car in cart_table)
                    {
                        car.Updated_date = DateTime.Now;
                        car.quantity = car.quantity + quantity;
                    }
                    context.SaveChanges();
                    status = 1;
                }
               
            }
            catch (Exception e)
            {
                Logdetails.LogError("Post Error", "addtocarterror customercontroller (46)", e.Message);
            }

            return status;
        }
        public int removefromcart(JObject parame)
        {
            int status = 0;
            //int user_id = 0;
            int item_id = 0;

            int cust_id = 0;
            int Login_status = 0;

            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["Login"]["User_id"];
            Login_status = (int)JObject.Parse(json)["Login"]["Login_status"];

            try
            {
                //user_id = cust_id;// (int)parame.GetValue("userid");
                item_id = (int)parame.GetValue("itemid");

                var cart_table = context.Carts.Where(a => a.cust_id == cust_id && a.item_id == item_id);
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
                Logdetails.LogError("Post Error", "removefromcarterror customercontroller (81)", e.Message);
            }

            return status;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/customer/GetMyprofile")]
        public JObject GetMyprofile()
        {
            int cust_id = 0;
            int Login_status = 0;

            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["Login"]["User_id"];
            Login_status = (int)JObject.Parse(json)["Login"]["Login_status"];
            
            var userlist = context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id && a.cust_status==1);
            JArray array = new JArray();
            foreach (Customer user in userlist)
            {
                JObject user_list = new JObject(
                    new JProperty("ID", user.cust_id),
                    new JProperty("name", user.cust_name),
                    new JProperty("Phone", user.cust_mobile),
                    new JProperty("email", user.cust_email),
                    new JProperty("Pincode", user.cust_pincode)
                    //new JProperty("last_login", user.last_login)
                    );
                array.Add(user_list);
            }
            JObject final = new JObject(
               new JProperty("Profile", array));

            return final;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/customer/GetWishlist")]
        public JObject GetWishlist()
        {
            int cust_id = 0;
            int Login_status = 0;

            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["Login"]["User_id"];
            Login_status = (int)JObject.Parse(json)["Login"]["Login_status"];

            var custwish = context.Wishlists.AsNoTracking().Where(a => a.cust_id == cust_id && a.wish_status == 1);
            JArray array = new JArray();
            foreach(Wishlist cus in custwish)
            {
                JObject wish_list = new JObject(
                new JProperty("Cust_id", cus.cust_id),
                new JProperty("item_id", cus.item_id),
                new JProperty("prod_id", cus.prod_id),
                new JProperty("product_name", context.Products.AsNoTracking().Where(a => a.prod_id == cus.prod_id).Select(a => a.prod_name).Single()),
                new JProperty("item_unit", cus.item_unit),
                new JProperty("item_spec", cus.item_spec),
                new JProperty("item_selling", cus.item_selling),
                new JProperty("item_MRP", context.Product_items.AsNoTracking().Where(a => a.id == cus.item_id).Select(a => a.item_mrp).Single()),
                new JProperty("item_img", cus.item_img),
                new JProperty("wish_status", cus.wish_status),
                new JProperty("Created_date", cus.Created_date),
                new JProperty("Updated_date", cus.Updated_date)
                );
                array.Add(wish_list);
            }
            JObject final = new JObject(
                new JProperty("Wish", array));
            return final;
        }

        [System.Web.Http.HttpPost]
        public int addtowish(JObject parame)
        {
            int status = 0;
            //int user_id = 0;
            int item_id = 0;
            int prod_id = 0;
            string item_spec = "";
            string item_unit = "";
            decimal item_selling = 0;
            string item_Img = "";
            int wishcount = 0;
            int cust_id = 0;
            int Login_status = 0;

            
            try
            {
                InitController Login = new InitController();

                JObject param = Login.Login();

                string json = JsonConvert.SerializeObject(param);

                cust_id = (int)JObject.Parse(json)["Login"]["User_id"];
                Login_status = (int)JObject.Parse(json)["Login"]["Login_status"];

                //user_id = cust_id;// (int)parame.GetValue("userid");
                item_id = (int)parame.GetValue("itemid");

                var wish_item = context.Product_items.AsNoTracking().Where(a => a.id == item_id);

                wishcount = context.Wishlists.AsNoTracking().Where(a => a.item_id == item_id && a.cust_id == cust_id).Count();
                if (wishcount == 0)
                {
                    foreach (Product_items cr in wish_item)
                    {
                        prod_id = (int)cr.prod_id;
                        item_spec = cr.item_spec;
                        item_unit = cr.item_unit;
                        item_selling = (decimal)cr.item_selling;
                        item_Img = cr.item_image;
                    }
                    Wishlist wish = new Wishlist();
                    wish.cust_id = cust_id;
                    wish.prod_id = prod_id;
                    wish.item_id = item_id;
                    wish.item_spec = item_spec;
                    wish.item_unit = item_unit;
                    wish.item_selling = item_selling;
                    wish.item_img = item_Img;
                    wish.Created_date = DateTime.Now;
                    wish.Updated_date = DateTime.Now;
                    wish.wish_status = 1;
                    context.Wishlists.Add(wish);
                    context.SaveChanges();
                    status = 1;
                }
                else if(wishcount!=0)
                {
                    var wish_table = context.Wishlists.Where(a => a.cust_id == cust_id && a.item_id == item_id);
                    foreach (Wishlist wish in wish_table)
                    {
                        wish.Updated_date = DateTime.Now;
                        wish.wish_status = 1;
                    }
                    context.SaveChanges();
                    status = 1;
                }
            }
            catch (Exception e)
            {
                Logdetails.LogError("Post Error", "addtowishlisterror customercontroller (154)", e.Message);
            }

            return status;
        }

        public int removefromwish(JObject parame)
        {
            int status = 0;
            //int cust_id = 0;
            //int prod_id = 0;
            int item_id = 0;
            int cust_id = 0;
            int Login_status = 0;

            try
            {
                InitController Login = new InitController();

                JObject param = Login.Login();

                string json = JsonConvert.SerializeObject(param);

                cust_id = (int)JObject.Parse(json)["Login"]["User_id"];
                Login_status = (int)JObject.Parse(json)["Login"]["Login_status"];

                //cust_id = (int)parame.GetValue("userid");
                //prod_id = (int)param.GetValue("prodid");
                item_id = (int)parame.GetValue("itemid");

                var wish_table = context.Wishlists.Where(a => a.cust_id == cust_id && a.item_id == item_id);
                foreach (Wishlist wish in wish_table)
                {
                    wish.Updated_date = DateTime.Now;
                    wish.wish_status = 0;
                }
                context.SaveChanges();
                status = 1;
            }
            catch (Exception e)
            {
                Logdetails.LogError("Post Error", "removefromwisherror customercontroller (188)", e.Message);
            }

            return status;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/customer/GetPostalCodeValidation/{Post_Code}")]
        public JObject GetPostalCodeValidation(int Post_Code)
        {
            JArray PostList = new JArray();
            JObject final = new JObject();
            string PostCode = Post_Code.ToString();
            try
            {
                int Post_code = context.Postal_Code.Where(a => a.area_code == PostCode && a.status == 1).Count();
                var Postal = context.Postal_Code.Where(a => a.area_code == PostCode && a.status == 1);
                if (Post_code!=0)
                {
                   foreach(Postal_Code PC in Postal)
                    {
                        JObject Post_List = new JObject(
                            new JProperty("Id", PC.Id),
                            new JProperty("area_name", PC.area_name),
                            new JProperty("area_code", PC.area_code)
                            );
                        PostList.Add(Post_List);
                    }
                    final = new JObject(
                            new JProperty("PostList", PostList));
                }
                else
                {
                    final = new JObject(
                            new JProperty("PostList", PostList));
                }
            }
            catch (Exception e)
            {
                Logdetails.LogError("Post Error", "PostalCodeValidation customercontroller (219)", e.Message);
            }

            return final;
        }
    }
}
