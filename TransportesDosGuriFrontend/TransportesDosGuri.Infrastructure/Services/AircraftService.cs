using System.Net.Http.Json;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Web.Services;

public class AircraftService : IAircraftService
{
    private readonly HttpClient _http;

    public AircraftService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<AircraftDTO>> GetAllAsync() =>
        await _http.GetFromJsonAsync<List<AircraftDTO>>("/api/v1/Aircraft") ?? new();

    public async Task<AircraftDTO?> GetByIdAsync(long id) =>
        await _http.GetFromJsonAsync<AircraftDTO>($"/api/v1/Aircraft/{id}");

    public async Task<bool> CreateAsync(AircraftDTO dto)
    {
        var response = await _http.PostAsJsonAsync("/api/v1/Aircraft", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(long id, AircraftDTO dto)
    {
        var response = await _http.PutAsJsonAsync($"/api/v1/Aircraft/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var response = await _http.DeleteAsync($"/api/v1/Aircraft/{id}");
        return response.IsSuccessStatusCode;
    }
}