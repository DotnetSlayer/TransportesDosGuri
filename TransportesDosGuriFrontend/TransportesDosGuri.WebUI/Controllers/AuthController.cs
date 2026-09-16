using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginDTO loginDto)
    {
        var success = await _authService.LoginAsync(loginDto);
        if (!success)
            return Redirect("/login?error=InvalidCredentials");

        return Redirect("/adminHome");
    }

    [HttpGet("logout")]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // 1. Invalida tokens na API remota e limpa LocalStorage
        await _authService.LogoutAsync();

        // 2. Destrói o Cookie local enviando o Set-Cookie de expiração nos cabeçalhos
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // 3. Redireciona para a tela inicial
        return Redirect("/");
    }
}