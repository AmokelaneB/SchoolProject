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
    public class StockDescriptionsController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: StockDescriptions
        public ActionResult Index()
        {
            var stockDescriptions = db.StockDescriptions.Include(s => s.Colour).Include(s => s.Size).Include(s => s.StockType);
            return View(stockDescriptions.ToList());
        }

        // GET: StockDescriptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockDescription stockDescription = db.StockDescriptions.Find(id);
            if (stockDescription == null)
            {
                return HttpNotFound();
            }
            return View(stockDescription);
        }

        // GET: StockDescriptions/Create
        public ActionResult Create()
        {
            ViewBag.ColourID = new SelectList(db.Colours, "ColourID", "ColourName");
            ViewBag.SizeID = new SelectList(db.Sizes, "SizeID", "SizeDescription");
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION");
            return View();
        }

        // POST: StockDescriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockDescriptionID,SizeID,StockTypeID,ColourID,RENTALSTOCKLEVEL,RETAILSTOCKLEVEL,RETAILTHRESHOLD,RENTALTHRESHOLD,FLAG")] StockDescription stockDescription)
        {
            if (ModelState.IsValid)
            {
                StockDescription std_ = db.StockDescriptions.FirstOrDefault(b => b.ColourID == stockDescription.ColourID && b.StockTypeID == stockDescription.StockTypeID && b.SizeID == stockDescription.SizeID);

                if (std_ != null)
                {
                    TempData["ErrorMessage"] = "Item already exists";

                    return RedirectToAction("Index");

                }
                else if (std_ == null)
                {
                    db.StockDescriptions.Add(stockDescription);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";

                    return RedirectToAction("Index");
                }

            }

            ViewBag.ColourID = new SelectList(db.Colours, "ColourID", "ColourName", stockDescription.ColourID);
            ViewBag.SizeID = new SelectList(db.Sizes, "SizeID", "SizeDescription", stockDescription.SizeID);
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION", stockDescription.StockTypeID);
            return View(stockDescription);
        }

        // GET: StockDescriptions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockDescription stockDescription = db.StockDescriptions.Find(id);
            if (stockDescription == null)
            {
                return HttpNotFound();
            }
            ViewBag.ColourID = new SelectList(db.Colours, "ColourID", "ColourName", stockDescription.ColourID);
            ViewBag.SizeID = new SelectList(db.Sizes, "SizeID", "SizeDescription", stockDescription.SizeID);
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION", stockDescription.StockTypeID);
            return View(stockDescription);
        }

        // POST: StockDescriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockDescriptionID,SizeID,StockTypeID,ColourID,RENTALSTOCKLEVEL,RETAILSTOCKLEVEL,RETAILTHRESHOLD,RENTALTHRESHOLD,FLAG")] StockDescription stockDescription)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockDescription).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Updated Successfully";
                return RedirectToAction("Index");
            }
            ViewBag.ColourID = new SelectList(db.Colours, "ColourID", "ColourName", stockDescription.ColourID);
            ViewBag.SizeID = new SelectList(db.Sizes, "SizeID", "SizeDescription", stockDescription.SizeID);
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION", stockDescription.StockTypeID);
            return View(stockDescription);
        }

        // GET: StockDescriptions/Delete/5
        public ActionResult Delete(int? id)
        {
            StockDescription std = db.StockDescriptions.Find(id);
            SupplierOrderLine sol = db.SupplierOrderLines.Where(x => x.StockDescriptionID == std.StockDescriptionID).FirstOrDefault();
            Stock stock = db.Stocks.Where(x => x.StockDescriptionID == std.StockDescriptionID).FirstOrDefault();


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (sol != null)
            {
                TempData["ErrorMessage"] = "There is a Supplier Order Line with this Stock Description. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else if (stock != null)
            {
                TempData["ErrorMessage"] = "There is Stock with this Stock Description. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else
            {
                db.StockDescriptions.Remove(std);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: StockDescriptions/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    StockDescription stockDescription = db.StockDescriptions.Find(id);
        //    db.StockDescriptions.Remove(stockDescription);
        //    db.SaveChanges();
        //    TempData["SuccessMessage"] = "Deleted Successfully";
        //    return RedirectToAction("Index");
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
