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

        /// <summary>
        /// Endpoint to REGISTER USER in AspNetUsers Table
        /// </summary>
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, errors, userId) = await _identityService.RegisterAsync(register);

            if (!success)
            {
                return BadRequest(new { Errors = errors });
            }

            return Ok(new { Message = "User Successfully Registered!", UserId = userId, register.Email });
        }

        /// <summary>
        /// Endpoint to LOGIN USER in AspNetUsers Table
        /// </summary>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, errors, userId, jwt) = await _identityService.LoginAsync(login);

            if (!success)
            {
                return Unauthorized(new { Errors = errors });
            }

            return Ok(new { Message = "User Successfully Logged In!", UserId = userId, Email = login.Email, Token = jwt });
        }

        /// <summary>
        /// Endpoint to GET USER BY ID in AspNetUsers Table
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var user = await _identityService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { Message = "User Not Found!" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Endpoint to GET ALL USERS in AspNetUsers Table
        /// </summary>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _identityService.GetAllAsync();

            return Ok(users);
        }

        /// <summary>
        /// Endpoint to UPDATE USER stored in AspNetUsers Table
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(long id, [FromBody] UpdateDTO update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, errors) = await _identityService.UpdateAsync(id, update);

            if (!success)
            {
                return BadRequest(new { Errors = errors });
            }

            return Ok(new { Message = "User Successfully Updated!" });
        }

        /// <summary>
        /// Endpoint to DELETE USER stored in AspNetUsers Table
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var (success, errors) = await _identityService.DeleteAsync(id);

            if (!success)
            {
                return BadRequest(new { Errors = errors });
            }

            return Ok(new { Message = "User Successfully Deleted!" });
        }

        /// <summary>
        /// Endpoint to GENERATE a new and valid USER ACCESS TOKEN 
        /// </summary>
        [HttpPost("generate-new-jwt-token")]
        public async Task<IActionResult> GenerateNewAccessToken([FromBody] TokenModelDTO tokenModel)
        {
            var (success, response, errors) = await _identityService.GenerateNewAccessTokenAsync(tokenModel);

            if (!success)
            {
                return BadRequest(new { Errors = errors });
            }

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to Logout a valid USER session 
        /// </summary>
        [HttpPost("Logout")]
        [Authorize] // Garante que apenas usuários autenticados chamem ou passe os tokens via body
        public async Task<IActionResult> Logout([FromBody] TokenModelDTO tokenModel)
        {
            var (success, errors) = await _identityService.LogoutAsync(tokenModel);

            if (!success)
            {
                return BadRequest(new { Errors = errors });
            }

            return Ok(new { Message = "User Successfully Logged Out!" });
        }
    }
}
