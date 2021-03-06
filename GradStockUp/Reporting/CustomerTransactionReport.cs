using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using Newtonsoft.Json;

using Image = iTextSharp.text.Image;


namespace GradStockUp.Reporting
{
    public class CustomerTransactionReport
    {

        int _totalColumn = 5;
        Document doc;
        Font _FontStyle;
        PdfPTable pdfPtTable = new PdfPTable(5);
        PdfPCell pdfCell;
        MemoryStream ms = new MemoryStream();
        string path;
        List<RetrieveData> TransferItems = new List<RetrieveData>();
        List<CustomerDetails> customerD = new List<CustomerDetails>();
        public byte[] PrepareReport(List<RetrieveData> stockItems, string ImgPath, List<CustomerDetails> customerDetails)
        {
            TransferItems = stockItems;
            customerD = customerDetails;
            path = ImgPath;
            doc = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
            doc.SetPageSize(PageSize.A4);
            doc.SetMargins(20f, 20f, 20f, 20f);
            pdfPtTable.WidthPercentage = 100;
            pdfPtTable.HorizontalAlignment = Element.ALIGN_LEFT;
            _FontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();
            pdfPtTable.SetWidths(new float[] { 40f, 40f, 40f, 40f, 40f });


            this.ReportHeader();
            this.ReportBody();
            pdfPtTable.HeaderRows = 2;
            doc.Add(pdfPtTable);
            doc.Close();
            return ms.ToArray();
        }


        string CustonerName;
         string PurchaseOrderid;
           string RentalOrderId;
           string EmployeName;
        private void ReportHeader()
        {
            
            foreach(CustomerDetails b in customerD)
            {
                CustonerName = b.CustomerName;
                PurchaseOrderid = b.PurchaseID;
                RentalOrderId = b.RentalID;
                EmployeName = b.EmployeeName;


            }
                          
            _FontStyle = FontFactory.GetFont("Tahoma", 14f, 1);
            Image image = Image.GetInstance(path);
            image.ScaleAbsolute(800f, 90f);

            pdfCell = new PdfPCell(image);
            pdfPtTable.AddCell(pdfCell);
            pdfPtTable.CompleteRow();

            _FontStyle = FontFactory.GetFont("Tahoma", 14f, 1);
            pdfCell = new PdfPCell(new Phrase("Customer Transaction Report", _FontStyle));
            pdfCell.Colspan = _totalColumn;
            pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCell.Border = 0;
            pdfCell.BackgroundColor = BaseColor.WHITE;
            pdfCell.ExtraParagraphSpace = 0;
            pdfPtTable.AddCell(pdfCell);
            pdfPtTable.CompleteRow();

            _FontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
            pdfCell = new PdfPCell(new Phrase("Customer Name: "+ CustonerName, _FontStyle));
            pdfCell.Colspan = _totalColumn;
            pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfCell.Border = 0;
            pdfCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            pdfCell.ExtraParagraphSpace = 0;
            pdfPtTable.AddCell(pdfCell);
            pdfPtTable.CompleteRow();

            _FontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
            pdfCell = new PdfPCell(new Phrase("Order Date:" + DateTime.Now, _FontStyle));
            pdfCell.Colspan = _totalColumn;
            pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfCell.Border = 0;
            pdfCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            pdfCell.ExtraParagraphSpace = 0;
            pdfPtTable.AddCell(pdfCell);
            pdfPtTable.CompleteRow();
        }

