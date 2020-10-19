using AutobuyDirectApi.Models;
using EcomErrorLog;
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
    public class OrderController : ApiController
    {
        EcommEntities context = new EcommEntities();

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Order/GetOrder/{cust_id}")]
        public JObject GetOrder(int cust_id)
        {
            int cus_id = cust_id;
            var Pro_Order = context.Products_order.AsNoTracking().Where(a => a.cust_id == cus_id && a.order_status == 1);
            JArray Order_array = new JArray();
            foreach (Products_order Prod_Order in Pro_Order)
            {
                JObject Order = new JObject(
                    new JProperty("id", Prod_Order.id),
                    new JProperty("Basket_session", Prod_Order.basket_session),
                    new JProperty("Show_id", Prod_Order.show_id),
                    new JProperty("cust_id", Prod_Order.cust_id),
                    new JProperty("Total_amount", Prod_Order.total_amount),
                    new JProperty("Pay_Status", Prod_Order.pay_status),
                    new JProperty("Cancel_reason", Prod_Order.cancel_reason),
                    new JProperty("Shipping_notes", Prod_Order.shipping_notes),
                    new JProperty("Delivery_boy_id", Prod_Order.delivery_boy_id),
                    new JProperty("Order_status", Prod_Order.order_status),
                    new JProperty("Confirmed_date", Prod_Order.Confirmed_date),
                    new JProperty("Packed_date", Prod_Order.packed_date),
                    new JProperty("Shipped_date", Prod_Order.shipped_date),
                    new JProperty("Delivered_date", Prod_Order.delivered_date),
                    new JProperty("Cancelled_date", Prod_Order.cancelled_date),
                    new JProperty("Created_date", Prod_Order.Created_date),
                    new JProperty("Updated_date", Prod_Order.Updated_date)

                    );
                Order_array.Add(Order);
            }
            JObject final = new JObject(
               new JProperty("Order", Order_array));

            return final;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Order/GetOrderList/{cust_id}")]
        public JObject GetOrderList(int cust_id)
        {
            int cus_id = cust_id;
            var Pro_Order_List = context.Products_order_list.AsNoTracking().Where(a => a.cust_id == cus_id && a.order_list_status == 1);
            JArray OrderList_array = new JArray();
            foreach (Products_order_list Prod_Order_List in Pro_Order_List)
            {
                JObject Order_List = new JObject(
                    new JProperty("id", Prod_Order_List.id),
                    new JProperty("order_id", Prod_Order_List.order_id),
                    new JProperty("product_id", Prod_Order_List.product_id),
                    new JProperty("product_item_id", Prod_Order_List.product_item_id),
                    new JProperty("cust_id", Prod_Order_List.cust_id),
                    new JProperty("product_amount_sell", Prod_Order_List.product_amount_sell),
                    new JProperty("product_amount_mrp", Prod_Order_List.product_amount_mrp),
                    new JProperty("order_list_status", Prod_Order_List.order_list_status),
                    new JProperty("product_details", Prod_Order_List.product_details),
                    new JProperty("product_category", Prod_Order_List.product_category),
                    new JProperty("product_quantity", Prod_Order_List.product_quantity),
                    new JProperty("product_total_amount", Prod_Order_List.product_total_amount),
                    new JProperty("Created_date", Prod_Order_List.Created_date),
                    new JProperty("Updated_date", Prod_Order_List.Updated_date)

                    );
                OrderList_array.Add(Order_List);
            }
            JObject final = new JObject(
               new JProperty("OrderList", OrderList_array));

            return final;
        }
    }
}
