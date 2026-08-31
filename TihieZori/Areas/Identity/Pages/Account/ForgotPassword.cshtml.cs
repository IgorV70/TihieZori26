using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TihieZori.Models;

namespace TihieZori.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ForgotPasswordModel> _logger;

        public ForgotPasswordModel(
            UserManager<AppUser> userManager,
            ILogger<ForgotPasswordModel> logger)
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
            [Display(Name = "Email")]
            public string Email { get; set; } = "";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Очищаем старые сообщения
            ViewData["SuccessMessage"] = null;
            ViewData["ErrorMessage"] = null;
            ViewData["EmailError"] = null;

            // Проверяем валидность
            if (string.IsNullOrWhiteSpace(Input.Email))
            {
                ViewData["EmailError"] = "Введите email";
                return Page();
            }

            if (!new EmailAddressAttribute().IsValid(Input.Email))
            {
                ViewData["EmailError"] = "Некорректный email";
                return Page();
            }

            try
            {
                // Ищем пользователя
                var user = await _userManager.FindByEmailAsync(Input.Email);

                // Для безопасности не показываем, существует пользователь или нет
                // Просто говорим, что если пользователь существует - отправлено письмо
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    _logger.LogWarning("Forgot password attempt for non-existent or unconfirmed email: {Email}", Input.Email);

                    // Показываем успех даже если пользователь не найден (безопасность)
                    ViewData["SuccessMessage"] = "Если пользователь с таким email существует, мы отправили ссылку для сброса пароля";
                    return Page();
                }

                // Генерируем токен сброса пароля
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Создаем ссылку для сброса пароля
                var resetUrl = Url.Page(
                    "./ResetPassword",
                    pageHandler: null,
                    values: new { code = resetToken, email = Input.Email },
                    protocol: Request.Scheme);

                // TODO: Здесь нужно отправить email с ссылкой
                // Пока просто логируем
                _logger.LogInformation("Password reset link for {Email}: {ResetUrl}", Input.Email, resetUrl);

                // Для отладки показываем ссылку (в production это нужно убрать)
                ViewData["SuccessMessage"] = $"📧 Ссылка для сброса пароля отправлена на {Input.Email}";

                // В реальном приложении здесь будет отправка email
                // await _emailSender.SendEmailAsync(Input.Email, "Сброс пароля", 
                //     $"Для сброса пароля перейдите по ссылке: <a href='{resetUrl}'>Сбросить пароль</a>");

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