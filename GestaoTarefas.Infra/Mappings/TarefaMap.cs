using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Infra.Mappings
{
    public class TarefaMap : IEntityTypeConfiguration<Tarefa>
    {
        public void Configure(EntityTypeBuilder<Tarefa> builder)
        {
            builder.ToTable("tarefa");
            builder.HasKey(x => x.TarefaId);

            builder.Property(x => x.Titulo)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(x => x.Descricao)
                   .HasMaxLength(500);

            builder.Property(x => x.Pontos)
                   .IsRequired();

            builder.Property(x => x.DataCriacao)
                   .IsRequired();

            builder.HasOne(x => x.Filho)
                   .WithMany()
                   .HasForeignKey(x => x.FilhoId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
