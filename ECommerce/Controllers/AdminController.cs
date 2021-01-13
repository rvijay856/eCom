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
                    new JProperty("category_img", cat.cat_img),
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
            JObject bo = new JObject();
            JObject it = new JObject();
            var SubCatProduct = context.Products.AsNoTracking().Where(a=>a.prod_status==1 && a.prod_subcategory==SCatID);
            JArray array = new JArray();
            JArray items = new JArray();
            foreach (Product pro in SubCatProduct)
            {
                var item = context.Product_items.AsNoTracking().Where(a => a.prod_id == pro.prod_id);
                items = new JArray();
                foreach (Product_items pi in item)
                {
                    it = new JObject(
                        new JProperty("id", pi.id),
                        new JProperty("item_image", pi.item_image),
                        new JProperty("item_mrp", pi.item_mrp),
                        new JProperty("item_selling", pi.item_selling),
                        new JProperty("item_spec", pi.item_spec),
                        new JProperty("item_status", pi.item_status),
                        new JProperty("item_stock", pi.item_stock),
                        new JProperty("item_unit", pi.item_unit),
                        new JProperty("prod_id", pi.prod_id),
                        new JProperty("iswishlist", context.Wishlists.AsNoTracking().Where(a => a.item_id == pi.id).Count())
                        );
                    items.Add(it);
                }

                 bo = new JObject(
                    new JProperty("product_id", pro.prod_id),
                    new JProperty("product_name", pro.prod_name),
                    new JProperty("product_slug", pro.prod_slug),
                    new JProperty("product_category", pro.prod_category),
                    new JProperty("product_brand", pro.prod_brand),
                    new JProperty("product_desc", pro.prod_desc),
                    new JProperty("product_sub_category", pro.prod_subcategory),
                    new JProperty("product_created_date", pro.Created_date),
                    new JProperty("product_updated_date", pro.Updated_date),
                    new JProperty("Status", pro.prod_status),
                    new JProperty("Item_spec",items)
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
                    new JProperty("item_status", Bitems.item_status),
                    new JProperty("iswishlist",context.Wishlists.AsNoTracking().Where(a=>a.item_id== Bitems.prod_id).Count())
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
                else if(category_type == 2)
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
            string unit="";
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
                else if (Pro_count!=0 & proitem_count==0)
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

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/admin/GetAdminCategory")]
        public JObject GetAdminCategory()
        {
            var Category = context.Product_Category.AsNoTracking().Where(a=>a.cat_parent==0);
            var SubCategory = context.Product_Category.AsNoTracking().Where(a => a.cat_parent != 0);
            var Product = context.Products.AsNoTracking();
            JArray ParentCatarray = new JArray();
            JArray SubCatarray = new JArray();
            JArray Productarray = new JArray();

            foreach (Product_Category cat in Category)
            {
                JObject bo = new JObject(
                    new JProperty("category_id", cat.id),
                    new JProperty("category_name", cat.cat_name),
                    new JProperty("category_parent", cat.cat_parent),
                    new JProperty("category_slug", cat.cat_slug),
                    new JProperty("category_img", cat.cat_img),
                    new JProperty("Created_Date", cat.Created_date),
                    new JProperty("updated_Date", cat.Updated_date),
                    new JProperty("Status", cat.cat_status)
                    );
                ParentCatarray.Add(bo);
            }
            foreach (Product_Category sub in SubCategory)
            {
                JObject su = new JObject(
                    new JProperty("category_id", sub.id),
                    new JProperty("category_name", sub.cat_name),
                    new JProperty("category_parent", sub.cat_parent),
                    new JProperty("category_slug", sub.cat_slug),
                    new JProperty("category_img", sub.cat_img),
                    new JProperty("Created_Date", sub.Created_date),
                    new JProperty("updated_Date", sub.Updated_date),
                    new JProperty("Status", sub.cat_status)
                    );
                SubCatarray.Add(su);
            }
            foreach (Product pro in Product)
            {
                JObject po = new JObject(
                    new JProperty("prod_id", pro.prod_id),
                    new JProperty("prod_name", pro.prod_name),
                    new JProperty("prod_slug", pro.prod_slug),
                    new JProperty("prod_category", pro.prod_category),
                    new JProperty("prod_subcategory", pro.prod_subcategory),
                    new JProperty("prod_brand", pro.prod_brand),
                    new JProperty("prod_desc", pro.prod_desc),
                    new JProperty("prod_status", pro.prod_status),
                    new JProperty("Created_date", pro.Created_date),
                    new JProperty("Updated_date", pro.Updated_date)
                    );
                Productarray.Add(po);
            }
            JObject final = new JObject(
               new JProperty("ParentCategory_Details", ParentCatarray),
               new JProperty("SubCatarray_Details", SubCatarray),
               new JProperty("Product_Details", Productarray)
               );
            return final;
        }
        
    }
}
