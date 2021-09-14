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
    public class BackOrderStatusController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: BackOrderStatus
        public ActionResult Index()
        {
            return View(db.BackOrderStatus.ToList());
        }

        // GET: BackOrderStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BackOrderStatu backOrderStatu = db.BackOrderStatus.Find(id);
            if (backOrderStatu == null)
            {
                return HttpNotFound();
            }
            return View(backOrderStatu);
        }

        // GET: BackOrderStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BackOrderStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BackOrderStatusID,BackOrderDescription")] BackOrderStatu backOrderStatu)
        {
            if (ModelState.IsValid)
            {
                BackOrderStatu _backorderStatus = db.BackOrderStatus.Where(x => x.BackOrderDescription == backOrderStatu.BackOrderDescription).FirstOrDefault();
                if (_backorderStatus == null)
                {
                    db.BackOrderStatus.Add(backOrderStatu);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_backorderStatus.BackOrderDescription == backOrderStatu.BackOrderDescription)
                {
                    TempData["ErrorMessage"] = "Back Order Status Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.BackOrderStatus.Add(backOrderStatu);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }


            }

            return View(backOrderStatu);
        }

        // GET: BackOrderStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BackOrderStatu backOrderStatu = db.BackOrderStatus.Find(id);
            if (backOrderStatu == null)
            {
                return HttpNotFound();
            }
            return View(backOrderStatu);
        }

        // POST: BackOrderStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BackOrderStatusID,BackOrderDescription")] BackOrderStatu backOrderStatu)
        {
            if (ModelState.IsValid)
            {
                BackOrderStatu _backorderStatus = db.BackOrderStatus.Where(x => x.BackOrderDescription == backOrderStatu.BackOrderDescription).FirstOrDefault();

                if (_backorderStatus == null)
                {
                    db.Entry(backOrderStatu).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
                else if (_backorderStatus.BackOrderDescription == backOrderStatu.BackOrderDescription)
                {
                    TempData["ErrorMessage"] = "No Changes Made";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(backOrderStatu).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }

            }
            return View(backOrderStatu);
        }

        // GET: BackOrderStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            BackOrderStatu backorderstatus = db.BackOrderStatus.Find(id);
            BackOrder _backorder = db.BackOrders.Where(x => x.BackOrderStatusID == backorderstatus.BackOrderStatusID).FirstOrDefault();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (_backorder != null)
            {
                TempData["ErrorMessage"] = "A Back Order has this status. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else if (_backorder == null)
            {
                db.BackOrderStatus.Remove(backorderstatus);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                db.BackOrderStatus.Remove(backorderstatus);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: BackOrderStatus/Delete/5
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
