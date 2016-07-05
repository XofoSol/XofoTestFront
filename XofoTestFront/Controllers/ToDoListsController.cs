using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using XofoTestFront.Models;
using Microsoft.AspNet.Identity;

namespace XofoTestFront.Controllers
{
    public class ToDoListsController : ApiController
    {
        private XofoTestFrontContext db = new XofoTestFrontContext();

        // GET: api/ToDoLists
        [Authorize]
        public IQueryable<ToDoList> GetToDoLists()
        {
            string userId = User.Identity.GetUserId();
            return db.ToDoLists
                .OrderBy(b => b.done)
                .ThenBy(c => c.due)
                .Where(a => a.UserId == userId);
        }

        // GET: api/ToDoLists/5
        [Authorize]
        [ResponseType(typeof(ToDoList))]
        public async Task<IHttpActionResult> GetToDoList(int id)
        {
            ToDoList toDoList = await db.ToDoLists.FindAsync(id);
            if (toDoList.UserId != User.Identity.GetUserId())
            {
                return BadRequest();
            }
            if (toDoList == null)
            {
                return NotFound();
            }

            return Ok(toDoList);
        }

        // PUT: api/ToDoLists/5
        [Authorize]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutToDoList(int id, ToDoList toDoList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != toDoList.Id)
            {
                return BadRequest();
            }

            var rowToEdit = await db.ToDoLists
                .Where(a => a.Id == id)
                .Select(b => new {
                    Id = b.Id,
                    UserId = b.UserId
                })
                .FirstOrDefaultAsync();

            if (rowToEdit.UserId != User.Identity.GetUserId())
            {
                return Unauthorized();
            }

            db.Entry(toDoList).State = EntityState.Modified;
            db.Entry(toDoList).Property("UserId").IsModified = false;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoListExists(id))
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

        // POST: api/ToDoLists
        [ResponseType(typeof(ToDoList))]
        [Authorize]
        public async Task<IHttpActionResult> PostToDoList(ToDoList toDoList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            toDoList.UserId = User.Identity.GetUserId();
            db.ToDoLists.Add(toDoList);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = toDoList.Id }, toDoList);
        }

        // DELETE: api/ToDoLists/5
        [ResponseType(typeof(ToDoList))]
        [Authorize]
        public async Task<IHttpActionResult> DeleteToDoList(int id)
        {
            ToDoList toDoList = await db.ToDoLists.FindAsync(id);
            if (toDoList == null)
            {
                return NotFound();
            }
            if(toDoList.UserId != User.Identity.GetUserId())
            {
                return BadRequest();
            }

            db.ToDoLists.Remove(toDoList);
            await db.SaveChangesAsync();

            return Ok(toDoList);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ToDoListExists(int id)
        {
            return db.ToDoLists.Count(e => e.Id == id) > 0;
        }
    }
}