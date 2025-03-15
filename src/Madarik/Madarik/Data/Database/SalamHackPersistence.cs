using Madarik.Madarik.Data.Database.Migrations;
using Madarik.Madarik.Data.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Madarik.Madarik.Data.Database;

public sealed class SalamHackPersistence(DbContextOptions<SalamHackPersistence> options) : IdentityDbContext<User>(options)
{
    private const string Schema = "SalamHack";
    
    public DbSet<Roadmap.Roadmap> Roadmaps => Set<Roadmap.Roadmap>();
    public DbSet<Topic.Topic> Topics => Set<Topic.Topic>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(Schema);
        builder.ApplyConfiguration(new RoadmapEntityConfiguration());
        builder.ApplyConfiguration(new TopicEntityConfiguration());
    }
}