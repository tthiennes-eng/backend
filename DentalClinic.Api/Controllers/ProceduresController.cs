using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.Api.Controllers;

/// <summary>
/// Controller para gestão do catálogo de procedimentos da clínica escola.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProceduresController : ControllerBase
{
    private readonly IProcedureRepository _repository;

    public ProceduresController(IProcedureRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Lista todos os procedimentos disponíveis.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var procedures = await _repository.GetAllAsync();
        return Ok(procedures);
    }

    /// <summary>
    /// Lista procedimentos filtrados por especialidade.
    /// </summary>
    [HttpGet("specialty/{specialty}")]
    public async Task<IActionResult> GetBySpecialty(Specialty specialty)
    {
        var procedures = await _repository.GetBySpecialtyAsync(specialty);
        return Ok(procedures);
    }

    /// <summary>
    /// Cadastra um novo procedimento no catálogo.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Coordinator")]
    public async Task<IActionResult> Create([FromBody] Procedure procedure)
    {
        await _repository.AddAsync(procedure);
        return CreatedAtAction(nameof(GetAll), new { id = procedure.Id }, procedure);
    }
}
