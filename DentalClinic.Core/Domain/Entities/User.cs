using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DentalClinic.Core.Domain.ValueObjects;

namespace DentalClinic.Core.Domain.Entities;

public class User : Entity
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty; // Mapeado para nome_completo no config

    [Required]
    public Email EmailAddress { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = string.Empty; // Mapeado para senha_hash

    public CPF? CPF { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    public Address? Address { get; set; }

    public UserRole Role { get; set; } = UserRole.Student;

    public List<UserRole> Roles { get; set; } = new();

    public bool IsActive { get; set; } = true; // Mapeado para 'ativo'

    public bool EmailConfirmed { get; set; } = false;
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LastLoginAt { get; set; }
    public DateTime? BlockedAt { get; set; }

    // Relacionamentos
    public ICollection<Appointment>? Appointments { get; set; }
    public ICollection<Evolution>? Evolutions { get; set; }

    protected User()
    {
        Roles = new List<UserRole>();
    }

    public User(string name, Email email, string passwordHash, UserRole role = UserRole.Student)
    {
        Id = Guid.NewGuid();
        Name = name;
        EmailAddress = email;
        PasswordHash = passwordHash;
        Role = role;
        Roles = new List<UserRole> { role };
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static User Create(string name, string email, string cpf, DateTime? dateOfBirth, string? phone, Address? address, UserRole role)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            EmailAddress = Email.Create(email),
            CPF = !string.IsNullOrEmpty(cpf) ? CPF.Create(cpf) : null,
            DateOfBirth = dateOfBirth,
            Phone = phone,
            Address = address,
            Role = role,
            Roles = new List<UserRole> { role },
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void SetPasswordHash(string hash) => PasswordHash = hash;
    public void ConfirmEmail() => EmailConfirmed = true;
    public void IncrementFailedLogin() => FailedLoginAttempts++;
    public void ResetFailedLogin() => FailedLoginAttempts = 0;

    public void UpdateLoginInfo()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
    }
}
