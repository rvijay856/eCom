using AutobuyDirectApi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EcomErrorLog;

namespace AutobuyDirectApi.Controllers
{
    public class AdminController : ApiController
    {
        EcommEntities context = new EcommEntities();

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/admin/GetCategory")]
        public JObject GetCategory()
        {
            var Category = context.Product_Category.AsNoTracking().OrderBy(a => a.Created_date);
            JArray array = new JArray();
            foreach (Product_Category cat in Category.Where(a => a.cat_status == 1))
            {
                JObject bo = new JObject(
                    new JProperty("category_id", cat.id),
                    new JProperty("category_name", cat.cat_name),
                    new JProperty("category_parent", cat.cat_parent),
                    new JProperty("category_slug", cat.cat_slug),
                    new JProperty("Created_Date", cat.Created_date),
                    new JProperty("updated_Date", cat.Updated_date),
                    new JProperty("Status", cat.cat_status)
                    );
                array.Add(bo);
            }


            JObject final = new JObject(
               new JProperty("Category_Details", array));

            return final;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/admin/GetProduct")]
        public JObject GetProduct()
        {
            var Product = context.Product1.AsNoTracking().OrderBy(a => a.prod_name);
            JArray array = new JArray();
            foreach (Product1 pro in Product.Where(a => a.prod_status == 1))
            {
                JObject bo = new JObject(
                    new JProperty("product_id", pro.prod_id),
                    new JProperty("product_name", pro.prod_name),
                    new JProperty("product_slug", pro.prod_slug),
                    new JProperty("product_category", pro.prod_category),
                    new JProperty("product_brand", pro.prod_brand),
                    new JProperty("product_desc", pro.prod_desc),
                    new JProperty("product_sub_category", pro.prod_subcategory),
                    new JProperty("product_created_date", pro.Created_date),
                    new JProperty("product_updated_date", pro.Updated_date),
                    new JProperty("Status", pro.prod_status)
                    );
                array.Add(bo);
            }


            JObject final = new JObject(
               new JProperty("Product_Details", array));

            return final;
        }

        [System.Web.Http.HttpPost]
        public int CreateCategory(JObject param)
        {

            int status = 0;
            string category_name = "";
            int category_parent = 0;
            string category_slug = "";
            try
            {
                category_name = (string)param.GetValue("categoryname");
                category_parent = (int)param.GetValue("category_parent");
                category_slug= (string)param.GetValue("categoryslug");

                Product_Category cat = new Product_Category();
                cat.cat_name = category_name;
                cat.cat_parent = category_parent;
                cat.cat_slug = category_slug;
                cat.cat_status = 1;
                cat.Created_date = DateTime.Now;
                context.Product_Category.Add(cat);
                context.SaveChanges();
                status = 1;
            }
            catch (Exception e)
            {
                Logdetails.LogError("Post Error", "CreateCategoryerror admincontroller (69)", e.Message);
            }

            return status;
        }

        [System.Web.Http.HttpPost]
        public int CreateProduct(JObject param)
        {

            int status = 0;
            string product_name = "";
            int product_category = 0;
            string product_brand = "";
            string product_desc = "";
            string product_slug = "";
            decimal saleprice = 0;
            int product_sub_Cat = 0;
            string status_str = "";
           
            try
            {
                product_name = (string)param.GetValue("product_title");
                product_category = (int)param.GetValue("product_category");
                product_brand = (string)param.GetValue("product_brand");
                product_desc = (string)param.GetValue("product_description");
                product_slug = (string)param.GetValue("product_slug");
                product_sub_Cat = (int)param.GetValue("product_sub_category");
                status_str = (string)param.GetValue("status");
              
                
                Product1 pro = new Product1();
                pro.prod_name = product_name;
                pro.prod_category =product_category;
                pro.prod_brand = product_brand;
                pro.prod_desc = product_desc;
                pro.prod_slug = product_slug;
                pro.prod_subcategory = product_sub_Cat;
                pro.Created_date = DateTime.Now;
                if (status_str == "Active")
                {
                    pro.prod_status = 1;
                }
                else
                {
                    pro.prod_status = 0;
                }
                context.Product1.Add(pro);
                context.SaveChanges();
                status = 1;
            }
            catch (Exception e)
            {
                Logdetails.LogError("Post Error", "CreateProducterror admincontroller (100)", e.Message);
            }

            return status;
        }
    }
}
