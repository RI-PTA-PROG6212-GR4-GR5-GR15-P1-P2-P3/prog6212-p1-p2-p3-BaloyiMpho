using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers;

public class AccountController : Controller
{
    private readonly ApiService _api;
    public AccountController(ApiService api) => _api = api;

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var (data, error) = await _api.PostAndReadAsync<LoginResult>("api/auth/login", model);

        if (data == null || string.IsNullOrEmpty(data.Token))
        {
            ModelState.AddModelError("", error ?? "Invalid email or password.");
            return View(model);
        }

       
        HttpContext.Session.SetString("JWToken", data.Token);
        HttpContext.Session.SetString("UserRole", data.Role);
        HttpContext.Session.SetString("UserName", data.UserName);

        return RedirectToAction("Index", "Events");
    }
    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var (success, _) = await _api.PostAsync("api/auth/register",
            new { model.Name, model.Email, model.Password, model.Role });
        if (!success)
        {
            ModelState.AddModelError("", "Registration failed. Email may already exist.");
            return View(model);
        }
        TempData["Success"] = "Registered! Please log in.";
        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    private class LoginResult
    {
        public string Token { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Role { get; set; } = "";
    }
}
