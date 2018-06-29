using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using La_Game.Models;
using System.Web.Helpers;

namespace La_Game.Controllers
{
    /// <summary>
    /// Member Controller
    /// </summary>
    [AuthorizeAdmin]
    public class MembersController : Controller
    {
        // Database context
        private LaGameDBContext db = new LaGameDBContext();

        /// <summary>
        /// GET: /Members
        /// Get a overview of all member accounts.
        /// </summary>
        public ActionResult Index()
        {
            // Return overview of all members excluding the person accessing the list
            return View(db.Members.ToList().Where(u => u.email != User.Identity.Name));
        }

        #region Detail page and Register/Edit/Delete functions
        /// <summary>
        /// GET: /Members/Details?idMember=[idMember]
        /// Get the details of a member and show it on a separate page.
        /// </summary>
        /// <param name="idMember"> Id of the member. </param>
        public ActionResult Details(int? idMember)
        {
            // Check if id was given
            if (idMember == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the member, if it does not exist return 404
            Member member = db.Members.Find(idMember);
            if (member == null)
            {
                return HttpNotFound();
            }

            // Redirect to the detail page
            return View(member);
        }

        /// <summary>
        /// GET: /Members/Register
        /// Go to the page where a new member can be created.
        /// </summary>
        public ActionResult Register()
        {
            // Go to register page
            return View();
        }

        /// <summary>
        /// POST: /Members/Register
        /// After pressing the button register the new account for the member.
        /// </summary>
        /// <param name="member"> The member that has to be added. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "email,password,firstname,lastname,isAdmin")] Member member)
        {
            var test = member.isAdmin;

            // Check if the data is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Hash the password and activate the member
                    member.password = Crypto.HashPassword(member.password);
                    member.isActive = 1;

                    // If everything is valid, add it to the database
                    db.Members.Add(member);
                    db.SaveChanges();

                    // Redirect back to overview
                    return RedirectToAction("Index");
                }
                catch
                {
                    // Error
                    ModelState.AddModelError(string.Empty, "Error while adding the account.");
                }
            }

            // If not valid, stay on the register page with the current data
            return View();
        }

        /// <summary>
        /// GET: /Members/Edit?idMember=[idMember]
        /// Find the member that has to be changed and redirect to seperate edit page.
        /// </summary>
        /// <param name="idMember"> Id of the member that has to be changed. </param>
        public ActionResult Edit(int? idMember)
        {
            // Check if id was given
            if (idMember == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the member, if it does not exist return 404
            Member member = db.Members.Find(idMember);
            if (member == null)
            {
                return HttpNotFound();
            }

            // Redirect to the edit page
            return View(member);
        }

        /// <summary>
        /// POST: /Members/Edit
        /// </summary>
        /// <param name="member"> The data that has to be saved. </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMember,password,email,firstname,lastname,isAdmin")] Member member)
        {
            // Check if the data is valid
            if (ModelState.IsValid)
            {
                // If valid, save it to the database
                db.Entry(member).State = EntityState.Modified;
                db.SaveChanges();

                // Redirect to index
                return RedirectToAction("Index");
            }

            // If not valid, stay on the edit page with the current data
            return View(member);
        }

        /// <summary>
        /// GET: /Members/Activation?idMember=[idMember]
        /// Find the member that has to be activated/deactivated and redirect to a seperate page for confirmation.
        /// </summary>
        /// <param name="idMember"> Id of the member that has to be deactivated/activated. </param>
        public ActionResult Activation(int? idMember)
        {
            // Check if id was given
            if (idMember == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Try to find the list, if it does not exist return 404
            Member member = db.Members.Find(idMember);
            if (member == null)
            {
                return HttpNotFound();
            }

            // Return the activation page with the member information
            return View(member);
        }

        /// <summary>
        /// POST: /Members/Activate?idMember=[idMember]
        /// </summary>
        /// <param name="idMember"> Id of the member that has to be activated/deactivated. </param>
        /// <returns></returns>
        [HttpPost, ActionName("Activation")]
        [ValidateAntiForgeryToken]
        public ActionResult ActivationConfirmed(int idMember)
        {
            try
            {
                // Find the member and check the status
                Member member = db.Members.Find(idMember);
                if (member.isActive == 1)
                {
                    // If member was active, deactivate
                    member.isActive = 0;
                }
                else
                {
                    // If member was not active, activate
                    member.isActive = 1;
                }

                // Save the changes to the database
                db.Entry(member).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                // Activation failed
            }

            // Redirect to index
            return RedirectToAction("Index");
        }
        #endregion

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
