using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DentalClinic.Core.Domain.ValueObjects;

namespace DentalClinic.Core.Domain.Entities;

public class Patient : Entity
{
    // O ID já vem da classe base Entity como Guid

    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(14)]
    public string CPF { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? RG { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Gender { get; set; }

    public DateTime BirthDate { get; set; }

    public Address? Address { get; set; }

    public bool LgpdConsent { get; set; }

    public bool IsActive { get; set; } = true;

    // Relacionamentos
    public ICollection<Appointment>? Appointments { get; set; }
    public ICollection<Evolution>? Evolutions { get; set; }
}
