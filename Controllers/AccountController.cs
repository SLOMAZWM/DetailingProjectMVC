using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using ProjektLABDetailing.Models.User;
using ProjektLABDetailing.Models.User.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using ProjektLABDetailing.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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

            var model = new LoginRegisterViewModel
            {
                RegisterUser = new RegisterUserViewModel(),
                LoginUser = new LoginUserViewModel()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(LoginRegisterViewModel model)
        {
            var errorList = ValidateRegisterModel(model.RegisterUser);
            if (errorList.Count == 0)
            {
                var user = new User
                {
                    UserName = model.RegisterUser.Email,
                    Email = model.RegisterUser.Email,
                    FirstName = model.RegisterUser.FirstName,
                    LastName = model.RegisterUser.LastName,
                    PhoneNumber = model.RegisterUser.PhoneNumber,
                    Role = UserRole.Client
                };

                var result = await _userManager.CreateAsync(user, model.RegisterUser.Password);

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
            else
            {
                foreach (var error in errorList)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            model.LoginUser = new LoginUserViewModel();
            return View("LoginRegister", model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRegisterViewModel model)
        {
            var errorList = ValidateLoginModel(model.LoginUser);
            if (errorList.Count == 0)
            {
                var user = await _userManager.FindByEmailAsync(model.LoginUser.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.LoginUser.Password, isPersistent: false, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        HttpContext.Session.SetString("UserId", user.Id.ToString());
                        HttpContext.Session.SetString("UserType", user.Role == UserRole.Client ? "Client" : "Employee");

                        string redirectPage = user.Role == UserRole.Client ? "ClientUserPanel" : "EmployeeUserPanel";
                        return RedirectToAction(redirectPage, user.Role == UserRole.Client ? "Client" : "Employee");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Nieprawidłowy email lub hasło.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Nieprawidłowy email lub hasło.");
                }
            }
            else
            {
                foreach (var error in errorList)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            model.RegisterUser = new RegisterUserViewModel();
            return View("LoginRegister", model);
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

        private List<string> ValidateRegisterModel(RegisterUserViewModel model)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(model.FirstName))
            {
                errors.Add("Imię jest wymagane.");
            }
            if (string.IsNullOrEmpty(model.LastName))
            {
                errors.Add("Nazwisko jest wymagane.");
            }
            if (string.IsNullOrEmpty(model.Email) || !new EmailAddressAttribute().IsValid(model.Email))
            {
                errors.Add("Nieprawidłowy format email.");
            }
            if (string.IsNullOrEmpty(model.Password) || model.Password.Length < 8 || model.Password.Length > 256 || !Regex.IsMatch(model.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d\s:]).*$"))
            {
                errors.Add("Hasło musi mieć od 8 do 256 znaków i zawierać co najmniej jedną dużą literę, jedną małą literę, jedną cyfrę i jeden znak specjalny.");
            }
            if (string.IsNullOrEmpty(model.PhoneNumber) || !Regex.IsMatch(model.PhoneNumber, @"^\+\d{1,3}\s?\d{1,3}\s?\d{3}\s?\d{3}\s?\d{3}$"))
            {
                errors.Add("Numer telefonu musi być w formacie +00 000 000 000.");
            }

            return errors;
        }

        private List<string> ValidateLoginModel(LoginUserViewModel model)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(model.Email) || !new EmailAddressAttribute().IsValid(model.Email))
            {
                errors.Add("Nieprawidłowy format email.");
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                errors.Add("Hasło jest wymagane.");
            }

            return errors;
        }
    }
}
