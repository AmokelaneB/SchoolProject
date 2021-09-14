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
    public class EmployeeTypesController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: EmployeeTypes
        public ActionResult Index()
        {
            return View(db.EmployeeTypes.ToList());
        }

        // GET: EmployeeTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeType employeeType = db.EmployeeTypes.Find(id);
            if (employeeType == null)
            {
                return HttpNotFound();
            }
            return View(employeeType);
        }

        // GET: EmployeeTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeeTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeTypeID,DESCRIPTION")] EmployeeType employeeType)
        {
            if (ModelState.IsValid)
            {
                EmployeeType _employeeType = db.EmployeeTypes.Where(x => x.DESCRIPTION == employeeType.DESCRIPTION).FirstOrDefault();
                if (_employeeType == null)
                {
                    db.EmployeeTypes.Add(employeeType);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
                else if (_employeeType.DESCRIPTION == employeeType.DESCRIPTION)
                {
                    TempData["ErrorMessage"] = "Employee Type Already Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    db.EmployeeTypes.Add(employeeType);

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Saved Successfully";
                    return RedirectToAction("Index");
                }
            }

            return View(employeeType);
        }

        // GET: EmployeeTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeType employeeType = db.EmployeeTypes.Find(id);
            if (employeeType == null)
            {
                return HttpNotFound();
            }
            return View(employeeType);
        }

        // POST: EmployeeTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeTypeID,DESCRIPTION")] EmployeeType employeeType)
        {
            if (ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {
                    EmployeeType _employeeType = db.EmployeeTypes.Where(x => x.DESCRIPTION == employeeType.DESCRIPTION).FirstOrDefault();
                    if (_employeeType == null)
                    {
                        db.Entry(employeeType).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Updated Successfully";
                        return RedirectToAction("Index");
                    }
                    else if (_employeeType.DESCRIPTION == employeeType.DESCRIPTION)
                    {
                        TempData["ErrorMessage"] = "No changes were made.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        db.Entry(employeeType).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Updated Successfully";
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(employeeType);

        }

        // GET: EmployeeTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            EmployeeType employeetype = db.EmployeeTypes.Find(id);
            Employee _employee = db.Employees.Where(x => x.EmployeeTypeID == employeetype.EmployeeTypeID).FirstOrDefault();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (_employee != null)
            {
                TempData["ErrorMessage"] = "There is an Employee of this Type. Delete Terminated.";
                return RedirectToAction("Index");
            }
            else
            {
                db.EmployeeTypes.Remove(employeetype);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }

        }

        //// POST: EmployeeTypes/Delete/5
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
