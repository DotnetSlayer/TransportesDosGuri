using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TransportesDosGuri.Core.Application.DTOs;
using TransportesDosGuri.Core.Application.DTOs.ViaCep;

namespace TransportesDosGuri.API.Controllers.ViaCep.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/ViaCep/[controller]")]
    [AllowAnonymous]
    public class ZipCodeController : CustomControllerBase
    {
        private readonly HttpClient _httpClient;

        public ZipCodeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }


        /// <summary>
        /// Endpoint para REQUISITAR INFORMAÇÕES DO CEP ao VIACEP
        /// </summary>
        [HttpGet("{code}")]
        public async Task<IActionResult> Get(string code)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length != 8)
                return BadRequest(new { message = "CEP inválido." });

            var url = $"https://viacep.com.br/ws/{code}/json/";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return NotFound(new { message = "CEP não encontrado." });

                var json = await response.Content.ReadAsStringAsync();
                var viaCep = JsonSerializer.Deserialize<ViaCepResponse>(json);

                if (viaCep == null || string.IsNullOrWhiteSpace(viaCep.cep))
                    return NotFound(new { message = "CEP não encontrado." });

                var result = new ZipCodeResponseDTO
                {
                    Code = viaCep.cep,
                    Address = viaCep.logradouro,
                    District = viaCep.bairro,
                    City = viaCep.localidade,
                    State = viaCep.uf
                };

                return Ok(result);
            }
            catch
            {
                return StatusCode(500, new { message = "Erro ao consultar o CEP." });
            }
        }

        private class ViaCepResponse
        {
            public string? cep { get; set; }
            public string? logradouro { get; set; }
            public string? bairro { get; set; }
            public string? localidade { get; set; }
            public string? uf { get; set; }
        }
    }
}