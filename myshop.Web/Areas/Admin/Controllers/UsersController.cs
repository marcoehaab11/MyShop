using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.DataAccess.Data;
using System.Security.Claims;
using Utilities;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userid = claim.Value;
             
            
            return View(_context.ApplicationUsers.Where(x=>x.Id!=userid));
        }
        public IActionResult LockUnlock(string? id)
        {
            var user=_context.ApplicationUsers.FirstOrDefault(x=>x.Id==id);
            if(user==null)
            {
                return NotFound();
            }
            if(user.LockoutEnd==null||user.LockoutEnd<DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddDays(5);
            }
            else
            {
                user.LockoutEnd= DateTime.Now;
            }
            _context.SaveChanges();

            return RedirectToAction("Index", "Users", new { area = "Admin" });
        }
    }
}
