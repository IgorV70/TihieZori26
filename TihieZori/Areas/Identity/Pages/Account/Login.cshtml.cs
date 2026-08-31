using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TihieZori.Models;

namespace TihieZori.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Введите email")]
            [EmailAddress(ErrorMessage = "Некорректный email")]
            [Display(Name = "Email")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Введите пароль")]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; } = "";

            [Display(Name = "Запомнить меня")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ViewData["LoginError"] = ErrorMessage;
            }

            returnUrl ??= Url.Content("~/");

            // Очищаем существующий cookie
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            // Очищаем старые ошибки
            ViewData["LoginError"] = null;
            ViewData["EmailError"] = null;
            ViewData["PasswordError"] = null;

            // Проверяем обязательные поля
            if (string.IsNullOrWhiteSpace(Input.Email))
            {
                ViewData["EmailError"] = "Введите email";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Input.Password))
            {
                ViewData["PasswordError"] = "Введите пароль";
                return Page();
            }

            // Проверяем валидность email
            if (!new EmailAddressAttribute().IsValid(Input.Email))
            {
                ViewData["EmailError"] = "Некорректный email";
                return Page();
            }

            try
            {
                // Проверяем, существует ли пользователь
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    ViewData["LoginError"] = "Неверный email или пароль";
                    _logger.LogWarning("Login attempt with non-existent email: {Email}", Input.Email);
                    Input.Password = "";
                    return Page();
                }

                // Проверяем, активен ли пользователь
                if (!user.IsActive)
                {
                    ViewData["LoginError"] = "Аккаунт заблокирован. Обратитесь к администратору";
                    _logger.LogWarning("Inactive user tried to login: {Email}", Input.Email);
                    Input.Password = "";
                    return Page();
                }

                // Пытаемся войти
                var result = await _signInManager.PasswordSignInAsync(
                    Input.Email,
                    Input.Password,
                    Input.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in: {Email}", Input.Email);
                    return LocalRedirect(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    ViewData["LoginError"] = "Аккаунт временно заблокирован. Попробуйте позже";
                    _logger.LogWarning("User account locked out: {Email}", Input.Email);
                    return Page();
                }

                // Неправильный пароль
                ViewData["LoginError"] = "Неверный email или пароль";
                _logger.LogWarning("Invalid password for: {Email}", Input.Email);
                Input.Password = "";
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for: {Email}", Input?.Email ?? "unknown");
                ViewData["LoginError"] = "Произошла ошибка при входе. Попробуйте позже";
                return Page();
            }
        }
    }
}