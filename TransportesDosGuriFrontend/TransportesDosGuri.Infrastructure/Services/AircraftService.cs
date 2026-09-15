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

    public async Task<List<AircraftDto>> GetAllAsync() =>
        await _http.GetFromJsonAsync<List<AircraftDto>>("") ?? new();

    public async Task<AircraftDto?> GetByIdAsync(long id) =>
        await _http.GetFromJsonAsync<AircraftDto>($"");

    public async Task<bool> CreateAsync(AircraftDto dto)
    {
        var response = await _http.PostAsJsonAsync("", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(long id, AircraftDto dto)
    {
        var response = await _http.PutAsJsonAsync($"", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var response = await _http.DeleteAsync($"");
        return response.IsSuccessStatusCode;
    }
}