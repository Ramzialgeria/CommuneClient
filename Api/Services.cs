using CommuneClient.DTOs.Models;
using CommuneClient.Models;
using CommuneClient.Models.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommuneClient.Api
{
    public class Services
    {
        private readonly string CommuneServerUrl;
        private readonly HttpClient _httpClient;
        public string Token { get; private set; }

        public Services(string communeServerUrl)
        {
            CommuneServerUrl = communeServerUrl.TrimEnd('/') + "/";

            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<LoginResponse> LoginAsync(
     string userName,
     string password)
        {
            var request = new LoginRequest
            {
                UserName = userName,
                Password = password
            };

            string json = JsonConvert.SerializeObject(request);

            var url = CommuneServerUrl + "api/auth/login";

            using (var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"))
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Login URL: " + url);

                    System.Diagnostics.Debug.WriteLine(
                        "Login request: " + DateTime.Now);

                    HttpResponseMessage response =
                        await _httpClient.PostAsync(url, content);

                    using (response)
                    {
                        string responseJson =
                            await response.Content.ReadAsStringAsync();

                        System.Diagnostics.Debug.WriteLine(
                            "Login HTTP status: " +
                            (int)response.StatusCode);

                        if (response.IsSuccessStatusCode)
                        {
                            LoginResponse result =
                                JsonConvert.DeserializeObject<LoginResponse>(
                                    responseJson);

                            if (result == null ||
                                string.IsNullOrWhiteSpace(result.accessToken))
                            {
                                throw new Exception(
                                    "Réponse de connexion invalide.");
                            }

                            Token = result.accessToken;
                            return result;
                        }

                        if (response.StatusCode ==
                            System.Net.HttpStatusCode.Unauthorized)
                        {
                            throw new Exception(
                                "اسم المستخدم أو كلمة المرور غير صحيحة.");
                        }

                        throw new Exception(
                            "خطأ في الاتصال بالخادم. HTTP " +
                            (int)response.StatusCode +
                            ". " + responseJson);
                    }
                }
                catch (TaskCanceledException ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Login cancellation/timeout: " + ex);

                    throw new Exception(
                        "انتهت مهلة الاتصال بالخادم: " + url, ex);
                }
                catch (HttpRequestException ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Login HTTP error: " + ex);

                    throw new Exception(
                        "تعذر الاتصال بالخادم: " + url, ex);
                }
            }
        }

        private void SetAuthorization()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        Token);
            }
        }

        public async Task<ActivationDto> GetActivationAsync()
        {
            SetAuthorization();

            HttpResponseMessage response =
                await _httpClient.GetAsync(
                    CommuneServerUrl +
                    "api/client/activation");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("تعذر تشغيل البرنامج.");
            }

            string json = await response.Content.ReadAsStringAsync();

            ActivationDto activation = JsonConvert.DeserializeObject<ActivationDto>(json);

            if (activation == null)
            {
                throw new Exception("Réponse d'activation invalide.");
            }

            return activation;
        }

        public async Task<List<CommuneDto>> GetAvailableCommunesAsync()
        {
            SetAuthorization();

            HttpResponseMessage response =
                await _httpClient.GetAsync(
                    CommuneServerUrl +
                    "api/client/communes/available");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("لا توجد بلديات.");
            }

            string json =
                await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<CommuneDto>>(json)
                ?? new List<CommuneDto>();
        }


        public async Task<List<CentreDto>> GetAvailableCentresAsync(
     string codeCommune = null)
        {
            SetAuthorization();

            string url =
                CommuneServerUrl +
                "api/client/centres/available";

            if (!string.IsNullOrWhiteSpace(codeCommune))
            {
                url += "?codeCommune=" +
                    Uri.EscapeDataString(codeCommune);
            }

            HttpResponseMessage response =
                await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    "Impossible de charger les centres. HTTP " +
                    (int)response.StatusCode);
            }

            string json =
                await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<CentreDto>>(json)
                ?? new List<CentreDto>();
        }

        public async Task<List<BureauDto>> GetAvailableBureauxAsync(
    string codeCentre,
    string codeCommune = null)
        {
            SetAuthorization();

            string url =
                CommuneServerUrl +
                "api/client/bureaux/available?codeCentre=" +
                Uri.EscapeDataString(codeCentre);

            if (!string.IsNullOrWhiteSpace(codeCommune))
            {
                url += "&codeCommune=" +
                    Uri.EscapeDataString(codeCommune);
            }

            HttpResponseMessage response =
                await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    "Impossible de charger les bureaux. HTTP " +
                    (int)response.StatusCode);
            }

            string json =
                await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<BureauDto>>(json)
                ?? new List<BureauDto>();
        }

        public async Task<List<ListeDto>> GetListesAsync()
        {
            SetAuthorization();

            HttpResponseMessage response =
                await _httpClient.GetAsync(
                    CommuneServerUrl +
                    "api/client/listes");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    "Impossible de charger les listes. HTTP " +
                    (int)response.StatusCode);
            }

            string json =
                await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<ListeDto>>(json)
                ?? new List<ListeDto>();
        }

        public async Task<List<CandidatDto>> GetCandidatsAsync(int id)
        {
            SetAuthorization();

            HttpResponseMessage response =
                await _httpClient.GetAsync(
                    CommuneServerUrl +
                    "api/client/listes/" +
                    id +
                    "/candidats");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    "Impossible de charger les candidats. HTTP " +
                    (int)response.StatusCode);
            }

            string json =
                await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<CandidatDto>>(json)
                   ?? new List<CandidatDto>();
        }
    }
}
 

