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
    public class InstitutionFacultiesController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: InstitutionFaculties
        public ActionResult Index()
        {
            var institutionFaculties = db.InstitutionFaculties.Include(i => i.Colour).Include(i => i.Faculty).Include(i => i.Institution);
            return View(institutionFaculties.ToList());
        }
        public ActionResult CreateInstitutionFaculty()
        {
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName");
            ViewBag.Faculties = db.Faculties;
            ViewBag.Colours = db.Colours;

            return View();
        }
        [HttpPost]
        public ActionResult CreateInstitutionFaculty(int InstitutionID, int[] FacultyIDs, int[] ColourIDs)
        {
            if (FacultyIDs !=null && ColourIDs !=null)
            {
                for (int i = 0; i < FacultyIDs.Length; i++)
                {
                    InstitutionFaculty institutionFaculty = new InstitutionFaculty();
                    institutionFaculty.InstitutionID = InstitutionID;
                    institutionFaculty.FacultyID = FacultyIDs[i];
                    institutionFaculty.ColourID = ColourIDs[i];
                    db.InstitutionFaculties.Add(institutionFaculty);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Colour and Faculty were not Selected. Process Terminated";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult getFaculties()
        {
            var _faculties = from Institut in db.Institutions
                        from Fac in Institut.Faculties
                        select new
                        {
                            InstID = Institut.InstitutionID,
                            description = Fac.Description,
                            FacID = Fac.FacultyID
                        };

            return Json(_faculties.ToList(), JsonRequestBehavior.AllowGet);
        }
        // GET: InstitutionFaculties/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstitutionFaculty institutionFaculty = db.InstitutionFaculties.Find(id);
            if (institutionFaculty == null)
            {
                return HttpNotFound();
            }
            return View(institutionFaculty);
        }

        // GET: InstitutionFaculties/Create
        public ActionResult Create()
        {
            ViewBag.ColourID = new SelectList(db.Colours, "ColourID", "ColourName");
            ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Description");
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName");
            return View();
        }

        // POST: InstitutionFaculties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FacultyID,InstitutionID,ColourID")] InstitutionFaculty institutionFaculty)
        {
            if (ModelState.IsValid)
            {
                db.InstitutionFaculties.Add(institutionFaculty);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ColourID = new SelectList(db.Colours, "ColourID", "ColourName", institutionFaculty.ColourID);
            ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Description", institutionFaculty.FacultyID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName", institutionFaculty.InstitutionID);
            return View(institutionFaculty);
        }

        // GET: InstitutionFaculties/Edit/5
        public ActionResult Edit(int? id, int? facultyid, int? institutionid)
        {
            if (id == null && facultyid == null && institutionid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstitutionFaculty institutionFaculty = db.InstitutionFaculties.Find(id,facultyid,institutionid);
            if (institutionFaculty == null)
            {
                return HttpNotFound();
            }
            ViewBag.ColourID = new SelectList(db.Colours, "ColourID", "ColourName", institutionFaculty.ColourID);
            ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Description", institutionFaculty.FacultyID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName", institutionFaculty.InstitutionID);
            return View(institutionFaculty);
        }

        // POST: InstitutionFaculties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FacultyID,InstitutionID,ColourID")] InstitutionFaculty institutionFaculty)
        {
            if (ModelState.IsValid)
            {
                db.Entry(institutionFaculty).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ColourID = new SelectList(db.Colours, "ColourID", "ColourName", institutionFaculty.ColourID);
            ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Description", institutionFaculty.FacultyID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName", institutionFaculty.InstitutionID);
            return View(institutionFaculty);
        }

        // GET: InstitutionFaculties/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstitutionFaculty institutionFaculty = db.InstitutionFaculties.Find(id);
            if (institutionFaculty == null)
            {
                return HttpNotFound();
            }
            return View(institutionFaculty);
        }

        // POST: InstitutionFaculties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InstitutionFaculty institutionFaculty = db.InstitutionFaculties.Find(id);
            db.InstitutionFaculties.Remove(institutionFaculty);
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
