using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebStore_Contrast.Models.Data;
using WebStore_Contrast.Models.ViewModels.Account;

namespace WebStore_Contrast.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: account/create-account/model
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            // Check the model in validity
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            // Check the compliance of the password (відповідність)
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password don't match!");
                return View("CreateAccount", model);
            }

            using (Db db = new Db())
            {
                // Check the name in unicity 
                if (db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", $"Username {model.Username} is taken.");
                    model.Username = "";
                    return View("CreateAccount", model);
                }

                // Create the example of class UserDTO
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    Username = model.Username,
                    Password = model.Password
                };

                // Add data to model 
                db.Users.Add(userDTO);

                // Save data
                db.SaveChanges();

                // Add the role of user
                int id = userDTO.Id;

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2   // Hard-Code ))))))
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            // Write message in TempData
            TempData["SM"] = "You are now registered and can login.";

            // Redirect user
            return RedirectToAction("Login");
        }

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            // Confirm that the user is not authorized
            string userName = User.Identity.Name;

            if (!string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("user-profile");
            }

            // Return View()
            return View();
        }

        // POST: Account/Login/model
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            // Check the model in validity
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check a user in validity
            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                    isValid = true;

                if (!isValid)
                {
                    ModelState.AddModelError("", "Invalid username of password.");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
            }
        }

        // GET: /account/logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }  

        public ActionResult UserNavPartial()
        {
            // Get the name of user
            string userName = User.Identity.Name;

            // Assign the model
            UserNavPartialVM model = new UserNavPartialVM();

            using (Db db = new Db())
            {
                // Get the user
                UserDTO dto = new UserDTO();
                dto = db.Users.FirstOrDefault(x => x.Username == userName);

                // Fill the model ot data from context (DTO)
                if (dto != null)
                {
                    model = new UserNavPartialVM()
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName
                    };
                }
            }

            // Return the PartialView() with model
            return PartialView(model);
        }

        // GET: /account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            // Get the name of user
            string userName = User.Identity.Name;

            // Assign the model
            UserProfileVM model;

            using (Db db = new Db())
            {
                // Get the user
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                // Initialize the model to data
                model = new UserProfileVM(dto);
            }

            // Return the View()
            return View("UserProfile", model);
        }

        // POST: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfileVM model)
        {
            bool userNameIsChanged = false;

            // Check the model in validity
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            // Check the password (if user replace it)
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Passwords don't match.");
                    return View("UserProfile", model);
                }
            }

            using (Db db = new Db())
            {
                // Get the name of user
                string userName = User.Identity.Name;

                // Check, that the username has changed 
                if (userName != model.Username)
                {
                    userName = model.Username;
                    userNameIsChanged = true;
                }

                // Check the name in unicity
                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.Username == userName))
                {
                    ModelState.AddModelError("", $"Username {model.Username} already exists.");
                    model.Username = "";
                    return View("UserProfile", model);
                }

                // Edit model for the context of data
                UserDTO dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAdress = model.EmailAdress;
                dto.Username = model.Username;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }

                // Save changes
                db.SaveChanges();
            }

            // Set the message in TempData
            TempData["SM"] = "You have edited your profile!";

            if (!userNameIsChanged)
            {
                // Return the PartialView() with model
                return View("UserProfile", model);
            }
            else
            {
                return RedirectToAction("Logout");
            }
        }
    }
}