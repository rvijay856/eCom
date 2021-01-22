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
    public class CommonController : ApiController
    {
        EcommEntities context = new EcommEntities();

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Common/GetCategory")]
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
        [System.Web.Http.Route("api/Common/GetSubcategory/{CatID}")]
        public JObject GetSubcategory(int CatID)
        {
            int cat_id = CatID;
            var subcategory = context.Product_Category.AsNoTracking().Where(a => a.cat_status == 1 && a.cat_parent == CatID);
            JArray SubCat = new JArray();
            foreach (Product_Category subcat in subcategory)
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
        [System.Web.Http.Route("api/Common/GetBrand/{SCatID}")]
        public JObject GetBrand(int SCatID)
        {
            int cat_id = SCatID;
            int cust_id = 0;
            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["User_id"];

            JObject bo = new JObject();
            JObject it = new JObject();
            var SubCatProduct = context.Products.AsNoTracking().Where(a => a.prod_status == 1 && a.prod_subcategory == SCatID);
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
                        new JProperty("iswishlist", context.Wishlists.AsNoTracking().Where(a => a.item_id == pi.id && a.cust_id == cust_id).Count())
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
                   new JProperty("Item_spec", items)
                   );
                array.Add(bo);
            }
            JObject final = new JObject(
               new JProperty("Product_Details", array));

            return final;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Common/GetBrandDetails/{BID}")]
        public JObject GetBrandDetails(int BID)
        {
            int Brand_ID = BID;
            int cust_id = 0;
            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["User_id"];

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
                    new JProperty("item_spec", Bitems.item_spec + " " + Bitems.item_unit),
                    new JProperty("item_mrp", Bitems.item_mrp),
                    new JProperty("item_selling", Bitems.item_selling),
                    new JProperty("item_stock", Bitems.item_stock),
                    new JProperty("item_image", Bitems.item_image),
                    new JProperty("item_status", Bitems.item_status),
                    new JProperty("iswishlist", context.Wishlists.AsNoTracking().Where(a => a.item_id == Bitems.prod_id && a.cust_id == cust_id).Count())
                    );
                Brand_details_Spec.Add(Spec);
            }


            JObject final = new JObject(
               new JProperty("Product_Details", Brand_details),
               new JProperty("Product_Spec", Brand_details_Spec)
               );

            return final;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Common/GetAdminCategory")]
        public JObject GetAdminCategory()
        {
            JArray ParentCatarray = new JArray();
            JArray SubCatarray = new JArray();
            JArray Productarray = new JArray();
            try
            {
                var Category = context.Product_Category.AsNoTracking().Where(a => a.cat_parent == 0);
                var SubCategory = context.Product_Category.AsNoTracking().Where(a => a.cat_parent != 0);
                var Product = context.Products.AsNoTracking();

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
            }
            catch (Exception e)
            {
                Logdetails.LogError("Post Error", "GetAdminCategoryerror admincontroller (362)", e.Message);
            }
            JObject final = new JObject(
            new JProperty("ParentCategory_Details", ParentCatarray),
            new JProperty("SubCatarray_Details", SubCatarray),
            new JProperty("Product_Details", Productarray)
            );
            return final;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Common/GetPostalCodeValidation/{Post_Code}")]
        public JObject GetPostalCodeValidation(int Post_Code)
        {
            JArray PostList = new JArray();
            JObject final = new JObject();
            string PostCode = Post_Code.ToString();
            try
            {
                int Post_code = context.Postal_Code.Where(a => a.area_code == PostCode && a.status == 1).Count();
                var Postal = context.Postal_Code.Where(a => a.area_code == PostCode && a.status == 1);
                if (Post_code != 0)
                {
                    foreach (Postal_Code PC in Postal)
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
