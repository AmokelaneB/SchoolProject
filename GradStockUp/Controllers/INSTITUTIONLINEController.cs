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
    public class INSTITUTIONLINEsController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: INSTITUTIONLINEs
        public ActionResult Index()
        {
            var iNSTITUTIONLINEs = db.INSTITUTIONLINEs.Include(i => i.Establishment).Include(i => i.Institution).Include(i => i.StockType);
            return View(iNSTITUTIONLINEs.ToList());
        }
        public ActionResult CreateInstitutionLine()
        {
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName");
            ViewBag.EstablishmentID = new SelectList(db.Establishments, "EstablishmentID", "EstaName");
            ViewBag.StockTypes = db.StockTypes;
            return View();
        }
        [HttpPost]
        public ActionResult CreateInstitutionLine(int[] StockTypeIDs, int InstitutionID, int EstablishmentID)
        {


            for (int i = 0; i < StockTypeIDs.Length; i++)
            {
                INSTITUTIONLINE institutionLine = new INSTITUTIONLINE();
                institutionLine.StockTypeID = StockTypeIDs[i];
                institutionLine.InstitutionID = InstitutionID;
                institutionLine.EstablishmentID = EstablishmentID;
                db.INSTITUTIONLINEs.Add(institutionLine);
                db.SaveChanges();
            }
            //for (int QualificationTypeID = 1; QualificationTypeID < QualificationTypeIDs.Length; QualificationTypeID++)
            //{
            //    for (int FacultyID = 1; FacultyID < FacultyIDs.Length; FacultyID++)
            //    {

            //        FacultyQualification facultyQualification = new FacultyQualification();
            //        facultyQualification.QualificationTypeID = QualificationTypeID;
            //        facultyQualification.InstitutionID = InstitutionID;
            //        facultyQualification.FacultyID = FacultyID;
            //        db.FacultyQualifications.Add(facultyQualification);
            //        db.SaveChanges();
            //    }
            //
            //}


            return RedirectToAction("Index");
        }

        // GET: INSTITUTIONLINEs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            INSTITUTIONLINE iNSTITUTIONLINE = db.INSTITUTIONLINEs.Find(id);
            if (iNSTITUTIONLINE == null)
            {
                return HttpNotFound();
            }
            return View(iNSTITUTIONLINE);
        }

        // GET: INSTITUTIONLINEs/Create
        public ActionResult Create()
        {
            ViewBag.EstablishmentID = new SelectList(db.Establishments, "EstablishmentID", "EstaName");
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName");
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION");
            return View();
        }

        // POST: INSTITUTIONLINEs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockTypeID,EstablishmentID,InstitutionID")] INSTITUTIONLINE iNSTITUTIONLINE)
        {
            if (ModelState.IsValid)
            {
                db.INSTITUTIONLINEs.Add(iNSTITUTIONLINE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EstablishmentID = new SelectList(db.Establishments, "EstablishmentID", "EstaName", iNSTITUTIONLINE.EstablishmentID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName", iNSTITUTIONLINE.InstitutionID);
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION", iNSTITUTIONLINE.StockTypeID);
            return View(iNSTITUTIONLINE);
        }

        // GET: INSTITUTIONLINEs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            INSTITUTIONLINE iNSTITUTIONLINE = db.INSTITUTIONLINEs.Find(id);
            if (iNSTITUTIONLINE == null)
            {
                return HttpNotFound();
            }
            ViewBag.EstablishmentID = new SelectList(db.Establishments, "EstablishmentID", "EstaName", iNSTITUTIONLINE.EstablishmentID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName", iNSTITUTIONLINE.InstitutionID);
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION", iNSTITUTIONLINE.StockTypeID);
            return View(iNSTITUTIONLINE);
        }

        // POST: INSTITUTIONLINEs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockTypeID,EstablishmentID,InstitutionID")] INSTITUTIONLINE iNSTITUTIONLINE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iNSTITUTIONLINE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EstablishmentID = new SelectList(db.Establishments, "EstablishmentID", "EstaName", iNSTITUTIONLINE.EstablishmentID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName", iNSTITUTIONLINE.InstitutionID);
            ViewBag.StockTypeID = new SelectList(db.StockTypes, "StockTypeID", "DESCRIPTION", iNSTITUTIONLINE.StockTypeID);
            return View(iNSTITUTIONLINE);
        }

        // GET: INSTITUTIONLINEs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            INSTITUTIONLINE iNSTITUTIONLINE = db.INSTITUTIONLINEs.Find(id);
            if (iNSTITUTIONLINE == null)
            {
                return HttpNotFound();
            }
            return View(iNSTITUTIONLINE);
        }

        // POST: INSTITUTIONLINEs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            INSTITUTIONLINE iNSTITUTIONLINE = db.INSTITUTIONLINEs.Find(id);
            db.INSTITUTIONLINEs.Remove(iNSTITUTIONLINE);
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
