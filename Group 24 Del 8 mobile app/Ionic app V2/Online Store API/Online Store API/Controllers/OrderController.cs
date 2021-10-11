using Online_Store_API.Communication;
using Online_Store_API.Models;
using Online_Store_API.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Data.Entity;
using System.Web.Http.Description;

namespace Online_Store_API.Controllers
{
    [EnableCors(origins: "http://localhost:8100", headers: "*", methods: "*")]
    public class OrderController : ApiController
    {

        GradStockUpEntities db = new GradStockUpEntities();

        [Route("api/Order/Add")]
        [HttpPost]
        public IHttpActionResult AddOrder(OrderResource model)
        {

            if (ModelState.IsValid)
            {

                var customer = db.Customers.FirstOrDefault(item => item.Email.ToLower().Equals(model.Email.ToLower()));
                var status = db.OrderStatus.FirstOrDefault(x => x.OrderStatusID.Equals(1));

                if (customer == null)
                {
                    return BadRequest();

                }

                var items = @"";
                foreach (var item in model.OrderItems)
                {


                    if (item.ordertype.Equals("Rental"))
                    {
                        //Rental order

                        var order = new OnlineOrder
                        {
                            OrderItem = item.item,
                            OrderType = item.ordertype,
                            Price = item.price,
                            Quantity = item.quantity,
                            CustomerID = customer.CustomerID,
                            OrderStatusID = status.OrderStatusID,
                            OrderDate = DateTime.Now,
                            Size = item.size,

                        };
                        db.OnlineOrders.Add(order);
                        db.SaveChanges();

                        var emailItem = @" 
                           <tr>
                           <td> {0} </td>
                           <td></td>
                           <td> {1} </td>
                           <td></td>
                           <td> R {2} </td>
                           <td></td>
                           <td> {3} </td>
                           <td></td>
                           <td> {4} </td>
                           </tr>";
                        var formatedItem = String.Format(emailItem, item.item, item.size, item.price, item.quantity, item.ordertype);
                        items = items + formatedItem;



                    }

                    if (item.ordertype.Equals("Retail"))
                    {
                        //Purchase order

                        var order = new OnlineOrder
                        {
                            OrderItem = item.item,
                            OrderType = item.ordertype,
                            Price = item.price,
                            Quantity = item.quantity,
                            CustomerID = customer.CustomerID,
                            OrderStatusID = status.OrderStatusID,
                            OrderDate=DateTime.Now,
                            Size = item.size,
                        };
                        db.OnlineOrders.Add(order);
                        db.SaveChanges();

                        var emailItem = @" 
                           <tr>
                           <td> {0} </td>
                           <td></td>
                            <td> {1} </td>
                           <td></td>
                           <td> R {2} </td>
                           <td></td>
                           <td> {3} </td>
                           <td></td>
                           <td> {4} </td>
                           </tr>";
                        var formatedItem = String.Format(emailItem, item.item, item.size, item.price, item.quantity, item.ordertype);
                        items = items + formatedItem;

                    }
                }



                var emailMessage = @"
                    <p>Customer Name: {0}</p>
                    <p>Customer Email Address: {1}</p>
                    <p>Customer Contact Number: {2} </p>
                    <p>Customer Delivery Address: {3}</p>
                    <p></p>
                    <p><strong><u> Order Details </u></strong></p>
                    <table>
                    <body>
                    <tr>
                    <td><strong> Item </strong></td>
                    <td></td>
                    <td><strong> Size </strong></td>
                    <td></td>
                    <td><strong> Price </strong></td>
                    <td></td>
                    <td><strong> Quantity </strong></td>
                    <td></td>
                    <td><strong> Order Type </strong></td>
                    </tr>
                        {4}
                    </body>
                    </table>
                    <p> </p>
                    <p>Total Paid: {5} </p>
                         ";

                var formattedEmailMessage = String.Format(emailMessage, model.Name, model.Email, model.ContactNumber, model.Address, items, model.CardTotal);
                
                Email.SendPlaceOrderEmail("amokelanebaloh10@gmail.com", "Takunda", formattedEmailMessage);
                Email.SendPlaceOrderEmail(customer.Email, customer.CustomerName, formattedEmailMessage);
                return Ok();
            }
            return BadRequest();


        }

