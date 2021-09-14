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
    public class DepartmentsController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Departments
        public ActionResult Index()
        {
            return View(db.Departments.ToList());
        }

        // GET: Departments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentID,DESCRIPTION")] Department department)
        {
            if (ModelState.IsValid)
            {
                Department _department = db.Departments.Where(x => x.DESCRIPTION == department.DESCRIPTION).FirstOrDefault();
                if (_department == null)
                {
                    db.Departments.Add(department);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_department.DESCRIPTION == department.DESCRIPTION)
                {
                    TempData["ErrorMessage"] = $"{_department.DESCRIPTION} Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Departments.Add(department);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }

            }

            return View(department);
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentID,DESCRIPTION")] Department department)
        {
            if (ModelState.IsValid)
            {
                Department _department = db.Departments.Where(x => x.DESCRIPTION == department.DESCRIPTION).FirstOrDefault();
                if (_department == null)
                {
                    db.Entry(department).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = $"Updated Successfully";
                    return RedirectToAction("Index");
                }
                else if (_department.DESCRIPTION == department.DESCRIPTION)
                {
                    TempData["ErrorMessage"] = "No changes were made.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(department).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
            }
                return View(department);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id)
        {
            Department department = db.Departments.Find(id);
            Stock _stock = db.Stocks.Where(x => x.DepartmentID == department.DepartmentID).FirstOrDefault();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (_stock != null)
            {
                TempData["ErrorMessage"] = "There is Stock in this Department. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else
            {
                db.Departments.Remove(department);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: Departments/Delete/5
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
