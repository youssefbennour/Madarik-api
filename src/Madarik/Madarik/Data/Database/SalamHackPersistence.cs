using Madarik.Madarik.Data.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Madarik.Madarik.Data.Database;

internal sealed class SalamHackPersistence(DbContextOptions<SalamHackPersistence> options) : IdentityDbContext<User>(options)
{
    private const string Schema = "SalamHack";
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(Schema);
    }
}