        [Route("api/Order/GetEstablishments")]
        [HttpGet]
        public IEnumerable<GetEstablishmentResource> GetEstablishments()
        {
            var establishments = db.Establishments
                .Include(item => item.StockTypes)
                .ToList();

            var stockTypes = db.StockTypes
                .Include(item => item.Establishments)
                .ToList();

            var sizes = db.Sizes
                .Include(item => item.StockTypes)
                .ToList();
            var prices = db.Prices.ToList();

            var stockdescription = db.StockDescriptions.ToList();

            var returnValue = establishments.Select(item => new GetEstablishmentResource
            {
                EstablishmentID = item.EstablishmentID,
                EstablishmentName = item.EstaName,
                StockTypes = stockTypes
                //.Where(type => type.EstablishmentID == item.EstablishmentID)
                .Select(stock => new GetStockTypeResource
                {
                    StockTypeID = stock.StockTypeID,
                    StockTypeName = stock.DESCRIPTION,

                    //EstablishmentID = stock.EstablishmentID,
                    RentalPrice = (prices.FirstOrDefault(price => price.StockTypeID == stock.StockTypeID) != null)
                    ? prices.FirstOrDefault(price => price.StockTypeID == stock.StockTypeID).RentalPrice
                    : Convert.ToDecimal(0),
                    RetailPrice = (prices.FirstOrDefault(price => price.StockTypeID == stock.StockTypeID) != null)
                    ? prices.FirstOrDefault(price => price.StockTypeID == stock.StockTypeID).RetailPrice
                    : Convert.ToDecimal(0),
                    //RentalStockLevel = stock.RentalStockLevel,
                    //RetailStockLevel = stock.RetailStockLevel,
                    //StockTypeName = stock.StockTypeName,
                    Sizes = sizes
                    //.Where(size => size.StockTypeID == stock.StockTypeID)
                    .Select(currentSize => new GetSizeResource
                    {
                        SizeID = currentSize.SizeID,
                        //StockTypeID = currentSize.StockTypeID,
                        SizeName = currentSize.SizeDescription,

                        Prices = prices
                    .Select(currentPrice => new GetPriceResource
                    {
                        PriceID = currentPrice.PriceID,
                        RentalPrice = currentPrice.RentalPrice,
                        RetailPrice = currentPrice.RetailPrice,

                        StockDescription = stockdescription
                    .Select(currentDes => new GetStockDescriptionResource
                    {
                        StockDescriptionID = currentDes.StockDescriptionID,
                        RentalStockLevel = currentDes.RENTALSTOCKLEVEL,
                        RetailStockLevel = currentDes.RETAILSTOCKLEVEL
                    })
                    })

                    }).ToList()
                }).ToList()

            }).ToList();

            return returnValue;
        }

        [Route("api/Order/getInstitutions")]
        [HttpGet]
        public IEnumerable<GetInstitutionResource> GetInstitutions()
        {
            var institutions = db.Institutions.Include(item => item.Faculties).ToList();
            var facultyFromDb = db.Faculties.Include(item => item.Institutions).ToList();



            var returnValue = institutions.Select(item => new GetInstitutionResource
            {
                InstitutionID = item.InstitutionID,
                Address = item.Address,
                InstitutionName = item.InstitutionName,

                Faculties = facultyFromDb
                //.Where(current => (current.InstitutionID == item.InstitutionID))
                .Select(faculty => new GetFacultyResource
                {
                    FacultyID = faculty.FacultyID,
                    Description = faculty.Description,
                    //InstitutionID = faculty.InstitutionID
                }).ToList()
            }).ToList();

            return returnValue;
        }

        [Route("api/Order/getQualifications")]
        [HttpGet]
        public IEnumerable<GetQualificationResource> GetQualifications()
        {
            return db.Qualifications.Select(item => new GetQualificationResource
            {
                QualificationID = item.QualificationID,
                QualificationName = item.QualificationName
            }).ToList();
        }

        //Getting past orders details for customer 
        [Route("api/Order/PastOrders")]
        [HttpGet]
        public IEnumerable<GetPastOrders> PastOrders(OrderResource model)
        {

           
                var customer = db.Customers.FirstOrDefault(item => item.Email.ToLower().Equals(model.Email.ToLower()));
                var custorders = db.OnlineOrders.FirstOrDefault(xx => xx.CustomerID == customer.CustomerID);
                return db.OnlineOrders.Select(x => new GetPastOrders
                {
                    OrderItem= custorders.OrderItem,
                    Size= custorders.Size,
                    Quantity= custorders.Quantity,
                    OrderType= custorders.OrderType,
                    OrderDate= custorders.OrderDate
                    
                }).ToList();
            
        }
    }
}
