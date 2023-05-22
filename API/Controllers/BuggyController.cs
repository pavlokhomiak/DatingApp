using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;

        public BuggyController(DataContext dataContext) 
        {
            _context = dataContext;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret() {
            return "secret";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound() {
            var thing = _context.Users.Find(-1);

            if (thing == null) return NotFound();

            return thing;
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError() {
            
            // WE HANDLE EXCEPTION ON middleware LEVEL (Middleware/ExceptionMiddleware)

            // try {
            //     var thing = _context.Users.Find(-1);

            //     var thingToReturn = thing.ToString();

            //     return thingToReturn;
            // } catch (Exception e) {
            //     return StatusCode(50, "NO!");
            // }

            var thing = _context.Users.Find(-1);

            var thingToReturn = thing.ToString();

            return thingToReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest() {
            return BadRequest("Not good request");
        }
    }
}