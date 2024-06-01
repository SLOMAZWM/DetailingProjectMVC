using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using ProjektLABDetailing.Models.User;
using ProjektLABDetailing.Models.User.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using ProjektLABDetailing.Data;

namespace ProjektLABDetailing.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AccountController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult LoginRegister()
        {
            if (HttpContext.Session.GetString("UserType") != null)
            {
                var userType = HttpContext.Session.GetString("UserType");
                var redirectUrl = userType == "Client" ? "/Client/ClientUserPanel" : "/Employee/EmployeeUserPanel";
                return Redirect(redirectUrl);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserViewModel registerUser)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = registerUser.Email,
                    Email = registerUser.Email,
                    FirstName = registerUser.FirstName,
                    LastName = registerUser.LastName,
                    PhoneNumber = registerUser.PhoneNumber,
                    Role = UserRole.Client
                };

                var result = await _userManager.CreateAsync(user, registerUser.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Client");

                    var client = new Client
                    {
                        UserId = user.Id
                    };

                    _context.Clients.Add(client);
                    await _context.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("UserType", "Client");

                    return RedirectToAction("ClientUserPanel", "Client");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View("LoginRegister", registerUser);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserViewModel loginUser)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginUser.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginUser.Password, isPersistent: false, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        HttpContext.Session.SetString("UserId", user.Id.ToString());
                        HttpContext.Session.SetString("UserType", user.Role == UserRole.Client ? "Client" : "Employee");

                        string redirectPage = user.Role == UserRole.Client ? "ClientUserPanel" : "EmployeeUserPanel";
                        return RedirectToAction(redirectPage, user.Role == UserRole.Client ? "Client" : "Employee");
                    }
                }

                ModelState.AddModelError(string.Empty, "Nieprawidłowy email lub hasło.");
            }

            return View("LoginRegister", loginUser);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ChangeDataUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found, redirecting to LoginRegister.");
                return RedirectToAction("LoginRegister");
            }

            var model = new ChangeDataUserViewModel
            {
                CurrentUser = user,
                ChangePasswordData = new ChangePasswordViewModel(),
                ChangeEmailData = new ChangeEmailViewModel(),
                ChangePhoneNumberData = new ChangePhoneNumberViewModel()
            };

            _logger.LogInformation("ChangeDataUser page loaded successfully.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangeDataUserViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("LoginRegister", "Account");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.ChangePasswordData.CurrentPassword, model.ChangePasswordData.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Hasło zostało pomyślnie zmienione.";
                return RedirectToAction("ChangeDataUser");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("ChangeDataUser", model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ChangeDataUserViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("LoginRegister", "Account");
            }

            var result = await _userManager.SetEmailAsync(user, model.ChangeEmailData.NewEmail);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Email został pomyślnie zmieniony.";
                return RedirectToAction("ChangeDataUser");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("ChangeDataUser", model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePhoneNumber(ChangeDataUserViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("LoginRegister", "Account");
            }            

            var result = await _userManager.SetPhoneNumberAsync(user, model.ChangePhoneNumberData.NewPhoneNumber);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Numer telefonu został pomyślnie zmieniony.";
                return RedirectToAction("ChangeDataUser");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("ChangeDataUser", model);
        }
    }
}
