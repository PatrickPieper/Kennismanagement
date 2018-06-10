﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        /// <param name="returnUrl"> URL of the previous page. </param> 
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

                // Verification
                if (loginInfo != null && loginInfo.Count() > 0)
                {
                    try
                    {
                        // Setting  
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, loginInfo.First().username)
                        };
                        var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                        var authenticationManager = Request.GetOwinContext().Authentication;

                        // Sign in   
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
                    // Error    
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
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

        /// <summary>
        /// The functions used to manage the account.
        /// </summary>
        #region Manage account
        /// <summary>
        /// GET: /Authentication/AccountDetails
        /// Go to the account page where the user can see their information and edit the settings.
        /// </summary>
        public ActionResult AccountDetails()
        {
            // Get member from database and go to dashboard
            Member member = db.Members.Where(u => u.username == User.Identity.Name).FirstOrDefault();
            return View(member);
        }

        /// <summary>
        /// GET: /Authentication/ChangePassword
        /// Go to a seperate page where the password can be changed.
        /// </summary>
        public ActionResult ChangePassword()
        {
            // Go to the AccountDetails page
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
                // Get member from database
                Member member = db.Members.Where(u => u.username == User.Identity.Name).FirstOrDefault();
                
                // Make sure the right password was entered
                if(member.password == model.OldPassword)
                {
                    // Edit the password
                    member.password = model.NewPassword; // Normal text for now
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
    }
}