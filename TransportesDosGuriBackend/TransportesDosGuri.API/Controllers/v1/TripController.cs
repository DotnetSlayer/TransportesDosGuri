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
    public class TripController : CustomControllerBase
    {
        private readonly ITripService _tripService;

        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        /// <summary>
        /// Endpoint para RETORNAR TODAS AS VIAGENS presentes na tabela Trip
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var getAll = await _tripService.GetAllAsync();

            return Ok(getAll);
        }

        /// <summary>
        /// Endpoint para RETORNAR UMA VIAGEM POR ID presente na tabela Trip
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var getById = await _tripService.GetByIdAsync(id);

            return Ok(getById);
        }

        /// <summary>
        /// Endpoint para CRIAR VIAGEM na tabela Trip
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(TripDTO trip)
        {
            var create = await _tripService.CreateAsync(trip);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para ATUALIZAR UMA VIAGEM POR ID presente na tabela Trip
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(long id, TripDTO trip)
        {
            var update = await _tripService.UpdateAsync(id, trip);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para EXCLUIR PERMANENTEMENTE UMA VIAGEM POR ID presente na tabela Trip
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            try
            {
                var delete = await _tripService.DeleteAsync(id);

                if (!delete)
                    return NotFound(new { message = "Viagem não encontrada." });

                return Ok(new { message = "Viagem excluída com sucesso." });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return Conflict(new
                {
                    message = "Não é possível excluir esta viagem pois existem voos ou compras associadas a ela. " +
                              "Exclua ou reatribua esses registros primeiro."
                });
            }
        }

        /// <summary>
        /// Endpoint para PESQUISAR VIAGENS
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? searchTerm)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var trips =
                await _tripService.SearchAsync(searchTerm);

            return Ok(trips);
        }

        /// <summary>
        /// Endpoint para PESQUISAR DETALHES DE VIAGEM ESPECÍFICA presente na tabela Trip
        /// </summary>
        [HttpGet("{id:long}/details")]
        public async Task<IActionResult> GetDetails(long id)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var trip =
                await _tripService.GetDetailsAsync(id);

            if (trip == null)
                return NotFound(new
                {
                    message = "Viagem Não Encontrada."
                });

            return Ok(trip);
        }
    }
}
