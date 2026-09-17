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
    public class FlightSeatController : CustomControllerBase
    {
        private readonly IFlightSeatService _flightSeatService;

        public FlightSeatController(IFlightSeatService flightSeatService)
        {
            _flightSeatService = flightSeatService;
        }

        /// <summary>
        /// Endpoint para RETORNAR TODOS OS ASSENTOS DE VOOS presentes na tabela FlightSeat
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAsync()
        {
            var getAll = await _flightSeatService.GetAllAsync();

            return Ok(getAll);
        }

        /// <summary>
        /// Endpoint para RETORNAR UM ASSENTO DE VOO POR ID presente na tabela FlightSeat
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var getById = await _flightSeatService.GetByIdAsync(id);

            return Ok(getById);
        }

        /// <summary>
        /// Endpoint para CRIAR ASSENTO DE VOO na tabela FlightSeat
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(FlightSeatDTO flightSeat)
        {
            var create = await _flightSeatService.CreateAsync(flightSeat);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para ATUALIZAR UM ASSENTO DE VOO POR ID presente na tabela FligthSeat
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(long id, FlightSeatDTO flightSeat)
        {
            var update = await _flightSeatService.UpdateAsync(id, flightSeat);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para EXCLUIR PERMANENTEMENTE UM ASSENTO DE VOO POR ID presente na tabela FlightSeat
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var delete = await _flightSeatService.DeleteAsync(id);

            return Ok("Base de Dados Atualizada!");
        }
    }
}
