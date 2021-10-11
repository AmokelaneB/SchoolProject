/*==============================================================*/
/* DBMS name:      SAP SQL Anywhere 16                          */
/* Created on:     2020/08/05 00:41:34                          */
/*==============================================================*/


CREATE DATABASE CompdreDb;



/*==============================================================*/
/* Table: EMPLOYEETYPE                                          */
/*==============================================================*/
create table EmployeeType 
(
   EmployeeTypeID      int IDENTITY(1,1) Primary KEY not null ,
   DESCRIPTION          varchar(100)                   null,

);

/*==============================================================*/
/* Table: EMPLOYEE                                              */
/*==============================================================*/
create table Employee
(
   EmployeeID           int IDENTITY(1,1) Primary KEY not null ,
   EmployeeTypeID       int                        not null,
   Name                 varchar(50)                    null,
   Surname              varchar(50)                    null,
   PhoneNumber          numeric(10)                    null,
   Email                varchar(100)                   null,
   EmployeeAddress      varchar(100)                   null,
     FOREIGN KEY(EmployeeTypeID)
		REFERENCES EmployeeType(EmployeeTypeID)
);

/*==============================================================*/
/* Table: ORGANISATION                                          */
/*==============================================================*/
create table Organization 
(
   OrganizationID     int IDENTITY(1,1) Primary KEY not null ,
   Name                 varchar(100)          not         null,
   Address              varchar(100)           not        null,
   Contact              numeric(10)            not        null,
   RegistrationNO       varchar(50)           not         null,
);


/*==============================================================*/
/* Table: CUSTOMER                                              */
/*==============================================================*/
create table Customer 
(
   CustomerID           int IDENTITY(1,1) Primary KEY not null ,
   OrganizationID       int                        null,
   CustomerName         varchar(20)                 not   null,
   CustomerSurname      varchar(20)                not    null,
   CustAddress          varchar(100)             not      null,
   IDnumber             varchar(13)             not       null,
   Email                varchar(100)           not        null,
   PhoneNumber          numeric(10)           not         null,
   NextoFKin          varchar(50)             not       null,
   kinPhone         numeric(10)               not     null,
  FOREIGN KEY(OrganizationID)
		REFERENCES Organization(OrganizationID)
);


/*==============================================================*/
/* Table: "USER"                                                */
/*==============================================================*/
create table Users 
(
   UserID              int IDENTITY(1,1) Primary KEY not null ,
   CustomerID          int         not null,
   EmployeeID           integer                        not null,
   UserName             varchar(20)                    null,
   UserPassword         varchar(15)                    null,
   DateCreated         timestamp                       null,
 FOREIGN KEY (EmployeeID)
    REFERENCES Employee(EmployeeID),
 FOREIGN KEY (CustomerID)
    REFERENCES Customer(CustomerID)
);

/*==============================================================*/
/* Table: USERROLE                                              */
/*==============================================================*/
create table UserRole
(
   UserRoleID          int IDENTITY(1,1) Primary KEY not null ,
   UserID               int                        null,
   UserDescription          varchar(50)          null,
   FOREIGN KEY (UserID)
    REFERENCES Users(UserID)
  
);
/*==============================================================*/
/* Table: AUDITTRIAL                                            */
/*==============================================================*/

create table AuditTrial 
(
   AuditID		int IDENTITY(1,1) Primary KEY not null ,
   UserID               int   not null,
   LoginDate       datetime    not null,
   LogoutDate          datetime        not    null,
   TaskPerfomed        varchar(100) not  null,
   FOREIGN KEY (UserID)
    REFERENCES Users(UserID)
);
/*==============================================================*/
/* Table: BACKORDERSTATUS                                       */
/*==============================================================*/

create table BackOrderStatus 
(
   BackOrderStatusID    int IDENTITY(1,1) Primary KEY not null ,
   BackOrderDescription    varchar(30)      not  null,

);

/*==============================================================*/
/* Table: BANKS                                                 */
/*==============================================================*/
create table Bank 
(
   BankID     int IDENTITY(1,1) Primary KEY not null ,
   BankName   varchar(50)   not  null,
   BranchCode int  not     null,
  
);


