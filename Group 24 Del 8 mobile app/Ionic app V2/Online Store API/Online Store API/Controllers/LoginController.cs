using System;
using Online_Store_API.Models;
using System.Web.Http;
using System.Dynamic;
using System.Web.Http.Cors;
using Online_Store_API.VM;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Online_Store_API.Communication;
using System.Web.Http.Description;

namespace Online_Store_API.Controllers
{
    [EnableCors(origins: "http://localhost:8100", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
        
        GradStockUpEntities db = new GradStockUpEntities();
        
        [Route("api/Login/Registration")]
        [HttpPost]
        [AllowAnonymous]
        public object Registration(Register Reg)
        {

            //Check if provided  username does not exist
            var username = db.AppUsers.FirstOrDefault(item => item.UserEmail.ToLower().Equals(Reg.Email.ToLower()));

            if (username == null) //&&(password==null))
            {
                var hash = GenerateHash(Reg.userPassword);

                try
                {
                    Customer NC = new Customer();
                    AppUser usr = new AppUser();

                    if (NC.CustomerID == 0)
                    {

                        NC.CustomerName = Reg.CustomerName;
                        NC.CustomerSurname = Reg.CustomerSurname;
                        NC.IDnumber = Reg.IDnumber;
                        NC.Email = Reg.Email.ToLower();
                        NC.PhoneNumber = Reg.PhoneNumber;
                        NC.NextoFKin = Reg.NextoFKin;
                        NC.kinPhone = Reg.kinPhone;
                        NC.CustAddress = Reg.CustAddress;
                        db.Customers.Add(NC);
                        db.SaveChanges();

                        if (usr.AppUserID == 0)
                        {
                            usr.UserPassword = hash;
                            usr.UserEmail = Reg.Email.ToLower();
                           
                            //usr.EmployeeID = 0;
                            db.AppUsers.Add(usr);
                            db.SaveChanges();
                        }



                        return new Response { Status = "Success", Message = "Registration successful." };

                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return new Response { Status = "Error", Message = "You entered invalid information" };
            }
            //first if statement return
            return new { data = "Registration" };
        }


        //generating hash
        public static string GenerateHash(string inputString)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }
        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        //logging in a user
        [System.Web.Http.Route("api/Login/UserLogin")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult UserLogin(Login login)
        {
            var user = db.AppUsers.FirstOrDefault(item => item.UserEmail.ToLower().Equals(login.UserName.ToLower()));

            //user with provided username exists
            var message = "";//this will hold an error message.
            if (user != null)
            {
                //if user password matches the one in db
                //has user input and compare with hased password already in db
                var hashedpass = GenerateHash(login.UserPassword);
                if (user.UserPassword.Equals(hashedpass))
                {
                    //log user in. generate token 

                    string key = "my_secret_key_12345"; //Secret key which will be used later during validation    
                    var issuer = "http://mysite.com";  //normally this will be your site URL    

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    //Create a List of Claims, Keep claims name short    
                    var permClaims = new List<Claim>();
                    permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                    permClaims.Add(new Claim("username", user.UserEmail));

                    //Create Security Token object by giving required parameters    
                    var jwtToken = new JwtSecurityToken(issuer, //Issure    
                                    issuer,  //Audience    
                                    permClaims,
                                    expires: DateTime.Now.AddDays(1),
                                    signingCredentials: credentials);
                    var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                    
                    return Ok(new { token });
                }
                else
                {
                    message = "Incorrect username or password";
                    return Ok(new { message });
                }
            }

            message = "Please register";
            return Ok(new { message });
        }





        //Getting delivery details for payment 
        [Route("api/Login/DeliveryDetails/{Email}")]
        [HttpGet]
        //[ResponseType(typeof(Customer))]
        public IHttpActionResult DeliveryDetails(string Email)
        {
            Customer cust = db.Customers.FirstOrDefault(x => x.Email == Email);
            if (cust==null)
            {
                return NotFound();
            }
            return Ok(cust);
        }  
            
       
            
       

        [Route("api/Login/EditAddress")]
        [HttpPut]

        public IHttpActionResult Edit(Customer customer)
        {
            Customer cust = new Customer();
            cust = db.Customers.Find(customer.Email);
            if (cust != null)
            {
                cust.CustAddress = customer.CustAddress;
                db.SaveChanges();
            }
            return Ok();
        }

      


    }


    
}