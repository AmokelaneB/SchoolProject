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
    public class StockTypesController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: StockTypes
        public ActionResult Index()
        {
            return View(db.StockTypes.ToList());
        }

        // GET: StockTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockType stockType = db.StockTypes.Find(id);
            if (stockType == null)
            {
                return HttpNotFound();
            }
            return View(stockType);
        }

        // GET: StockTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StockTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockTypeID,DESCRIPTION")] StockType stockType)
        {
            if (ModelState.IsValid)
            {
                StockType _stockType = db.StockTypes.Where(x => x.DESCRIPTION == stockType.DESCRIPTION).FirstOrDefault();
                if (_stockType == null)
                {
                    db.StockTypes.Add(stockType);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_stockType.DESCRIPTION == stockType.DESCRIPTION)
                {
                    TempData["ErrorMessage"] = "Stock Type Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.StockTypes.Add(stockType);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
            }

            return View(stockType);
        }
        public ActionResult CreateStockTypeSize()
        {

            ViewBag.Sizes = db.Sizes;
            ViewBag.Establishments = db.Establishments;
            return View();
        }
        [HttpPost]
        public ActionResult CreateStockTypeSize(string StockTypeD, int[] SizeIDs, int[] EstablishmentIDs)
        {
            if (SizeIDs != null && StockTypeD != "" && EstablishmentIDs != null)
            {
                StockType _stockType = db.StockTypes.Where(x => x.DESCRIPTION == StockTypeD).FirstOrDefault();
                if (_stockType == null)
                {
                    StockType sty = new StockType();
                    sty.DESCRIPTION = StockTypeD;
                    db.StockTypes.Add(sty);
                    db.SaveChanges();

                    for (int i = 0; i < SizeIDs.Length; i++)
                    {
                        var temp = SizeIDs[i];
                        Size size_ = db.Sizes.FirstOrDefault(x => x.SizeID == temp);
                        db.StockTypes.Where(st => st.StockTypeID == sty.StockTypeID).FirstOrDefault().Sizes.Add(size_);
                        db.SaveChanges();
                    }
                    for (int k = 0; k < EstablishmentIDs.Length; k++)
                    {
                        var temp_ = EstablishmentIDs[k];
                        Establishment esta_ = db.Establishments.FirstOrDefault(e => e.EstablishmentID == temp_);
                        db.StockTypes.Where(s => s.StockTypeID == sty.StockTypeID).FirstOrDefault().Establishments.Add(esta_);
                        db.SaveChanges();
                    }
                    TempData["SuccessMessage"] = $"{sty.DESCRIPTION} Saved Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = $"{_stockType.DESCRIPTION} Already Exist";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Enter all the required information to add a Stock Type";
                return RedirectToAction("Index");
            }

        }
        // GET: StockTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockType stockType = db.StockTypes.Find(id);
            if (stockType == null)
            {
                return HttpNotFound();
            }
            return View(stockType);
        }

        // POST: StockTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockTypeID,DESCRIPTION")] StockType stockType)
        {
            if (ModelState.IsValid)
            {
                StockType _stockType = db.StockTypes.Where(x => x.DESCRIPTION == stockType.DESCRIPTION).FirstOrDefault();
                if (_stockType == null)
                {
                    db.Entry(stockType).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = $"Updated Successfully";
                    return RedirectToAction("Index");
                }
                else if (_stockType != null)
                {
                    TempData["ErrorMessage"] = "No Changes Made.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(stockType).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");

                }
            }
            return View(stockType);
        }

        // GET: StockTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            StockType stockType = db.StockTypes.Find(id);
            StockDescription _stockDescription = db.StockDescriptions.Where(x => x.StockTypeID == stockType.StockTypeID).FirstOrDefault();
            INSTITUTIONLINE _instLine = db.INSTITUTIONLINEs.Where(x => x.StockTypeID == stockType.StockTypeID).FirstOrDefault();
            InstitutionLineQualification _instQLine = db.InstitutionLineQualifications.Where(x => x.StockTypeID == stockType.StockTypeID).FirstOrDefault();

            var _size = from size in db.Sizes
                        from st in size.StockTypes
                        where st.StockTypeID == stockType.StockTypeID
                        select size;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (_stockDescription != null)
            {
                TempData["ErrorMessage"] = "There is Stock with this Stock Type. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else if (_instLine != null)
            {
                TempData["ErrorMessage"] = "There is an Institution Line with this Stock Type. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else if (_instQLine != null)
            {
                TempData["ErrorMessage"] = "There is an Institution Qualification Line with this Stock Type. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else if (_size != null)
            {
                TempData["ErrorMessage"] = "There is Stock with this Stock Type. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else
            {
                db.StockTypes.Remove(stockType);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        // POST: StockTypes/Delete/5
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
        public ActionResult CreateEstablishmentStockType()
        {
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION");
            ViewBag.Establishments = db.Establishments;


            return View();
        }
        [HttpPost]
        public ActionResult CreateEstablishmentStockType(int StockTypeID, int[] EstablishmentIDs)
        {
            if (EstablishmentIDs != null)
            {
                for (int i = 0; i < EstablishmentIDs.Length; i++)
                {
                    var temp = EstablishmentIDs[i];
                    Establishment _est = db.Establishments.FirstOrDefault(x => x.EstablishmentID == temp);
                    db.StockTypes.Where(st => st.StockTypeID == StockTypeID).FirstOrDefault().Establishments.Add(_est);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Establishments to Add-On Not Selected";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
        public ActionResult AddOnSize()
        {
            ViewBag.Sizes = db.Sizes;
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION");


            return View();
        }
        [HttpPost]
        public ActionResult AddOnSize(int[] SizeIDs, int? StockTypeID)
        {
            if (SizeIDs != null && StockTypeID != null)
            {

                for (int i = 0; i < SizeIDs.Length; i++)
                {
                    var temp = SizeIDs[i];
                    Size size_ = db.Sizes.FirstOrDefault(x => x.SizeID == temp);
                    db.StockTypes.Where(st => st.StockTypeID == StockTypeID).FirstOrDefault().Sizes.Add(size_);
                    db.SaveChanges();
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Size to Add-On Not Selected";

            }

            return RedirectToAction("Index");
        }
    }
}
