using System.Text.Json.Serialization;

namespace CommuneClient.Web.Models;

public sealed class ApiLoginResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = "";

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = "";

    [JsonPropertyName("role")]
    public string Role { get; set; } = "";

    [JsonPropertyName("codeStructure")]
    public string CodeStructure { get; set; } = "";
}

public sealed class ActivationDto
{
    public string ActivationType { get; set; } = "";
    public string CodeCommune { get; set; } = "";
}

public sealed class CommuneDto
{
    public string CodeCommune { get; set; } = "";
    public string NomCommune { get; set; } = "";
}

public sealed class CentreDto
{
    public long Id { get; set; }
    public string Code { get; set; } = "";
    public string Type { get; set; } = "";
    public string Lib { get; set; } = "";
    public int NbInscrit { get; set; }
    public string CodeCommune { get; set; } = "";
}

public sealed class DashboardViewModel
{
    public ActivationDto Activation { get; set; } = new();
    public List<CommuneDto> Communes { get; set; } = [];
    public List<CentreDto> Centres { get; set; } = [];
    public string? ErrorMessage { get; set; }
}
