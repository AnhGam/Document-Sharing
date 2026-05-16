using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using document_sharing_manager.Core.Data;
using document_sharing_manager.Core.DTOs;
using document_sharing_manager.Core.Domain;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace document_sharing_manager.Core.Services
{
    public class AuthServiceClient
    {
        private readonly HttpClient _httpClient;
        private string _baseUrl;

        public AuthServiceClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            var handler = new HttpClientHandler { UseProxy = false };
            _httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(15) };
        }

        public void UpdateBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public string? LastError { get; private set; }

        public async Task<bool> LoginAsync(string username, string password)
        {
            LastError = null;
            try
            {
                var request = new LoginRequest { Username = username, Password = password };
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/Auth/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var resultJson = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(resultJson);
                    
                    if (authResponse != null)
                    {
                        UserSession.AccessToken = authResponse.Token;
                        UserSession.RefreshToken = authResponse.RefreshToken;
                        UserSession.Username = authResponse.Username;
                        UserSession.CurrentUserId = authResponse.UserId;
                        return true;
                    }
                }
                else
                {
                    LastError = $"Server returned {(int)response.StatusCode} ({response.ReasonPhrase})";
                    var errorBody = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(errorBody)) LastError += ": " + errorBody;
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                if (ex.InnerException != null) LastError += " -> " + ex.InnerException.Message;
                System.Diagnostics.Debug.WriteLine("Login error: " + ex.ToString());
            }
            return false;
        }

        public async Task<bool> RefreshTokensAsync(string refreshToken)
        {
            try
            {
                var request = new RefreshRequest { RefreshToken = refreshToken };
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/Auth/refresh", content);
                if (response.IsSuccessStatusCode)
                {
                    var resultJson = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(resultJson);
                    
                    if (authResponse != null)
                    {
                        UserSession.AccessToken = authResponse.Token;
                        UserSession.RefreshToken = authResponse.RefreshToken;
                        UserSession.Username = authResponse.Username;
                        UserSession.CurrentUserId = authResponse.UserId;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Refresh error: " + ex.Message);
            }
            return false;
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            try
            {
                var request = new RefreshRequest { RefreshToken = refreshToken };
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/Auth/logout", content);
                
                // Clear local session regardless of server success
                UserSession.AccessToken = null;
                UserSession.RefreshToken = null;
                UserSession.Username = "LocalUser";
                UserSession.CurrentUserId = 1;
                
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        public async Task<List<ManagedServer>> FetchJoinedServersAsync()
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/api/Servers");
                if (!string.IsNullOrEmpty(UserSession.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", UserSession.AccessToken);
                }

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ManagedServer>>(json) ?? [];
                }
            }
            catch { }
            return [];
        }

        public async Task<bool> DeleteServerFromCloudAsync(int remoteId)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseUrl}/api/Servers/{remoteId}");
                if (!string.IsNullOrEmpty(UserSession.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", UserSession.AccessToken);
                }

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> SaveServerToCloudAsync(string name, string url, string accessToken, string? password = null)
        {
            try
            {
                var server = new ManagedServer { Name = name, BaseUrl = url, AccessToken = accessToken, ServerPassword = password };
                var settings = new JsonSerializerSettings 
                { 
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() 
                };
                var json = JsonConvert.SerializeObject(server, settings);
                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/Servers")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                if (!string.IsNullOrEmpty(UserSession.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", UserSession.AccessToken);
                }

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}
