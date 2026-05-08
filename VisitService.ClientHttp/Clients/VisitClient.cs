using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using VisitService.ClientHttp.Interfaces;
using VisitService.Shared.dtos;

namespace VisitService.ClientHttp.Clients
{
    public class VisitClient : IVisitClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VisitClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddAuthorizationHeader(HttpRequestMessage request)
        {
            string? authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task AddAsync(CreateVisitDto dto)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/visits");
            request.Content = JsonContent.Create(dto);
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
       
        public async Task ConfirmAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/visits/{id}/confirm");
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task RejectAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/visits/{id}/reject");
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/visits/{id}");
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(int id, UpdateVisitDto dto)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/visits/{id}");
            request.Content = JsonContent.Create(dto);
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<VisitDto>> GetAllAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/visits");
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<VisitDto>>() ?? [];
        }

        public async Task<VisitDto?> GetByIdAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/offer/{id}");
            AddAuthorizationHeader(request);

            var response = await _httpClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<VisitDto>();
        }
    }
}