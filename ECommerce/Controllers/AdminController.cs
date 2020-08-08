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
            var Category = context.categories.AsNoTracking().OrderBy(a => a.created_date);
            JArray array = new JArray();
            foreach (category cat in Category.Where(a => a.status == 1))
            {
                JObject bo = new JObject(
                    new JProperty("category_id", cat.category_id),
                    new JProperty("category_name", cat.category_name),
                    new JProperty("category_type", cat.category_type),
                    new JProperty("Created_Date", cat.created_date),
                    new JProperty("Status", cat.status)
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
            var Product = context.products.AsNoTracking().OrderBy(a => a.product_name);
            JArray array = new JArray();
            foreach (product pro in Product.Where(a => a.status == 1))
            {
                JObject bo = new JObject(
                    new JProperty("product_id", pro.product_id),
                    new JProperty("product_name", pro.product_name),
                    new JProperty("product_category", pro.product_category),
                    new JProperty("product_unit_qty", pro.product_unit_qty),
                    new JProperty("product_unit", pro.product_unit),
                    new JProperty("Status", pro.status)
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
            string category_type = "";
            
            try
            {
                category_name = (string)param.GetValue("categoryname");
                category_type = (string)param.GetValue("categorytype");
                

                category cat = new category();
                cat.category_name = category_name;
                cat.category_type = category_type;
                cat.status = 1;
                cat.created_date = DateTime.Now;
                context.categories.Add(cat);
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
            string product_category = "";
            int product_unit_qty = 0;
            string product_unit = "";
            decimal org_price=0;
            decimal saleprice = 0;
            string product_des = "";
            string shippingclass = "";
            string status_str = "";
            string stocks = "";
            try
            {
                product_name = (string)param.GetValue("product_title");
                product_category = (string)param.GetValue("category");
                product_unit_qty = (int)param.GetValue("quantity");
                product_unit = (string)param.GetValue("Weight");
                org_price = (decimal)param.GetValue("price");
                product_des = (string)param.GetValue("product_description");
                saleprice = (decimal)param.GetValue("sale_price");
                shippingclass = (string)param.GetValue("shipping_class");
                status_str = (string)param.GetValue("status");
                stocks = (string)param.GetValue("stock");

                
                product pro = new product();
                pro.product_name = product_name;
                pro.product_category =product_category;
                pro.product_unit_qty = product_unit_qty;
                pro.product_unit = product_unit;
                pro.orignal_price = org_price;
                pro.product_description = product_des;
                pro.sale_price = saleprice;
                pro.shipping_class = shippingclass;
                if (status_str == "Active")
                {
                    pro.status = 1;
                }
                else
                {
                    pro.status = 0;
                }
                pro.stock = stocks;
                context.products.Add(pro);
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
