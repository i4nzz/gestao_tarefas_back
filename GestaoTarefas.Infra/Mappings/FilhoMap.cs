using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Infra.Mappings;

public class FilhoMap : IEntityTypeConfiguration<Filho>
{
    public void Configure(EntityTypeBuilder<Filho> builder)
    {
        builder.ToTable("filho");

        builder.Property(x => x.DataNascimento)
            .IsRequired();
    }
}