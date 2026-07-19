using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.Api.Controllers;

/// <summary>
/// Controller para gestão das clínicas e especialidades da universidade.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClinicsController : ControllerBase
{
    private readonly IClinicRepository _repository;

    public ClinicsController(IClinicRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Lista todas as clínicas ativas.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clinics = await _repository.GetAllAsync();
        return Ok(clinics);
    }

    /// <summary>
    /// Cadastra uma nova clínica/departamento.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Clinic clinic)
    {
        await _repository.AddAsync(clinic);
        return CreatedAtAction(nameof(GetAll), new { id = clinic.Id }, clinic);
    }

    /// <summary>
    /// Atualiza os dados de uma clínica.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Coordinator")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Clinic clinic)
    {
        if (id != clinic.Id) return BadRequest();
        await _repository.UpdateAsync(clinic);
        return NoContent();
    }
}
