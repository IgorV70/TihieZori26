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
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Очищаем существующий cookie
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            // Логируем состояние для отладки
            _logger.LogInformation("Login attempt for: {Email}", Input?.Email ?? "null");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid");
                return Page();
            }

            try
            {
                // Проверяем, существует ли пользователь
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {Email}", Input.Email);
                    ModelState.AddModelError(string.Empty, "❌ Неверный email или пароль.");
                    Input.Password = "";
                    return Page();
                }

                // Проверяем, активен ли пользователь
                if (!user.IsActive)
                {
                    _logger.LogWarning("Inactive user tried to login: {Email}", Input.Email);
                    ModelState.AddModelError(string.Empty, "❌ Аккаунт заблокирован. Обратитесь к администратору.");
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
                    _logger.LogWarning("User account locked out: {Email}", Input.Email);
                    ModelState.AddModelError(string.Empty, "🔒 Аккаунт временно заблокирован. Попробуйте позже.");
                    return Page();
                }

                // Неправильный пароль
                _logger.LogWarning("Invalid password for: {Email}", Input.Email);
                ModelState.AddModelError(string.Empty, "❌ Неверный email или пароль.");
                Input.Password = "";
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for: {Email}", Input?.Email ?? "unknown");
                ModelState.AddModelError(string.Empty, "⚠️ Произошла ошибка при входе. Попробуйте позже.");
                return Page();
            }
        }
    }
}