//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AutobuyDirectApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order_details
    {
        public int id { get; set; }
        public Nullable<int> order_id { get; set; }
        public string order_no { get; set; }
        public Nullable<int> product_id { get; set; }
        public Nullable<int> item_id { get; set; }
        public Nullable<int> prod_quantity { get; set; }
        public Nullable<decimal> mrp_price { get; set; }
        public Nullable<decimal> selling_price { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
    }
}