namespace DentalClinic.Core.Domain.ValueObjects;

/// <summary>
/// Papéis de usuário no sistema.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Administrador do sistema.
    /// </summary>
    Administrator = 1,

    /// <summary>
    /// Coordenador da clínica.
    /// </summary>
    Coordinator = 2,

    /// <summary>
    /// Professor responsável.
    /// </summary>
    Professor = 3,

    /// <summary>
    /// Aluno.
    /// </summary>
    Student = 4,

    /// <summary>
    /// Recepcionista.
    /// </summary>
    Receptionist = 5,

    /// <summary>
    /// Secretária.
    /// </summary>
    Secretary = 6
}

/// <summary>
/// Status de usuário no sistema.
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// Usuário ativo e pode acessar o sistema.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Usuário desativado não pode acessar o sistema.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Usuário foi bloqueado por tentativas inválidas de login.
    /// </summary>
    Blocked = 3,

    /// <summary>
    /// Usuário aguardando confirmação de email.
    /// </summary>
    PendingEmailConfirmation = 4
}

/// <summary>
/// Status de paciente no sistema.
/// </summary>
public enum PatientStatus
{
    /// <summary>
    /// Paciente ativo no sistema.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Paciente desativado.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Paciente teve seus dados anonimizados por requisição LGPD.
    /// </summary>
    DataAnonymized = 3
}

/// <summary>
/// Especialidades dentárias das clínicas.
/// </summary>
public enum Specialty
{
    /// <summary>
    /// Endodontia.
    /// </summary>
    Endodontics = 1,

    /// <summary>
    /// Dentística.
    /// </summary>
    Dentistry = 2,

    /// <summary>
    /// Cirurgia.
    /// </summary>
    Surgery = 3,

    /// <summary>
    /// Prótese.
    /// </summary>
    Prosthetics = 4,

    /// <summary>
    /// Periodontia.
    /// </summary>
    Periodontics = 5,

    /// <summary>
    /// Odontopediatria.
    /// </summary>
    PediatricDentistry = 6,

    /// <summary>
    /// Implantodontia.
    /// </summary>
    Implantology = 7,

    /// <summary>
    /// Clínica Integrada.
    /// </summary>
    IntegratedClinic = 8
}

/// <summary>
/// Status de agendamento de consulta.
/// </summary>
public enum ScheduleStatus
{
    /// <summary>
    /// Slot disponível para agendamento.
    /// </summary>
    Available = 1,

    /// <summary>
    /// Slot agendado para um paciente.
    /// </summary>
    Scheduled = 2,

    /// <summary>
    /// Consulta foi concluída.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Consulta foi cancelada.
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Slot foi bloqueado (férias, manutenção, etc).
    /// </summary>
    Blocked = 5
}

/// <summary>
/// Status de prontuário médico.
/// </summary>
public enum MedicalRecordStatus
{
    /// <summary>
    /// Prontuário em rascunho.
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Prontuário aguardando revisão do professor.
    /// </summary>
    PendingReview = 2,

    /// <summary>
    /// Prontuário aprovado pelo professor.
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Prontuário foi rejeitado e precisa de correções.
    /// </summary>
    Rejected = 4
}

/// <summary>
/// Tipo de auditoria registrada.
/// </summary>
public enum AuditLogType
{
    /// <summary>
    /// Login no sistema.
    /// </summary>
    Login = 1,

    /// <summary>
    /// Logout do sistema.
    /// </summary>
    Logout = 2,

    /// <summary>
    /// Criação de registro.
    /// </summary>
    Create = 3,

    /// <summary>
    /// Atualização de registro.
    /// </summary>
    Update = 4,

    /// <summary>
    /// Exclusão de registro.
    /// </summary>
    Delete = 5,

    /// <summary>
    /// Acesso a prontuário.
    /// </summary>
    AccessMedicalRecord = 6,

    /// <summary>
    /// Tentativa de login inválida.
    /// </summary>
    FailedLogin = 7,

    /// <summary>
    /// Alteração de permissões.
    /// </summary>
    PermissionChange = 8
}
