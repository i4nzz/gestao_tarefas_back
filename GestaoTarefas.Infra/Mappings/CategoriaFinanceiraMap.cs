using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoTarefas.Domain.Entities;

namespace GestaoTarefas.Infra.Mappings
{
    public class CategoriaFinanceiraMap : IEntityTypeConfiguration<CategoriaFinanceira>
    {
        public void Configure(EntityTypeBuilder<CategoriaFinanceira> builder)
        {
            builder.ToTable("categoria_financeira");

            builder.HasKey(x => x.CategoriaFinanceiraId);

            builder.Property(x => x.Nome)
                   .IsRequired()
                   .HasMaxLength(100);
        }
    }

}
