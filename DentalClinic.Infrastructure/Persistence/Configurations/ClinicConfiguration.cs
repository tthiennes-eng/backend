using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
{
    public void Configure(EntityTypeBuilder<Clinic> builder)
    {
        builder.ToTable("clinicas");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.Specialty)
            .HasConversion<int>()
            .IsRequired();

        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("endereco_rua").HasMaxLength(255);
            address.Property(a => a.Number).HasColumnName("endereco_numero").HasMaxLength(20);
            address.Property(a => a.Neighborhood).HasColumnName("endereco_bairro").HasMaxLength(100);
            address.Property(a => a.City).HasColumnName("endereco_cidade").HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("endereco_estado").HasMaxLength(2);
            address.Property(a => a.ZipCode).HasColumnName("endereco_cep").HasMaxLength(10);
        });

        builder.Property(c => c.OpeningTime)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(c => c.ClosingTime)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(c => c.MaxCapacity)
            .IsRequired();

        builder.Property(c => c.CoordinatorUserId)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);
    }
}
