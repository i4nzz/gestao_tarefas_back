using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoTarefas.Infra.Mappings
{
    public class ComprovacaoTarefaMap : IEntityTypeConfiguration<ComprovacaoTarefa>
    {
        public void Configure(EntityTypeBuilder<ComprovacaoTarefa> builder)
        {
            builder.ToTable("comprovacao_tarefa");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UrlFoto)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(x => x.DataEnvio)
                   .IsRequired();

            builder.Property(x => x.Status)
                   .HasConversion<int>()
                   .HasDefaultValue(StatusValidacaoTarefaEnum.Pendente)
                   .HasSentinel(StatusValidacaoTarefaEnum.Pendente);

            builder.HasOne(x => x.Tarefa)
                   .WithMany(t => t.Comprovacoes)
                   .HasForeignKey(x => x.TarefaId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
