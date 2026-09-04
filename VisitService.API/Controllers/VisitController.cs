using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VisitService.Business.Interfaces;
using VisitService.Shared.dtos;

namespace VisitService.Api.Controllers
{
    [ApiController]
    [Route("api/visits")]
    [Authorize]
    public class VisitsController : ControllerBase
    {
        private readonly IVisitService visitService;

        public VisitsController(IVisitService visitService)
        {
            this.visitService = visitService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVisitDto dto, CancellationToken ct)
        {
            int userId = GetUserId();

            await visitService.AddAsync(dto, userId, ct);
            return Created();
        }

        [HttpPatch("{id:int}/confirm")]
        public async Task<IActionResult> Confirm(int id, CancellationToken ct)
        {
            int userId = GetUserId();

            await visitService.ConfirmVisitAsync(id, userId, ct);
            return Ok((new { message = "Visita confermata con successo" }));
        }

        [HttpPatch("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id, CancellationToken ct)
        {
            int userId = GetUserId();

            await visitService.RejectVisitAsync(id, userId, ct);
            return Ok((new { message = "Visita rifiutata con successo" }));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            int userId = GetUserId();

            await visitService.DeleteAsync(id, userId, ct);
            return Ok((new { message = "Visita eliminata con successo" }));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateVisitDto dto, CancellationToken ct)
        {
            int userId = GetUserId();

            await visitService.UpdateAsync(id, dto, userId, ct);
            return Ok((new { message = "Visita aggiornata con successo" }));
        }

        [HttpGet]
        public async Task<ActionResult<List<VisitDto>>> GetAll(CancellationToken ct)
        {
            int userId = GetUserId();

            var visits = await visitService.GetAllAsync(userId, ct);
            return Ok(visits);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VisitDto>> GetById(int id, CancellationToken ct)
        {
            int userId = GetUserId();

            var visit = await visitService.GetByIdAsync(id, userId, ct);
            return Ok(visit);
        }

        private int GetUserId()
        {
            string? userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("ID utente non trovato nel token");

            return int.Parse(userIdClaim);
        }
    }
}
