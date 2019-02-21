﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_reverse_engineer.Models;

namespace test_reverse_engineer.Controllers.API
{
    [Route("api/users")]
    [ApiController]
    public class UsersAPIController : ControllerBase
    {
        private readonly pmo_db5Context _context;

        public UsersAPIController(pmo_db5Context context)
        {
            _context = context;
        }

        // GET: api/UsersAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/UsersAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users/Register
        [HttpPost("{action}")]
        public async Task<ActionResult<User>> Register(User user)
        {
            var result = await _context.User.FirstOrDefaultAsync(u =>
                u.UserName == user.UserName || u.UserEmail == user.UserEmail);

            if (result != null)
            {
                return Conflict();
            }

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        // POST: api/Users/Login
        [HttpPost("{action}")]
        public async Task<ActionResult<User>> Login(User user)
        {
            var result = await _context.User.FirstOrDefaultAsync(u =>
                u.UserName == user.UserName && u.UserPassword == user.UserPassword);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        // PUT: api/UsersAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/UsersAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}
