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
    public class ReservationController : CustomControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        /// <summary>
        /// Endpoint para RETORNAR TODAS AS RESERVAS presentes na tabela Reservation
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAsync()
        {
            var getAll = await _reservationService.GetAllAsync();

            return Ok(getAll);
        }

        /// <summary>
        /// Endpoint para RETORNAR UMA RESERVA POR ID presente na tabela Reservations
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var getById = await _reservationService.GetByIdAsync(id);

            return Ok(getById);
        }

        /// <summary>
        /// Endpoint para CRIAR RESERVA na tabela Reservation
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(ReservationDTO reservation)
        {
            var create = await _reservationService.CreateAsync(reservation);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para ATUALIZAR UMA RESERVA POR ID presente na tabela Reservation
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(long id, ReservationDTO reservation)
        {
            var update = await _reservationService.UpdateAsync(id, reservation);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para EXCLUIR PERMANENTEMENTE UMA RESERVA POR ID presente na tabela Reservation
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            try
            {
                var delete = await _reservationService.DeleteAsync(id);

                if (!delete)
                    return NotFound(new { message = "Reserva não encontrada." });

                return Ok(new { message = "Reserva excluída com sucesso." });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return Conflict(new
                {
                    message = "Não é possível excluir esta reserva pois existem registros associados a ela. " +
                              "Verifique os vínculos antes de tentar novamente."
                });
            }
        }
    }
}
