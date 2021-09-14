using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GradStockUp.Models;

using System.IO;
using CrystalDecisions.CrystalReports.Engine;

namespace OnlineDBReports.Controllers
{
    public class ReportsController : Controller
    {
        private GradStockUpEntities db = new GradStockUpEntities();

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SupplierOrder()
        {
            //1. Supplier DropdownList
            List<Supplier> suppliersList = db.Suppliers.ToList();
            ViewData["Suppliers"] = suppliersList;
            return View();
        }

        public ActionResult ExportSupplierOrderReport()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/CrystalRepo/SupplierOrderReport.rpt")));
            rd.SetDataSource(db.SupplierOrderLines.Select(p => new
            {
                SupplierOrderID = p.SupplierOrderID,
                SupplierName = db.Suppliers.FirstOrDefault(s => s.SupplierID == p.SupplierOrder.SupplierID).SupplierName,

                StockDescription = db.Colours.FirstOrDefault(s => s.ColourID == p.StockDescription.ColourID).ColourName + " " + db.StockTypes.FirstOrDefault(s => s.StockTypeID == p.StockDescription.StockDescriptionID).DESCRIPTION + " "
                + db.Sizes.FirstOrDefault(s => s.SizeID == p.StockDescription.SizeID).SizeDescription,
                TotalCost = p.SupplierOrder.TotalCost,
                OrderStatus = db.OrderStatus.FirstOrDefault(s => s.OrderStatusID == p.SupplierOrder.OrderStatusID).DESCRIPTION,
                Quantity = p.Quantity

            }).ToList());
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "SupplierOrderReport.pdf");

        }
        public ActionResult DownloadSupOrderRpt(int supplier)
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/CrystalRepo/ParametisedSupplierOrderReport.rpt")));
            rd.SetDataSource(db.SupplierOrderLines.Select(p => new
            {
                SupplierOrderID = p.SupplierOrderID,
                SupplierName = db.Suppliers.FirstOrDefault(s => s.SupplierID == p.SupplierOrder.SupplierID).SupplierName,

                StockDescription = db.Colours.FirstOrDefault(s => s.ColourID == p.StockDescription.ColourID).ColourName + " " + db.StockTypes.FirstOrDefault(s => s.StockTypeID == p.StockDescription.StockDescriptionID).DESCRIPTION + " "
                + db.Sizes.FirstOrDefault(s => s.SizeID == p.StockDescription.SizeID).SizeDescription,
                TotalCost = p.SupplierOrder.TotalCost,
                OrderStatus = db.OrderStatus.FirstOrDefault(s => s.OrderStatusID == p.SupplierOrder.OrderStatusID).DESCRIPTION,
                Quantity = p.Quantity

            }).ToList());
            var SupplierName = db.Suppliers.FirstOrDefault(s => s.SupplierID == supplier).SupplierName;
            rd.SetParameterValue("Supplier", SupplierName);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream =
            rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "SpecifiedSupplierOrderReport.pdf");
        }


        public ActionResult SalesReport()
        {
            return View();
        }
        public ActionResult ExportPDFSalesReport()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/CrystalRepo/SalesReport.rpt")));
            rd.SetDataSource(db.PurchaseOrders.Select(p => new
            {
                PurchaseOrderID = p.PurchaseOrderID,
                IDNumber = db.Customers.FirstOrDefault(s => s.CustomerID == p.Customer.CustomerID).IDnumber,
                Name = db.Customers.FirstOrDefault(s => s.CustomerID == p.Customer.CustomerID).CustomerName,
                Surname = db.Customers.FirstOrDefault(s => s.CustomerID == p.Customer.CustomerID).CustomerSurname,
                PaymentMethod = db.PaymentMethods.FirstOrDefault(s => s.PaymentMethodID == p.PaymentMethod.PaymentMethodID).Description,
                OrderStatus = db.OrderStatus.FirstOrDefault(s => s.OrderStatusID == p.OrderStatu.OrderStatusID).DESCRIPTION,
                OrderDate = p.OrderDateTime,
                AmountPaid = p.AmountPaid

            }).ToList());
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "SalesReport.pdf");
        }

        public ActionResult CustomerRentalReport()
        {
            return View();
        }
        public ActionResult ExportCustomerRentalPDF()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/CrystalRepo/CustomerRentalReport.rpt")));
            rd.SetDataSource(db.CustomerRentals.Select(p => new
            {
                CustomerRentalID = p.CustomerRentalID,
                IDNumber = db.Customers.FirstOrDefault(s => s.CustomerID == p.Customer.CustomerID).IDnumber,
                Name = db.Customers.FirstOrDefault(s => s.CustomerID == p.Customer.CustomerID).CustomerName,
                Surname = db.Customers.FirstOrDefault(s => s.CustomerID == p.Customer.CustomerID).CustomerSurname,
                PaymentMethod = db.PaymentMethods.FirstOrDefault(s => s.PaymentMethodID == p.PaymentMethod.PaymentMethodID).Description,
                OrderStatus = db.OrderStatus.FirstOrDefault(s => s.OrderStatusID == p.OrderStatu.OrderStatusID).DESCRIPTION,
                OrderDate = p.OrderDateTime,
                AmountPaid = p.AmountPaid

            }).ToList());
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "CustomerRentalsReport.pdf");
        }
        public ActionResult OnlineSalesRpt()
        {
            return View();
        }

    }
}