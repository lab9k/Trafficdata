using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebDownload
{
    [Route("api/[controller]")]
    public class LogoutController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get()
        {
            HttpContext.SignOutAsync().Wait();
            return RedirectToPage("/Login");
        }

        [HttpPost]
        public IActionResult Post()
        {
            HttpContext.SignOutAsync().Wait();
            return RedirectToPage("/Login");
        }
    }
}
