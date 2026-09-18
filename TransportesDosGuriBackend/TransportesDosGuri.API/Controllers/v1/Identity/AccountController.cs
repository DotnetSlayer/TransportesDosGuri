using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportesDosGuri.Core.Application.DTOs.Identity;
using TransportesDosGuri.Core.Application.DTOs.Jwt;
using TransportesDosGuri.Core.Application.ServiceContracts.Identity;

namespace TransportesDosGuri.API.Controllers.v1.Identity
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/[controller]")]
    public class AccountController : CustomControllerBase
    {
        private readonly IIdentityService _identityService;

        public AccountController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        // =====================================================================
        // AUTENTICAÇÃO (anônimo)
        // =====================================================================

        /// <summary>
        /// Endpoint para REGISTRAR USUÁRIO na tabela AspNetUsers
        /// </summary>
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, errors, userId) = await _identityService.RegisterAsync(register);

            if (!success)
                return BadRequest(new { Errors = errors });

            return Ok(new
            {
                Message = "Usuário Registrado com Sucesso!",
                UserId = userId,
                register.Email
            });
        }

        /// <summary>
        /// Endpoint para LOGAR USUÁRIO presente na tabela AspNetUsers
        /// </summary>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, errors, userId, jwt) = await _identityService.LoginAsync(login);

            if (!success)
                return Unauthorized(new { Errors = errors });

            return Ok(new
            {
                Message = "Usuário Logado com Sucesso!",
                UserId = userId,
                Email = login.Email,
                Token = jwt
            });
        }

        // =====================================================================
        // ROTAS LITERAIS (declarar ANTES das rotas com {id})
        // =====================================================================

        /// <summary>
        /// Endpoint para RETORNAR OS DADOS DO USUÁRIO LOGADO
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var user = await _identityService.GetByIdAsync(userId);

            if (user == null)
                return NotFound(new { Message = "Usuário Não Encontrado!" });

            return Ok(user);
        }

        /// <summary>
        /// Endpoint para ATUALIZAR OS DADOS DO USUÁRIO LOGADO
        /// </summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateDTO update)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, errors) = await _identityService.UpdateAsync(userId, update);

            if (!success)
                return BadRequest(new { Errors = errors });

            return Ok(new { Message = "Usuário Atualizado com Sucesso!" });
        }

        /// <summary>
        /// Endpoint para EXCLUIR A PRÓPRIA CONTA (usuário logado)
        /// </summary>
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMe()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            try
            {
                var (success, errors) = await _identityService.DeleteAsync(userId);

                if (!success)
                    return BadRequest(new { Errors = errors });

                return Ok(new { Message = "Sua conta foi excluída com sucesso." });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return Conflict(new
                {
                    message = "Não é possível excluir sua conta pois existem compras, reservas ou solicitações associadas. " +
                              "Entre em contato com o suporte."
                });
            }
        }

        /// <summary>
        /// Endpoint para RETORNAR TODOS OS USUÁRIOS presentes na tabela AspNetUsers
        /// </summary>
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _identityService.GetAllAsync();
            return Ok(users);
        }

        // =====================================================================
        // ROTAS COM {id} (somente Admin)
        // =====================================================================

        /// <summary>
        /// Endpoint para RETORNAR UM USUÁRIO POR ID (Admin)
        /// </summary>
        [HttpGet("{id:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(long id)
        {
            var user = await _identityService.GetByIdAsync(id);

            if (user == null)
                return NotFound(new { Message = "Usuário Não Encontrado!" });

            return Ok(user);
        }

        /// <summary>
        /// Endpoint para ATUALIZAR UM USUÁRIO POR ID (Admin)
        /// </summary>
        [HttpPut("{id:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long id, [FromBody] UpdateDTO update)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, errors) = await _identityService.UpdateAsync(id, update);

            if (!success)
                return BadRequest(new { Errors = errors });

            return Ok(new { Message = "Usuário Atualizado com Sucesso!" });
        }

        /// <summary>
        /// Endpoint para EXCLUIR UM USUÁRIO POR ID (Admin)
        /// </summary>
        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var (success, errors) = await _identityService.DeleteAsync(id);

                if (!success)
                    return BadRequest(new { Errors = errors });

                return Ok(new { Message = "Usuário Excluído com Sucesso!" });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return Conflict(new
                {
                    message = "Não é possível excluir este usuário pois existem compras, reservas ou solicitações associadas a ele. " +
                              "Considere desativar o usuário ao invés de excluí-lo."
                });
            }
        }

        // =====================================================================
        // JWT / LOGOUT
        // =====================================================================

        /// <summary>
        /// Endpoint para GERAR UM NOVO TOKEN
        /// </summary>
        [HttpPost("generate-new-jwt-token")]
        public async Task<IActionResult> GenerateNewAccessToken([FromBody] TokenModelDTO tokenModel)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var (success, response, errors) = await _identityService.GenerateNewAccessTokenAsync(tokenModel);

            if (!success)
                return BadRequest(new { Errors = errors });

            return Ok(response);
        }

        /// <summary>
        /// Endpoint para FAZER LOGOUT
        /// </summary>
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] TokenModelDTO tokenModel)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var (success, errors) = await _identityService.LogoutAsync(tokenModel);

            if (!success)
                return BadRequest(new { Errors = errors });

            return Ok(new { Message = "Usuário Deslogado com Sucesso!" });
        }
    }
}