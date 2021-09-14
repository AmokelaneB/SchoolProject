using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GradStockUp.Models;
using System.Data;
using System.Data.Entity;
using GradStockUp.Reporting;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;




namespace GradStockUp.Controllers
{
    public class CustomerReports : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CustomerReport()
        {
            ViewBag.Message = "Customer Report.";

            return View(getSimpleData());
        }

        private CustomerModel getSimpleData()
        {
            //Create new instance of CriticalStockModel class - this is basically a dataset that was specifically created to hold the simple report's data
            CustomerModel simpleReportData = new CustomerModel();

            //Create new instance of entity model so that we can interact with DB
            using (GradStockUpEntities db = new GradStockUpEntities())
            {
                //Disable proxy creation
                db.Configuration.ProxyCreationEnabled = false;

                //Get all products where the sum of all its inventories is less than or equal to the product's reorder point
                //Select new anonymous object for each result that maps directly to the rows of the Product table of the CriticalStockModel dataset
                var list = db.Customers.Include("ORGANISATIONs").Select(rr => new
                {
                    ID = rr.CustomerID,
                    CustomerName = rr.CustomerName,
                    CustomerSurname = rr.CustomerSurname,
                    CustAddress = rr.CustAddress,
                    IDNumber = rr.IDnumber,
                    Email = rr.Email,
                    PhoneNumber = rr.PhoneNumber,
                    NextKinName = rr.NextoFKin,
                    NextKinPhoneNumber = rr.kinPhone,
                    OrganizationName = db.Organizations.Where(pp => pp.OrganizationID == rr.OrganizationID).Select(x => x.Name).FirstOrDefault(),
                }).ToList();

                //Create a row for each of the objects that was returned from our DB query
                simpleReportData.Customer.Rows.Clear();
                foreach (var item in list)
                {
                    DataRow row = simpleReportData.Customer.NewRow();
                    row["ID"] = item.ID;
                    row["CustomerName"] = item.CustomerName;
                    row["CustomerSurname"] = item.CustomerSurname;
                    row["CustAddress"] = item.CustAddress;
                    row["IDNumber"] = item.IDNumber;
                    row["Email"] = item.Email;
                    row["PhoneNumber"] = item.PhoneNumber;
                    row["NextKinName"] = item.NextKinName;
                    row["NextKinPhoneNumber"] = item.NextKinPhoneNumber;
                    row["OrganizationName"] = item.OrganizationName;


                    simpleReportData.Customer.Rows.Add(row);
                }

                //Return the CriticalStockModel dataset
                return simpleReportData;
            }
        }
        public ActionResult ExportSimplePDF()
        {
            //Create new instance of ReportDocument
            ReportDocument report = new ReportDocument();

            //Configure the report document to use the logic and format of the CriticalStock report, which we have designed in Crystal Reports designer view
            report.Load(Path.Combine(Server.MapPath("~/Reports/CustomerReport1.rpt")));

            //Set the data source of the report to the DataSet that is returned by the getSimpleData method
            report.SetDataSource(getSimpleData());

            //Configure response parameters
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            //Write the report into a stream in Portable Doc Format
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            //Set stream 'cursor' to beginnning
            stream.Seek(0, SeekOrigin.Begin);

            //Return file so that it downloads on the View Side - note the content type parameter is configured for a pdf file
            return File(stream, "application/pdf", "CustomerReport.pdf");
        }
        public ActionResult ExportSimpleWord()
        {
            //Create new instance of ReportDocument
            ReportDocument report = new ReportDocument();

            //Configure the report document to use the logic and format of the CriticalStock report, which we have designed in Crystal Reports designer view
            report.Load(Path.Combine(Server.MapPath("~/Reports/CustomerReport1.rpt")));

            //Set the data source of the report to the DataSet that is returned by the getSimpleData method
            report.SetDataSource(getSimpleData());

            //Configure response parameters
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            //Write the report into a stream in Microsoft Word Format
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);

            //Set stream 'cursor' to beginnning
            stream.Seek(0, SeekOrigin.Begin);

            //Return file so that it downloads on the View Side - note the content type parameter is configured for a word file
            return File(stream, "application/msword", "CustomerReport.doc");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


    }
    
}