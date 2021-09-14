using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GradStockUp.Models;
using iTextSharp.text.pdf;

namespace GradStockUp.Controllers
{
    public class InstitutionsController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();
      

        // GET: Institutions
        public ActionResult Index()
        {
            return View(db.Institutions.Include(e => e.Establishments).ToList());
        }

        // GET: Institutions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return HttpNotFound();
            }
            return View(institution);
        }


        // GET: Institutions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Institutions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InstitutionID,InstitutionName,Address")] Institution institution)
        {
            var establishment = db.Establishments.ToList();


            if (ModelState.IsValid)
            {
                Institution _institution = db.Institutions.Where(x => x.InstitutionName == institution.InstitutionName).FirstOrDefault();
                if (_institution == null)
                {
                    db.Institutions.Add(institution);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = $"{institution.InstitutionName} Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_institution.InstitutionName == institution.InstitutionName)
                {
                    TempData["ErrorMessage"] = "Institution Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Institutions.Add(institution);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
            }

            return View(institution);
        }
        public ActionResult CreateEstablishmentInstitution()
        {

            ViewBag.Establishments = db.Establishments;
            ViewBag.Faculties = db.Faculties;


            return View();
        }
        [HttpPost]
        public ActionResult CreateEstablishmentInstitution(string InstitutionN, string Address, int[] EstablishmentIDs, int[] FacultyIDs)
        {
            if (EstablishmentIDs != null && InstitutionN != ""&& Address != "")
            {
                Institution _i = db.Institutions.Where(x => x.InstitutionName == InstitutionN).FirstOrDefault();
                if (_i == null)
                {
                    Institution inst = new Institution();
                    inst.InstitutionName = InstitutionN;
                    inst.Address = Address;
                    db.Institutions.Add(inst);
                    db.SaveChanges();

                    for (int i = 0; i < EstablishmentIDs.Length; i++)
                    {
                        var temp = EstablishmentIDs[i];
                        Establishment est_ = db.Establishments.FirstOrDefault(x => x.EstablishmentID == temp);
                        db.Institutions.Where(in_ => in_.InstitutionID == inst.InstitutionID).FirstOrDefault().Establishments.Add(est_);
                        db.SaveChanges();
                        TempData["SuccessMessage"] = $"Saved Successfully";
                    }
                    if (FacultyIDs != null)
                    {
                        for (int k = 0; k < FacultyIDs.Length; k++)
                        {
                            var temp_ = FacultyIDs[k];
                            Faculty fac_ = db.Faculties.FirstOrDefault(x => x.FacultyID == temp_);
                            db.Institutions.Where(inst_ => inst_.InstitutionID == inst.InstitutionID).FirstOrDefault().Faculties.Add(fac_);
                            db.SaveChanges();
                           
                        }
                    
                    }
                    TempData["SuccessMessage"] = $" {inst.InstitutionName} Saved Successfully. See Details";
                    return RedirectToAction("Index");

                }
                else
                {
                    TempData["ErrorMessage"] = $"{_i.InstitutionName} Already Exists";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Enter all the required information to add a new Institution";
                return RedirectToAction("Index");
            }
            
        }
        public ActionResult CreateInstitutionFaculty()
        {
            ViewBag.InstitutionID = new SelectList(db.Institutions, "InstitutionID", "InstitutionName");
            ViewBag.Faculties = db.Faculties;


            return View();
        }
        [HttpPost]
        public ActionResult CreateInstitutionFaculty(int InstitutionID, int[] FacultyIDs)
        {
            if (FacultyIDs != null)
            {
                for (int i = 0; i < FacultyIDs.Length; i++)
                {
                    var temp = FacultyIDs[i];
                    Faculty fac_ = db.Faculties.FirstOrDefault(x => x.FacultyID == temp);
                    db.Institutions.Where(inst => inst.InstitutionID == InstitutionID).FirstOrDefault().Faculties.Add(fac_);
                    db.SaveChanges();
                   

                }
                TempData["SuccessMessage"] = "Added-On Successfully. See Details";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Faculties to Add-On Not Selected";
               
            }

            return RedirectToAction("Index");
        }
        // GET: Institutions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return HttpNotFound();
            }
            return View(institution);
        }

        // POST: Institutions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InstitutionID,InstitutionName,Address")] Institution institution)
        {
            if (ModelState.IsValid)
            {
                Institution _institution = db.Institutions.Where(x => x.InstitutionName == institution.InstitutionName).FirstOrDefault();
                if (_institution == null)
                {
                    db.Entry(institution).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = $"Updated {institution.InstitutionName} Successfully";
                    return RedirectToAction("Index");
                }
                else if (_institution != null)
                {
                    TempData["ErrorMessage"] = "No Changes Made.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(institution).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
            }
            return View(institution);
        }

        // GET: Institutions/Delete/5
        public ActionResult Delete(int? id)
        {
            Institution institution = db.Institutions.Find(id);
            INSTITUTIONLINE _institutionLine = db.INSTITUTIONLINEs.Where(x => x.InstitutionID == institution.InstitutionID).FirstOrDefault();
            var instFac = from facut in db.Faculties
                          from inst in facut.Institutions
                          select new
                          {
                              InST = inst.InstitutionID,

                          };

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (_institutionLine != null)
            {
                TempData["ErrorMessage"] = "There are Faculties and Qualifications under this Institution. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else if (instFac != null)
            {
                TempData["ErrorMessage"] = "There are Faculties under this Institution. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else
            {
                db.Institutions.Remove(institution);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: Institutions/Delete/5
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
