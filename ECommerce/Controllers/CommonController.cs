using AutobuyDirectApi.Models;
using EcomErrorLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections;
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
                    new JProperty("category_image", subcat.cat_img),
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
        [System.Web.Http.Route("api/Common/GetBrandDetails/{ITID}")]
        public JObject GetBrandDetails(int ITID)
        {
            int item_ID = ITID;
            int cust_id = 0;
            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["User_id"];

            var Bdetails = context.Product_items.AsNoTracking().Where(a => a.item_status == 1 && a.id == item_ID);
            JArray Brand_details = new JArray();
            JArray Brand_details_Spec = new JArray();
            foreach (Product_items Bitems in Bdetails)
            {
                JObject Details = new JObject(
                    new JProperty("Item_id", Bitems.id),
                    new JProperty("product_id", Bitems.prod_id),
                    new JProperty("product_Name", context.Products.AsNoTracking().Where(a=>a.prod_id==Bitems.prod_id).Select(a=>a.prod_name).Single()),
                    new JProperty("product_Desc", context.Products.AsNoTracking().Where(a => a.prod_id == Bitems.prod_id).Select(a => a.prod_desc).Single()),
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
            JArray Productitemarray = new JArray();
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
                    Productitemarray = new JArray();
                    var item = context.Product_items.AsNoTracking().Where(a => a.prod_id == pro.prod_id);
                    foreach (Product_items pi in item)
                    {
                        JObject it = new JObject(
                             new JProperty("id", pi.id),
                             new JProperty("item_image", pi.item_image),
                             new JProperty("item_mrp", pi.item_mrp),
                             new JProperty("item_selling", pi.item_selling),
                             new JProperty("item_spec", pi.item_spec),
                             new JProperty("item_status", pi.item_status),
                             new JProperty("item_stock", pi.item_stock),
                             new JProperty("item_unit", pi.item_unit),
                             new JProperty("prod_id", pi.prod_id)
                             );
                        Productitemarray.Add(it);
                    }

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
                        new JProperty("Updated_date", pro.Updated_date),
                        new JProperty("Productitemarray", Productitemarray)
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

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Common/GetBrandList")]
        public JObject GetBrandList()
        {
            JArray brandlist = new JArray();
            JArray Fbrandlist = new JArray();
            JObject final = new JObject();
            JObject Brand_Lis = new JObject();
            var BL = context.Brand_Menu.AsNoTracking().Where(a => a.Brand_Status == 1).Select(a => a.Barnd_Section).Distinct();

            foreach(string bli in BL)
            {
                
                var BLD = context.Brand_Menu.AsNoTracking().Where(a => a.Barnd_Section == bli);
                brandlist = new JArray();
                foreach(Brand_Menu br in BLD)
                {
                    JObject Brand_List = new JObject(
                        new JProperty("Brand_Img", br.Brand_Img),
                        new JProperty("Brand_Order", br.Brand_Order),
                        new JProperty("Brand_Status", br.Brand_Status),
                        new JProperty("Brand_Title", br.Brand_Title),
                        new JProperty("Id", br.Id),
                        new JProperty("Updated_Date", br.Updated_Date),
                        new JProperty("Created_Date", br.Created_Date)
                    );
                    brandlist.Add(Brand_List);
                }
                    Brand_Lis = new JObject(
                        new JProperty("Barnd_Section", bli),
                        new JProperty("brandLi", brandlist)
                        );
                Fbrandlist.Add(Brand_Lis);
            }
            final = new JObject(
                    new JProperty("brandList", Fbrandlist));

            return final;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Common/GetBrandItemList/{BID}")]
        public JObject GetBrandItemList(int BID)
        {
            int cust_id = 0;
            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["User_id"];

            JObject final = new JObject();
            JArray branditemlist = new JArray();
            JObject bo = new JObject();
            JObject it = new JObject();
            JArray array = new JArray();
            JArray items = new JArray();

            var BList = context.Brand_Product.AsNoTracking().Where(a => a.Brand_id == BID);

            foreach (Brand_Product BI in BList)
            {
                var SubCatProduct = context.Products.AsNoTracking().Where(a => a.prod_status == 1 && a.prod_id == BI.Prod_id);

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
            }

                final = new JObject(
                   new JProperty("brandItemList", array));        

            return final;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Common/GetCarouselList")]
        public JObject GetCarouselList()
        {
            JArray Carousellist = new JArray();
            JObject final = new JObject();

            var CL = context.Carousel_Menu.AsNoTracking().Where(a => a.Carousel_Status == 1);

            foreach (Carousel_Menu bli in CL)
            {
                JObject Brand_List = new JObject(
                    new JProperty("Carousel_Section", bli.Carousel_Section),
                    new JProperty("Carousel_Img", bli.Carousel_Img),
                    new JProperty("Carousel_Order", bli.Carousel_Order),
                    new JProperty("Carousel_Status", bli.Carousel_Status),
                    new JProperty("Carousel_Title", bli.Carousel_Title),
                    new JProperty("Id", bli.Id),
                    new JProperty("Updated_Date", bli.Updated_Date),
                    new JProperty("Created_Date", bli.Created_Date)
                    );
                Carousellist.Add(Brand_List);
            }
            final = new JObject(
                    new JProperty("CarouselList", Carousellist));

            return final;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Common/GetCarouselItemList/{CID}")]
        public JObject GetCarouselItemList(int CID)
        {
            int cust_id = 0;
            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["User_id"];

            JObject final = new JObject();
            JArray branditemlist = new JArray();
            JObject bo = new JObject();
            JObject it = new JObject();
            JArray array = new JArray();
            JArray items = new JArray();

            var CList = context.Carousel_Product.AsNoTracking().Where(a => a.Carousel_id == CID);

            foreach (Carousel_Product CI in CList)
            {
                var SubCatProduct = context.Products.AsNoTracking().Where(a => a.prod_status == 1 && a.prod_id == CI.Prod_id);

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
            }

            final = new JObject(
               new JProperty("CarouselItemList", array));

            return final;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Common/GetTrendingList")]
        public JObject GetTrendingList()
        {
            int cust_id = 0;
            InitController Login = new InitController();

            JObject param = Login.Login();

            string json = JsonConvert.SerializeObject(param);

            cust_id = (int)JObject.Parse(json)["User_id"];

            JArray Trendinglist = new JArray();
            JObject final = new JObject();
            JObject bo = new JObject();
            JObject it = new JObject();
            JArray array = new JArray();
            JArray items = new JArray();
            JArray Title = new JArray();
            JObject Titleobj = new JObject();

            var Trending_Id = context.Trending_Menu.AsNoTracking().Where(a => a.Trending_Status == 1);
            foreach (Trending_Menu TM in Trending_Id)
            {
                var trend_pro = context.Trending_Product.AsNoTracking().Where(a => a.Trending_id == TM.Id);

                foreach (Trending_Product TP in trend_pro)
                {
                    var SubCatProduct = context.Products.AsNoTracking().Where(a => a.prod_status == 1 && a.prod_id == TP.Prod_id);

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
                }

                Titleobj = new JObject(
                    new JProperty("Trending_Title", TM.Trending_Title),
                    new JProperty("Trending_Deatils", array)
                    );
                Title.Add(Titleobj);
            }
            final = new JObject(
               //new JProperty("TrendingItemList", array),
               new JProperty("Trending_Title", Title)
               );

            return final;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Common/GetProduct")]
        public JObject GetProduct(string pro)
        {
            
            var ProductList = context.Products.AsNoTracking().Where(a => a.prod_name.Contains(pro) && a.prod_status==1);
            JArray ProductList_arr = new JArray();
            ArrayList arr = new ArrayList();
            foreach (Product pr in ProductList)
            {
                if (!arr.Contains(pr.prod_id))
                {
                    JObject Pur = new JObject(new JProperty("id", pr.prod_id), 
                                  new JProperty("text", pr.prod_name)
                                  );
                    ProductList_arr.Add(Pur);
                    arr.Add(pr.prod_id);
                }
            }
            JObject rs = new JObject(
                new JProperty("Product_List", ProductList_arr));
            return rs;
        }

    }
}
