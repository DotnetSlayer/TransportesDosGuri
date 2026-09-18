using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportesDosGuri.Core.Application.DTOs;
using TransportesDosGuri.Core.Application.ServiceContracts;
using TransportesDosGuri.Core.Domain.Entities;

namespace TransportesDosGuri.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/[controller]")]
    public class PurchaseController : CustomControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        /// <summary>
        /// Endpoint para RETORNAR TODAS AS COMPRAS presentes na tabela Purchase
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAsync()
        {
            var getAll = await _purchaseService.GetAllAsync();

            return Ok(getAll);
        }

        /// <summary>
        /// Endpoint para RETORNAR UMA COMPRA POR ID presente na tabela Purchase
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var getById = await _purchaseService.GetByIdAsync(id);

            return Ok(getById);
        }

        /// <summary>
        /// Endpoint para CRIAR COMPRA na tabela Purchase
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(PurchaseDTO purchase)
        {
            var create = await _purchaseService.CreateAsync(purchase);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para ATUALIZAR UMA COPMRA POR ID presente na tabela Purchase
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(long id, PurchaseDTO purchase)
        {
            var update = await _purchaseService.UpdateAsync(id, purchase);

            return Ok("Base de Dados Atualizada!");
        }

        /// <summary>
        /// Endpoint para EXCLUIR PERMANENTEMENTE UMA COMPRA POR ID presente na tabela Purchase
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            try
            {
                var delete = await _purchaseService.DeleteAsync(id);

                if (!delete)
                    return NotFound(new { message = "Compra não encontrada." });

                return Ok(new { message = "Compra excluída com sucesso." });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return Conflict(new
                {
                    message = "Não é possível excluir esta compra pois existem reservas associadas a ela. " +
                              "Exclua ou reatribua essas reservas primeiro."
                });
            }
        }

        /// <summary>
        /// Endpoint para COMPRAR ASSENTO DE VOO 
        /// </summary>
        [HttpPost("buy")]
        public async Task<IActionResult> BuySeats([FromBody] BuySeatRequestDTO request)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            try
            {
                var result = await _purchaseService.BuySeatsAsync(
                    userId,
                    request);

                return CreatedAtAction(
                    nameof(GetMyPurchase),
                    new { id = result.PurchaseId },
                    result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Endpoint para VERIFICAR COMPRAS RELACIONADAS A USUÁRIO ESPECÍFICO
        /// </summary>
        [HttpGet("my/{id:long}")]
        public async Task<IActionResult> GetMyPurchase(long id)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var purchase =
                await _purchaseService.GetUserPurchaseAsync(
                    id,
                    userId);

            if (purchase == null)
                return NotFound(new
                {
                    message = "Compra Não Encontrada."
                });

            return Ok(purchase);
        }

        /// <summary>
        /// Endpoint para PROCESSAR PAGAMENTO
        /// </summary>
        [HttpPost("my/{id:long}/payment")]
        public async Task<IActionResult> ProcessPayment(long id)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var processed =
                await _purchaseService.ProcessPaymentAsync(
                    id,
                    userId);

            if (!processed)
            {
                return BadRequest(new
                {
                    message =
                        "A compra Não Existe, Não pertence ao Usuário " +
                        "Ou Não está aguardando pagamento."
                });
            }

            return Ok(new
            {
                purchaseId = id,
                status = PurchaseStatus.Confirmado,
                message = "Pagamento Processado com Sucesso."
            });
        }

        /// <summary>
        /// Endpoint para VERIFICAR VIAGENS RELACINOADAS A USUÁRIO ESPECÍFICO
        /// </summary>
        [HttpGet("my-trips")]
        public async Task<IActionResult> GetMyTrips()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var trips =
                await _purchaseService.GetMyTripsAsync(userId);

            return Ok(trips);
        }

        /// <summary>
        /// Endpoint para BAIXAR O RECIBO EM PDF DE UMA COMPRA
        /// </summary>
        [HttpGet("my/{id:long}/receipt")]
        public async Task<IActionResult> DownloadReceipt(long id)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var pdfBytes = await _purchaseService.GenerateReceiptPdfAsync(id, userId);

            if (pdfBytes == null)
                return NotFound(new { message = "Recibo não encontrado ou compra não confirmada." });

            return File(pdfBytes, "application/pdf", $"Passagem_{id}.pdf");
        }

        /// <summary>
        /// Endpoint para CRIAR CHECKOUT E CONEXÃO ASAAS
        /// </summary>
        [HttpPost("{purchaseId:long}/checkout")]
        public async Task<IActionResult> CreateCheckout(long purchaseId)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var result = await _purchaseService.CreateCheckoutAsync(purchaseId, userId);

            if (result is null)
                return NotFound(new { message = "Compra não encontrada ou não pertence ao usuário." });

            return Ok(result);
        }
    }
}
