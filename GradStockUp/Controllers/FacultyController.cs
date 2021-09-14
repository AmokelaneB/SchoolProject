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
    public class FacultiesController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Faculties
        public ActionResult Index()
        {
            return View(db.Faculties.ToList());
        }

        // GET: Faculties/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faculty faculty = db.Faculties.Find(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }
            return View(faculty);
        }

        // GET: Faculties/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Faculties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FacultyID,Description")] Faculty faculty)
        {
            Faculty _faculty= db.Faculties.Where(x => x.Description == faculty.Description).FirstOrDefault();
            if (_faculty == null)
            {
                db.Faculties.Add(faculty);

                db.SaveChanges();
                TempData["SuccessMessage"] = $"{faculty.Description} Saved Successfully";
                return RedirectToAction("Index");
            }
            else if (_faculty.Description == faculty.Description)
            {
                TempData["ErrorMessage"] = $"Faculty {_faculty.Description} Already Exists";
                return RedirectToAction("Index");
            }
            else
            {
                db.Faculties.Add(faculty);

                db.SaveChanges();
                TempData["SuccessMessage"] = $"{faculty.Description} Saved Successfully";
                return RedirectToAction("Index");
            }
            
        }

        // GET: Faculties/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faculty faculty = db.Faculties.Find(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }
            return View(faculty);
        }

        // POST: Faculties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FacultyID,Description")] Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                Faculty _faculty = db.Faculties.Where(x => x.Description == faculty.Description).FirstOrDefault();
                if (_faculty == null)
                {
                    db.Faculties.Add(faculty);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_faculty.Description == faculty.Description)
                {
                    TempData["ErrorMessage"] = "No Changes Made";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Faculties.Add(faculty);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
            }
            return View(faculty);
        }

        // GET: Faculties/Delete/5
        public ActionResult Delete(int? id)
        {
            Faculty faculty = db.Faculties.Find(id);
            FacultyQualification _facultyqualification = db.FacultyQualifications.Where(x => x.FacultyID == faculty.FacultyID).FirstOrDefault();
            InstitutionFaculty _institutionfaculty = db.InstitutionFaculties.Where(x => x.FacultyID == faculty.FacultyID).FirstOrDefault();
            var fac = from institution in db.Institutions
                      from Fac in institution.Faculties
                      select new
                      {
                          Facult = Fac.FacultyID,

                      };

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (_facultyqualification != null || _institutionfaculty != null || fac !=null)
            {
                TempData["ErrorMessage"] = "An Institution has this Faculty. Delete Terminated.";
                return RedirectToAction("Index");
            }

            else
            {
                db.Faculties.Remove(faculty);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: Faculties/Delete/5
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
        public ActionResult CreateFacultyInstitution()
        {
            ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Description");
            ViewBag.Institutions = db.Institutions;

            return View();
        }
        [HttpPost]
        public ActionResult CreateFacultyInstitution(int []InstitutionIDs, string FacultyD)
        {
            if (InstitutionIDs != null && FacultyD != "")
            {
                Faculty fac_ = db.Faculties.Where(f => f.Description == FacultyD).FirstOrDefault();
                if (fac_ == null)
                {
                    Faculty f = new Faculty();
                    f.Description = FacultyD;
                    db.Faculties.Add(f);
                    db.SaveChanges();
                    for (int i = 0; i < InstitutionIDs.Length; i++)
                    {
                        var temp = InstitutionIDs[i];
                        Institution inst_ = db.Institutions.FirstOrDefault(x => x.InstitutionID == temp);
                        db.Faculties.Where(inst => inst.FacultyID == f.FacultyID).FirstOrDefault().Institutions.Add(inst_);
                        db.SaveChanges();
                    }
                    TempData["SuccessMessage"] = "Added Successfully. See Details";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Faculty Already Exists.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please enter all required information to add a new Faculty";
                return RedirectToAction("Index");
            }

            
        }
        public ActionResult AddFacultyInstitution()
        {
            ViewBag.FacultyID = new SelectList(db.Faculties, "FacultyID", "Description");
            ViewBag.Institutions = db.Institutions;

            return View();
        }
        [HttpPost]
        public ActionResult AddFacultyInstitution(int[] InstitutionIDs, int FacultyID)
        {
            if (InstitutionIDs != null)
            {
                for (int i = 0; i < InstitutionIDs.Length; i++)
                {
                    var temp = InstitutionIDs[i];
                    Institution inst_ = db.Institutions.FirstOrDefault(x => x.InstitutionID == temp);
                    db.Faculties.Where(inst => inst.FacultyID == FacultyID).FirstOrDefault().Institutions.Add(inst_);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Added-On Successfully. See Details";

                }
            }
            else
            {
                TempData["ErrorMessage"] = "Institutions to Add-On Not Selected";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
