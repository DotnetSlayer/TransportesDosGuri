using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportesDosGuri.Core.Application.DTOs;
using TransportesDosGuri.Core.Application.ServiceContracts;

namespace TransportesDosGuri.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/[controller]")]
    public class FlightController : CustomControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        /// <summary>
        /// Endpoint para RETORNAR TODAS OS VOOS presentes na tabela Flight
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAsync()
        {
            var getAll = await _flightService.GetAllAsync();

            return Ok(getAll);
        }

        /// <summary>
        /// Endpoint para RETORNAR UM VOO POR ID presente na tabela Flight
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var getById = await _flightService.GetByIdAsync(id);

            return Ok(getById);
        }

        /// <summary>
        /// Endpoint para CRIAR VOO na tabela Flight
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(FlightDTO flight)
        {
            var create = await _flightService.CreateAsync(flight);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para ATUALIZAR UM VOO POR ID presente na tabela Flight
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(long id, FlightDTO flight)
        {
            var update = await _flightService.UpdateAsync(id, flight);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para EXCLUIR PERMANENTEMENTE UM VOO POR ID presente na tabela Flight
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            try
            {
                var delete = await _flightService.DeleteAsync(id);

                if (!delete)
                    return NotFound(new { message = "Voo não encontrado." });

                return Ok(new { message = "Voo excluído com sucesso." });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return Conflict(new
                {
                    message = "Não é possível excluir este voo pois existem assentos ou escalas associados a ele. " +
                              "Exclua ou reatribua esses registros primeiro."
                });
            }
        }
    }
}