/*==============================================================*/
/* Table: COLOUR                                                */
/*==============================================================*/
create table Colour 
(
   ColourID          int IDENTITY(1,1) Primary KEY not null ,
   ColourName            varchar(100)    not     null,
   
);
/*==============================================================*/
/* Table: SIZE                                                  */
/*==============================================================*/
create table Size 
(
   SizeID     int IDENTITY(1,1) Primary KEY not null ,
   SizeDescription      varchar(50)     not null,

);


/*==============================================================*/
/* Table: STATUS                                                */
/*==============================================================*/
create table Status 
(
   StockStatusID      int IDENTITY(1,1) Primary KEY not null ,
   StatusDescription          varchar(20)   not     null,

);

/*==============================================================*/
/* Table: STOCKSTATE                                            */
/*==============================================================*/
create table StockState 
(
   StockStateID        int IDENTITY(1,1) Primary KEY not null ,
   STATE                varchar(100)                   null,

);

/*==============================================================*/
/* Table: STOCKTYPE                                             */
/*==============================================================*/
create table StockType 
(
   StockTypeID         int IDENTITY(1,1) Primary KEY not null ,
   DESCRIPTION          varchar(50)                    null,
 
);

/*==============================================================*/
/* Table: STOCKTYPESIZELINE                                     */
/*==============================================================*/
create table StockTypeSizeLine 
(
   StockTypeID          int                        not null,
   SizeID               int                        not null,
   FOREIGN KEY(StockTypeID)
		REFERENCES StockType(StockTypeID),
 FOREIGN KEY(SizeID)
		REFERENCES Size(SizeID),
);
/*==============================================================*/
/* Table: DEPARTMENT                                            */
/*==============================================================*/
create table Department
(
   DepartmentID       int IDENTITY(1,1) Primary KEY not null ,
   DESCRIPTION          varchar(20)                    null,

);


/*==============================================================*/
/* Table: DOCUMENTS                                             */
/*==============================================================*/
create table Document
(
   DocumentID          int IDENTITY(1,1) Primary KEY not null ,
   DocumentName         varchar(20)                       null,
   DocumentDateTime     datetime                       null,

);
/*==============================================================*/
/* Table: CITY                                                  */
/*==============================================================*/
create table City 
(
   CityID            int IDENTITY(1,1) Primary KEY not null ,
  CityName            varchar(100)                   null,
);


/*==============================================================*/
/* Table: ESTABLISHMENT                                         */
/*==============================================================*/
create table Establishment 
(
   EstablishmentID      int IDENTITY(1,1) Primary KEY not null ,
   EstaName             varchar(50)                    null,

);
/*==============================================================*/
/* Table: LOCATION                                              */
/*==============================================================*/
create table Location 
(
   LocationID         int IDENTITY(1,1) Primary KEY not null ,
   DESCRIPTION          varchar(100)                   null,

);
/*==============================================================*/
/* Table: DELIVERYSTATUS                                        */
/*==============================================================*/
create table DeliveryStatus 
(
   DeliveryStatusID    int IDENTITY(1,1) Primary KEY not null ,
   DESCRIPTION          varchar(100)                   null,

);

/*==============================================================*/
/* Table: DELIVERY                                              */
/*==============================================================*/
create table Delivery 
(
   DeliveryID         int IDENTITY(1,1) Primary KEY not null ,
   DeliveryStatusID     integer                        not null,
   DeliveryDateTime     datetime                       null,
   DeliveryAddress              varchar(100)                   null,
   FOREIGN KEY(DeliveryStatusID)
		REFERENCES DeliveryStatus(DeliveryStatusID)
);





/*==============================================================*/
/* Table: ESTABLISHMENTSTOCKTYPELINE                            */
/*==============================================================*/
create table EstablishmentStockTypeLine 
(
   StockTypeID          int                        not null,
   EstablishmentID      int                        not null,
 
 FOREIGN KEY(StockTypeID)
		REFERENCES StockType(StockTypeID),
FOREIGN KEY(EstablishmentID)
		REFERENCES Establishment(EstablishmentID),
);

/*==============================================================*/
/* Table: FACULTY                                               */
/*==============================================================*/
create table Faculty 
(
   FacultyID            int IDENTITY(1,1) Primary KEY not null ,
   Description          varchar(50)                    null,

);

