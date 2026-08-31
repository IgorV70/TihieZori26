using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TihieZori.Models;

namespace TihieZori.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ResetPasswordModel> _logger;

        public ResetPasswordModel(
            UserManager<AppUser> userManager,
            ILogger<ResetPasswordModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Введите email")]
            [EmailAddress(ErrorMessage = "Некорректный email")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Введите пароль")]
            [StringLength(100, ErrorMessage = "Пароль должен содержать минимум {2} символов", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Новый пароль")]
            public string Password { get; set; } = "";

            [DataType(DataType.Password)]
            [Display(Name = "Подтвердите пароль")]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; } = "";

            public string Code { get; set; } = "";
        }

        public IActionResult OnGet(string? code = null, string? email = null)
        {
            if (code == null || email == null)
            {
                ViewData["ErrorMessage"] = "Неверная ссылка для сброса пароля";
                return Page();
            }

            Input.Code = code;
            Input.Email = email;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    ViewData["ErrorMessage"] = "Пользователь не найден";
                    return Page();
                }

                var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);

                if (result.Succeeded)
                {
                    ViewData["SuccessMessage"] = "Пароль успешно сброшен! Теперь вы можете войти с новым паролем.";
                    _logger.LogInformation("Password reset successfully for: {Email}", Input.Email);
                    return Page();
                }

                foreach (var error in result.Errors)
                {
                    ViewData["ErrorMessage"] = error.Description;
                    _logger.LogWarning("Password reset error for {Email}: {Error}", Input.Email, error.Description);
                }
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for: {Email}", Input.Email);
                ViewData["ErrorMessage"] = "Произошла ошибка. Попробуйте позже.";
                return Page();
            }
        }
    }
}