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
    public class ScheduleController : CustomControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// Endpoint para RETORNAR TODAS AS ESCALAS presentes na tabela Schedule
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAsync()
        {
            var getAll = await _scheduleService.GetAllAsync();

            return Ok(getAll);
        }

        /// <summary>
        /// Endpoint para RETORNAR UMA ESCALA POR ID presente na tabela Schedule
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var getById = await _scheduleService.GetByIdAsync(id);

            return Ok(getById);
        }

        /// <summary>
        /// Endpoint para CRIAR ESCALA na tabela Schedule
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(ScheduleDTO schedule)
        {
            var create = await _scheduleService.CreateAsync(schedule);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para ATUALIZAR UMA ESCALA POR ID presente na tabela Schedule
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(long id, ScheduleDTO schedule)
        {
            var update = await _scheduleService.UpdateAsync(id, schedule);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para EXCLUIR PERMANENTEMENTE UMA ESCALA POR ID presente na tabela Schedule
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            try
            {
                var delete = await _scheduleService.DeleteAsync(id);

                if (!delete)
                    return NotFound(new { message = "Escala não encontrada." });

                return Ok(new { message = "Escala excluída com sucesso." });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return Conflict(new
                {
                    message = "Não é possível excluir esta escala pois existem registros associados a ela. " +
                              "Verifique os vínculos antes de tentar novamente."
                });
            }
        }
    }
}
