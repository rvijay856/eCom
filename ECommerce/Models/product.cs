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
    
    public partial class product
    {
        public long product_id { get; set; }
        public string product_name { get; set; }
        public string product_category { get; set; }
        public decimal product_unit_qty { get; set; }
        public string product_unit { get; set; }
        public int status { get; set; }
        public decimal orignal_price { get; set; }
        public string product_description { get; set; }
        public decimal sale_price { get; set; }
        public string shipping_class { get; set; }
        public string stock { get; set; }
    }
}