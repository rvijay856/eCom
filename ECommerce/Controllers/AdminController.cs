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
            int brand_title_count = 0;
            int brand_id = 0;
            try
            {
                brand_title = (string)param.GetValue("brand_title");
                brand_type=(string)param.GetValue("brand_type");
                prod = (JArray)param.GetValue("products");
                sub_cate= (string)param.GetValue("sub_cate");
                url_img= (string)param.GetValue("url");
                brand_type_count = context.Brand_Menu.AsNoTracking().Where(a => a.Barnd_Section == brand_type && a.Brand_Status == 1).Count();
                brand_title_count = context.Brand_Menu.AsNoTracking().Where(a => a.Brand_Title.Equals(brand_title.Trim()) && a.Brand_Status == 1).Count();


                if (brand_type_count < 4 & brand_title_count==0)
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
                    status = "Limit Exist or Title Already Exist";
                }
                foreach(JObject item in prod)
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
            return status;
        }
    }
}
