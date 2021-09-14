using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security;
using System.Web;
using System.Web.Mvc;
using GradStockUp.Models;

namespace GradStockUp.Controllers
{
    public class SizesController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Sizes
        public ActionResult Index()
        {
            return View(db.Sizes.ToList());
        }

        // GET: Sizes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Size size = db.Sizes.Find(id);
            if (size == null)
            {
                return HttpNotFound();
            }
            return View(size);
        }

        // GET: Sizes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sizes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SizeID,SizeDescription")] Size size)
        {
            if (ModelState.IsValid)
            {
                Size _size = db.Sizes.Where(x => x.SizeDescription == size.SizeDescription).FirstOrDefault();
                if (_size == null)
                {
                    db.Sizes.Add(size);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }

                else if (_size.SizeDescription == size.SizeDescription)
                {
                    TempData["ErrorMessage"] = "Size Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Sizes.Add(size);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
            }

            return View(size);
        }

        // GET: Sizes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Size size = db.Sizes.Find(id);
            if (size == null)
            {
                return HttpNotFound();
            }
            return View(size);
        }

        // POST: Sizes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SizeID,SizeDescription")] Size size)
        {
            if (ModelState.IsValid)
            {
                Size _size = db.Sizes.Where(x => x.SizeDescription == size.SizeDescription).FirstOrDefault();
                if (_size == null)
                {
                    db.Entry(size).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
                else if (_size != null)
                {
                    TempData["ErrorMessage"] = "No Changes Made.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(size).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
            }
            return View(size);
        }

        // GET: Sizes/Delete/5
        public ActionResult Delete(int? id)
        {
            Size _size = db.Sizes.Find(id);

            var _stockType = from stockType in db.StockTypes
                             from size in stockType.Sizes
                             select new
                             {
                                 size = size.SizeID,

                             };

            StockDescription _stockDescription = db.StockDescriptions.Where(x => x.SizeID == _size.SizeID).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (_stockDescription != null)
            {
                TempData["ErrorMessage"] = "There is Stock with this Size. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else if (_stockType != null)
            {
                TempData["ErrorMessage"] = "There is a Stock Type with this Size. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else
            {
                db.Sizes.Remove(_size);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: Sizes/Delete/5
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
        public ActionResult CreateSizeStockType()
        {

            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION");
            return View();
        }
        [HttpPost]
        public ActionResult CreateSizeStockType(int? StockTypeID, string Size)
        {
            if ( Size != "")
            {
                Size _size = db.Sizes.Where(x => x.SizeDescription == Size).FirstOrDefault();
                if (_size == null)
                {
                    Size sz = new Size();
                    sz.SizeDescription = Size;
                    db.Sizes.Add(sz);
                    db.SaveChanges();

                    if (StockTypeID != null)
                    {
                        var temp = StockTypeID;
                        StockType stockType_ = db.StockTypes.FirstOrDefault(x => x.StockTypeID == temp);
                        db.Sizes.Where(siz => siz.SizeID == sz.SizeID).FirstOrDefault().StockTypes.Add(stockType_);
                        db.SaveChanges();
                    }
;

                    TempData["SuccessMessage"] = $"{sz.SizeDescription} Saved Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = $"{_size.SizeDescription} Already Exists";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please enter all the required information to add a Size";
                return RedirectToAction("Index");
            }

        }
        public ActionResult AddOnStockType()
        {
            ViewBag.SizeID = new SelectList(db.Sizes, "SizeID", "SizeDescription");
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION");


            return View();
        }
        [HttpPost]
        public ActionResult AddOnStockType(int ? SizeID, int ? StockTypeID)
        {
            if (SizeID != null && StockTypeID !=null)
            {
               
                    var temp = StockTypeID;
                    StockType stockType_ = db.StockTypes.FirstOrDefault(x => x.StockTypeID == temp);
                    db.Sizes.Where(siz => siz.SizeID == SizeID).FirstOrDefault().StockTypes.Add(stockType_);
                    db.SaveChanges();

                TempData["SuccessMessage"] = "Added-On Successfully. See Details";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Size to Add-On Not Selected";

            }

            return RedirectToAction("Index");
        }
    }
}
