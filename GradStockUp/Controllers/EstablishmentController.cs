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
    public class EstablishmentsController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Establishments
        public ActionResult Index()
        {
            
            return View(db.Establishments.ToList());
        }

        // GET: Establishments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Establishment establishment = db.Establishments.Find(id);
            if (establishment == null)
            {
                return HttpNotFound();
            }
            return View(establishment);
        }

        // GET: Establishments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Establishments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EstablishmentID,EstaName")] Establishment establishment)
        {
            if (ModelState.IsValid)
            {
                Establishment _establishment = db.Establishments.Where(x => x.EstaName == establishment.EstaName).FirstOrDefault();
                if (_establishment == null)
                {
                    db.Establishments.Add(establishment);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_establishment.EstaName == establishment.EstaName)
                {
                    TempData["ErrorMessage"] = $"Establishment {_establishment.EstaName} Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Establishments.Add(establishment);

                    db.SaveChanges();

                }
            }

            return View(establishment);
        }
        public ActionResult CreateEstablishmentStockType()
        {

            
            ViewBag.StockTypes = db.StockTypes;
            ViewBag.Institutions = db.Institutions;

            return View();
        }
        [HttpPost]
        public ActionResult CreateEstablishmentStockType(string EstaName, int[] StockTypeIDs, int[] InstitutionIDs)
        {
            if (EstaName != null && StockTypeIDs != null && InstitutionIDs != null)
            {
                var res = db.Establishments.FirstOrDefault(b => b.EstaName == EstaName);
                if (res == null)
                {
                    Establishment e = new Establishment();
                    e.EstaName = EstaName;
                    db.Establishments.Add(e);
                    db.SaveChanges();

                    for (int i = 0; i < StockTypeIDs.Length; i++)
                    {
                        var temp = StockTypeIDs[i];
                        StockType _st = db.StockTypes.FirstOrDefault(x => x.StockTypeID == temp);
                        db.Establishments.Where(esta => esta.EstablishmentID == e.EstablishmentID).FirstOrDefault().StockTypes.Add(_st);
                        db.SaveChanges();

                    }
                    for (int k = 0; k < InstitutionIDs.Length; k++)
                    {
                        var _temp = InstitutionIDs[k];
                        Institution _i = db.Institutions.FirstOrDefault(r => r.InstitutionID == _temp);
                        db.Establishments.Where(esta => esta.EstablishmentID == e.EstablishmentID).FirstOrDefault().Institutions.Add(_i);
                        db.SaveChanges();
                    }
                    TempData["SuccessMessage"] = $"Saved {EstaName} Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = $"Establishment {res.EstaName} Already Exists";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please enter all the required information to add an Establishment";
                return RedirectToAction("Index");
            }
        }
        // GET: Establishments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Establishment establishment = db.Establishments.Find(id);
            if (establishment == null)
            {
                return HttpNotFound();
            }
            return View(establishment);
        }

        // POST: Establishments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EstablishmentID,EstaName")] Establishment establishment, int[] StockTypeIDs, int[] InstitutionIDs)
        {
            if (ModelState.IsValid)
            {
                Establishment _establishment = db.Establishments.Where(x => x.EstaName == establishment.EstaName).FirstOrDefault();
                if (_establishment == null)
                {
                    db.Entry(establishment).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = $"Updated {establishment.EstaName} Successfully";
                    return RedirectToAction("Index");
                }
                else if (_establishment != null)
                {
                    TempData["ErrorMessage"] = "No changes were made.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(establishment).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
            }
            return View(establishment);
        }

        // GET: Establishments/Delete/5
        public ActionResult Delete(int? id)
        {
            Establishment establishment = db.Establishments.Find(id);
            INSTITUTIONLINE _institutionLine = db.INSTITUTIONLINEs.Where(x => x.EstablishmentID == establishment.EstablishmentID).FirstOrDefault();
            var est = from stockType in db.StockTypes
                      from esT in stockType.Establishments
                      where esT.EstablishmentID == establishment.EstablishmentID
                      select esT;


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (_institutionLine != null)
            {
                TempData["ErrorMessage"] = "An Institution has stock under this Establishment. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else if (est != null)
            {
                TempData["ErrorMessage"] = "There is a Stock Type under this Establishment. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else
            {
                db.Establishments.Remove(establishment);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: Establishments/Delete/5
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
        public ActionResult CreateInstitutionEstablishment()
        {
            ViewBag.EstablishmentID = new SelectList(db.Establishments, "EstablishmentID", "EstaName");
            ViewBag.Institutions = db.Institutions;


            return View();
        }
        [HttpPost]
        public ActionResult CreateInstitutionEstablishment(int[] InstitutionIDs, int ? EstablishmentID)
        {
            if (InstitutionIDs != null && EstablishmentID != null)
            {
                for (int i = 0; i < InstitutionIDs.Length; i++)
                {
                    var temp = InstitutionIDs[i];
                    Institution inst_ = db.Institutions.FirstOrDefault(x => x.InstitutionID == temp);
                    db.Establishments.Where(est => est.EstablishmentID == EstablishmentID).FirstOrDefault().Institutions.Add(inst_);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Added-On Successfully. See Details";

                }
            }
            else
            {
                TempData["ErrorMessage"] = "Enough information is needed to add-on an Institution";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        public ActionResult CreateStockTypeAddOn()
        {
            ViewBag.EstablishmentID = new SelectList(db.Establishments, "EstablishmentID", "EstaName");
            ViewBag.StockTypes = db.StockTypes;


            return View();
        }
        [HttpPost]
        public ActionResult CreateStockTypeAddOn(int[] StocktypeIDs, int? EstablishmentID)
        {
            if (StocktypeIDs != null && EstablishmentID != null)
            {
                for (int i = 0; i < StocktypeIDs.Length; i++)
                {
                    var temp = StocktypeIDs[i];
                    StockType st_ = db.StockTypes.FirstOrDefault(x => x.StockTypeID == temp);
                    db.Establishments.Where(est => est.EstablishmentID == EstablishmentID).FirstOrDefault().StockTypes.Add(st_);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Added-On Successfully. See Details";

                }
            }
            else
            {
                TempData["ErrorMessage"] = "Enough information is needed to add-on a Stock Type";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
