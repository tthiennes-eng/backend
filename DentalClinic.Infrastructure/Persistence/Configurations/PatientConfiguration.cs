using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("pacientes");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.OwnsOne(p => p.CPF, cpf =>
        {
            cpf.Property(c => c.Value)
                .HasColumnName("cpf")
                .IsRequired()
                .HasMaxLength(11);

            cpf.HasIndex(c => c.Value).IsUnique();
        });

        builder.Property(p => p.DateOfBirth)
            .IsRequired();

        builder.OwnsOne(p => p.EmailAddress, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .HasMaxLength(255);
        });

        builder.Property(p => p.Phone)
            .HasMaxLength(20);

        builder.OwnsOne(p => p.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("endereco_rua").HasMaxLength(255);
            address.Property(a => a.Number).HasColumnName("endereco_numero").HasMaxLength(20);
            address.Property(a => a.Neighborhood).HasColumnName("endereco_bairro").HasMaxLength(100);
            address.Property(a => a.City).HasColumnName("endereco_cidade").HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("endereco_estado").HasMaxLength(2);
            address.Property(a => a.ZipCode).HasColumnName("endereco_cep").HasMaxLength(10);
        });

        builder.Property(p => p.Status)
            .HasConversion<int>();

        // Auditoria LGPD: Índice para buscas frequentes por CPF
        builder.HasIndex(p => p.Name);
    }
}
