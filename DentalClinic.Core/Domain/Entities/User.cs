using DentalClinic.Core.Domain.ValueObjects;

namespace DentalClinic.Core.Domain.Entities;

/// <summary>
/// Entidade que representa um usuário do sistema.
/// </summary>
public sealed class User : Entity
{
    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Email do usuário (Value Object).
    /// </summary>
    public Email EmailAddress { get; private set; }

    /// <summary>
    /// Hash da senha armazenado (BCrypt).
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    /// CPF do usuário (Value Object).
    /// </summary>
    public CPF CPF { get; private set; }

    /// <summary>
    /// Data de nascimento.
    /// </summary>
    public DateTime DateOfBirth { get; private set; }

    /// <summary>
    /// Telefone de contato.
    /// </summary>
    public string Phone { get; private set; }

    /// <summary>
    /// Endereço (Value Object).
    /// </summary>
    public Address Address { get; private set; }

    /// <summary>
    /// Papéis do usuário.
    /// </summary>
    public List<UserRole> Roles { get; private set; }

    /// <summary>
    /// Status do usuário.
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// Indica se o email foi confirmado.
    /// </summary>
    public bool EmailConfirmed { get; private set; }

    /// <summary>
    /// Data e hora do último login.
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// Tentativas de login inválidas consecutivas.
    /// </summary>
    public int FailedLoginAttempts { get; private set; }

    /// <summary>
    /// Data e hora do bloqueio (se bloqueado).
    /// </summary>
    public DateTime? BlockedAt { get; private set; }

    private User()
    {
        Name = string.Empty;
        EmailAddress = Email.Create(string.Empty);
        PasswordHash = string.Empty;
        CPF = CPF.Create("00000000000");
        Phone = string.Empty;
        Address = Address.Create(".", ".", null, ".", ".", "SP", "00000-000");
        Roles = [];
        Status = UserStatus.Active;
    }

    /// <summary>
    /// Cria um novo usuário com validações.
    /// </summary>
    /// <param name="name">Nome completo.</param>
    /// <param name="email">Email (será validado como Value Object).</param>
    /// <param name="cpf">CPF (será validado como Value Object).</param>
    /// <param name="dateOfBirth">Data de nascimento.</param>
    /// <param name="phone">Telefone.</param>
    /// <param name="address">Endereço (Value Object).</param>
    /// <param name="role">Papel do usuário.</param>
    /// <returns>Um novo usuário com senha não setada ainda.</returns>
    public static User Create(
        string name,
        string email,
        string cpf,
        DateTime dateOfBirth,
        string phone,
        Address address,
        UserRole role)
    {
        ValidateName(name);
        ValidateDateOfBirth(dateOfBirth);
        ValidatePhone(phone);

        var user = new User
        {
            Name = name.Trim(),
            EmailAddress = Email.Create(email),
            CPF = CPF.Create(cpf),
            DateOfBirth = dateOfBirth,
            Phone = phone.Trim(),
            Address = address,
            Status = UserStatus.PendingEmailConfirmation,
            EmailConfirmed = false,
            FailedLoginAttempts = 0,
            Roles = [role]
        };

        return user;
    }

    /// <summary>
    /// Define o hash da senha.
    /// </summary>
    /// <param name="passwordHash">Hash da senha (deve ser gerado com BCrypt).</param>
    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new InvalidOperationException("Hash da senha não pode ser vazio.");

        PasswordHash = passwordHash;
    }

    /// <summary>
    /// Marca o email como confirmado e ativa o usuário.
    /// </summary>
    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        Status = UserStatus.Active;
    }

    /// <summary>
    /// Registra um login bem-sucedido.
    /// </summary>
    public void RegisterSuccessfulLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;

        if (Status == UserStatus.Blocked)
            Status = UserStatus.Active;

        BlockedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Registra uma tentativa de login inválida.
    /// </summary>
    public void RegisterFailedLoginAttempt()
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= 5)
        {
            Status = UserStatus.Blocked;
            BlockedAt = DateTime.UtcNow;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Desbloqueia o usuário.
    /// </summary>
    public void Unblock()
    {
        Status = UserStatus.Active;
        FailedLoginAttempts = 0;
        BlockedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Desativa o usuário.
    /// </summary>
    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Ativa o usuário.
    /// </summary>
    public void Activate()
    {
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adiciona um papel ao usuário.
    /// </summary>
    /// <param name="role">Papel a adicionar.</param>
    public void AddRole(UserRole role)
    {
        if (!Roles.Contains(role))
        {
            Roles.Add(role);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Remove um papel do usuário.
    /// </summary>
    /// <param name="role">Papel a remover.</param>
    public void RemoveRole(UserRole role)
    {
        if (Roles.Remove(role))
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Verifica se o usuário pode acessar o sistema.
    /// </summary>
    /// <returns>Verdadeiro se ativo, falso caso contrário.</returns>
    public bool CanAccess() => Status == UserStatus.Active && EmailConfirmed;

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Nome não pode ser vazio.");

        if (name.Length < 3)
            throw new InvalidOperationException("Nome deve ter no mínimo 3 caracteres.");

        if (name.Length > 255)
            throw new InvalidOperationException("Nome não pode ter mais de 255 caracteres.");
    }

    private static void ValidateDateOfBirth(DateTime dateOfBirth)
    {
        var age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.Today)
            age--;

        if (age < 18)
            throw new InvalidOperationException("Usuário deve ter no mínimo 18 anos.");

        if (age > 120)
            throw new InvalidOperationException("Data de nascimento inválida.");
    }

    private static void ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new InvalidOperationException("Telefone não pode ser vazio.");

        var onlyNumbers = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");
        if (onlyNumbers.Length < 10 || onlyNumbers.Length > 11)
            throw new InvalidOperationException("Telefone deve ter entre 10 e 11 dígitos.");
    }
}
