using System.Security.Claims;
using CommuneClient.Web.Models;
using CommuneClient.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommuneClient.Web.Controllers;

public sealed class AccountController(CommuneApiClient apiClient) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        string? returnUrl,
        CancellationToken cancellationToken)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var apiLogin = await apiClient.LoginAsync(
                model.UserName,
                model.Password,
                cancellationToken);

            // Confirm that this installation has a valid active activation
            // before creating a browser session.
            await apiClient.GetActivationAsync(
                apiLogin.AccessToken,
                cancellationToken);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, apiLogin.UserName),
                new(ClaimTypes.Role, apiLogin.Role),
                new("code_structure", apiLogin.CodeStructure),
                new("commune_api_token", apiLogin.AccessToken)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
        catch (UnauthorizedAccessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "تعذر الاتصال بخادم البلدية. تحقق من تشغيل CommuneServer وخدمة API.");
        }
        catch (Exception)
        {
            ModelState.AddModelError(
                string.Empty,
                "تعذر تسجيل الدخول أو التحقق من تفعيل هذه النسخة.");
        }

        model.Password = "";
        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied() => View();
}
