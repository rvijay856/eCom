﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EcommEntities : DbContext
    {
        public EcommEntities()
            : base("name=EcommEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Brand_Menu> Brand_Menu { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<customer_address> customer_address { get; set; }
        public virtual DbSet<Customer_Payment> Customer_Payment { get; set; }
        public virtual DbSet<Postal_Code> Postal_Code { get; set; }
        public virtual DbSet<Product_Category> Product_Category { get; set; }
        public virtual DbSet<Product_items> Product_items { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<user_details> user_details { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }
        public virtual DbSet<Brand_Product> Brand_Product { get; set; }
        public virtual DbSet<Carousel_Menu> Carousel_Menu { get; set; }
        public virtual DbSet<Carousel_Product> Carousel_Product { get; set; }
        public virtual DbSet<Trending_Menu> Trending_Menu { get; set; }
        public virtual DbSet<Trending_Product> Trending_Product { get; set; }
        public virtual DbSet<Order_details> Order_details { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
    }
}
