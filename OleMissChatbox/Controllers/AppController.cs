using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OleMissChatbox.Data.Entities;
using System.Text.Json;

namespace OleMissChatbox.Controllers
{
    public class AppController : Controller
    {
        [HttpGet("Settings")]
        public IActionResult Settings()
        {
            ViewBag.Title = "Account Settings";

            if (HttpContext.Session.GetString("Current User") != null)
            {
                return View();
            }
            else
            {
                return View("Unauthorized");
            }
        }

        [Route("Chat")]
        [HttpGet("Chat")]
        public IActionResult Chat()
        {
            ViewBag.Title = "Chat";

            var currentUser = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("Current User"));

            ViewBag.Message = $"{currentUser.Email}"; 


            if (HttpContext.Session.GetString("Current User") != null)
            {
                return View("Chat");
            }
            else
            {
                return View("Unauthorized");
            }
        }
    }
}
