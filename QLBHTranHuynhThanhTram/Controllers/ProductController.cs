using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBHTranHuynhThanhTram.Models;

namespace QLBHTranHuynhThanhTram.Controllers
{
    public class ProductController : Controller
    {
        NWDataContext da = new NWDataContext();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
        //Hien thi danh sach SP
        public ActionResult ListProduct()
        {
            //Lay danh sach SP
            List<Product> dssp = da.Products.Select(s=>s).ToList();
            return View(dssp);
        }
        //Hien thi chi tiet 1 SP
        public ActionResult Details(int id)
        {
            Product p = da.Products.Where(s => s.ProductID == id).FirstOrDefault();
            return View(p);
        }
        //Them 1 SP
        public ActionResult Create()
        {
            //Lay danh sach loai SP
            //Lay danh sach NCC
            ViewData["LSP"] = new SelectList(da.Categories, "CategoryID", "CategoryName");
            ViewData["NCC"] = new SelectList(da.Suppliers, "SupplierID", "CompanyName");
            return View();
        }
        //Xu ly them
        [HttpPost]
        public ActionResult Create(FormCollection collection, Product p)
        {

            //Xu ly them
            var tenSP = collection["ProductName"];//tenSP la bien nguoi dung truyen vao//kiem tra rong
            if (String.IsNullOrEmpty(tenSP))//neu tenSP chua duoc truyen vao (tenSP trong) thi hien thi thong bao
            {
                ViewData["Loi"] = "Tên sản phẩm không được rỗng"; 
            }
            else//ten SP duoc truyen vao thicho phep sua
            {
                p.CategoryID = int.Parse(collection["LSP"]);
                p.SupplierID = int.Parse(collection["NCC"]);
                da.Products.InsertOnSubmit(p);
                da.SubmitChanges();
                return RedirectToAction("ListProduct");
            }
            return this.Create();//khong them
        }
        //Xoa 1 SP
        public ActionResult Delete(int id)
        {
            //Hien thi SP muon xoa
            Product p = da.Products.FirstOrDefault(s => s.ProductID == id);
            return View(p);
        }
        //Xu ly xoa
        [HttpPost]
        public ActionResult Delete(FormCollection collection, int id)
        {
            //Xu ly xoa
            Product p = da.Products.FirstOrDefault(s => s.ProductID == id);//hien thi sap muon xoa
            da.Products.DeleteOnSubmit(p);//lay sp do xoa
            da.SubmitChanges();//cap nhat lai
            return RedirectToAction("ListProduct");
        }
        //Sua 1 SP
        public ActionResult Edit(int id)
        {
            //Hien thi SP muon sua
            Product p = da.Products.FirstOrDefault(s => s.ProductID == id);
            ViewData["LSP"] = new SelectList(da.Categories, "CategoryID", "CategoryName");
            ViewData["NCC"] = new SelectList(da.Suppliers, "SupplierID", "CompanyName");
            return View(p);
        }
        //Xu ly sua
        [HttpPost]
        public ActionResult Edit(FormCollection collection, int id)
        {
            //Xu ly sua
            Product p = da.Products.FirstOrDefault(s => s.ProductID == id);//hien thi sap muon sua
            p.ProductName = collection["ProductName"];
            p.CategoryID = int.Parse(collection["LSP"]);
            p.UnitPrice = decimal.Parse(collection["UnitPrice"]);
            UpdateModel(p);//cap nhat SP muon sua
            da.SubmitChanges();//cap nhat lai
            return RedirectToAction("ListProduct");
        }
    }
}