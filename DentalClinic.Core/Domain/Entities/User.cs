using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DentalClinic.Core.Domain.ValueObjects;

namespace DentalClinic.Core.Domain.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Email EmailAddress { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public CPF? CPF { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    public Address? Address { get; set; }

    // Qualificação total para evitar ambiguidade com ValueObjects.UserRole
    public DentalClinic.Core.Domain.Entities.UserRole Role { get; set; } = DentalClinic.Core.Domain.Entities.UserRole.Student;

    public List<DentalClinic.Core.Domain.Entities.UserRole> Roles { get; set; } = new();

    public int Status { get; set; } = 0; // 0 = Active, 1 = Blocked

    public bool EmailConfirmed { get; set; } = false;
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LastLoginAt { get; set; }
    public DateTime? BlockedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Appointment>? Appointments { get; set; }
    public ICollection<Attachment>? Attachments { get; set; }
    public ICollection<Evolution>? Evolutions { get; set; }

    protected User()
    {
        Roles = new List<DentalClinic.Core.Domain.Entities.UserRole>();
    }

    public User(string name, Email email, string passwordHash, DentalClinic.Core.Domain.Entities.UserRole role = DentalClinic.Core.Domain.Entities.UserRole.Student)
    {
        Id = Guid.NewGuid();
        Name = name;
        EmailAddress = email;
        PasswordHash = passwordHash;
        Role = role;
        Roles = new List<DentalClinic.Core.Domain.Entities.UserRole> { role };
        CreatedAt = DateTime.UtcNow;
        Status = 0;
    }

    public static User Create(string name, string email, string cpf, DateTime? dateOfBirth, string? phone, Address? address, DentalClinic.Core.Domain.Entities.UserRole role)
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
            Roles = new List<DentalClinic.Core.Domain.Entities.UserRole> { role },
            CreatedAt = DateTime.UtcNow,
            Status = 0
        };
    }

    public void SetPasswordHash(string hash)
    {
        PasswordHash = hash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = 0;
        BlockedAt = null;
        FailedLoginAttempts = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = 1;
        BlockedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLoginInfo()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
    }

    public void IncrementFailedLogin()
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= 5)
        {
            BlockedAt = DateTime.UtcNow;
            Status = 1;
        }
    }

    public void ResetFailedLogin()
    {
        FailedLoginAttempts = 0;
        BlockedAt = null;
        if (Status == 1) Status = 0;
    }
}
