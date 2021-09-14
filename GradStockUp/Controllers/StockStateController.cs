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
    public class StockStatesController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: StockStates
        public ActionResult Index()
        {
            return View(db.StockStates.ToList());
        }

        // GET: StockStates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockState stockState = db.StockStates.Find(id);
            if (stockState == null)
            {
                return HttpNotFound();
            }
            return View(stockState);
        }

        // GET: StockStates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StockStates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockStateID,STATE")] StockState stockState)
        {
            if (ModelState.IsValid)
            {
                StockState _stockState = db.StockStates.Where(x => x.STATE == stockState.STATE).FirstOrDefault();
                if (_stockState == null)
                {
                    db.StockStates.Add(stockState);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_stockState.STATE == stockState.STATE)
                {
                    TempData["ErrorMessage"] = "Stock State Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.StockStates.Add(stockState);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
            }

            return View(stockState);
        }

        // GET: StockStates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockState stockState = db.StockStates.Find(id);
            if (stockState == null)
            {
                return HttpNotFound();
            }
            return View(stockState);
        }

        // POST: StockStates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockStateID,STATE")] StockState stockState)
        {
            if (ModelState.IsValid)
            {
                StockState _stockState = db.StockStates.Where(x => x.StockStateID == stockState.StockStateID).FirstOrDefault();
                if (_stockState == null)
                {
                    db.Entry(stockState).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
                else if (_stockState != null)
                {
                    TempData["ErrorMessage"] = "No Changes Made.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(_stockState).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
            }
            return View(stockState);
        }

        // GET: StockStates/Delete/5
        public ActionResult Delete(int? id)
        {
            StockState stockState = db.StockStates.Find(id);
            Stock _stock = db.Stocks.Where(x => x.StockStateID == stockState.StockStateID).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (_stock != null)
            {
                TempData["ErrorMessage"] = "There is Stock with this Stock State. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else
            {
                db.StockStates.Remove(stockState);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: StockStates/Delete/5
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
