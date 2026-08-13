using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Infra.Mappings;

public class UsuarioMap : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuario");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.SenhaHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Ativo)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.Perfil)
            .IsRequired();

        builder.Property(x => x.EmailConfirmado)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.TokenConfirmacaoEmail)
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.TokenConfirmacaoExpiracao)
            .IsRequired(false);

        builder.Property(x => x.TokenResetSenha)
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.TokenResetSenhaExpiracao)
            .IsRequired(false);
    }
}