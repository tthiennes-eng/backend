using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DentalClinic.Api.Controllers;

/// <summary>
/// Controller para gestão de notificações e alertas do sistema.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _repository;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationRepository repository, ILogger<NotificationsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Recupera as notificações do usuário autenticado.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var notifications = await _repository.GetByUserIdAsync(Guid.Parse(userId));
        return Ok(notifications);
    }

    /// <summary>
    /// Marca uma notificação específica como lida.
    /// </summary>
    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var notification = await _repository.GetByIdAsync(id);
        if (notification == null) return NotFound();

        notification.MarkAsRead();
        await _repository.UpdateAsync(notification);
        return NoContent();
    }

    /// <summary>
    /// Marca todas as notificações do usuário como lidas.
    /// </summary>
    [HttpPost("mark-all-read")]
    public async Task<IActionResult> MarkAllRead()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _repository.MarkAllAsReadAsync(Guid.Parse(userId));
        return NoContent();
    }
}
