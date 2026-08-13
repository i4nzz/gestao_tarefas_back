using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Infra.Mappings
{
    public class MesadaMap : IEntityTypeConfiguration<Mesada>
    {
        public void Configure(EntityTypeBuilder<Mesada> builder)
        {
            builder.ToTable("mesada");
            builder.HasKey(x => x.MesadaId);

            builder.Property(x => x.Valor)
                   .HasColumnType("decimal(10,2)")
                   .IsRequired();

            builder.Property(x => x.Mes)
                   .IsRequired();

            builder.Property(x => x.Ano)
                   .IsRequired();

            builder.HasOne(x => x.Filho)
                   .WithMany()
                   .HasForeignKey(x => x.FilhoId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
