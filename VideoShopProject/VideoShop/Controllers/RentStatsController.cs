﻿using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VideoShop.Models;

namespace VideoShop.Controllers
{
    public class RentStatsController : Controller
    {
        private MovieDatabaseEntities db = new MovieDatabaseEntities();
     
       
       
            string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
           
        
        // GET: RentStats
        [Authorize(Roles ="Admin")]
        public ActionResult Index(int? page)
        {
            
            var rentStats = db.RentStats.Include(r => r.Customer).Include(r => r.Movie);
            return (View(rentStats.ToList().ToPagedList(pageNumber: page ?? 1,
    pageSize: 5)));

        }
       
     
        // GET: RentStats/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentStats rentStats = db.RentStats.Find(id);
            if (rentStats == null)
            {
                return HttpNotFound();
            }
            return View(rentStats);
        }

        // GET: RentStats/Create
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public  ActionResult Create()
        {
          
            ViewBag.CustomerId = new SelectList(db.Customer, "CustomerId", "FirstName");
            ViewBag.MovieId =  new  SelectList(db.Movie, "MovieId", "Title").Take(2000);
            return View();
        }
   
        private bool ValidateDate(RentStats rentStats)
        {
            bool error = false;
           
                if (db.RentStats.Any(x => x.EndDate >= rentStats.StartDate && x.MovieId == rentStats.MovieId&&x.CustomerId==rentStats.CustomerId
                )|| db.RentStats.Any(x=>x.MovieId == rentStats.MovieId && x.EndDate >= rentStats.StartDate) )
                {
                    error = true;
                    TempData["message"] = "The movie is already being rented!";
                
                }
                if (db.RentStats.Any(x => x.MovieId == rentStats.MovieId && x.StartDate == rentStats.StartDate && x.EndDate == rentStats.EndDate&&x.CustomerId ==rentStats.CustomerId))
                {

                     error = true;
                    TempData["message"] = "This rental is a duplicate and already exist";
                }
                if (rentStats.StartDate < DateTime.Now.Date || rentStats.StartDate > DateTime.Now.Date)
                {

                    error = true;
                    TempData["message"] = "Invalid date!";
                }
            
            return error;
        }
        // POST: RentStats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RentId,CustomerId,MovieId,StartDate,EndDate")] RentStats rentStats)
        {
            if (ModelState.IsValid)
            {
                rentStats.StartDate = DateTime.Now.Date;
                rentStats.StartDatetime = DateTime.Now;
                if (!ValidateDate(rentStats))
                {
                    try
                    {
                 
                        db.RentStats.Add(rentStats);
                        db.SaveChanges();

                    }
                    catch (Exception)
                    {
                        TempData["message"] = "Error occured,try again!";
                    }
                    
                }
              
                return RedirectToAction("Index");

            }

            ViewBag.CustomerId = new SelectList(db.Customer, "CustomerId", "FirstName", rentStats.CustomerId);
            ViewBag.MovieId = new SelectList(db.Movie, "MovieId", "Title", rentStats.MovieId);
            return View(rentStats);
        }


        // GET: RentStats/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentStats rentStats = db.RentStats.Find(id);
            if (rentStats == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.Customer, "CustomerId", "FirstName", rentStats.CustomerId);
            ViewBag.MovieId = new SelectList(db.Movie, "MovieId", "Title", rentStats.MovieId).OrderBy(x => x.Text);
            return View(rentStats);
        }

        // POST: RentStats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RentId,CustomerId,MovieId,StartDate,EndDate")] RentStats rentStats)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rentStats).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Customer, "CustomerId", "FirstName", rentStats.CustomerId);
            ViewBag.MovieId = new SelectList(db.Movie, "MovieId", "Title", rentStats.MovieId).OrderBy(x => x.Text).Skip(10000).Take(10000);
            return View(rentStats);
        }

        // GET: RentStats/Delete/5
      /*  public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentStats rentStats = db.RentStats.Find(id);
            if (rentStats == null)
            {
                return HttpNotFound();
            }
            return View(rentStats);
        }

        // POST: RentStats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RentStats rentStats = db.RentStats.Find(id);
            db.RentStats.Remove(rentStats);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
*/
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
