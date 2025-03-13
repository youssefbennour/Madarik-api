namespace Madarik.Common.Emailing;

internal sealed class EmailOptions
{
    public const string Key = "EmailOptions";
    public required string Server { get; set; } 
    public int Port { get; set; }
    public required string Sender { get; set; }
    public required string From { get; set; }
    public required string Password { get; set; }
}