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
        EcommEntities1 context = new EcommEntities1();

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/admin/GetCategory")]
        public JObject GetCategory()
        {
            var Category = context.Product_Category.AsNoTracking().Where(a => a.cat_status == 1 && a.cat_parent == 0);
            JArray array = new JArray();
            foreach (Product_Category cat in Category)
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
        [System.Web.Http.Route("api/admin/GetSubcategory/{CatID}")]
        public JObject GetSubcategory(int CatID)
        {
            int cat_id = CatID;
            var subcategory = context.Product_Category.AsNoTracking().Where(a => a.cat_status == 1 && a.cat_parent == CatID);
            JArray SubCat = new JArray();
            foreach(Product_Category subcat in subcategory)
            {
                JObject sc = new JObject(
                    new JProperty("category_id", subcat.id),
                    new JProperty("category_name", subcat.cat_name),
                    new JProperty("category_parent", subcat.cat_parent),
                    new JProperty("category_slug", subcat.cat_slug),
                    new JProperty("Created_Date", subcat.Created_date),
                    new JProperty("updated_Date", subcat.Updated_date),
                    new JProperty("Status", subcat.cat_status)
                    );
                SubCat.Add(sc);
            }
            JObject final = new JObject(
                new JProperty("Subcategory_List", SubCat));
            return final;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/admin/GetBrand/{SCatID}")]
        public JObject GetBrand(int SCatID)
        {
            int cat_id = SCatID;
            var SubCatProduct = context.Products.AsNoTracking().Where(a=>a.prod_status==1 && a.prod_subcategory==SCatID);
            JArray array = new JArray();
            foreach (Product pro in SubCatProduct)
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

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/admin/GetBrandDetails/{BID}")]
        public JObject GetBrandDetails(int BID)
        {
            int Brand_ID = BID;
            var Bdetails = context.Product_items.AsNoTracking().Where(a => a.item_status == 1 && a.prod_id == Brand_ID);
            JArray Brand_details = new JArray();
            JArray Brand_details_Spec = new JArray();
            foreach (Product_items Bitems in Bdetails)
            {
                JObject Details = new JObject(
                    new JProperty("Item_id", Bitems.id),
                    new JProperty("product_id", Bitems.prod_id),
                    new JProperty("item_code", Bitems.item_code),
                    new JProperty("Created_date", Bitems.Created_date),
                    new JProperty("Updated_date", Bitems.Updated_date)
                    );
                Brand_details.Add(Details);

                JObject Spec = new JObject(
                    new JProperty("Item_id", Bitems.id),
                    new JProperty("product_id", Bitems.prod_id),
                    new JProperty("item_spec", Bitems.item_spec+" "+ Bitems.item_unit),
                    new JProperty("item_mrp", Bitems.item_mrp),
                    new JProperty("item_selling", Bitems.item_selling),
                    new JProperty("item_stock", Bitems.item_stock),
                    new JProperty("item_image", Bitems.item_image),
                    new JProperty("item_status", Bitems.item_status)
                    );
                Brand_details_Spec.Add(Spec);
            }


            JObject final = new JObject(
               new JProperty("Product_Details", Brand_details),
               new JProperty("Product_Spec", Brand_details_Spec)
               );

            return final;
        }

        [System.Web.Http.HttpPost]
        public int CreateCategory(JObject param)
        {
            //Get main cat or not chekc box kkanum then condition podanum
            int status = 0;
            string category_name = "";
            int category_parent = 0;
            string category_slug = "";
            string[] replaceables = new[] { "+", "!", "(", ")", "{", "}", "[", "]", "^", "~", "*", "?", ":", "\\", "\"", " ", "-", ";", "&", "|", "/", "_", "@", "#", "$", "%", "<", ">", "=" };
            int count = 0;
            Random random = new Random();
            try
            {
                category_name = (string)param.GetValue("categoryname");
                category_parent = (int)param.GetValue("category_parent");
                category_slug= (string)param.GetValue("categoryslug");

                for (int i = 0; i < replaceables.Length; i++)
                {
                    category_slug = category_slug.Replace(replaceables[i], String.Empty);
                }
                count = context.Product_Category.AsNoTracking().Where(a => a.cat_slug == category_slug).Count();
                if (count != 0)
                {
                    category_slug = category_slug + "-" + random.Next(0, 1000).ToString();
                }

                Product_Category cat = new Product_Category();
                cat.cat_name = category_name;
                cat.cat_parent = category_parent;
                cat.cat_slug = category_slug;
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
        public int CreateProduct(JObject param)
        {

            int status = 0;
            string product_name = "";
            int product_category = 0;
            string product_brand = "";
            string product_desc = "";
            string product_slug = "";
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
              
                
                Product pro = new Product();
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
                context.Products.Add(pro);
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
