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
    public class InstitutionLineQualificationsController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: InstitutionLineQualifications
        public ActionResult Index()
        {
            var institutionLineQualifications = db.InstitutionLineQualifications.Include(i => i.Establishment).Include(i => i.Institution).Include(i => i.Qualification).Include(i => i.StockType);
            return View(institutionLineQualifications.ToList());
        }
        public ActionResult CreateInstitutionLineQualifications()
        {
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName");
            ViewBag.EstablishmentID = new SelectList(db.Establishments, "EstablishmentID", "EstaName");
            ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "QualificationName");
            ViewBag.StockTypes = db.StockTypes;
           

            return View();
        }
        [HttpPost]
        public ActionResult CreateInstitutionLineQualifications(int InstitutionID, int EstablishmentID, int QualificationID, int[] StockTypeIDs)
        {
            if (StockTypeIDs != null )
            {
                for (int i = 0; i < StockTypeIDs.Length; i++)
                {
                    InstitutionLineQualification institutionLineQualification = new InstitutionLineQualification();
                    institutionLineQualification.InstitutionID = InstitutionID;
                    institutionLineQualification.EstablishmentID = EstablishmentID;
                    institutionLineQualification.QualificationID = QualificationID;
                    institutionLineQualification.StockTypeID = StockTypeIDs[i];
                    db.InstitutionLineQualifications.Add(institutionLineQualification);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                }
            }
            else if (StockTypeIDs == null)
            {
                TempData["ErrorMessage"] = "No Stock Type Was Selected. Process Terminated";
                return RedirectToAction("Index");
            }

            else
            {
                TempData["ErrorMessage"] = "Colour and Faculty were not Selected. Process Terminated";
                return RedirectToAction("Index");
            }

            //foreach (int FacultyID in FacultyIDs)
            //{
            //    foreach (int ColourID in ColourIDs)
            //    {
            //        InstitutionFaculty institutionFaculty = new InstitutionFaculty();
            //        institutionFaculty.InstitutionID = InstitutionID;
            //        institutionFaculty.FacultyID = FacultyID;
            //        institutionFaculty.ColourID = ColourID;
            //        db.InstitutionFaculties.Add(institutionFaculty);
            //        db.SaveChanges();
            //    }
            //}
            return RedirectToAction("Index");
        }
        // GET: InstitutionLineQualifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstitutionLineQualification institutionLineQualification = db.InstitutionLineQualifications.Find(id);
            if (institutionLineQualification == null)
            {
                return HttpNotFound();
            }
            return View(institutionLineQualification);
        }

        // GET: InstitutionLineQualifications/Create
        public ActionResult Create()
        {
            ViewBag.EstablishmentID = new SelectList(db.Establishments, "EstablishmentID", "EstaName");
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName");
            ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "QualificationName");
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION");
            return View();
        }

        // POST: InstitutionLineQualifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockTypeID,EstablishmentID,InstitutionID,QualificationName")] InstitutionLineQualification institutionLineQualification)
        {
            if (ModelState.IsValid)
            {
                db.InstitutionLineQualifications.Add(institutionLineQualification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EstablishmentID = new SelectList(db.Establishments, "EstablishmentID", "EstaName", institutionLineQualification.EstablishmentID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName", institutionLineQualification.InstitutionID);
            ViewBag.QualificationTypeID = new SelectList(db.Qualifications, "QualificationID", "QualificationName", institutionLineQualification.QualificationID);
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION", institutionLineQualification.StockTypeID);
            return View(institutionLineQualification);
        }

        // GET: InstitutionLineQualifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstitutionLineQualification institutionLineQualification = db.InstitutionLineQualifications.Find(id);
            if (institutionLineQualification == null)
            {
                return HttpNotFound();
            }
            ViewBag.EstablishmentID = new SelectList(db.Establishments, "EstablishmentID", "EstaName", institutionLineQualification.EstablishmentID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName", institutionLineQualification.InstitutionID);
            ViewBag.QualificationTypeID = new SelectList(db.Qualifications, "QualificationID", "QualificationName", institutionLineQualification.QualificationID);
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION", institutionLineQualification.StockTypeID);
            return View(institutionLineQualification);
        }

        // POST: InstitutionLineQualifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockTypeID,EstablishmentID,InstitutionID,QualificationID")] InstitutionLineQualification institutionLineQualification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(institutionLineQualification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EstablishmentID = new SelectList(db.Establishments, "EstablishmentID", "EstaName", institutionLineQualification.EstablishmentID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName", institutionLineQualification.InstitutionID);
            ViewBag.QualificationTypeID = new SelectList(db.QualificationTypes, "QualificationID", "QualificationName", institutionLineQualification.QualificationID);
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION", institutionLineQualification.StockTypeID);
            return View(institutionLineQualification);
        }

        // GET: InstitutionLineQualifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstitutionLineQualification institutionLineQualification = db.InstitutionLineQualifications.Find(id);
            if (institutionLineQualification == null)
            {
                return HttpNotFound();
            }
            return View(institutionLineQualification);
        }

        // POST: InstitutionLineQualifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InstitutionLineQualification institutionLineQualification = db.InstitutionLineQualifications.Find(id);
            db.InstitutionLineQualifications.Remove(institutionLineQualification);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
