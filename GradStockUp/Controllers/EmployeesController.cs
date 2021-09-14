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
    public class EmployeesController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Employees
        public ActionResult Index()
        {
            var employees = db.Employees.Include(e => e.EmployeeType);
            return View(employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.EmployeeTypeID = new SelectList(db.EmployeeTypes, "EmployeeTypeID", "DESCRIPTION");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,EmplyID,EmployeeTypeID,Name,Surname,PhoneNumber,Email,EmployeeAddress")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                Employee _employee = db.Employees.Where(x => x.EmplyID == employee.EmplyID).FirstOrDefault();
                if (_employee == null)
                {
                    db.Employees.Add(employee);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_employee.EmplyID == employee.EmplyID)
                {
                    TempData["ErrorMessage"] = "Employee Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Employees.Add(employee);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
            }

            ViewBag.EmployeeTypeID = new SelectList(db.EmployeeTypes, "EmployeeTypeID", "DESCRIPTION", employee.EmployeeTypeID);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeTypeID = new SelectList(db.EmployeeTypes, "EmployeeTypeID", "DESCRIPTION", employee.EmployeeTypeID);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,EmplyID,EmployeeTypeID,Name,Surname,PhoneNumber,Email,EmployeeAddress")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                Employee _employee = db.Employees.Where(x => x.EmplyID == employee.EmplyID && x.Name == employee.Name && x.Surname == employee.Surname && x.PhoneNumber == employee.PhoneNumber && x.Email == employee.Email && x.EmployeeAddress == employee.EmployeeAddress).FirstOrDefault();
                if (_employee == null)
                {
                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
                else if (_employee != null)
                {
                    TempData["ErrorMessage"] = "No changes were made.";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.EmployeeTypeID = new SelectList(db.EmployeeTypes, "EmployeeTypeID", "DESCRIPTION", employee.EmployeeTypeID);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Employee employee = db.Employees.Find(id);
                db.Employees.Remove(employee);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: Employees/Delete/5
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