/*==============================================================*/
/* Table: QUALIFICATION                                         */
/*==============================================================*/
create table Qualification 
(
   QualificationID      int IDENTITY(1,1) Primary KEY not null ,
   QualificationName   varchar(50)                    null,

);


/*==============================================================*/
/* Table: QUALIFICATIONTYPE                                     */
/*==============================================================*/
create table QualificationType
(
   QualificationTypeID  int IDENTITY(1,1) Primary KEY not null ,
   QualificationType     varchar(100)                   null,
 
);
/*==============================================================*/
/* Table: INSTITUTION                                           */
/*==============================================================*/
create table Institution 
(
   InstitutionID        int IDENTITY(1,1) Primary KEY not null ,
   CityID               int                        null,
   InstitutionName      varchar(100)                   null,
   Address              varchar(100)                   null,
	 FOREIGN KEY(CityID)
		REFERENCES City(CityID)
);

/*==============================================================*/
/* Table: INSTITUTIONFACULTY                                    */
/*==============================================================*/
create table InstitutionFaculty 
(
   FacultyID            int                        not null,
   InstitutionID        int                        not null,
   ColourID             int                        not null,
   QualificationID      int                        null,
	FOREIGN KEY(FacultyID)
		REFERENCES Faculty(FacultyID),
 FOREIGN KEY(ColourID)
		REFERENCES Colour(ColourID),
FOREIGN KEY(InstitutionID)
		REFERENCES Institution(InstitutionID),
FOREIGN KEY(QualificationID)
		REFERENCES Qualification(QualificationID),
);

/*==============================================================*/
/* Table: INSTITUTIONLINE                                       */
/*==============================================================*/
create table INSTITUTIONLINE 
(
   StockTypeID          int                        not null,
   EstablishmentID      int                        not null,
   InstitutionID        int                        not null,
  
FOREIGN KEY(InstitutionID)
		REFERENCES Institution(InstitutionID),

FOREIGN KEY(StockTypeID)
		REFERENCES StockType(StockTypeID),

     FOREIGN KEY(EstablishmentID)
		REFERENCES Establishment(EstablishmentID),


);

/*==============================================================*/
/* Table: INSTITUTIONLINEQUALIFICATION                          */
/*==============================================================*/
create table InstitutionLineQualification
(
   StockTypeID          int                        not null,
   EstablishmentID      int                        not null,
   InstitutionID        int                        not null,
   QualificationTypeID  int                        null,
		FOREIGN KEY(StockTypeID)
			REFERENCES StockType(StockTypeID),
		FOREIGN KEY(EstablishmentID)
			REFERENCES Establishment(EstablishmentID),
		FOREIGN KEY(InstitutionID)
			REFERENCES Institution(InstitutionID),
		FOREIGN KEY(QualificationTypeID)
			REFERENCES QualificationType(QualificationTypeID),

);

/*==============================================================*/
/* Table: ORDERSTATUS                                           */
/*==============================================================*/
create table OrderStatus 
(
   OrderStatusID       int IDENTITY(1,1) Primary KEY not null ,
   DESCRIPTION          varchar(100)                   null,

);


/*==============================================================*/
/* Table: STOCKDESCRIPTIONS                                     */
/*==============================================================*/

create table StockDescription
(
   StockDescriptionID  int IDENTITY(1,1) Primary KEY not null ,
   SizeID               int                        not null,
   StockTypeID          int                        not null,
   ColourID             int                     not   null,
   RENTALSTOCKLEVEL     int                  not      null,
   RETAILSTOCKLEVEL     int                   not     null,
   RETAILTHRESHOLD      int                  not      null,
   RENTALTHRESHOLD      int                 not       null,
   FLAG                 bit              not              null,
	 FOREIGN KEY(SizeID)
		REFERENCES Size(SizeID),
	FOREIGN KEY(StockTypeID)
		REFERENCES StockType(StockTypeID),
	FOREIGN KEY(ColourID)
		REFERENCES Colour(ColourID)
);

