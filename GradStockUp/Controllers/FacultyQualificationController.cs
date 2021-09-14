using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GradStockUp.Models;

namespace GradStockUp.Controllers
{
    public class FacultyQualificationsController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: FacultyQualifications
        public ActionResult Index()
        {
            var facultyQualifications = db.FacultyQualifications.Include(f => f.Faculty).Include(f => f.Institution).Include(f => f.Qualification);
            return View(facultyQualifications.ToList());
        }

        public ActionResult CreateFacultyQualificationTypes()
        {
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName");
            ViewBag.Faculties = db.Faculties;
            ViewBag.Qualifications = db.Qualifications;

            return View();
        }
        [HttpPost]
        public ActionResult CreateFacultyQualificationTypes(int[] FacultyIDs, int InstitutionID, int[] QualificationIDs)
        {

            for (int i = 0; i <QualificationIDs.Length; i++)
            {
                for (int k = 0; k < FacultyIDs.Length/2; k++)
                {
                    FacultyQualification facultyQualification = new FacultyQualification();
                    facultyQualification.InstitutionID = InstitutionID;
                    facultyQualification.QualificationID = QualificationIDs[i];
                    facultyQualification.FacultyID = FacultyIDs[k];
                    db.FacultyQualifications.Add(facultyQualification);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                }
            }
            return RedirectToAction("Index");
        }
        // GET: FacultyQualifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FacultyQualification facultyQualification = db.FacultyQualifications.Find(id);
            if (facultyQualification == null)
            {
                return HttpNotFound();
            }
            return View(facultyQualification);
        }

        // GET: FacultyQualifications/Create
        public ActionResult Create()
        {
            ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Description");
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName");
            ViewBag.QualificationID = new SelectList(db.Qualifications, "QualificationID", "QualificationName");
            return View();
        }

        // POST: FacultyQualifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FacultyID,QualificationTypeID,InstitutionID")] FacultyQualification facultyQualification)
        {
            if (ModelState.IsValid)
            {
                db.FacultyQualifications.Add(facultyQualification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Description", facultyQualification.FacultyID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName", facultyQualification.InstitutionID);
            ViewBag.QualificationTypeID = new SelectList(db.Qualifications, "QualificationID", "QualificationName", facultyQualification.QualificationID);
            return View(facultyQualification);
        }

        // GET: FacultyQualifications/Edit/5
        public ActionResult Edit(int? FacultyID, int? InstitutionID, int? QualificationTypeID)
        {
            if (FacultyID == null && InstitutionID == null && QualificationTypeID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FacultyQualification facultyQualification = db.FacultyQualifications.Find(FacultyID, InstitutionID, QualificationTypeID);
            if (facultyQualification == null)
            {
                return HttpNotFound();
            }
            else if (FacultyID != null && InstitutionID != null && QualificationTypeID != null)
            {
                ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Description", facultyQualification.FacultyID);
                ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName", facultyQualification.InstitutionID);
                ViewBag.QualificationTypeID = new SelectList(db.Qualifications, "QualificationTypeID", "QualificationType1", facultyQualification.QualificationID);
                return View(facultyQualification);
            }
            ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Description", facultyQualification.FacultyID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName", facultyQualification.InstitutionID);
            ViewBag.QualificationTypeID = new SelectList(db.Qualifications, "QualificationTypeID", "QualificationType1", facultyQualification.QualificationID);
            return View(facultyQualification);
        }

        // POST: FacultyQualifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FacultyID,QualificationTypeID,InstitutionID")] FacultyQualification facultyQualification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(facultyQualification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Description", facultyQualification.FacultyID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName", facultyQualification.InstitutionID);
            ViewBag.QualificationTypeID = new SelectList(db.Qualifications, "QualificationTypeID", "QualificationType1", facultyQualification.QualificationID);
            return View(facultyQualification);
        }

        // GET: FacultyQualifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FacultyQualification facultyQualification = db.FacultyQualifications.Find(id);
            if (facultyQualification == null)
            {
                return HttpNotFound();
            }
            return View(facultyQualification);
        }

        // POST: FacultyQualifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FacultyQualification facultyQualification = db.FacultyQualifications.Find(id);
            db.FacultyQualifications.Remove(facultyQualification);
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
        public ActionResult getFaculties()
        {
            var Faculties = from Institut in db.Institutions
                        from Fac in Institut.Faculties
                        select new
                        {
                            InstID = Institut.InstitutionID,
                            description = Fac.Description,
                            FacID = Fac.FacultyID
                        };

            return Json(Faculties.ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}
