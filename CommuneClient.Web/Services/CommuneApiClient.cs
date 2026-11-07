using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CommuneClient.Web.Models;

namespace CommuneClient.Web.Services;

public sealed class CommuneApiClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<ApiLoginResponse> LoginAsync(
        string userName,
        string password,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/auth/login",
            new { UserName = userName, Password = password },
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("اسم المستخدم أو كلمة المرور غير صحيحة.");

        await EnsureSuccessAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<ApiLoginResponse>(
            JsonOptions, cancellationToken);

        if (result is null || string.IsNullOrWhiteSpace(result.AccessToken))
            throw new InvalidOperationException("استجابة تسجيل الدخول من الخادم غير صالحة.");

        return result;
    }

    public Task<ActivationDto> GetActivationAsync(
        string token,
        CancellationToken cancellationToken = default) =>
        GetAsync<ActivationDto>("api/client/activation", token, cancellationToken);

    public Task<List<CommuneDto>> GetAvailableCommunesAsync(
        string token,
        CancellationToken cancellationToken = default) =>
        GetAsync<List<CommuneDto>>("api/client/communes/available", token, cancellationToken);

    public Task<List<CentreDto>> GetAvailableCentresAsync(
        string token,
        string? codeCommune = null,
        CancellationToken cancellationToken = default)
    {
        var path = "api/client/centres/available";

        if (!string.IsNullOrWhiteSpace(codeCommune))
            path += "?codeCommune=" + Uri.EscapeDataString(codeCommune);

        return GetAsync<List<CentreDto>>(path, token, cancellationToken);
    }

    public Task<List<JsonElement>> GetAvailableBureauxAsync(
        string token,
        string codeCentre,
        string? codeCommune = null,
        CancellationToken cancellationToken = default)
    {
        var path = "api/client/bureaux/available?codeCentre=" +
            Uri.EscapeDataString(codeCentre);

        if (!string.IsNullOrWhiteSpace(codeCommune))
            path += "&codeCommune=" + Uri.EscapeDataString(codeCommune);

        return GetAsync<List<JsonElement>>(path, token, cancellationToken);
    }

    public Task<List<JsonElement>> GetListesAsync(
        string token,
        CancellationToken cancellationToken = default) =>
        GetAsync<List<JsonElement>>("api/client/listes", token, cancellationToken);

    public Task<List<JsonElement>> GetCandidatsAsync(
        string token,
        long listeId,
        CancellationToken cancellationToken = default) =>
        GetAsync<List<JsonElement>>(
            $"api/client/listes/{listeId}/candidats", token, cancellationToken);

    private async Task<T> GetAsync<T>(
        string path,
        string token,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<T>(
            JsonOptions, cancellationToken);

        if (result is null)
            throw new InvalidOperationException("استجابة غير صالحة من خادم البلدية.");

        return result;
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        throw new HttpRequestException(
            $"CommuneServer API returned HTTP {(int)response.StatusCode}: {body}",
            null,
            response.StatusCode);
    }
}
