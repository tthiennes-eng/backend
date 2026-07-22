using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DentalClinic.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProntuarioController : ControllerBase
{
    private readonly IProntuarioRepository _prontuarioRepository;

    public ProntuarioController(IProntuarioRepository prontuarioRepository)
    {
        _prontuarioRepository = prontuarioRepository;
    }

    [HttpGet("{patientId}/odontogram")]
    public async Task<IActionResult> GetOdontogram(Guid patientId)
    {
        var odontogram = await _prontuarioRepository.GetOdontogramByPatientIdAsync(patientId);
        return Ok(odontogram);
    }

    [HttpGet("{patientId}/anamnese")]
    public async Task<IActionResult> GetAnamnese(Guid patientId)
    {
        var anamnese = await _prontuarioRepository.GetAnamneseByPatientIdAsync(patientId);
        return Ok(anamnese);
    }

    [HttpPost("{patientId}/anamnese")]
    public async Task<IActionResult> SaveAnamnese(Guid patientId, [FromBody] IDictionary<string, object> responses)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var json = System.Text.Json.JsonSerializer.Serialize(responses);
        var anamnese = Anamnese.Create(patientId, json, Guid.Parse(userId));

        await _prontuarioRepository.SaveAnamneseAsync(anamnese);
        return Ok();
    }

    [HttpGet("{patientId}/evolutions")]
    public async Task<IActionResult> GetEvolutions(Guid patientId)
    {
        var evolutions = await _prontuarioRepository.GetEvolutionHistoryAsync(patientId);
        return Ok(evolutions);
    }
}
