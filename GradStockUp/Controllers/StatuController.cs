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
    public class StatusController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Status
        public ActionResult Index()
        {
            return View(db.Status.ToList());
        }

        // GET: Status/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Status status = db.Status.Find(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            return View(status);
        }

        // GET: Status/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Status/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockStatusID,StatusDescription")] Status status)
        {
            if (ModelState.IsValid)
            {
                Status _status = db.Status.Where(x => x.StatusDescription == status.StatusDescription).FirstOrDefault();
                if (_status == null)
                {
                    db.Status.Add(status);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_status.StatusDescription == status.StatusDescription)
                {
                    TempData["ErrorMessage"] = "Stock Status Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Status.Add(status);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
            }

            return View(status);
        }

        // GET: Status/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Status status = db.Status.Find(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            return View(status);
        }

        // POST: Status/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockStatusID,StatusDescription")] Status status)
        {
            if (ModelState.IsValid)
            {
                Status _status = db.Status.Where(x => x.StockStatusID == status.StockStatusID).FirstOrDefault();
                if (_status == null)
                {
                    db.Entry(status).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
                else if (_status != null)
                {
                    TempData["ErrorMessage"] = "No Changes Made.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(status).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                } 
            }
            return View(status);
        }

        // GET: Status/Delete/5
        public ActionResult Delete(int? id)
        {
            Status status = db.Status.Find(id);
            Stock _stock = db.Stocks.Where(x => x.StockStatusID == status.StockStatusID).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (_stock != null)
            {
                TempData["ErrorMessage"] = "There is stock with this Status. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else
            {
                db.Status.Remove(status);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: Status/Delete/5
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
