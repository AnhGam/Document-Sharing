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
        private string? _customAccessToken;

        public string? AccessToken
        {
            get => _customAccessToken ?? UserSession.AccessToken;
            set => _customAccessToken = value;
        }

        public AuthServiceClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            var handler = new HttpClientHandler 
            { 
                UseProxy = false,
                // Bỏ qua xác thực chứng chỉ SSL để kết nối qua các Tunnel luôn thành công
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            _httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(60) };
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 DocumentSharingManager/1.0");
        }

        public void UpdateBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public string BaseUrl => _baseUrl;

        public string? LastError { get; private set; }

        public async Task<bool> RegisterAsync(string username, string password, string email)
        {
            LastError = null;
            try
            {
                var request = new RegisterRequest { Username = username, Password = password, Email = email };
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/Auth/register", content);
                if (!response.IsSuccessStatusCode)
                {
                    LastError = $"Server returned {(int)response.StatusCode} ({response.ReasonPhrase})";
                    var errorBody = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(errorBody)) LastError += ": " + errorBody;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

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
                if (!string.IsNullOrEmpty(this.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);
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
                if (!string.IsNullOrEmpty(this.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);
                }

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<int?> SaveServerToCloudAsync(string name, string url, string accessToken, string? password = null)
        {
            try
            {
                var server = new ManagedServer { Name = name, BaseUrl = url, AccessToken = accessToken, ServerPassword = password ?? string.Empty };
                var settings = new JsonSerializerSettings 
                { 
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() 
                };
                var json = JsonConvert.SerializeObject(server, settings);
                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/Servers")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                if (!string.IsNullOrEmpty(this.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);
                }

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var resJson = await response.Content.ReadAsStringAsync();
                    var savedServer = JsonConvert.DeserializeObject<ManagedServer>(resJson);
                    return savedServer?.Id;
                }
                return null;
            }
            catch { return null; }
        }

        // === Invite Link and Join Requests ===
        public async Task<List<InviteLink>> GetInvitesAsync()
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/api/invite");
                if (!string.IsNullOrEmpty(this.AccessToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);
                
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<InviteLink>>(json) ?? new List<InviteLink>();
                }
            }
            catch { }
            return new List<InviteLink>();
        }

        public async Task<InviteLink?> CreateInviteAsync(bool requiresApproval)
        {
            try
            {
                var payload = new { requiresApproval };
                var json = JsonConvert.SerializeObject(payload);
                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/invite")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                if (!string.IsNullOrEmpty(this.AccessToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);
                
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var resJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InviteLink>(resJson);
                }
            }
            catch { }
            return null;
        }

        public async Task<bool> RevokeInviteAsync(string code)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseUrl}/api/invite/{code}");
                if (!string.IsNullOrEmpty(this.AccessToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);
                
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<(bool valid, bool requiresApproval, string message)> GetInviteInfoAsync(string code)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/invite/{code}/info");
                if (response.IsSuccessStatusCode)
                {
                    var resJson = await response.Content.ReadAsStringAsync();
                    var info = Newtonsoft.Json.Linq.JObject.Parse(resJson);
                    return (true, (bool?)info["requiresApproval"] ?? false, "OK");
                }
                return (false, false, $"Mã mời không hợp lệ (HTTP {(int)response.StatusCode}).");
            }
            catch (TaskCanceledException) { return (false, false, $"Hết thời gian chờ phản hồi từ server ({_baseUrl}). Hãy kiểm tra Tunnel đã chạy chưa."); }
            catch (Exception ex) { return (false, false, $"Lỗi kết nối ({_baseUrl}): {ex.Message}"); }
        }

        public async Task<(bool success, string message)> JoinWithInviteAsync(string code, string displayName)
        {
            try
            {
                var payload = new { displayName };
                var json = JsonConvert.SerializeObject(payload);
                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/invite/{code}/join")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                if (!string.IsNullOrEmpty(this.AccessToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);

                var response = await _httpClient.SendAsync(request);
                var resJson = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    var result = Newtonsoft.Json.Linq.JObject.Parse(resJson);
                    return (true, result["message"]?.ToString() ?? "");
                }
                return (false, $"Lỗi từ server: {(int)response.StatusCode}");
            }
            catch (TaskCanceledException) { return (false, $"Hết thời gian chờ phản hồi từ server ({_baseUrl}). Hãy kiểm tra Tunnel đã chạy chưa."); }
            catch (Exception ex) { return (false, $"Lỗi kết nối ({_baseUrl}): {ex.Message}"); }
        }

        public async Task<List<JoinRequest>> GetPendingJoinRequestsAsync()
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/api/join-requests");
                if (!string.IsNullOrEmpty(this.AccessToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);
                
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<JoinRequest>>(json) ?? new List<JoinRequest>();
                }
            }
            catch { }
            return new List<JoinRequest>();
        }

        public async Task<bool> ApproveJoinRequestAsync(int id)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/join-requests/{id}/approve");
                if (!string.IsNullOrEmpty(this.AccessToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);
                
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> DenyJoinRequestAsync(int id)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/join-requests/{id}/deny");
                if (!string.IsNullOrEmpty(this.AccessToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);
                
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<List<AuditLog>> GetAuditLogsAsync(int page = 1, int limit = 50)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/api/audit-logs?page={page}&limit={limit}");
                if (!string.IsNullOrEmpty(this.AccessToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);
                
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<AuditLog>>(json) ?? new List<AuditLog>();
                }
            }
            catch { }
            return new List<AuditLog>();
        }
    }
}
