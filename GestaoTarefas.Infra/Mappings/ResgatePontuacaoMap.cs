using GestaoTarefas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoTarefas.API.Infra.Mappings;

public class ResgatePontuacaoMap : IEntityTypeConfiguration<ResgatePontuacao>
{
    public void Configure(EntityTypeBuilder<ResgatePontuacao> builder)
    {
        builder.ToTable("resgate_pontuacao");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Pontos).IsRequired();
        builder.Property(x => x.DataResgate).IsRequired();

        builder.HasOne(x => x.Filho)
               .WithMany()
               .HasForeignKey(x => x.FilhoId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Recompensa)
               .WithMany()
               .HasForeignKey(x => x.RecompensaId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}