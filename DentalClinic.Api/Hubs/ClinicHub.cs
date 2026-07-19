using Microsoft.AspNetCore.SignalR;

namespace DentalClinic.Api.Hubs;

/// <summary>
/// Hub para comunicação em tempo real entre o servidor e os aplicativos Flutter.
/// Gerencia atualizações de Odontograma, Notificações e Agenda.
/// </summary>
public class ClinicHub : Hub
{
    /// <summary>
    /// Adiciona o usuário a um grupo específico (ex: ID do Paciente) para receber atualizações do prontuário.
    /// </summary>
    public async Task JoinPatientGroup(string patientId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"patient_{patientId}");
    }

    /// <summary>
    /// Remove o usuário do grupo do paciente.
    /// </summary>
    public async Task LeavePatientGroup(string patientId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"patient_{patientId}");
    }

    /// <summary>
    /// Notifica todos os interessados sobre uma mudança no Odontograma.
    /// </summary>
    public async Task NotifyOdontogramUpdate(string patientId, object updateData)
    {
        await Clients.Group($"patient_{patientId}").SendAsync("ReceiveOdontogramUpdate", updateData);
    }
}
