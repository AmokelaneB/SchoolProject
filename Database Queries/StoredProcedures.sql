CREATE PROCEDURE GetEmployeeID @UserEmail nvarchar(200), @UserPassword nvarchar(200)
AS
SELECT EmplyID FROM AppUsers WHERE UserEmail = @UserEmail AND UserPassword = @UserPassword
GO

CREATE PROCEDURE GetEmployeeDetails @EmplyID nvarchar(50)
AS
SELECT * FROM Employee WHERE EmplyID = @EmplyID 
GO

select * from StockState

select * from location
select * from CustomerRentalLine
select * from StockReturn
insert into Status(StatusDescription) Values('Lost')

create table StockReturnLine
(


)

Create table StockReturn
(

)