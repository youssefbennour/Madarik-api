namespace Madarik.Common.DataAccess.Auditing;

public interface ISoftDeletable
{
    public DateTimeOffset? DeletedAt { get; }
}