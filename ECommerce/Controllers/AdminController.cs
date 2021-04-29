using AutobuyDirectApi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EcomErrorLog;
using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace AutobuyDirectApi.Controllers
{
    [System.Web.Http.Authorize]
    public class AdminController : ApiController
    {

        EcommEntities context = new EcommEntities();

        [System.Web.Http.HttpPost]
        public int CreateCategory(JObject param)
        {
            //Get main cat or not chekc box kkanum then condition podanum
            int status = 0;
            int cust_id = 0;
            int Login_status = 0;
            InitController Login = new InitController();
            JObject parame = Login.Login();
            string json = JsonConvert.SerializeObject(parame);
            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];
            int cust_type = (int)context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id).Select(a => a.cust_type).Single();
            if (cust_type == 2000 && Login_status==1)
            {
                string category_name = "";
                int category_type = 0;
                string category_parent = "";
                string category_slug = "";
                string[] replaceables = new[] { "+", "!", "(", ")", "{", "}", "[", "]", "^", "~", "*", "?", ":", "\\", "\"", " ", "-", ";", "&", "|", "/", "_", "@", "#", "$", "%", "<", ">", "=" };
                int count = 0;
                string category_img = "";
                Random random = new Random();
                try
                {
                    category_name = (string)param.GetValue("catname");
                    category_type = (int)param.GetValue("cattype");
                    category_parent = (string)param.GetValue("parent_id");
                    category_img = (string)param.GetValue("url");

                    for (int i = 0; i < replaceables.Length; i++)
                    {
                        category_slug = category_name.Replace(replaceables[i], String.Empty);
                    }
                    count = context.Product_Category.AsNoTracking().Where(a => a.cat_slug == category_slug).Count();
                    if (count != 0)
                    {
                        category_slug = category_slug + "-" + random.Next(0, 1000).ToString();
                    }
                    Product_Category cat = new Product_Category();
                    cat.cat_name = category_name;
                    if (category_type == 1)
                    {
                        cat.cat_parent = 0;
                    }
                    else if (category_type == 2)
                    {
                        cat.cat_parent = int.Parse(category_parent);
                    }
                    cat.cat_slug = category_slug;
                    cat.cat_img = category_img;
                    cat.cat_status = 1;
                    cat.Created_date = DateTime.Now;
                    cat.Updated_date = DateTime.Now;
                    context.Product_Category.Add(cat);
                    context.SaveChanges();
                    status = 1;
                }
                catch (Exception e)
                {
                    Logdetails.LogError("Post Error", "CreateCategoryerror admincontroller (134)", e.Message);
                }
            }
            return status;
        }

        [System.Web.Http.HttpPost]
        public int UpdateCategory(JObject param)
        {
            int status = 0;
            int cust_id = 0;
            int Login_status = 0;
            InitController Login = new InitController();
            JObject parame = Login.Login();
            string json = JsonConvert.SerializeObject(parame);
            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];
            int cust_type = (int)context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id).Select(a => a.cust_type).Single();
            if (cust_type == 2000 && Login_status == 1)
            {
                string category_name = "";
                int category_type = 0;
                string category_parent = "";
                string category_slug = "";
                int category_Id = 0;
                string[] replaceables = new[] { "+", "!", "(", ")", "{", "}", "[", "]", "^", "~", "*", "?", ":", "\\", "\"", " ", "-", ";", "&", "|", "/", "_", "@", "#", "$", "%", "<", ">", "=" };
                string category_img = "";
                Random random = new Random();
                try
                {
                    category_Id = (int)param.GetValue("catid");
                    category_name = (string)param.GetValue("catname");
                    category_type = (int)param.GetValue("cattype");
                    category_parent = (string)param.GetValue("parent_id");
                    category_img = (string)param.GetValue("url");

                    for (int i = 0; i < replaceables.Length; i++)
                    {
                        category_slug = category_name.Replace(replaceables[i], String.Empty);
                    }

                    var catupdate = context.Product_Category.Where(a=>a.id==category_Id);
                    foreach (Product_Category cat in catupdate)
                    {
                        cat.cat_name = category_name;
                        if (category_type == 1)
                        {
                            cat.cat_parent = 0;
                        }
                        else if (category_type == 2)
                        {
                            cat.cat_parent = int.Parse(category_parent);
                        }
                        cat.cat_slug = category_slug;
                        cat.cat_img = category_img;
                        cat.cat_status = 1;
                        cat.Updated_date = DateTime.Now;
                    }
                    context.SaveChanges();
                    status = 1;
                }
                catch (Exception e)
                {
                    Logdetails.LogError("Post Error", "UpdateCategoryerror admincontroller (134)", e.Message);
                }
            }
            return status;
        }

        [System.Web.Http.HttpPost]
        public string CreateProduct(JObject param)
        {
            string status = "Already Exist";
            string BrandName = "";
            string category = "";
            string code = "";
            string item_status = "";
            string item_stock = "";
            string price = "";
            string product_description = "";
            string product_name = "";
            string sale_price = "";
            string spec = "";
            string subcategory = "";
            string unit = "";
            string url = "";
            string[] replaceables = new[] { "+", "!", "(", ")", "{", "}", "[", "]", "^", "~", "*", "?", ":", "\\", "\"", " ", "-", ";", "&", "|", "/", "_", "@", "#", "$", "%", "<", ">", "=" };
            int count = 0;
            int Pro_count = 0;
            string product_slug = "";
            int prod_id = 0;
            int proitem_count = 0;
            Random random = new Random();
            int cust_id = 0;
            int Login_status = 0;
            InitController Login = new InitController();
            JObject parame = Login.Login();
            string json = JsonConvert.SerializeObject(parame);
            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];
            int cust_type = (int)context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id).Select(a => a.cust_type).Single();
            if (cust_type == 2000 && Login_status == 1)
            {
                try
                {
                    BrandName = (string)param.GetValue("BrandName");
                    category = (string)param.GetValue("category");
                    code = (string)param.GetValue("code");
                    item_status = (string)param.GetValue("item_status");
                    item_stock = (string)param.GetValue("item_stock");
                    price = (string)param.GetValue("price");
                    product_description = (string)param.GetValue("product_description");
                    product_name = (string)param.GetValue("product_name");
                    sale_price = (string)param.GetValue("sale_price");
                    spec = (string)param.GetValue("spec");
                    subcategory = (string)param.GetValue("subcategory");
                    unit = (string)param.GetValue("unit");
                    url = (string)param.GetValue("url");

                    BrandName = BrandName.Trim();
                    category = category.Trim();
                    code = code.Trim();
                    item_status = item_status.Trim();
                    item_stock = item_stock.Trim();
                    price = price.Trim();
                    product_description = product_description.Trim();
                    product_name = product_name.Trim();
                    sale_price = sale_price.Trim();
                    spec = spec.Trim();
                    subcategory = subcategory.Trim();
                    unit = unit.Trim();

                    for (int i = 0; i < replaceables.Length; i++)
                    {
                        product_slug = product_name.Replace(replaceables[i], String.Empty);
                    }
                    count = context.Products.AsNoTracking().Where(a => a.prod_slug == product_slug).Count();
                    Pro_count = context.Products.AsNoTracking().Where(a => a.prod_name == product_name && a.prod_slug == product_slug).Count();

                    if (Pro_count != 0)
                    {
                        prod_id = (int)context.Products.Where(a => a.prod_slug == product_slug && a.prod_name == product_name).Select(a => a.prod_id).Single();
                        proitem_count = context.Product_items.AsNoTracking().Where(a => a.prod_id == prod_id && a.item_spec == spec).Count();
                    }
                    if (count != 0)
                    {
                        product_slug = product_slug + "-" + random.Next(0, 1000).ToString();
                    }
                    if (Pro_count == 0)
                    {
                        Product pro = new Product();
                        pro.prod_name = product_name;
                        pro.prod_slug = product_slug;
                        pro.prod_category = int.Parse(category);
                        pro.prod_subcategory = int.Parse(subcategory);
                        pro.prod_brand = BrandName;
                        pro.prod_desc = product_description;
                        pro.prod_status = 1;
                        pro.Created_date = DateTime.Now;
                        pro.Updated_date = DateTime.Now;

                        context.Products.Add(pro);
                        context.SaveChanges();

                        prod_id = (int)context.Products.Where(a => a.prod_slug == product_slug && a.prod_name == product_name).Select(a => a.prod_id).Single();
                        Product_items proItem = new Product_items();
                        proItem.prod_id = prod_id;
                        proItem.item_code = int.Parse(code);
                        proItem.item_spec = spec;
                        proItem.item_unit = unit;
                        proItem.item_mrp = decimal.Parse(price);
                        proItem.item_selling = decimal.Parse(sale_price);
                        proItem.item_stock = int.Parse(item_stock);
                        proItem.item_image = url;
                        proItem.item_status = int.Parse(item_status);
                        proItem.Created_date = DateTime.Now;
                        proItem.Updated_date = DateTime.Now;

                        context.Product_items.Add(proItem);
                        context.SaveChanges();
                        status = "Success";
                    }
                    else if (Pro_count != 0 & proitem_count == 0)
                    {
                        Product_items proItem = new Product_items();
                        proItem.prod_id = prod_id;
                        proItem.item_code = int.Parse(code);
                        proItem.item_spec = spec;
                        proItem.item_unit = unit;
                        proItem.item_mrp = decimal.Parse(price);
                        proItem.item_selling = decimal.Parse(sale_price);
                        proItem.item_stock = int.Parse(item_stock);
                        proItem.item_image = url;
                        proItem.item_status = int.Parse(item_status);
                        proItem.Created_date = DateTime.Now;
                        proItem.Updated_date = DateTime.Now;

                        context.Product_items.Add(proItem);
                        context.SaveChanges();
                        status = "Success";
                    }
                }
                catch (Exception e)
                {
                    Logdetails.LogError("Post Error", "CreateProducterror admincontroller (100)", e.Message);
                }
            }
            return status;
        }

        [System.Web.Http.HttpPost]
        public string UpdateProduct(JObject param)
        {
            string status = "Updated Failed";
            string BrandName = "";
            string category = "";
            string code = "";
            string item_status = "";
            string item_stock = "";
            string price = "";
            string product_description = "";
            string product_name = "";
            string sale_price = "";
            string spec = "";
            string subcategory = "";
            string unit = "";
            string url = "";
            string[] replaceables = new[] { "+", "!", "(", ")", "{", "}", "[", "]", "^", "~", "*", "?", ":", "\\", "\"", " ", "-", ";", "&", "|", "/", "_", "@", "#", "$", "%", "<", ">", "=" };
            string product_slug = "";
            int prod_id = 0;
            Random random = new Random();
            int cust_id = 0;
            int Login_status = 0;
            InitController Login = new InitController();
            JObject parame = Login.Login();
            string json = JsonConvert.SerializeObject(parame);
            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];
            int cust_type = (int)context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id).Select(a => a.cust_type).Single();
            if (cust_type == 2000 && Login_status == 1)
            {
                try
                {
                    prod_id = (int)param.GetValue("proid");
                    BrandName = (string)param.GetValue("BrandName");
                    category = (string)param.GetValue("category");
                    code = (string)param.GetValue("code");
                    item_status = (string)param.GetValue("item_status");
                    item_stock = (string)param.GetValue("item_stock");
                    price = (string)param.GetValue("price");
                    product_description = (string)param.GetValue("product_description");
                    product_name = (string)param.GetValue("product_name");
                    sale_price = (string)param.GetValue("sale_price");
                    spec = (string)param.GetValue("spec");
                    subcategory = (string)param.GetValue("subcategory");
                    unit = (string)param.GetValue("unit");
                    url = (string)param.GetValue("url");

                    BrandName = BrandName.Trim();
                    category = category.Trim();
                    code = code.Trim();
                    item_status = item_status.Trim();
                    item_stock = item_stock.Trim();
                    price = price.Trim();
                    product_description = product_description.Trim();
                    product_name = product_name.Trim();
                    sale_price = sale_price.Trim();
                    spec = spec.Trim();
                    subcategory = subcategory.Trim();
                    unit = unit.Trim();

                    for (int i = 0; i < replaceables.Length; i++)
                    {
                        product_slug = product_name.Replace(replaceables[i], String.Empty);
                    }
                    var proupdate = context.Products.Where(a => a.prod_id == prod_id);
                    foreach (Product pro in proupdate)
                    {
                        pro.prod_name = product_name;
                        pro.prod_slug = product_slug;
                        pro.prod_category = int.Parse(category);
                        pro.prod_subcategory = int.Parse(subcategory);
                        pro.prod_brand = BrandName;
                        pro.prod_desc = product_description;
                        pro.Updated_date = DateTime.Now;

                        var itemupdate = context.Product_items.Where(a => a.prod_id == prod_id);
                        foreach (Product_items proItem in itemupdate)
                        {
                            proItem.item_code = int.Parse(code);
                            proItem.item_spec = spec;
                            proItem.item_unit = unit;
                            proItem.item_mrp = decimal.Parse(price);
                            proItem.item_selling = decimal.Parse(sale_price);
                            proItem.item_stock = int.Parse(item_stock);
                            proItem.item_image = url;
                            proItem.item_status = int.Parse(item_status);
                            proItem.Updated_date = DateTime.Now;
                        }

                    }
                        context.SaveChanges();
                        status = "Updated Success";
                }
                catch (Exception e)
                {
                    Logdetails.LogError("Post Error", "CreateProducterror admincontroller (100)", e.Message);
                }
            }
            return status;
        }

        [System.Web.Http.HttpPost]
        public string CreateBrand(JObject param)
        {
            string status = "";
            string brand_title = "";
            string brand_type = "";
            //string products = "";
            string sub_cate = "";
            string url_img = "";
            int prod_id = 0;
            string item_name = "";
            int prod_category = 0;
            int prod_subcategory = 0;
            JArray prod = new JArray();
            int brand_type_count = 0;
            //string brand_titles = "";
            int brand_id = 0;

            int cust_id = 0;
            int Login_status = 0;
            InitController Login = new InitController();
            JObject parame = Login.Login();
            string json = JsonConvert.SerializeObject(parame);
            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];
            int cust_type = (int)context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id).Select(a => a.cust_type).Single();
            if (cust_type == 2000 && Login_status == 1)
            {
                try
                {
                    brand_title = (string)param.GetValue("brand_title");
                    brand_type = (string)param.GetValue("brand_type");
                    prod = (JArray)param.GetValue("products");
                    sub_cate = (string)param.GetValue("sub_cate");
                    url_img = (string)param.GetValue("url");
                    brand_type_count = context.Brand_Menu.AsNoTracking().Where(a => a.Barnd_Section == brand_type && a.Brand_Status == 1).Count();
                    if (brand_type_count != 0)
                    {
                        brand_title = context.Brand_Menu.AsNoTracking().Where(a => a.Barnd_Section == brand_type).Select(a => a.Brand_Title).Max();
                    }
                    if (brand_type_count < 4)
                    {
                        Brand_Menu BM = new Brand_Menu();
                        BM.Brand_Title = brand_title.Trim();
                        BM.Barnd_Section = brand_type;
                        BM.Brand_Order = brand_type_count + 1;
                        BM.Brand_Img = url_img;
                        BM.Brand_Status = 1;
                        BM.Created_Date = DateTime.Now;
                        BM.Updated_Date = DateTime.Now;
                        status = "Success";
                        context.Brand_Menu.Add(BM);
                        context.SaveChanges();

                        brand_id = BM.Id;
                    }
                    else
                    {
                        status = "Limit Exist";
                    }
                    foreach (JObject item in prod)
                    {
                        prod_id = (int)item.GetValue("id");
                        item_name = (string)item.GetValue("itemName");
                        prod_category = (int)item.GetValue("prod_category");
                        prod_subcategory = (int)item.GetValue("prod_subcategory");

                        Brand_Product BP = new Brand_Product();
                        BP.Prod_id = prod_id;
                        BP.Prod_name = item_name;
                        BP.Prod_category = prod_category;
                        BP.Prod_subcategory = prod_subcategory;
                        BP.Brand_id = brand_id;
                        BP.status = 1;

                        context.Brand_Product.Add(BP);
                        context.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Logdetails.LogError("Post Error", "CreateBranderror admincontroller (424)", e.Message);
                }
            }
            return status;
        }

        [System.Web.Http.HttpPost]
        public string CreateCarousel(JObject param)
        {
            string status = "";
            string Carousel_title = "";
            string Carousel_type = "";
            //string products = "";
            string sub_cate = "";
            string url_img = "";
            int prod_id = 0;
            string item_name = "";
            int prod_category = 0;
            int prod_subcategory = 0;
            JArray prod = new JArray();
            int Carousel_type_count = 0;
            int Carousel_title_count = 0;
            int Carousel_id = 0;

            int cust_id = 0;
            int Login_status = 0;
            InitController Login = new InitController();
            JObject parame = Login.Login();
            string json = JsonConvert.SerializeObject(parame);
            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];
            int cust_type = (int)context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id).Select(a => a.cust_type).Single();
            if (cust_type == 2000 && Login_status == 1)
            {
                try
                {
                    Carousel_title = (string)param.GetValue("Carousel_title");
                    Carousel_type = (string)param.GetValue("Carousel_type");
                    prod = (JArray)param.GetValue("products");
                    sub_cate = (string)param.GetValue("sub_cate");
                    url_img = (string)param.GetValue("url");
                    Carousel_type_count = context.Carousel_Menu.AsNoTracking().Where(a => a.Carousel_Section == Carousel_type && a.Carousel_Status == 1).Count();
                    Carousel_title_count = context.Carousel_Menu.AsNoTracking().Where(a => a.Carousel_Title.Equals(Carousel_title.Trim()) && a.Carousel_Status == 1).Count();

                    if (Carousel_type_count < 3 & Carousel_title_count == 0)
                    {
                        Carousel_Menu BM = new Carousel_Menu();
                        BM.Carousel_Title = Carousel_title.Trim();
                        BM.Carousel_Section = Carousel_type;
                        BM.Carousel_Order = Carousel_type_count + 1;
                        BM.Carousel_Img = url_img;
                        BM.Carousel_Status = 1;
                        BM.Created_Date = DateTime.Now;
                        BM.Updated_Date = DateTime.Now;
                        status = "Success";
                        context.Carousel_Menu.Add(BM);
                        context.SaveChanges();

                        Carousel_id = BM.Id;
                    }
                    else
                    {
                        status = "Limit Exist or Title Already Exist";
                    }
                    foreach (JObject item in prod)
                    {
                        prod_id = (int)item.GetValue("id");
                        item_name = (string)item.GetValue("itemName");
                        prod_category = (int)item.GetValue("prod_category");
                        prod_subcategory = (int)item.GetValue("prod_subcategory");

                        Carousel_Product BP = new Carousel_Product();
                        BP.Prod_id = prod_id;
                        BP.Prod_name = item_name;
                        BP.Prod_category = prod_category;
                        BP.Prod_subcategory = prod_subcategory;
                        BP.Carousel_id = Carousel_id;
                        BP.status = 1;

                        context.Carousel_Product.Add(BP);
                        context.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Logdetails.LogError("Post Error", "CreateCarouselerror admincontroller (424)", e.Message);
                }
            }
            return status;
        }

        [System.Web.Http.HttpPost]
        public string CreateTrending(JObject param)
        {
            string status = "";
            string Trending_title = "";
            string sub_cate = "";
            int prod_id = 0;
            string item_name = "";
            int prod_category = 0;
            int prod_subcategory = 0;
            JArray prod = new JArray();
            int Trending_title_count = 0;
            int Trending_id = 0;

            int cust_id = 0;
            int Login_status = 0;
            InitController Login = new InitController();
            JObject parame = Login.Login();
            string json = JsonConvert.SerializeObject(parame);
            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];
            int cust_type = (int)context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id).Select(a => a.cust_type).Single();
            if (cust_type == 2000 && Login_status == 1)
            {
                try
                {
                    Trending_title = (string)param.GetValue("Trending_title");
                    prod = (JArray)param.GetValue("products");
                    sub_cate = (string)param.GetValue("sub_cate");
                    Trending_title_count = context.Trending_Menu.AsNoTracking().Where(a => a.Trending_Title.Equals(Trending_title.Trim()) && a.Trending_Status == 1).Count();
                    if (Trending_title_count == 0)
                    {
                        Trending_Menu TM = new Trending_Menu();
                        TM.Trending_Title = Trending_title.Trim();
                        TM.Trending_Status = 1;
                        TM.Created_Date = DateTime.Now;
                        TM.Updated_Date = DateTime.Now;
                        status = "Success";
                        context.Trending_Menu.Add(TM);
                        context.SaveChanges();

                        Trending_id = TM.Id;

                    }
                    else
                    {
                        status = "Title Already Exist";
                    }
                    foreach (JObject item in prod)
                    {
                        prod_id = (int)item.GetValue("id");
                        item_name = (string)item.GetValue("itemName");
                        prod_category = (int)item.GetValue("prod_category");
                        prod_subcategory = (int)item.GetValue("prod_subcategory");

                        Trending_Product TP = new Trending_Product();
                        TP.Prod_id = prod_id;
                        TP.Prod_name = item_name;
                        TP.Prod_category = prod_category;
                        TP.Prod_subcategory = prod_subcategory;
                        TP.Trending_id = Trending_id;
                        TP.status = 1;

                        context.Trending_Product.Add(TP);
                        context.SaveChanges();

                    }
                }
                catch (Exception e)
                {
                    Logdetails.LogError("Post Error", "CreateTrendingerror admincontroller (424)", e.Message);
                }
            }
            return status;
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Admin/GetAdminOrderDetails")]
        public JObject GetAdminOrderDetails()
        {
            JObject final = new JObject();
            JObject Order_li = new JObject();
            JObject Order_de = new JObject();
            JObject addList = new JObject();
            JArray Order_de_array = new JArray();
            JArray Order_addr_array = new JArray();
            JArray order_list_details = new JArray();
            JObject Order_pay = new JObject();
            JArray Order_pay_array = new JArray();

            string prod_name = "";
            string item_spec = "";
            //decimal total = 0;
            int payment_status = 100;
            string payment_state = "";

            int cust_id = 0;
            int Login_status = 0;
            InitController Login = new InitController();
            JObject parame = Login.Login();
            string json = JsonConvert.SerializeObject(parame);
            cust_id = (int)JObject.Parse(json)["User_id"];
            Login_status = (int)JObject.Parse(json)["Login_status"];
            int cust_type = (int)context.Customers.AsNoTracking().Where(a => a.cust_id == cust_id).Select(a => a.cust_type).Single();
            if (cust_type == 2000 && Login_status == 1)
            {
                var Orde = context.Orders.AsNoTracking();
                foreach (Order or in Orde)
                {
                    var Order_deta = context.Order_details.AsNoTracking().Where(a => a.order_id == or.order_id);
                    Order_de_array = new JArray();
                    foreach (Order_details od in Order_deta)
                    {
                        prod_name = context.Products.AsNoTracking().Where(a => a.prod_id == od.product_id).Select(a => a.prod_name).Single();
                        item_spec = context.Product_items.AsNoTracking().Where(a => a.id == od.item_id).Select(a => a.item_spec).Single();
                        item_spec += context.Product_items.AsNoTracking().Where(a => a.id == od.item_id).Select(a => a.item_unit).Single();

                        Order_de = new JObject(
                           new JProperty("product_id", od.product_id),
                           new JProperty("prod_name", prod_name),
                           new JProperty("prod_img", "prod_img"),
                           new JProperty("item_id", od.item_id),
                           new JProperty("item_spec", item_spec),
                           new JProperty("prod_quantity", od.prod_quantity),
                           new JProperty("mrp_price", od.mrp_price),
                           new JProperty("selling_price", od.selling_price)
                           );
                        Order_de_array.Add(Order_de);
                    }
                    var custaddress = context.customer_address.AsNoTracking().Where(a => a.id == or.address_id);
                    Order_addr_array = new JArray();
                    foreach (customer_address cusadd in custaddress)
                    {
                        addList = new JObject(
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
                        new JProperty("updated_date", cusadd.updated_date)
                       );
                        Order_addr_array.Add(addList);
                    }
                    var pay_id = context.Customer_Payment.AsNoTracking().Where(a => a.id == or.payment_id);
                    Order_pay_array = new JArray();
                    foreach (Customer_Payment cp in pay_id)
                    {
                        //total = cp.payment_amount;
                        payment_status = (int)cp.payment_status;

                        if (payment_status == 0)
                        {
                            payment_state = "Yet To Received Payment";
                        }
                        else if (payment_status == 1)
                        {
                            payment_state = "Payment Received Successful";
                        }
                        else if (payment_status == 2)
                        {
                            payment_state = "Cancelled";
                        }
                        else if (payment_status == 100)
                        {
                            payment_state = "Not Available";
                        }

                        Order_pay = new JObject(
                        new JProperty("payment_id", or.payment_id),
                        new JProperty("payment_status", payment_state),
                        new JProperty("payment_Type", cp.payment_type),
                        new JProperty("Total", cp.payment_amount)
                        );
                        Order_pay_array.Add(Order_pay);
                    }
                    Order_li = new JObject(
                        new JProperty("order_id", or.order_id),
                        new JProperty("order_no", or.order_no),
                        new JProperty("order_date", or.order_date),
                        new JProperty("payment_date", or.payment_date),
                        new JProperty("delivery_status", or.delivery_status),
                        new JProperty("Product_details", Order_de_array),
                        new JProperty("Address_details", Order_addr_array),
                        new JProperty("Payment_details", Order_pay_array)
                    );
                    order_list_details.Add(Order_li);
                }

                final = new JObject(
                   new JProperty("order_list_details", order_list_details));
            }
            return final;
        }


    }
}
