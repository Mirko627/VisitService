using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using VisitService.ClientHttp.Interfaces;
using VisitService.Shared.dtos;

namespace VisitService.Api.Client
{
    public class VisitClient : IVisitClient
    {
        private readonly HttpClient _httpClient;

        public VisitClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AddAsync(CreateVisitDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/visits", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task ConfirmAsync(int id)
        {
            var response = await _httpClient.PatchAsync($"api/visits/{id}/confirm", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RejectAsync(int id)
        {
            var response = await _httpClient.PatchAsync($"api/visits/{id}/reject", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/visits/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(int id, UpdateVisitDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/visits/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<VisitDto>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<VisitDto>>("api/offer") ?? [];
        }

        public async Task<VisitDto?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<VisitDto>($"api/offer/{id}");
        }
    }
}
