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
    public class UserRequestController : CustomControllerBase
    {
        private readonly IUserRequestService _userRequestService;

        public UserRequestController(IUserRequestService userRequestService)
        {
            _userRequestService = userRequestService;
        }

        /// <summary>
        /// Endpoint para RETORNAR TODAS AS REQUISIÇÕES DE USUÁRIOS presentes na tabela UserRequest
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAsync()
        {
            var getAll = await _userRequestService.GetAllAsync();

            return Ok(getAll);
        }

        /// <summary>
        /// Endpoint para RETORNAR UMA REQUISIÇÃO DE USUÁRIO POR ID presente na tabela UserRequest
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var getById = await _userRequestService.GetByIdAsync(id);

            return Ok(getById);
        }

        /// <summary>
        /// Endpoint para CRIAR REQUISIÇÃO DE USUÁRIO na tabela UserRequest
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(UserRequestDTO userRequest)
        {
            var create = await _userRequestService.CreateAsync(userRequest);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para ATUALIZAR UMA REQUISIÇÃO DE USUÁRIO POR ID presente na tabela UserRequest
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(long id, UserRequestDTO userRequest)
        {
            var update = await _userRequestService.UpdateAsync(id, userRequest);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para EXCLUIR PERMANENTEMENTE UMA REQUISIÇÃO DE USUÁRIO POR ID presente na tabela UserRequest
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            try
            {
                var delete = await _userRequestService.DeleteAsync(id);

                if (!delete)
                    return NotFound(new { message = "Requisição de usuário não encontrada." });

                return Ok(new { message = "Requisição de usuário excluída com sucesso." });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return Conflict(new
                {
                    message = "Não é possível excluir esta requisição pois existem registros associados a ela. " +
                              "Verifique os vínculos antes de tentar novamente."
                });
            }
        }

        /// <summary>
        /// Endpoint para GERAR REGISTRO DE COBRANÇA NO ASAAS
        /// </summary>
        [HttpPost("with-payment")]
        public async Task<IActionResult> CreateWithPaymentAsync([FromBody] UserRequestDTO userRequest)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            try
            {
                var result = await _userRequestService.CreateWithPaymentAsync(userRequest);
                return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
