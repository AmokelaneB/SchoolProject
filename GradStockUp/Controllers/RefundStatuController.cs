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
    public class RefundStatusController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: RefundStatus
        public ActionResult Index()
        {
            return View(db.RefundStatus.ToList());
        }

        // GET: RefundStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RefundStatu refundStatu = db.RefundStatus.Find(id);
            if (refundStatu == null)
            {
                return HttpNotFound();
            }
            return View(refundStatu);
        }

        // GET: RefundStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RefundStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RefundStatusID,Description")] RefundStatu refundStatu)
        {
            if (ModelState.IsValid)
            {
                RefundStatu _refundStatu = db.RefundStatus.Where(x => x.Description == refundStatu.Description).FirstOrDefault();
                if (_refundStatu == null)
                {
                    db.RefundStatus.Add(refundStatu);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_refundStatu.Description == refundStatu.Description)
                {
                    TempData["ErrorMessage"] = "Refund Status Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.RefundStatus.Add(refundStatu);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
            }

            return View(refundStatu);
        }

        // GET: RefundStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RefundStatu refundStatu = db.RefundStatus.Find(id);
            if (refundStatu == null)
            {
                return HttpNotFound();
            }
            return View(refundStatu);
        }

        // POST: RefundStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RefundStatusID,Description")] RefundStatu refundStatu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(refundStatu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(refundStatu);
        }

        // GET: RefundStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                RefundStatu refundStatu = db.RefundStatus.Find(id);
                db.RefundStatus.Remove(refundStatu);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: RefundStatus/Delete/5
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
