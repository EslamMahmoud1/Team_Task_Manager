using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.Shared;
using Team_Task_Manager.ViewModels.ForgotPassword;

namespace AspnetCoreMvcFull.Controllers;

public class AuthController : Controller
{
    private readonly IRoleService _roleService;
    private readonly UserManager<TaskUser> _userManager;
    private readonly IEmailService _emailService;


    public AuthController(IRoleService roleService, UserManager<TaskUser> userManager, IEmailService emailService)
    {
        _roleService = roleService;
        _userManager = userManager;
        _emailService = emailService;
    }
    public IActionResult LoginBasic() => View();

    public IActionResult RegisterBasic()
    {
        ViewBag.Roles = _roleService.GetAllRoles();
        return View();
    }
    public IActionResult AccessDenied() => View();
    public IActionResult ForgotPasswordConfirmation() => View();
    public IActionResult ResetPasswordConfirmation() => View();
    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        if (token == null || email == null)
            return BadRequest();
        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

        var model = new ResetPasswordViewModel
        {
            Token = decodedToken,
            Email = email
        };

        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
            return RedirectToAction("ResetPasswordConfirmation");

        var result = await _userManager.ResetPasswordAsync(
            user,
            model.Token,
            model.Password);

        if (result.Succeeded)
            return RedirectToAction("ResetPasswordConfirmation");

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(model);
    }

    public IActionResult ForgotPasswordBasic() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return RedirectToAction("ForgotPasswordConfirmation");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var resetLink = Url.Action("ResetPassword", "Auth",
        new { token, email = user.Email },
        Request.Scheme);

        var emailMetadata = new EmailMetadata(
                    user.Email,
                    "Reset Your Password",
                    $@"
                        Password Reset
                        Click the link below to reset your password:
                        {resetLink}'>Reset Password
                    "
                );

        await _emailService.Send(emailMetadata);

        return RedirectToAction("ForgotPasswordConfirmation");
    }
}
