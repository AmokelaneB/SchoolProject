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
    public class OrderStatusController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: OrderStatus
        public ActionResult Index()
        {
            return View(db.OrderStatus.ToList());
        }

        // GET: OrderStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderStatu orderStatu = db.OrderStatus.Find(id);
            if (orderStatu == null)
            {
                return HttpNotFound();
            }
            return View(orderStatu);
        }

        // GET: OrderStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderStatusID,DESCRIPTION")] OrderStatu orderStatu)
        {
            if (ModelState.IsValid)
            {
                OrderStatu _orderStatus = db.OrderStatus.Where(x => x.DESCRIPTION == orderStatu.DESCRIPTION).FirstOrDefault();
                if (_orderStatus == null)
                {
                    db.OrderStatus.Add(orderStatu);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_orderStatus.DESCRIPTION == orderStatu.DESCRIPTION)
                {
                    TempData["ErrorMessage"] = "Order Status Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.OrderStatus.Add(orderStatu);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
            }

            return View(orderStatu);
        }

        // GET: OrderStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderStatu orderStatu = db.OrderStatus.Find(id);
            if (orderStatu == null)
            {
                return HttpNotFound();
            }
            return View(orderStatu);
        }

        // POST: OrderStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderStatusID,DESCRIPTION")] OrderStatu orderStatu)
        {
            if (ModelState.IsValid)
            {
                OrderStatu _ordertstatus = db.OrderStatus.Where(x => x.DESCRIPTION == orderStatu.DESCRIPTION).FirstOrDefault();
                if (_ordertstatus == null)
                {
                    db.Entry(orderStatu).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
                else if (_ordertstatus != null)
                {
                    TempData["ErrorMessage"] = "No changes were made.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(orderStatu).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
            }
            return View(orderStatu);
        }

        // GET: OrderStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            OrderStatu orderStatus = db.OrderStatus.Find(id);
            SupplierOrder _supplierorder = db.SupplierOrders.Where(x => x.OrderStatusID == orderStatus.OrderStatusID).FirstOrDefault();
            OnlineOrder _onlineorder = db.OnlineOrders.Where(x => x.OrderStatusID == orderStatus.OrderStatusID).FirstOrDefault();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (_supplierorder != null)
            {
                TempData["ErrorMessage"] = "A Suplier Order has this Order Status. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else if (_onlineorder != null)
            {
                TempData["ErrorMessage"] = "An Online Order has this Order Status. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else
            {
                db.OrderStatus.Remove(orderStatus);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: OrderStatus/Delete/5
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
