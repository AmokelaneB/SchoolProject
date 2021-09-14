using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GradStockUp.Models;

namespace GradStockUp.Controllers
{
    public class BackOrderController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: BackOrders
        public ActionResult Index()
        {
            var backOrders = db.BackOrders.Include(b => b.BackOrderStatu).Include(b => b.SupplierOrder);
            return View(backOrders.ToList());
        }

        // GET: BackOrders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BackOrder backOrder = db.BackOrders.Find(id);
            if (backOrder == null)
            {
                return HttpNotFound();
            }
            return View(backOrder);
        }

        // GET: BackOrders/Create
        public ActionResult Create()
        {
            ViewBag.BackOrderStatusID = new SelectList(db.BackOrderStatus, "BackOrderStatusID", "BackOrderDescription");
            ViewBag.SupplierOrderID = new SelectList(db.SupplierOrders, "SupplierOrderID", "SupplierOrderID");
            return View();
        }

        // POST: BackOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BackOrderID,BackOrderStatusID,SupplierOrderID,QuantityOutstanding")] BackOrder backOrder)
        {
            if (ModelState.IsValid)
            {
                db.BackOrders.Add(backOrder);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Saved Successfully";
                return RedirectToAction("Index");
            }

            ViewBag.BackOrderStatusID = new SelectList(db.BackOrderStatus, "BackOrderStatusID", "BackOrderDescription", backOrder.BackOrderStatusID);
            ViewBag.SupplierOrderID = new SelectList(db.SupplierOrders, "SupplierOrderID", "SupplierOrderID", backOrder.SupplierOrderID);
            return View(backOrder);
        }

        // GET: BackOrders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BackOrder backOrder = db.BackOrders.Find(id);
            if (backOrder == null)
            {
                return HttpNotFound();
            }
            ViewBag.BackOrderStatusID = new SelectList(db.BackOrderStatus, "BackOrderStatusID", "BackOrderDescription", backOrder.BackOrderStatusID);
            ViewBag.SupplierOrderID = new SelectList(db.SupplierOrders, "SupplierOrderID", "SupplierOrderID", backOrder.SupplierOrderID);
            return View(backOrder);
        }

        // POST: BackOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BackOrderID,BackOrderStatusID,SupplierOrderID,QuantityOutstanding")] BackOrder backOrder)
        {
            if (ModelState.IsValid)
            {

                db.Entry(backOrder).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Updated Successfully";
                return RedirectToAction("Index");
            }
            ViewBag.BackOrderStatusID = new SelectList(db.BackOrderStatus, "BackOrderStatusID", "BackOrderDescription", backOrder.BackOrderStatusID);
            ViewBag.SupplierOrderID = new SelectList(db.SupplierOrders, "SupplierOrderID", "SupplierOrderID", backOrder.SupplierOrderID);
            return View(backOrder);
        }

        // GET: BackOrders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                BackOrder backOrder = db.BackOrders.Find(id);
                db.BackOrders.Remove(backOrder);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //POST: BackOrders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{

        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