/*==============================================================*/
/* Table: STOCK                                                 */
/*==============================================================*/
create table Stock 
(
   StockID              int IDENTITY(1,1) Primary KEY not null ,
   StockStatusID        int  not null,
   StockDescriptionID   int   not  null,
   StockStateID         int   not  null,
   LocationID           int   not    null,
   DepartmentID         int    not   null,
     FOREIGN KEY(StockStatusID)
		REFERENCES Status(StockStatusID),
	 FOREIGN KEY(StockDescriptionID)
		REFERENCES StockDescription(StockDescriptionID),
	 FOREIGN KEY(StockStateID)
		REFERENCES StockState(StockStateID),
	FOREIGN KEY(LocationID)
		REFERENCES Location(LocationID),
	 FOREIGN KEY(DepartmentID)
		REFERENCES Department(DepartmentID)
);

/*==============================================================*/
/* Table: PAYMENTMETHOD                                         */
/*==============================================================*/
create table PaymentMethod 
(
   PaymentMethodID     int IDENTITY(1,1) Primary KEY not null ,
   Description         varchar(50)                    null,

);
/*==============================================================*/
/* Table: CUSTOMERRENTAL                                        */
/*==============================================================*/
create table CustomerRental 
(
   CustomerRentalID    int IDENTITY(1,1) Primary KEY not null ,
   CustomerID           int                        not null,
   OrderStatusID        int                        not null,
   DeliveryID           int                        not null,
   PaymentMethodID      int                        not null,
   OrderDateTime        timestamp                      null,
   AmountPaid           money                          null,
  FOREIGN KEY(CustomerID)
		REFERENCES Customer(CustomerID),
  FOREIGN KEY(OrderStatusID)
		REFERENCES OrderStatus(OrderStatusID),
   FOREIGN KEY(DeliveryID)
		REFERENCES Delivery(DeliveryID),
  FOREIGN KEY(PaymentMethodID)
		REFERENCES PaymentMethod(PaymentMethodID)
);


/*==============================================================*/
/* Table: CUSTOMERRENTALLINE                                    */
/*==============================================================*/
create table CustomerRentalLine 
(
   StockID              int                        not null,
   CustomerRentalID     int                        not null,
   PriceID              int                        not null,

  FOREIGN KEY(StockID)
   REFERENCES Stock(StockID),
  FOREIGN KEY(CustomerRentalID)
   REFERENCES CustomerRental(CustomerRentalID)	

);


/*==============================================================*/
/* Table: CHECKINLAUNDRY                                        */
/*==============================================================*/
create table CheckInLaundry 
(
   CheckInLaundryID   int IDENTITY(1,1) Primary KEY not null ,
   CheckInDate     timestamp   not   null,
  Quantity            int     not    null,
 
);

/*==============================================================*/
/* Table: STOCKLINE                                             */
/*==============================================================*/
create table StockLine 
(
   FacultyID            int                        not null,
   InstitutionID        int                        not null,
   ColourID             int                        not null,
   StockDescriptionID   int                        not null,
 FOREIGN KEY(ColourID)
		REFERENCES Colour(ColourID),
 FOREIGN KEY(StockDescriptionID)
		REFERENCES StockDescription(StockDescriptionID),
FOREIGN KEY(InstitutionID)
		REFERENCES Institution(InstitutionID),
 FOREIGN KEY(FacultyID)
		REFERENCES Faculty(FacultyID),
);

/*==============================================================*/
/* Table: STOCKRETURNS                                          */
/*==============================================================*/
create table StockReturn 
(
   StockReturnID     int IDENTITY(1,1) Primary KEY not null ,
   ReturnDate               timestamp                      null,

);


/*==============================================================*/
/* Table: STOCKRETURNLINE                                       */
/*==============================================================*/
create table StockReturnLine 
(
   StockReturnID        int                        not null,
   StockID              int                        not null,
   CustomerRentalID     int                        not null,
   StockStateID         int                        not null,
   FOREIGN KEY(StockReturnID)
		REFERENCES StockReturn(StockReturnID),
	FOREIGN KEY(StockID)
		REFERENCES Stock(StockID),
	FOREIGN KEY(CustomerRentalID)
		REFERENCES CustomerRental(CustomerRentalID),
FOREIGN KEY(StockStateID)
		REFERENCES StockState(StockStateID)


);