        private void ReportBody()
        {

            Boolean Rentalflag = false;
            Boolean RentailFlag = false;
            foreach (RetrieveData retrieve in TransferItems)
            {
                if (retrieve.Department == "Rental")
                    Rentalflag = true;
                if (retrieve.Department == "Rental")
                    RentailFlag = true;
            }

            if (Rentalflag)
            {

                _FontStyle = FontFactory.GetFont("Tahoma", 9f, 0);
                pdfCell = new PdfPCell(new Phrase("", _FontStyle));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.BackgroundColor = BaseColor.WHITE;
                pdfPtTable.AddCell(pdfCell);
                pdfPtTable.CompleteRow();

                _FontStyle = FontFactory.GetFont("Xanthie ", 14f, 1);
                pdfCell = new PdfPCell(new Phrase("Rented Items: "+ RentalOrderId, _FontStyle));
                pdfCell.Colspan = _totalColumn;
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.BackgroundColor = BaseColor.WHITE;
                pdfCell.ExtraParagraphSpace = 0;
                pdfPtTable.AddCell(pdfCell);
                pdfPtTable.CompleteRow();

                #region Table Header
                _FontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                pdfCell = new PdfPCell(new Phrase("Stock Barcode", _FontStyle));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pdfPtTable.AddCell(pdfCell);

                _FontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                pdfCell = new PdfPCell(new Phrase("Stock Type", _FontStyle));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pdfPtTable.AddCell(pdfCell);


                _FontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                pdfCell = new PdfPCell(new Phrase("Stock Colour", _FontStyle));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pdfPtTable.AddCell(pdfCell);

                _FontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                pdfCell = new PdfPCell(new Phrase("Stock Size", _FontStyle));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pdfPtTable.AddCell(pdfCell);

                _FontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                pdfCell = new PdfPCell(new Phrase("Price", _FontStyle));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pdfPtTable.AddCell(pdfCell);
                #endregion

                #region table body

                foreach (RetrieveData retrieve in TransferItems)
                {
                    if(retrieve.Department == "Rental")
                    {

                        _FontStyle = FontFactory.GetFont("Tahoma", 9f, 0);
                        pdfCell = new PdfPCell(new Phrase(retrieve.StockBarcode, _FontStyle));
                        pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfCell.BackgroundColor = BaseColor.WHITE;
                        pdfPtTable.AddCell(pdfCell);

                        _FontStyle = FontFactory.GetFont("Tahoma", 9f, 0);
                        pdfCell = new PdfPCell(new Phrase(retrieve.StockType, _FontStyle));
                        pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfCell.BackgroundColor = BaseColor.WHITE;
                        pdfPtTable.AddCell(pdfCell);

                        _FontStyle = FontFactory.GetFont("Tahoma", 9f, 0);
                        pdfCell = new PdfPCell(new Phrase(retrieve.StockColour, _FontStyle));
                        pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfCell.BackgroundColor = BaseColor.WHITE;
                        pdfPtTable.AddCell(pdfCell);

                        _FontStyle = FontFactory.GetFont("Tahoma", 9f, 0);
                        pdfCell = new PdfPCell(new Phrase(retrieve.StockSize, _FontStyle));
                        pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfCell.BackgroundColor = BaseColor.WHITE;
                        pdfPtTable.AddCell(pdfCell);

                        _FontStyle = FontFactory.GetFont("Tahoma", 9f, 0);
                        pdfCell = new PdfPCell(new Phrase("R"+retrieve.Price, _FontStyle));
                        pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfCell.BackgroundColor = BaseColor.WHITE;
                        pdfPtTable.AddCell(pdfCell);
                    }


                }
             
                #endregion
                pdfPtTable.CompleteRow();

            }

            if (RentailFlag)
            {
               
                _FontStyle = FontFactory.GetFont("Xanthie ", 14f, 1);
                pdfCell = new PdfPCell(new Phrase("Rented Items: " + PurchaseOrderid, _FontStyle));
                pdfCell.Colspan = _totalColumn;
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.BackgroundColor = BaseColor.WHITE;
                pdfCell.ExtraParagraphSpace = 0;
                pdfPtTable.AddCell(pdfCell);
                pdfPtTable.CompleteRow();


                #region Table Header
                _FontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                pdfCell = new PdfPCell(new Phrase("Stock Barcode:", _FontStyle));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pdfPtTable.AddCell(pdfCell);

                _FontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                pdfCell = new PdfPCell(new Phrase("Stock Type:", _FontStyle));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pdfPtTable.AddCell(pdfCell);


                _FontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                pdfCell = new PdfPCell(new Phrase("Stock Colour:", _FontStyle));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pdfPtTable.AddCell(pdfCell);

                _FontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                pdfCell = new PdfPCell(new Phrase("Stock Size:", _FontStyle));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pdfPtTable.AddCell(pdfCell);

                _FontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
                pdfCell = new PdfPCell(new Phrase("Price:", _FontStyle));
                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pdfPtTable.AddCell(pdfCell);
                #endregion

                #region table body

                foreach (RetrieveData retrieve in TransferItems)
                {
                    if (retrieve.Department == "Retail")
                    {

                        _FontStyle = FontFactory.GetFont("Tahoma", 9f, 0);
                        pdfCell = new PdfPCell(new Phrase(retrieve.StockBarcode, _FontStyle));
                        pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfCell.BackgroundColor = BaseColor.WHITE;
                        pdfPtTable.AddCell(pdfCell);

                        _FontStyle = FontFactory.GetFont("Tahoma", 9f, 0);
                        pdfCell = new PdfPCell(new Phrase(retrieve.StockType, _FontStyle));
                        pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfCell.BackgroundColor = BaseColor.WHITE;
                        pdfPtTable.AddCell(pdfCell);

                        _FontStyle = FontFactory.GetFont("Tahoma", 9f, 0);
                        pdfCell = new PdfPCell(new Phrase(retrieve.StockColour, _FontStyle));
                        pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfCell.BackgroundColor = BaseColor.WHITE;
                        pdfPtTable.AddCell(pdfCell);

                        _FontStyle = FontFactory.GetFont("Tahoma", 9f, 0);
                        pdfCell = new PdfPCell(new Phrase(retrieve.StockSize, _FontStyle));
                        pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfCell.BackgroundColor = BaseColor.WHITE;
                        pdfPtTable.AddCell(pdfCell);

                        _FontStyle = FontFactory.GetFont("Tahoma", 9f, 0);
                        pdfCell = new PdfPCell(new Phrase("R"+retrieve.Price, _FontStyle));
                        pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfCell.BackgroundColor = BaseColor.WHITE;
                        pdfPtTable.AddCell(pdfCell);


                    }
                   
                }
                #endregion
                pdfPtTable.CompleteRow();




            }


           

        }
    }
}