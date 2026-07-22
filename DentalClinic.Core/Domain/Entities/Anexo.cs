using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinic.Core.Domain.Entities
{
    public class Anexo
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PatientId { get; set; } // Alterado de int para Guid

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        [Required]
        [MaxLength(50)]
        public string Tipo { get; set; } = string.Empty; // 'Radiografia', 'Foto', 'Documento'

        [Required]
        [MaxLength(255)]
        public string NomeArquivo { get; set; } = string.Empty;

        [Required]
        public string UrlStorage { get; set; } = string.Empty;

        public string? MetaData { get; set; } // Armazenado como string se não houver suporte a jsonb aqui

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        public Guid? CriadoPorId { get; set; } // Alterado de int? para Guid?
    }
}