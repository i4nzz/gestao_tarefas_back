using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Infra.Mappings;

public class PaiMap : IEntityTypeConfiguration<Pai>
{
    public void Configure(EntityTypeBuilder<Pai> builder)
    {
        builder.ToTable("pai");
    }
}