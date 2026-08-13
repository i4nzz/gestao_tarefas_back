using GestaoTarefas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoTarefas.API.Infra.Mappings;

public class RefreshTokenMap : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_token");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DataCriacao)
            .IsRequired();

        builder.Property(x => x.DataExpiracao)
            .IsRequired();

        builder.Property(x => x.DataRevogacao)
            .IsRequired(false);

        builder.Property(x => x.SubstituidoPorToken)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.HasIndex(x => x.Token)
            .IsUnique();

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
