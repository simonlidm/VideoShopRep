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
    public class RentReturnsController : Controller
    {
        private MovieDatabaseEntities db = new MovieDatabaseEntities();
  
        private string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

      
        // GET: RentReturns
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var rentReturn = db.RentReturn.Include(r => r.RentStats);
            return View(rentReturn.ToList());
        }

        // GET: RentReturns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentReturn rentReturn = db.RentReturn.Find(id);
            if (rentReturn == null)
            {
                return HttpNotFound();
            }
            return View(rentReturn);
        }

        // GET: RentReturns/Create
      /*  public ActionResult Create()
        {
            ViewBag.RentId = new SelectList(db.RentStats, "RentId", "RentId");
            return View();
        }

        // POST: RentReturns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReturnId,RentId")] RentReturn rentReturn)
        {
            if (ModelState.IsValid)
            {
                db.RentReturn.Add(rentReturn);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RentId = new SelectList(db.RentStats, "RentId", "RentId", rentReturn.RentId);
            return View(rentReturn);
        }
*/
        // GET: RentReturns/Edit/5
     /*   public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentReturn rentReturn = db.RentReturn.Find(id);
            if (rentReturn == null)
            {
                return HttpNotFound();
            }
            ViewBag.RentId = new SelectList(db.RentStats, "RentId", "RentId", rentReturn.RentId);
            return View(rentReturn);
        }
        
        // POST: RentReturns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReturnId,RentId")] RentReturn rentReturn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rentReturn).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RentId = new SelectList(db.RentStats, "RentId", "RentId", rentReturn.RentId);
            return View(rentReturn);
        }
        */
        // GET: RentReturns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentReturn rentReturn = db.RentReturn.Find(id);
            if (rentReturn == null)
            {
                return HttpNotFound();
            }
            return View(rentReturn);
        }

        // POST: RentReturns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RentReturn rentReturn = db.RentReturn.Find(id);
           
            if(db.RentStats.Any(x=>x.RentId == rentReturn.RentId && DateTime.Now <= x.EndDate))
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateOnReturn", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@RentID", SqlDbType.Int).Value = rentReturn.RentId;
                        cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value = DateTime.Now.Date.AddDays(-1);
                        cmd.Parameters.Add("@EndTime", SqlDbType.DateTime).Value = DateTime.Now;
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            db.RentReturn.Remove(rentReturn);
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
    }
}
