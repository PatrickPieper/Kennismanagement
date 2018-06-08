using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
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

        /// <summary>
        /// All the functions that are needed to log in and out of the application.
        /// </summary>
        #region Login and Logout Functions
        /// <summary>  
        /// GET: /Authentication/Login  
        /// Go to the login page.
        /// </summary>  
        /// <param name="returnUrl"> Return URL string. </param> 
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            // Verification     
            if (Request.IsAuthenticated)
            {
                // User is already logged in  
                if (Url.IsLocalUrl(returnUrl))
                {
                    // Redirect to the given URL  
                    return Redirect(returnUrl);
                }

                // If the URL was invalid, redirect to home page  
                return RedirectToAction("Index", "Home");
            }

            // Go to login page    
            return View();
        }

        /// <summary>  
        /// POST: /Authentication/Login    
        /// Try to log into the application using the data that was entered.
        /// </summary>  
        /// <param name="model"> The data that was entered. </param>  
        /// <param name="returnUrl"> Return URL string. </param> 
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            // Check if the data is valid
            if (ModelState.IsValid)
            {
                // Try to find the loginInfo in the database   
                var loginInfo = db.Members.Where(s => s.username == model.Username && s.password == model.Password);

                // Verification.    
                if (loginInfo != null && loginInfo.Count() > 0)
                {
                    try
                    {
                        // Initialization.    
                        var logindetails = loginInfo.First();

                        // Setting  
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, logindetails.username)
                        };
                        var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                        var authenticationManager = Request.GetOwinContext().Authentication;

                        // Sign In.    
                        authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, claimIdenties);

                        // Redirect to Dashboard  
                        return RedirectToAction("Dashboard", "Home");
                    }
                    catch (Exception ex)
                    {
                        // Logging in failed
                        throw ex;
                    }
                }
                else
                {
                    // Setting.    
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }

            // If not valid, stay on the login page
            return View(model);
        }

        /// <summary>  
        /// POST: /Authentication/LogOff    
        /// When you press the button, log out of the application.
        /// </summary>  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
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

        /// <summary>
        /// The functions used to manage the account.
        /// </summary>
        #region Manage account
        /*
        /// <summary>
        /// GET: Authentication/AccountDetails
        /// </summary>
        public ActionResult AccountDetails()
        {
            // Verification     
            if (Request.IsAuthenticated)
            {
                // Find the user details and return them in the view
                var user = db.Members.Find(User.Identity.GetUserId());
                return View(user);
            }
            else
            {
                return
            }
        }

        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = "Password was succesfully changed." });
            }

            return View(model);
        }
        */
        #endregion
    }
}