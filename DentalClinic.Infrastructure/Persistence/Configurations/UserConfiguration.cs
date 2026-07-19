using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.OwnsOne(u => u.EmailAddress, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(255);

            email.HasIndex(e => e.Value).IsUnique();
        });

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.OwnsOne(u => u.CPF, cpf =>
        {
            cpf.Property(c => c.Value)
                .HasColumnName("cpf")
                .IsRequired()
                .HasMaxLength(11);

            cpf.HasIndex(c => c.Value).IsUnique();
        });

        builder.Property(u => u.Phone)
            .HasMaxLength(20);

        builder.OwnsOne(u => u.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("endereco_rua").HasMaxLength(255);
            address.Property(a => a.Number).HasColumnName("endereco_numero").HasMaxLength(20);
            address.Property(a => a.Neighborhood).HasColumnName("endereco_bairro").HasMaxLength(100);
            address.Property(a => a.City).HasColumnName("endereco_cidade").HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("endereco_estado").HasMaxLength(2);
            address.Property(a => a.ZipCode).HasColumnName("endereco_cep").HasMaxLength(10);
        });

        builder.Property(u => u.Roles)
            .HasPostgresArrayConversion(); // Requires Npgsql.EntityFrameworkCore.PostgreSQL

        builder.Property(u => u.Status)
            .HasConversion<int>();
    }
}
