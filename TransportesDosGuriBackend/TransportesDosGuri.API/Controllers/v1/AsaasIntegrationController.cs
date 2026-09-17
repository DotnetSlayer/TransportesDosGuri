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
    public class AsaasIntegrationController : CustomControllerBase
    {
        private readonly IAsaasIntegrationService _asaasIntegrationsService;

        public AsaasIntegrationController(IAsaasIntegrationService asaasIntegrationsService)
        {
            _asaasIntegrationsService = asaasIntegrationsService;
        }

        /// <summary>
        /// Endpoint para RETORNAR TODAS AS INTEGRAÇÕES ASAAS presentes na tabela AsaasIntegration
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAsync()
        {
            var getAll = await _asaasIntegrationsService.GetAllAsync();

            return Ok(getAll);
        }

        /// <summary>
        /// Endpoint para RETORNAR UMA INTEGRAÇÃO ASAAS POR ID presente na tabela AsaasIntegration
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var getById = await _asaasIntegrationsService.GetByIdAsync(id);

            return Ok(getById);
        }

        /// <summary>
        /// Endpoint para CRIAR INTEGRAÇÃO ASAAS na tabela AsaasIntegration
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(AsaasIntegrationDTO asaasIntegration)
        {
            var create = await _asaasIntegrationsService.CreateAsync(asaasIntegration);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para ATUALIZAR UMA INTEGRAÇÃO ASAAS POR ID presente na tabela AsaasIntegration
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(long id, AsaasIntegrationDTO asaasIntegration)
        {
            var update = await _asaasIntegrationsService.UpdateAsync(id, asaasIntegration);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para EXCLUIR PERMANENTEMENTE UMA INTEGRAÇÃO ASAAS POR ID presente na tabela AsaasIntegration
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var delete = await _asaasIntegrationsService.DeleteAsync(id);

            return Ok("Base de Dados Atualizada!");
        }
    }
}
