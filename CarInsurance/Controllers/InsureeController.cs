using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                //start with the required base monthly quote of 50$.
                decimal quote = 50m;

                //get todays date so that we can calculate the age from user date of birth entry
                //
                DateTime today = DateTime.Now;

                //first compute the difference between todays year and date of birth year
                int age = today.Year - insuree.DateOfBirth.Year;
                //This adjustment check whether the user birthday already passed this year
                //if not substract 1 so age is accurate.
                if (insuree.DateOfBirth > today.AddYears(-age))
                {
                    age--;
                }
                //apply age based quote rules
                //if user is 18 or younger , add 100$.
                if ( age <= 18 )
                {
                    quote += 100;
                }    
                //if user is of age anywere in range 19 till 25 , add $50
                else if (age >= 19 && age <= 25 )
                {
                    quote += 50;

                }
                

                //else user is 26 or older add $25
                else
                {
                    quote += 25;
                }   
                
                //now car year based if else condition to add to quote

                //if users car year is before 2000 then add 25$.
                if (insuree.CarYear < 2000)
                {
                    quote += 25;

                }
                //next if car year entered is after 2015 add 25 $
                if ( insuree.CarYear > 2015 )
                {
                    quote += 25;
                }

                //next condition if porsche add 25,
                //ToLower() is used to make sure this comparison works effectievely.
                if ( insuree.CarMake.ToLower() == "porsche" && insuree.CarModel.ToLower() == "911 carrera")
                {
                    quote += 25;
                }
                //Add $10  for  each speeding ticket the user entered.
                quote += insuree.SpeedingTickets * 10;

                //if the user has a DUI add 25%  to the current total quote.
                if ( insuree.DUI)
                {
                    quote = quote + (quote * 0.25m);
                }
                //if the user selected full coverage , add 50% to the current quote
                // in this coverage type if checked is true ie then its considered as fullcovertate then  do this add 50% computation
                if (insuree.CoverageType)
                {
                    quote = quote + (quote * 0.50m);
                }

                // store the final quote value to the quote field of the insuree object
                // this value will be updated to the db when SaveChanges() runs.
                insuree.Quote = quote;

                /// add the insuree object , including the calculated quote, to the  db context.
                db.Insurees.Add(insuree);

                //save the new insuree record to the db.
                //db.SaveChanges();

                //after saving redirect to action toe index view.
                //return RedirectToAction("Index");

                var insurees = db.Insurees.ToList();
                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Admin()
        {
            //Retreive all insuree records from the database.
            //and convert them to a list so they can be sent to the view.
            var insuree = db.Insurees.ToList();

            //return the admin view and pass teh full list of insurees
            return View( insuree );
        }
    }
}
