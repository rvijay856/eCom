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
    
    public partial class Product_Category
    {
        public int id { get; set; }
        public string cat_name { get; set; }
        public string cat_slug { get; set; }
        public Nullable<int> cat_parent { get; set; }
        public Nullable<int> cat_status { get; set; }
        public Nullable<System.DateTime> Created_date { get; set; }
        public Nullable<System.DateTime> Updated_date { get; set; }
    }
}