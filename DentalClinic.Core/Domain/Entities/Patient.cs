using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DentalClinic.Core.Domain.ValueObjects;

namespace DentalClinic.Core.Domain.Entities;

public class Patient : Entity
{
    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(14)]
    public string CPF { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    [MaxLength(1)]
    public string? Gender { get; set; }

    // Endereço mapeado como JSONB
    public Address? Address { get; set; }

    public bool LgpdConsent { get; set; }

    public bool IsActive { get; set; } = true;

    // Relacionamentos
    public ICollection<Appointment>? Appointments { get; set; }
    public ICollection<Evolution>? Evolutions { get; set; }
}
