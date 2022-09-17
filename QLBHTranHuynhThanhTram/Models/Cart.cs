using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBHTranHuynhThanhTram.Models
{
    public class Cart
    {
        private NWDataContext da =new NWDataContext();
        public int ProductID { get; set; }
        public string ProductName { get; set; } 
        public decimal UnitPrice { get; set; }  
        public int Quantity { get; set; }
        public decimal Total { get { return UnitPrice * Quantity; } }
        public Cart(int productID)
        {
            this.ProductID = productID;
            Product p = da.Products.Single(n => n.ProductID == productID);
            ProductName = p.ProductName;
            UnitPrice = (decimal)p.UnitPrice;
            Quantity = 1;
        }
    }
}