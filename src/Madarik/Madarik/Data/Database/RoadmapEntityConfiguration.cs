using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Madarik.Madarik.Data.Database;

internal sealed class RoadmapEntityConfiguration : IEntityTypeConfiguration<Roadmap.Roadmap>
{
    public void Configure(EntityTypeBuilder<Roadmap.Roadmap> builder)
    {
        builder.ToTable("Roadmap");
        builder.HasKey(x => x.Id);
        builder.OwnsOne(c => c.FlowChart, d =>
        {
            d.ToJson();
            d.OwnsMany(c => c.Edges, d => 
                d.OwnsOne(d => d.Style));
            d.OwnsMany(c => c.Nodes, d =>
            {
                d.OwnsOne(d => d.Style);
                d.OwnsOne(d => d.Position);
                d.OwnsOne(d => d.Data);
            });

        });
    }
}