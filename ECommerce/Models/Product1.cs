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
    
    public partial class Product1
    {
        public int prod_id { get; set; }
        public string prod_name { get; set; }
        public string prod_slug { get; set; }
        public Nullable<int> prod_category { get; set; }
        public Nullable<int> prod_subcategory { get; set; }
        public string prod_brand { get; set; }
        public string prod_desc { get; set; }
        public Nullable<int> prod_status { get; set; }
        public Nullable<System.DateTime> Created_date { get; set; }
        public Nullable<System.DateTime> Updated_date { get; set; }
    }
}