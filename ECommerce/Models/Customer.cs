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
    
    public partial class Customer
    {
        public int cust_id { get; set; }
        public string cust_name { get; set; }
        public string cust_email { get; set; }
        public string cust_mobile { get; set; }
        public Nullable<int> cust_type { get; set; }
        public Nullable<int> cust_status { get; set; }
        public Nullable<int> cust_address_id { get; set; }
        public Nullable<int> cust_pincode { get; set; }
        public string cust_otp { get; set; }
        public string cat_password { get; set; }
        public Nullable<System.DateTime> Created_date { get; set; }
        public Nullable<System.DateTime> Updated_date { get; set; }
        public Nullable<System.DateTime> Last_login { get; set; }
    }
}
