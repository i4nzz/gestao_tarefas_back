using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Infra.Mappings
{
    public class RecompensaMap : IEntityTypeConfiguration<Recompensa>
    {
        public void Configure(EntityTypeBuilder<Recompensa> builder)
        {
            builder.ToTable("recompensa");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Descricao)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.PontosNecessarios)
                   .IsRequired();

            builder.HasOne(x => x.Filho)
                   .WithMany()
                   .HasForeignKey(x => x.FilhoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Ativa)
                   .IsRequired()
                   .HasDefaultValue(true);
        }
    }

}
