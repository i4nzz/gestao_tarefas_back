using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Infra.Mappings;

public class PaisFilhosMap : IEntityTypeConfiguration<PaisFilhos>
{
    public void Configure(EntityTypeBuilder<PaisFilhos> builder)
    {
        builder.ToTable("pais_filhos");

        builder.HasKey(x => new { x.PaiId, x.FilhoId });

        builder.HasOne(x => x.Pai)
               .WithMany(p => p.PaisFilhos)
               .HasForeignKey(x => x.PaiId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Filho)
               .WithMany(f => f.PaisFilhos)
               .HasForeignKey(x => x.FilhoId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}