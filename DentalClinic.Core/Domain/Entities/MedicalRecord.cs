using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinic.Core.Domain.Entities
{
    public class MedicalRecord
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PatientId { get; set; } // Alterado de int para Guid

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        public DateTime DateOpened { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        public string Status { get; set; } = "Active";
    }
}