using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinic.Core.Domain.Entities
{
    public class Attachment
    {
        [Key]
        public Guid Id { get; set; } // Alterado para Guid se necessário

        public Guid PatientId { get; set; } // Alterado de int para Guid

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        public Guid? MedicalRecordId { get; set; } // Alterado de int? para Guid?

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FileType { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public Guid? UploadedByUserId { get; set; } // Alterado de int? para Guid?
    }
}