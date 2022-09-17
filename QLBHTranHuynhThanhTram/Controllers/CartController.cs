using QLBHTranHuynhThanhTram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;

namespace QLBHTranHuynhThanhTram.Controllers
{
    public class CartController : Controller
    {
        private NWDataContext da = new NWDataContext();
        // GET: Cart
        public List<Cart> GetListCarts() //Lấy DS giỏ hàng
        {
            List<Cart> carts = Session["Cart"] as List<Cart>;
            //Chưa có SP nào trong giỏ hàng
            if(carts == null)
            {
                carts = new List<Cart>(); //tạo mới giỏ hàng
                Session["Cart"] = carts;
            }
            return carts;
        }
        private int Count()
        {
            int n = 0;
            List<Cart> carts = Session["Cart"] as List<Cart>;
            if(carts != null)
            {
                n = carts.Sum(s => s.Quantity);
            }
            return n;
        }
        private decimal Total()
        {
            decimal total = 0;
            List<Cart> carts = Session["Cart"] as List<Cart>;
            if (carts != null)
            {
                total = carts.Sum(s => s.Total);
            }
            return total;
        }
        public ActionResult AddCart(int id)
        {
            //Kiểm tra giỏ hàng có SP chưa
            List<Cart> carts = GetListCarts(); // Lấy DS giỏ hàng
            Cart c = carts.Find(s => s.ProductID == id);
            //Giỏ hàng chưa có SP
            if (c == null)
            {
                c = new Cart(id);
                carts.Add(c); //Thêm SP vào giỏ hàng
            }
            else // đã có
            {
                c.Quantity++; //tăng số lượng
            }
            return RedirectToAction("ListCarts");
        }
        public ActionResult ListCarts() //hiển thị giỏ hàng
        {
            List<Cart> carts = GetListCarts();
            if (carts.Count == 0) //giỏ hàng chưa có SP
            {
                return RedirectToAction("ListProduct", "Product"); //trang DSSP
            }
            //ViewBag.CountProduct = Count();
            //ViewBag.Total = Total();

            ViewBag.CountProduct = carts.Sum(s => s.Quantity);
            ViewBag.Total = carts.Sum(s => s.Total); 

            return View(carts);
        }
        public ActionResult Delete(int id)
        {
            List<Cart> carts = GetListCarts(); //Lấy DS giỏ hàng
            Cart c = carts.Find(s => s.ProductID == id); //tìm SP muốn xóa trong gỏ hàng
            if(c!=null) //tìm thấy SP
            {
                carts.RemoveAll(s => s.ProductID == id); //xóa SP khỏi DS giỏ hàng
                return RedirectToAction("ListCarts");
            }
            if (carts.Count == 0)
            {
                return RedirectToAction("ListProduct", "Product");
            }
            return RedirectToAction("ListCarts");
        }
        public ActionResult OrderProduct(FormCollection formCollection)
        {
            using (TransactionScope transactionScope=new TransactionScope())
            {
                try
                {
                    Order order = new Order();
                    order.OrderDate = DateTime.Now;
                    da.Orders.InsertOnSubmit(order);
                    da.SubmitChanges();

                    List<Cart> carts = GetListCarts();
                    foreach(var item in carts)
                    {
                        Order_Detail d = new Order_Detail();
                        d.OrderID = order.OrderID;
                        d.ProductID = item.ProductID;
                        d.Quantity = short.Parse(item.Quantity.ToString());
                        d.UnitPrice = item.UnitPrice;
                        d.Discount = 0;

                        da.Order_Details.InsertOnSubmit(d);
                    }
                    da.SubmitChanges();
                    transactionScope.Complete();
                    Session["Cart"] = null;
                }
                catch
                {
                    transactionScope.Dispose();
                    return RedirectToAction("ListCarts");
                }
                return RedirectToAction("ListProduct", "Product");
            }
        }
        public ActionResult ListProduct()
        {
            return RedirectToAction("ListProduct", "Product");
        }
    }
}