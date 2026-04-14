using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Team_Task_Manager.Models.Entities.User;
using Team_Task_Manager.Services.Interfaces;
using Team_Task_Manager.Shared;
using Team_Task_Manager.ViewModels.User;

namespace Team_Task_Manager.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<TaskUser> _userManager;
    private readonly SignInManager<TaskUser> _signInManager;
    private readonly IRoleService _roleService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(SignInManager<TaskUser> signInManager, UserManager<TaskUser> userManager, IRoleService roleService, IHttpContextAccessor httpContextAccessor)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleService = roleService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<TaskUser>> CreateUser(UserViewModel userViewModel)
    {
        if (userViewModel is null) return Result<TaskUser>.Failure(new List<string>() { "User data is required" });

        var availableEmail = await _userManager.FindByEmailAsync(userViewModel.Email);
        if (availableEmail is not null) return Result<TaskUser>.Failure(new List<string>() { "User with this email already exists" });

        var emailRegex = new Regex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");
        if (!emailRegex.IsMatch(userViewModel.Email)) Result<TaskUser>.Failure(new List<string>() { "Invalid email format" });

        var role = await _roleService.GetRoleById(userViewModel.SelectedRoleId);
        var user = new TaskUser() { Email = userViewModel.Email, UserName = userViewModel.Name, UserRoleId = role.Id };

        var userResult = await _userManager.CreateAsync(user, userViewModel.Password);
        if (!userResult.Succeeded) return Result<TaskUser>.Failure(userResult.Errors.Select(e => e.Description).ToList());

        return Result<TaskUser>.Success(user);
    }

    public async Task<Result<TaskUser>> SignInUser(SignInViewModel signInUser)
    {
        var user = await _userManager.FindByEmailAsync(signInUser.Email);
        if (user is null) return Result<TaskUser>.Failure(new List<string>() { "User with this email does not exist" });

        var result = await _signInManager.PasswordSignInAsync(user, signInUser.Password, false, false);
        if (!result.Succeeded) return Result<TaskUser>.Failure(new List<string>() { "Wrong Password" });

        // Get user roles
        var userRole = await _roleService.GetRoleById(user.UserRoleId);

        // Get user permissions (implement this based on your permission model)
        var permissions = await _roleService.GetRolePermissions(user.UserRoleId);

        // Create claims
        var claims = new List<Claim>
    {
            
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email)
    };

        // Add role claims
        claims.Add(new Claim(ClaimTypes.Role, userRole.Name));

        // Add permission claims
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("Permission", permission.Name.ToString()));
        }

        // Create identity and principal
        var identity = new ClaimsIdentity(claims, "login");
        var principal = new ClaimsPrincipal(identity);

        await _httpContextAccessor.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

        return Result<TaskUser>.Success(user);
    }

    public async Task LogoutUser()
    {
        await _signInManager.SignOutAsync();
    }
}
