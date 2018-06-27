using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using La_Game.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace La_Game.Controllers
{
    /// <summary>  
    /// Authentication Controller.    
    /// </summary>  
    public class AuthenticationController : Controller
    {
        // Database Context
        private LaGameDBContext db = new LaGameDBContext();

        #region Login and Logout Functions
        /// <summary>  
        /// GET: /Authentication/Login  
        /// Go to the login page.
        /// </summary>  
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Verification     
            if (Request.IsAuthenticated)
            {
                // User is already logged in; redirect to accountdetail page
                return RedirectToAction("AccountDetails", "Authentication");
            }

            // Go to login page    
            return View();
        }

        /// <summary>  
        /// POST: /Authentication/Login    
        /// Try to log into the application using the data that was entered.
        /// </summary>  
        /// <param name="model"> The data that was entered. </param>  
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            // Check if the data is valid
            if (ModelState.IsValid)
            {
                // Check the email that was entered and get the hashed password
                var login = db.Members.Where(e => e.email == model.Email).FirstOrDefault();

                // Verification
                if (login != null && Crypto.VerifyHashedPassword(login.password, model.Password))
                {
                    if (login.isActive == 1)
                    {
                        try
                        {
                            // Setting  
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, login.email),
                                new Claim(ClaimTypes.Role, login.isAdmin.ToString())
                            };
                            var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                            var authenticationManager = Request.GetOwinContext().Authentication;

                            // Sign in   
                            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, claimIdenties);

                            // Redirect to Dashboard  
                            return RedirectToAction("Index", "Dashboard");
                        }
                        catch (Exception ex)
                        {
                            // Logging in failed
                            throw ex;
                        }
                    }
                    else
                    {
                        // Account has been deactivated 
                        ModelState.AddModelError(string.Empty, "Account has been deactivated, contact an admin for more information.");
                    }
                }
                else
                {
                    // Account information was invalid 
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                }
            }

            // If not valid, stay on the login page
            return View(model);
        }

        /// <summary>  
        /// POST: /Authentication/Logout    
        /// When you press the button, log out of the application.
        /// </summary>  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            try
            {
                // Get the authenticationManager and sign out    
                var authenticationManager = Request.GetOwinContext().Authentication;
                authenticationManager.SignOut();
            }
            catch (Exception ex)
            {
                // Logging out failed
                throw ex;
            }

            // Redirect to login page
            return RedirectToAction("Login", "Authentication");
        }
        #endregion

        #region Manage account
        /// <summary>
        /// GET: /Authentication/AccountDetails
        /// Go to the account page where the user can see their information and edit the settings.
        /// </summary>
        public ActionResult AccountDetails()
        {
            // Get member from database and go to dashboard
            Member member = db.Members.Where(u => u.email == User.Identity.Name).FirstOrDefault();
            return View(member);
        }

        /// <summary>
        /// GET: /Authentication/ChangePassword
        /// Go to a seperate page where the password can be changed.
        /// </summary>
        public ActionResult ChangePassword()
        {
            // Go to the ChangePassword page
            return View();
        }

        /// <summary>
        /// POST: /Authentication/ChangePassword
        /// Allows the user to change the password of their account.
        /// </summary>
        /// <param name="model"> Password data. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            // Check if the data is valid
            if (ModelState.IsValid)
            {
                // Get member from database and retrieve the hashed password
                Member member = db.Members.Where(u => u.email == User.Identity.Name).FirstOrDefault();
                var hashedPassword = member.password;

                // Verify the password
                if (Crypto.VerifyHashedPassword(hashedPassword, model.OldPassword))
                {
                    // Hash the new password and save it to the database
                    member.password = Crypto.HashPassword(model.NewPassword);
                    db.Entry(member).State = EntityState.Modified;
                    db.SaveChanges();

                    // Return to the previous page
                    return RedirectToAction("AccountDetails");
                }

                // Error    
                ModelState.AddModelError(string.Empty, "The entered password was incorrect.");
            }

            // If not valid, stay on the page
            return View(model);
        }
        #endregion

        /// <summary>  
        /// GET: /Authentication/Forbidden 
        /// Redirect to page when member is not authorized to perform the action.
        /// </summary>  
        public ActionResult Forbidden()
        {
            return View();
        }

        /// <summary>
        /// Dispose of the database connection.
        /// </summary>
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