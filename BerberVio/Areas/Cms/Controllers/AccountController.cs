using BerberVio.Entities;
using BerberVio.Models;
using BerberVio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace BerberVio.Areas.Cms.Controllers;

[Area("Cms")]
public class AccountController : Controller
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly ApiClient _apiClient;
    private readonly IConfiguration _configuration;

    public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ApiClient apiClient, IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _apiClient = apiClient;
        _configuration = configuration;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Dashboard", new { area = "Cms" });

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var apiResponse = await _apiClient.PostAsync("api/auth/login", new { email = model.Email, password = model.Password });

            if (apiResponse.IsSuccessStatusCode)
            {
                var authResult = await apiResponse.Content.ReadFromJsonAsync<ApiAuthResponse>();

                if (authResult != null)
                {
                    var expireMinutes = int.Parse(_configuration["ApiSettings:ExpireMinutes"] ?? "60");

                    Response.Cookies.Append("jwt", authResult.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(expireMinutes)
                    });
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Email veya şifre hatalı.");
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new AppUser
        {
            UserName = model.Email,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Name = model.Name,
            Surname = model.Surname
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        Response.Cookies.Delete("jwt");
        return RedirectToAction("Index", "Home");
    }
}
