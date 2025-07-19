using System.Security.Claims;
using itRoot.Models;
using itRoot.ModelViews;
using itRoot.Repos.IRepos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using itRoot.UnitOfWorks.IUnitOfWorks;
using itRoot.Services.IServices;
namespace itRoot.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<AccountController> logger, IEmailService emailService)
        {
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            try
            {
                if (remoteError != null)
                {
                    ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                    return View("Login");
                }

                var result = await HttpContext.AuthenticateAsync("Google");
                if (!result.Succeeded)
                {
                    return RedirectToAction("Login");
                }

                var claims = result.Principal.Claims;
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var phone = claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    ModelState.AddModelError(string.Empty, "Email not received from external provider.");
                    return View("Login");
                }

                var existingUser = _unitOfWork.UserRepo.GetUserByEmail(email);

                if (existingUser == null)
                {
                    var newUser = new user
                    {
                        fullName = name ?? "Unknown",
                        email = email,
                        phone = phone ?? "0",
                        userName = $"user_{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                        password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                        isConfirmed = true
                    };

                    _unitOfWork.UserRepo.Insert(newUser);
                    _unitOfWork.Save();
                    existingUser = newUser;
                }
                else
                {
                    if (string.IsNullOrEmpty(existingUser.fullName) && !string.IsNullOrEmpty(name))
                    {
                        existingUser.fullName = name;
                    }
                    if (string.IsNullOrEmpty(existingUser.phone) && !string.IsNullOrEmpty(phone))
                    {
                        existingUser.phone = phone;
                    }
                    existingUser.isConfirmed = true;
                    _unitOfWork.Save();
                }

                var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingUser.userName ?? "Unknown"),
                new Claim(ClaimTypes.NameIdentifier, existingUser.userId.ToString()),
                new Claim(ClaimTypes.Email, existingUser.email ?? "")
            };

                var claimsIdentity = new ClaimsIdentity(userClaims, "Cookies");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("Cookies", claimsPrincipal);

                return RedirectToLocal(returnUrl);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "error in external login call back");
                return RedirectToAction("Login");
            }
            
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }catch(Exception ex)
            {
                _logger.LogError(ex, "error in redirect to local action");
                return RedirectToAction("Index", "Home");
            }
            
        }
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "invoice");
            }
            ViewBag.RecaptchaSiteKey = _configuration["Recaptcha:SiteKey"];
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.RecaptchaSiteKey = _configuration["Recaptcha:SiteKey"];
                    return View(model);
                }
                if (!await VerifyRecaptcha(model.RecaptchaToken))
                {
                    ModelState.AddModelError("", "reCAPTCHA verification failed.");
                    ViewBag.RecaptchaSiteKey = _configuration["Recaptcha:SiteKey"];
                    return View(model);
                }
                var user = _unitOfWork.UserRepo.GetUserByUserName(model.username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(model.password, user.password))
                {
                    ViewBag.RecaptchaSiteKey = _configuration["Recaptcha:SiteKey"];
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }
                if (user.isConfirmed != true)
                {
                    ViewBag.RecaptchaSiteKey = _configuration["Recaptcha:SiteKey"];
                    ModelState.AddModelError("", "email has not verfied");
                    return View(model);
                }
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.userName),
                new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
            };
                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("Cookies", principal);

                return RedirectToAction("Index", "invoice");
            }catch(Exception ex)
            {
                _logger.LogError(ex, "error in login");
                return RedirectToAction("Login");
            }
         }
         public IActionResult Registration()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "invoice");
            }
            ViewBag.RecaptchaSiteKey = _configuration["Recaptcha:SiteKey"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.RecaptchaSiteKey = _configuration["Recaptcha:SiteKey"];
                    return View(model);
                }

                if (!await VerifyRecaptcha(model.RecaptchaToken))
                {
                    ModelState.AddModelError("", "reCAPTCHA verification failed.");
                    ViewBag.RecaptchaSiteKey = _configuration["Recaptcha:SiteKey"];
                    return View(model);
                }

                var username = _unitOfWork.UserRepo.GetUserByUserName(model.UserName);
                if (username != null)
                {
                    ModelState.AddModelError("UserName", "Username is already taken.");
                    ViewBag.RecaptchaSiteKey = _configuration["Recaptcha:SiteKey"];
                    return View(model);
                }

                var email = _unitOfWork.UserRepo.GetUserByEmail(model.Email);
                if (email != null)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    ViewBag.RecaptchaSiteKey = _configuration["Recaptcha:SiteKey"];
                    return View(model);
                }

                var user = new user
                {
                    userName = model.UserName,
                    email = model.Email,
                    fullName = model.FullName,
                    password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    phone = model.Phone,
                    isConfirmed = false
                };

                _unitOfWork.UserRepo.Insert(user);
                _unitOfWork.Save();

                var token = Guid.NewGuid().ToString();
                var confirmationToken = new EmailConfirmationToken
                {
                    userId = user.userId,
                    token = token, 
                    expiresAt = DateTime.Now.AddHours(24),
                    isUsed = false
                };

                _unitOfWork.EmailConfirmationTokenRepo.Insert(confirmationToken);
                _unitOfWork.Save();

                var confirmationUrl = Url.Action("ConfirmEmail", "Account",
                    new { token = token }, Request.Scheme);

                var emailService = HttpContext.RequestServices.GetService<IEmailService>();
                var emailSent = await emailService.SendEmailConfirmationAsync(
                    user.email, user.fullName, confirmationUrl);

                if (emailSent)
                {
                    TempData["Message"] = "Registration successful! Please check your email to confirm your account.";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["Error"] = "Registration successful, but we couldn't send the confirmation email. Please contact support.";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering user");
                TempData["Error"] = "An error occurred during registration. Please try again.";
                return RedirectToAction("Registration");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "Invalid confirmation link.";
                    return RedirectToAction("Login");
                }

                var confirmationToken = _unitOfWork.EmailConfirmationTokenRepo
                    .GetAll()
                    .FirstOrDefault(t => t.token == token && !t.isUsed);

                if (confirmationToken == null)
                {
                    TempData["Error"] = "Invalid or already used confirmation link.";
                    return RedirectToAction("Login");
                }

                if (confirmationToken.expiresAt < DateTime.Now)
                {
                    TempData["Error"] = "Confirmation link has expired. Please register again.";
                    return RedirectToAction("Registration");
                }

                var user = _unitOfWork.UserRepo.GetById(confirmationToken.userId);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Login");
                }

                user.isConfirmed = true;

                confirmationToken.isUsed = true;

                _unitOfWork.Save();

                TempData["Success"] = "Email confirmed successfully! You can now login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email with token: {Token}", token);
                TempData["Error"] = "An error occurred while confirming your email. Please try again.";
                return RedirectToAction("Login");
            }
        }


        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return RedirectToAction("Login");
        }

        private async Task<bool> VerifyRecaptcha(string token)
        {
            var secretKey = _configuration["Recaptcha:SecretKey"];
            var client = new HttpClient();
            var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}", null);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
            return result.success;
        }
    }
}
