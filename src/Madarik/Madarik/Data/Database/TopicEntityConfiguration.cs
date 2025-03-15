using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Madarik.Madarik.Data.Database;

internal sealed class TopicEntityConfiguration : IEntityTypeConfiguration<Topic.Topic>
{
    public void Configure(EntityTypeBuilder<Topic.Topic> builder)
    {
        builder.ToTable("Topic");
        builder.HasKey(x => x.Id);
        builder.OwnsMany(c => c.Chapters, d =>
        {
            d.ToJson();
            d.OwnsMany(c => c.Articles);
        });
    }
}