/*==============================================================*/
/* Table: SUPPLIER                                              */
/*==============================================================*/
create table Supplier 
(
   SupplierID           int IDENTITY(1,1) Primary KEY not null ,
   SupplierName     varchar(50)               not     null,
   ADDRESS              varchar(100)                   null,
   PHONENUMBER          numeric(10)              not      null,
   EMAIL                varchar(100)                   null,

);


/*==============================================================*/
/* Table: SUPPLIERORDER                                         */
/*==============================================================*/
create table SupplierOrder 
(
   SupplierOrderID     int IDENTITY(1,1) Primary KEY not null ,
   SupplierID           int                        not null,
   OrderStatusID        int                        not null,
   TotalCost            money                          null,
   DateCreated          datetime                       null,
   FOREIGN KEY(SupplierID)
		REFERENCES Supplier(SupplierID),
  FOREIGN KEY(OrderStatusID)
		REFERENCES OrderStatus(OrderStatusID)


   
);


/*==============================================================*/
/* Table: SUPPLIERORDERLINE                                     */
/*==============================================================*/
create table SupplierOrderLine 
(
   StockDescriptionID   integer                        not null,
   SupplierOrderID      integer                        not null,
   Quantity             integer                        null,
   FOREIGN KEY(SupplierOrderID)
		REFERENCES SupplierOrder(SupplierOrderID),
 FOREIGN KEY(StockDescriptionID)
		REFERENCES StockDescription(StockDescriptionID),
);
/*==============================================================*/
/* Table: BACKORDER                                             */
/*==============================================================*/

create table BackOrder
(
   BackOrderID   int IDENTITY(1,1) Primary KEY not null ,
   BackOrderStatusID    int    not  null,
      SupplierOrderID   int  not null,
   QuantityOutstanding  int    not  null,
  FOREIGN KEY(BackOrderStatusID)
		REFERENCES BackOrderStatus(BackOrderStatusID),
   FOREIGN KEY(SupplierOrderID)
		REFERENCES SupplierOrder(SupplierOrderID)
);

/*==============================================================*/
/* Table: BACKORDERLINE                                         */
/*==============================================================*/

create table BackOrderLine 
(
   StockDescriptionID   int   not null,
   BackOrderID          int   not null,
   Quantity             int    not   null,
     FOREIGN KEY(StockDescriptionID)
		REFERENCES StockDescription(StockDescriptionID),
	FOREIGN KEY(BackOrderID)
		REFERENCES BackOrder(BackOrderID)  
);


/*==============================================================*/
/* Table: LAUNDRYLINE                                           */
/*==============================================================*/
create table LaundryLine 
(
   StockID              int                        not null,
   CheckInLaundryID     int                        not null,
   StockStatusID        int                        null,

   	 FOREIGN KEY(StockID)
		REFERENCES Stock(StockID),

	FOREIGN KEY(CheckInLaundryID)
		REFERENCES CheckInLaundry(CheckInLaundryID),

	 FOREIGN KEY(StockStatusID)
		REFERENCES Status(StockStatusID),
);





/*==============================================================*/
/* Table: OUTFIT                                                */
/*==============================================================*/
create table Outfit 
(
   OutfitID            int IDENTITY(1,1) Primary KEY not null ,
   StockTypeID          int                        null,
   InstitutionID        int                        null,

   	FOREIGN KEY(StockTypeID)
		REFERENCES StockType(StockTypeID),

  FOREIGN KEY(InstitutionID)
		REFERENCES Institution(InstitutionID)
);





/*==============================================================*/
/* Table: PRICE                                                 */
/*==============================================================*/
create table Price 
(
   PriceID              int IDENTITY(1,1) Primary KEY not null ,
   RetailPrice          money                          null,
   StockTypeID          int                        not null,
   EstablishmentID      int                        null,
   InstitutionID        int                        null,
   RentalPrice          money                          null,
   PriceDate            date                           null,

   	FOREIGN KEY(StockTypeID)
		REFERENCES StockType(StockTypeID),

	FOREIGN KEY(EstablishmentID)
		REFERENCES Establishment(EstablishmentID),

	FOREIGN KEY(InstitutionID)
		REFERENCES Institution(InstitutionID)

);



