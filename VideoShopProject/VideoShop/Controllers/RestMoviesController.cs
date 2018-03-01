using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using VideoShop.Models;

namespace VideoShop.Controllers
{
    [Authorize]
    public class RestMoviesController : ApiController
    {
        private MovieDatabaseEntities db = new MovieDatabaseEntities();
        public RestMoviesController()
        {

            db.Configuration.ProxyCreationEnabled = false;
        }
        [HttpPost]
        [Route("api/restmovies")]
        public IEnumerable<Movie> GetMovieAutocomplete(string prefix)
        {
            return db.Movie.Where(x => x.Title.StartsWith(prefix)).ToList();
        }
        // GET: api/RestMovies
        [HttpGet]
        [Route("api/restmovies")]
        public IEnumerable<Movie> GetMovie()
        {
          
            return db.Movie;
        }
        [Route("api/restmovies/get")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]Movie movie)
        {

            int max = db.Movie.Max(x => x.MovieId);
            movie = db.Movie.Where(x=> x.MovieId==movie.MovieId).SingleOrDefault();

            

            if (movie == null)
            {

               var movie2 = db.Movie.Where(x => x.MovieId ==max );

                return Ok(movie2);
            }
            return Ok(movie);   
         
        }
        [Route("api/restmovies/{id:int}")]
        [HttpGet]
        // GET: api/RestMovies/5
        [ResponseType(typeof(Movie))]
        public IHttpActionResult GetMovie(int id)
        {
            Movie movie = db.Movie.Find(id);
          
           
            if (movie == null)
            {
                return NotFound();
            }
          
            return Ok(movie);
        }
        
        // PUT: api/RestMovies/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMovie(int id, Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != movie.MovieId)
            {
                return BadRequest();
            }

            db.Entry(movie).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
      
        // POST: api/RestMovies
        [Route("api/restmovies")]
        [ResponseType(typeof(Movie))]
        public IHttpActionResult PostMovie(Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Movie.Add(movie);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = movie.MovieId }, movie);
        }

        // DELETE: api/RestMovies/5
        [ResponseType(typeof(Movie))]
        public IHttpActionResult DeleteMovie(int id)
        {
            Movie movie = db.Movie.Find(id);
            if (movie == null)
            {
                return NotFound();
            }

            db.Movie.Remove(movie);
            db.SaveChanges();

            return Ok(movie);
        }
       
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MovieExists(int id)
        {
            return db.Movie.Count(e => e.MovieId == id) > 0;
        }
    }
}