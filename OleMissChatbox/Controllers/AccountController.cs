using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OleMissChatbox.Data;
using OleMissChatbox.Services;
using OleMissChatbox.ViewModels;
using System.Text.Json;

namespace OleMissChatbox.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(IOleMissChatboxRepository repo)
        {
            _accountService = new AccountService(repo);
        }

        [Route("")]
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost("")]
        public IActionResult Login(LoginViewModel model)
        {
            var currentUser = _accountService.GetAuthenticatedUser(model.Email, model.Password);

            // return one of two views- chat or unauthorized
            if (currentUser != null)
            {
                HttpContext.Session.SetString("Current User", JsonSerializer.Serialize(currentUser));

                return View("Chat");
            }
            else
            {
                return View("Unauthorized");
            }

        }

        [Route("Account/Unauthorized")]
        [HttpGet("Unauthorized")]
        public IActionResult UnauthorizedRequest()
        {
            return View("Unauthorized");
        }

        [Route("Account/LogOut")]
        [HttpGet]
        public IActionResult LogOut()
        {
            return RedirectToAction("Login");
        }

        [HttpGet("SignUp")]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost("SignUp")]
        public IActionResult SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_accountService.RegisterUser(model.FirstName, model.LastName, model.Email, model.Password))
                {
                    return View("Login");
                }
            }

            return View("SignUp");
        }
    }
}
