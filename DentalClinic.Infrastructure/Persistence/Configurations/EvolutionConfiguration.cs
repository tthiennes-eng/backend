using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class EvolutionConfiguration : IEntityTypeConfiguration<Evolution>
{
    public void Configure(EntityTypeBuilder<Evolution> builder)
    {
        // Nome da tabela conforme DATABASE_SCHEMA.sql
        builder.ToTable("evolucoes");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        // No SQL a evolução se liga ao prontuário, mas para simplificar o MVP
        // vamos mapear o PatientId para prontuario_id (ou paciente_id se você ajustar o SQL)
        // Se o seu SQL tem prontuario_id, precisamos garantir que essa relação exista.
        builder.Property(e => e.PatientId).HasColumnName("prontuario_id");

        builder.Property(e => e.StudentId).HasColumnName("aluno_id");
        builder.Property(e => e.ProfessorId).HasColumnName("professor_id");
        builder.Property(e => e.ClinicId).HasColumnName("clinica_id");

        builder.Property(e => e.Description)
            .HasColumnName("descricao")
            .IsRequired()
            .HasColumnType("text");

        builder.Property(e => e.IsSignedByProfessor)
            .HasColumnName("assinatura_professor_digital")
            .HasConversion(
                v => v ? "ASSINADO" : null, // Salva um texto se estiver assinado
                v => v != null
            );

        builder.Property(e => e.CreatedAt).HasColumnName("data_atendimento");
        builder.Ignore(e => e.SignedAt);
        builder.Ignore(e => e.UpdatedAt);

        // Relacionamentos
        builder.HasOne(e => e.Patient)
            .WithMany(p => p.Evolutions)
            .HasForeignKey(e => e.PatientId);

        builder.HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId);

        builder.HasOne(e => e.Professor)
            .WithMany()
            .HasForeignKey(e => e.ProfessorId);
    }
}
