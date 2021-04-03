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
    [System.Web.Http.Authorize]
    public class CustomerController : ApiController
    {
        EcommEntities context = new EcommEntities();

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/customer/GetCart")]
        public JObject GetCart()
        {
            int cust_id = 0;
            int Login_status = 0;
            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];

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

            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];
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
                        car.quantity = quantity;
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

            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];

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

            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];

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

            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];

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

                cust_id = (int)JObject.Parse(json)["User_id"];
                Login_status = (int)JObject.Parse(json)["Login_status"];

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
            int prod_id = 0;
            int item_id = 0;
            int cust_id = 0;
            int Login_status = 0;

            try
            {
                InitController Login = new InitController();

                JObject param = Login.Login();

                string json = JsonConvert.SerializeObject(param);

                cust_id = (int)JObject.Parse(json)["User_id"];
                Login_status = (int)JObject.Parse(json)["Login_status"];

                //cust_id = (int)parame.GetValue("userid");
                prod_id = (int)parame.GetValue("prodid");
                item_id = (int)parame.GetValue("itemid");

                var wish_table = context.Wishlists.Where(a => a.cust_id == cust_id && a.item_id == item_id && a.prod_id==prod_id);
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

        public int CreateOrder(JObject parame)
        {
            int status = 0;
            int cust_id = 0;
            int Login_status = 0;
            int item_id = 0;
            int Qty = 0;
            try
            {
                InitController Login = new InitController();

                JObject param = Login.Login();

                string json = JsonConvert.SerializeObject(param);

                cust_id = (int)JObject.Parse(json)["User_id"];
                Login_status = (int)JObject.Parse(json)["Login_status"];

                item_id = (int)parame.GetValue("itemid");
                Qty= (int)parame.GetValue("Qty");
            }
            catch(Exception e)
            {
                Logdetails.LogError("Post Error", "createorder customercontroller (188)", e.Message);
            }

            return status;
        }
        public int AddNewAddress(JObject parame)
        {
            int status = 0;
            int cust_id = 0;
            int Login_status = 0;
            string customer_name = "";
            string customer_address = "";
            string customer_area = "";
            string Landmark = "";
            string customer_city = "";
            int customer_pincode = 0;
            string customer_state = "";
            string customer_country = "";
            string customer_mobile = "";
            int address_count = 0;
            string defult_address = "";
            try
            {
                InitController Login = new InitController();

                JObject param = Login.Login();

                string json = JsonConvert.SerializeObject(param);

                cust_id = (int)JObject.Parse(json)["User_id"];
                Login_status = (int)JObject.Parse(json)["Login_status"];

                address_count = context.customer_address.AsNoTracking().Where(a => a.customer_id == cust_id && a.address_status == 1).Count();

                customer_name = (string)parame.GetValue("customer_name");
                customer_address = (string)parame.GetValue("customer_address");
                customer_area = (string)parame.GetValue("customer_area");
                Landmark = (string)parame.GetValue("Landmark");
                customer_city = (string)parame.GetValue("customer_city");
                customer_pincode = (int)parame.GetValue("customer_pincode");
                customer_state = (string)parame.GetValue("customer_state");
                customer_country = (string)parame.GetValue("customer_country");
                customer_mobile = (string)parame.GetValue("customer_mobile");
                defult_address = (string)parame.GetValue("defult_address");

                customer_address CA = new customer_address();
                CA.customer_id = cust_id;
                CA.customer_name = customer_name;
                CA.customer_address1 = customer_address;
                CA.customer_area = customer_area;
                CA.Landmark = Landmark;
                CA.customer_city = customer_city;
                CA.customer_pincode = customer_pincode;
                CA.customer_state = customer_state;
                CA.customer_country = customer_country;
                CA.customer_mobile = customer_mobile;
                CA.created_date = DateTime.Now;
                CA.address_status = 1;

                context.customer_address.Add(CA);
                context.SaveChanges();

                if (address_count==0)
                {
                    var cust = context.Customers.Where(a => a.cust_id == cust_id);
                    foreach(Customer cu in cust)
                    {
                        cu.cust_address_id = (int)context.customer_address.AsNoTracking().Where(a => a.customer_id == cust_id).Select(a => a.id).Single();
                        cu.cust_pincode = customer_pincode;
                    }
                    context.SaveChanges();
                }
                else if (defult_address == "Yes")
                {
                    var cust = context.Customers.Where(a => a.cust_id == cust_id);
                    foreach (Customer cu in cust)
                    {
                        cu.cust_address_id = (int)context.customer_address.AsNoTracking().Where(a => a.customer_id == cust_id).Select(a => a.id).Max();
                        cu.cust_pincode = customer_pincode;
                    }
                    context.SaveChanges();
                }
                status = 1;
            }
            catch (Exception e)
            {
                Logdetails.LogError("Post Error", "AddNewAddress customercontroller (188)", e.Message);
            }

            return status;
        }

        public int EditAddress(JObject parame)
        {
            int status = 0;
            int cust_id = 0;
            int Login_status = 0;
            int address_id = 0;
            string customer_name = "";
            string customer_address = "";
            string customer_area = "";
            string Landmark = "";
            string customer_city = "";
            int customer_pincode = 0;
            string customer_state = "";
            string customer_country = "";
            string customer_mobile = "";
            string defult_address = "";
            try
            {
                InitController Login = new InitController();

                JObject param = Login.Login();

                string json = JsonConvert.SerializeObject(param);

                cust_id = (int)JObject.Parse(json)["User_id"];
                Login_status = (int)JObject.Parse(json)["Login_status"];

                address_id = (int)parame.GetValue("address_id");
                customer_name = (string)parame.GetValue("customer_name");
                customer_address = (string)parame.GetValue("customer_address");
                customer_area = (string)parame.GetValue("customer_area");
                Landmark = (string)parame.GetValue("Landmark");
                customer_city = (string)parame.GetValue("customer_city");
                customer_pincode = (int)parame.GetValue("customer_pincode");
                customer_state = (string)parame.GetValue("customer_state");
                customer_country = (string)parame.GetValue("customer_country");
                customer_mobile = (string)parame.GetValue("customer_mobile");
                defult_address = (string)parame.GetValue("defult_address");

                var address = context.customer_address.Where(a => a.customer_id == cust_id && a.id == address_id);

                foreach (customer_address CA in address)
                {
                    CA.customer_id = cust_id;
                    CA.customer_name = customer_name;
                    CA.customer_address1 = customer_address;
                    CA.customer_area = customer_area;
                    CA.Landmark = Landmark;
                    CA.customer_city = customer_city;
                    CA.customer_pincode = customer_pincode;
                    CA.customer_state = customer_state;
                    CA.customer_country = customer_country;
                    CA.customer_mobile = customer_mobile;
                    CA.created_date = DateTime.Now;
                    CA.address_status = 1;
                }
                context.SaveChanges();
                if (defult_address == "Yes")
                {
                    var cust = context.Customers.Where(a => a.cust_id == cust_id);
                    foreach (Customer cu in cust)
                    {
                        cu.cust_address_id = address_id;
                        cu.cust_pincode = customer_pincode;
                    }
                    context.SaveChanges();
                }
                status = 1;
            }
            catch (Exception e)
            {
                Logdetails.LogError("Post Error", "EditAddress customercontroller (188)", e.Message);
            }

            return status;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/customer/Getaddress")]
        public JObject Getaddress()
        {
            int cust_id = 0;
            int Login_status = 0;
            string defult_address = "";
            int defult_address_id = 0;
            int address_count = 0;

            InitController Login = new InitController();
            JObject param = Login.Login();
            string json = JsonConvert.SerializeObject(param);
            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];

            JArray array = new JArray();
            address_count = context.customer_address.AsNoTracking().Where(a => a.customer_id == cust_id && a.address_status == 1).Count();
            if (address_count != 0)
            {
                defult_address_id = (int)context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id).Select(a => a.cust_address_id).Single();
                var custaddress = context.customer_address.AsNoTracking().Where(a => a.customer_id == cust_id && a.address_status == 1);
                foreach (customer_address cusadd in custaddress)
                {
                    if (defult_address_id == cusadd.id)
                    {
                        defult_address = "Yes";
                    }
                    else
                    {
                        defult_address = "";
                    }
                    JObject addList = new JObject(
                    new JProperty("id", cusadd.id.ToString()),
                    new JProperty("customer_id", cusadd.customer_id),
                    new JProperty("customer_name", cusadd.customer_name),
                    new JProperty("customer_address1", cusadd.customer_address1),
                    new JProperty("customer_area", cusadd.customer_area),
                    new JProperty("Landmark", cusadd.Landmark),
                    new JProperty("customer_city", cusadd.customer_city),
                    new JProperty("customer_pincode", cusadd.customer_pincode),
                    new JProperty("customer_state", cusadd.customer_state),
                    new JProperty("customer_country", cusadd.customer_country),
                    new JProperty("customer_mobile", cusadd.customer_mobile),
                    new JProperty("created_date", cusadd.created_date),
                    new JProperty("updated_date", cusadd.updated_date),
                    new JProperty("defult_address", defult_address)
                   );
                    array.Add(addList);
                }
            }
            JObject final = new JObject(
                new JProperty("Address", array));
            return final;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/customer/DeleteAddress/{ID}")]
        public JObject DeleteAddress(int ID)
        {
            int cust_id = 0;
            int Login_status = 0;
            int defult_address = 0;
            int newdefult_address = 0;
            int pincode = 0;
            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];

            JObject final = new JObject();
            var DeleteAd = context.customer_address.Where(a => a.id == ID);
                foreach (customer_address cu in DeleteAd)
                {
                    cu.address_status = 0;
                }
                context.SaveChanges();
            defult_address = (int)context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id).Select(a => a.cust_address_id).Single();
            if (defult_address==ID)
            {
                newdefult_address = (int)context.customer_address.AsNoTracking().Where(a => a.customer_id == cust_id && a.address_status == 1).Select(a => a.id).Max();
                pincode = (int)context.customer_address.AsNoTracking().Where(a => a.id == newdefult_address).Select(a => a.customer_pincode).Single();
                var cust = context.Customers.Where(a => a.cust_id == cust_id);
                foreach(Customer cu in cust)
                {
                    cu.cust_address_id = newdefult_address;
                    cu.cust_pincode = pincode;
                }
                context.SaveChanges();
            }

                final = new JObject(
               new JProperty("Delete_Address", "Success"));
            
            return final;
        }
    }
}
