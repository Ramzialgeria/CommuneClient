using System.Diagnostics;
using System.Security.Claims;
using CommuneClient.Web.Models;
using CommuneClient.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommuneClient.Web.Controllers;

[Authorize]
public sealed class HomeController(CommuneApiClient apiClient) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var token = User.FindFirstValue("commune_api_token");

        if (string.IsNullOrWhiteSpace(token))
            return RedirectToAction("Login", "Account");

        var model = new DashboardViewModel();

        try
        {
            model.Activation = await apiClient.GetActivationAsync(token, cancellationToken);

            if (string.Equals(model.Activation.ActivationType, "APW", StringComparison.OrdinalIgnoreCase))
            {
                model.Communes = await apiClient.GetAvailableCommunesAsync(token, cancellationToken);
            }
            else if (string.Equals(model.Activation.ActivationType, "APC", StringComparison.OrdinalIgnoreCase))
            {
                model.Centres = await apiClient.GetAvailableCentresAsync(token, cancellationToken: cancellationToken);
            }
            else
            {
                model.ErrorMessage = "نوع التفعيل غير معروف.";
            }
        }
        catch (HttpRequestException)
        {
            model.ErrorMessage = "تعذر الاتصال بخادم البلدية. تحقق من تشغيل CommuneServer وواجهة API.";
        }
        catch (Exception)
        {
            model.ErrorMessage = "تعذر تحميل بيانات التفعيل.";
        }

        return View(model);
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}

public sealed class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
