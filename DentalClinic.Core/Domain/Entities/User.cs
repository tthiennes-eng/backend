using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DentalClinic.Core.Domain.ValueObjects;

namespace DentalClinic.Core.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        // O Email agora é uma string simples para evitar erros de validação no carregamento do EF
        // A validação de formato deve ocorrer na camada de aplicação (Service/Validator)
        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(11)]
        public string? Cpf { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        // Endereço desnormalizado para evitar problemas com Owned Types complexos neste momento
        [MaxLength(255)]
        public string? EnderecoRua { get; set; }
        [MaxLength(50)]
        public string? EnderecoNumero { get; set; }
        [MaxLength(100)]
        public string? EnderecoComplemento { get; set; }
        [MaxLength(100)]
        public string? EnderecoBairro { get; set; }
        [MaxLength(100)]
        public string? EnderecoCidade { get; set; }
        [MaxLength(2)]
        public string? EnderecoEstado { get; set; }
        [MaxLength(9)]
        public string? EnderecoCep { get; set; }

        // Roles armazenado como JSON string ou separado em outra tabela se preferir
        // Para simplificar, usaremos uma string ou coluna separada se o modelo exigir
        [MaxLength(50)]
        public string Role { get; set; } = "User"; 

        public int Status { get; set; } = 0; // 0 = Active, 1 = Blocked, etc.
        
        public bool EmailConfirmed { get; set; } = false;
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LastLoginAt { get; set; }
        public DateTime? BlockedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Relacionamentos
        public ICollection<Appointment>? Appointments { get; set; }
        public ICollection<Attachment>? Attachments { get; set; }
        public ICollection<Evolution>? Evolutions { get; set; }

        // Construtor padrão seguro para o Entity Framework
        public User()
        {
            // Inicializações seguras para evitar nulls durante o materialização do EF
            Name = string.Empty;
            Email = string.Empty;
            PasswordHash = string.Empty;
            Role = "User";
        }
        
        // Construtor para criação de novos usuários com validação na camada de domínio
        public User(string name, string email, string passwordHash, string role = "User")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));
            
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            // Validação simples de formato de email
            if (!email.Contains("@") || !email.Contains("."))
                throw new ArgumentException("Invalid email format", nameof(email));

            Id = Guid.NewGuid();
            Name = name;
            Email = email.ToLower();
            PasswordHash = passwordHash;
            Role = role;
            CreatedAt = DateTime.UtcNow;
            Status = 0; // Active
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
                Status = 1; // Blocked
            }
        }

        public void ResetFailedLogin()
        {
            FailedLoginAttempts = 0;
            BlockedAt = null;
            if (Status == 1) Status = 0; // Unblock
        }
    }
}