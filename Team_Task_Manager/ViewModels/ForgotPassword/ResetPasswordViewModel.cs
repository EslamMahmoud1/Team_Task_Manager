using System.ComponentModel.DataAnnotations;

namespace Team_Task_Manager.ViewModels.ForgotPassword;

public class ResetPasswordViewModel
{
    public required string Email { get; set; }

    public required string Token { get; set; }

    [DataType(DataType.Password)]
    public string Password { get; set; } 

    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}
