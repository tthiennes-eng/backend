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

        // Mapeamos o Status (int) para a coluna 'ativo' (bool) com conversão
        builder.Property(u => u.Status)
            .HasColumnName("ativo")
            .HasConversion(
                v => v == 0, // Se Status for 0 (Active), salva como true
                v => v ? 0 : 1 // Se for true, volta para 0
            );

        builder.Property(u => u.FailedLoginAttempts).HasColumnName("tentativas_falhas");
        builder.Property(u => u.LastLoginAt).HasColumnName("ultimo_login");
        builder.Property(u => u.BlockedAt).HasColumnName("bloqueado_ate");
        builder.Property(u => u.CreatedAt).HasColumnName("criado_em");
        builder.Property(u => u.UpdatedAt).HasColumnName("atualizado_em");

        // Roles como JSONB
        builder.Property(u => u.Roles)
            .HasColumnName("perfis_json") // Ajustado ou use a coluna perfil_id se preferir
            .HasColumnType("jsonb");
    }
}
