using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VideoShop.Models;

namespace VideoShop.Controllers
{
    public class RentStatsController : Controller
    {
        private MovieDatabaseEntities db = new MovieDatabaseEntities();
        private SqlConnection con;
        private void conn()
        {
            string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            con =new SqlConnection(connStr);
        }
        // GET: RentStats
        public ActionResult Index()
        {
            var rentStats = db.RentStats.Include(r => r.Customer).Include(r => r.Movie);
            return View(rentStats.ToList());
        }

        // GET: RentStats/Details/5
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
        public ActionResult Create()
        {
          
            ViewBag.CustomerId = new SelectList(db.Customer, "CustomerId", "FirstName");
            ViewBag.MovieId = new SelectList(db.Movie, "MovieId", "Title").Take(10000).OrderBy(x=>x.Text);
            return View();
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
                DateTime date = DateTime.Now;
                conn();
                con.Open();
                SqlCommand cmd = new SqlCommand("ValidateRent", con);
                cmd.CommandType = CommandType.StoredProcedure;
              
               
                SqlDataReader rdr = cmd.ExecuteReader();
                if (db.RentStats.Any(e => e.EndDate >= date) && rdr.HasRows)
                {
                  
                    rdr.Close();
                    con.Close();

                    
                    TempData["message"] = "The movie is already being rented!";

                    return RedirectToAction("Index");
                }
                else
                {
                    rdr.Close();
                    con.Close();
                    db.RentStats.Add(rentStats);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
               

            }

            ViewBag.CustomerId = new SelectList(db.Customer, "CustomerId", "FirstName", rentStats.CustomerId);
            ViewBag.MovieId = new SelectList(db.Movie, "MovieId", "Title", rentStats.MovieId);
            return View(rentStats);
        }

        // GET: RentStats/Edit/5
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
            ViewBag.MovieId = new SelectList(db.Movie, "MovieId", "Title", rentStats.MovieId);
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
            ViewBag.MovieId = new SelectList(db.Movie, "MovieId", "Title", rentStats.MovieId);
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
