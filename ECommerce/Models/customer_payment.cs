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
    
    public partial class Customer_Payment
    {
        public int id { get; set; }
        public int customer_id { get; set; }
        public string Card_no { get; set; }
        public string Card_type { get; set; }
        public string Expiration_date { get; set; }
        public string Security_code { get; set; }
        public string CardHolderName { get; set; }
        public Nullable<int> payment_status { get; set; }
        public Nullable<int> card_status { get; set; }
        public System.DateTime created_date { get; set; }
        public string payment_type { get; set; }
        public decimal payment_amount { get; set; }
        public Nullable<System.DateTime> payment_date { get; set; }
    }
}