/*==============================================================*/
/* Table: PURCHASEORDER                                         */
/*==============================================================*/
create table PurchaseOrder 
(
   PurchaseOrderID     int IDENTITY(1,1) Primary KEY not null ,
   CustomerID           int                        not null,
   OrderStatusID        int                        not null,
   PaymentMethodID      int                        not null,
   DeliveryID           int                        not null,
   OrderDateTime        datetime                       null,
   AmountPaid           money                          null,
 
 	 FOREIGN KEY(CustomerID)
		REFERENCES Customer(CustomerID),

	FOREIGN KEY(OrderStatusID)
		REFERENCES OrderStatus(OrderStatusID),

	FOREIGN KEY(PaymentMethodID)
		REFERENCES PaymentMethod(PaymentMethodID),

	 FOREIGN KEY(DeliveryID)
		REFERENCES Delivery(DeliveryID)


);



/*==============================================================*/
/* Table: PURCHASEORDERLINE                                     */
/*==============================================================*/
create table PurchaseOrderLine 
(
   PurchaseOrderID      int                       not null,
   StockID              int                       not null,
   PriceID              int                       not null,
   
FOREIGN KEY(PurchaseOrderID)
		REFERENCES PurchaseOrder(PurchaseOrderID),
 FOREIGN KEY(StockID)
		REFERENCES Stock(StockID),
FOREIGN KEY(PriceID)
		REFERENCES Price(PriceID)

);




/*==============================================================*/
/* Table: QUALIFICATIONSTOCKLINE                                */
/*==============================================================*/
create table QualificationStockLine 
(
   StockDescriptionID   int                        not null,
   QualificationTypeID  int                        not null,
  
  FOREIGN KEY(StockDescriptionID)
		REFERENCES StockDescription(StockDescriptionID),
FOREIGN KEY(QualificationTypeID)
		REFERENCES QualificationType(QualificationTypeID),

);



/*==============================================================*/
/* Table: REFUNDSTATUS                                          */
/*==============================================================*/
create table RefundStatus 
(
   RefundStatusID       int IDENTITY(1,1) Primary KEY not null ,
   Description          varchar(100)                   null,

);
/*==============================================================*/
/* Table: REFUNDS                                               */
/*==============================================================*/
create table Refund 
(
   RefundID             int IDENTITY(1,1) Primary KEY not null ,
   BankID               int                        not null,
   RefundStatusID       int                        not null,
   CustomerRentalID     int                        null,
   DateCreated          timestamp                      null,
   SubmitDate           date                           null,
   AccountNUmber        numeric(18)                    null,

FOREIGN KEY(BankID)
		REFERENCES Bank(BankID),

 FOREIGN KEY(CustomerRentalID)
		REFERENCES CustomerRental (CustomerRentalID),

 FOREIGN KEY(RefundStatusID)
		REFERENCES RefundStatus(RefundStatusID),

);

/*==============================================================*/
/* Table: RETURNFROMLAUNDRY                                     */
/*==============================================================*/
create table ReturnFromLaundry
(
   ReturnID   int IDENTITY(1,1) Primary KEY             not null ,
   LaundryReturnDate               datetime                      not null,
   Quantity             int                       not  null,

);



/*==============================================================*/
/* Table: RETURNLINE                                            */
/*==============================================================*/
create table ReturnLine 
(
   ReturnID             int                        not null,
   StockID              int                        not null,
   CheckInLaundryID     int                        not null,
  
	FOREIGN KEY(ReturnID)
		REFERENCES ReturnFromLaundry(ReturnID),
	FOREIGN KEY(StockID)
		REFERENCES Stock(StockID),

 FOREIGN KEY(CheckInLaundryID)
		REFERENCES CheckInLaundry(CheckInLaundryID),

);




/*==============================================================*/
/* Table: WRITEOFF                                              */
/*==============================================================*/
create table WriteOff 
(
   WriteOffID         int IDENTITY(1,1) Primary KEY not null ,
   WriteOffDate            datetime                       null,

);


/*==============================================================*/
/* Table: WRITEOFFLINE                                          */
/*==============================================================*/
create table WriteOffLine 
(
   WriteOffID           int                        not null,
   StockID              int                        not null,
   Reason               varchar(100)                   null,
  
 FOREIGN KEY(WriteOffID)
		REFERENCES WriteOff(WriteOffID),

 FOREIGN KEY(StockID)
		REFERENCES Stock(StockID),


);
