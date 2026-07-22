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
        builder.Property(u => u.Id).HasColumnName("id");

        builder.Property(u => u.Name)
            .HasColumnName("nome_completo")
            .IsRequired()
            .HasMaxLength(255);

        builder.OwnsOne(u => u.EmailAddress, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(255);

            emailBuilder.HasIndex(e => e.Value).IsUnique();
        });

        builder.Property(u => u.PasswordHash)
            .HasColumnName("senha_hash")
            .IsRequired();

        builder.OwnsOne(u => u.CPF, cpfBuilder =>
        {
            cpfBuilder.Property(c => c.Value)
                .HasColumnName("cpf")
                .HasMaxLength(11);

            cpfBuilder.HasIndex(c => c.Value).IsUnique();
        });

        builder.Property(u => u.Phone).HasColumnName("telefone").HasMaxLength(20);
        builder.Property(u => u.DateOfBirth).HasColumnName("data_nascimento");

        // Sincronizado com a nova propriedade IsActive (bool) da entidade User
        builder.Property(u => u.IsActive)
            .HasColumnName("ativo")
            .HasDefaultValue(true);

        builder.Property(u => u.FailedLoginAttempts).HasColumnName("tentativas_falhas");
        builder.Property(u => u.LastLoginAt).HasColumnName("ultimo_login");
        builder.Property(u => u.BlockedAt).HasColumnName("bloqueado_ate");
        builder.Property(u => u.CreatedAt).HasColumnName("criado_em");
        builder.Property(u => u.UpdatedAt).HasColumnName("atualizado_em");

        // Roles como JSONB
        builder.Property(u => u.Roles)
            .HasColumnName("perfis_json")
            .HasColumnType("jsonb");

        // Ignora a Role única se ela for apenas um helper da lista de Roles
        builder.Ignore(u => u.Role);
    }
}
