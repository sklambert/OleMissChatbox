using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                return View("Views/Account/Unauthorized");
            }
        }

        [Route("Chat")]
        [HttpGet("Chat")]
        public IActionResult Chat()
        {
            ViewBag.Title = "Chat";

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
